using System.Collections.Generic;
using System.Linq;
using Tools.DialogueSystem.Elements;
using Tools.DialogueSystem.UI;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Tools.DialogueSystem.Utilities
{
    public static class DSIOUtility
    {
        public static void CreateFolderIfNotExists(string parentPath, string folderName)
        {
            string fullPath = $"{parentPath}/{folderName}";

            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                AssetDatabase.CreateFolder(parentPath, folderName);
                AssetDatabase.Refresh();
            }
        }

        public static DSGraphSO PromptAndLoad()
        {
            CreateFolderIfNotExists("Assets", "Conversations");

            string path = EditorUtility.OpenFilePanel(
                "Load Dialogue Graph",
                Application.dataPath + "/Conversations",
                "asset"
            );

            if (string.IsNullOrEmpty(path))
                return null;

            if (!path.StartsWith(Application.dataPath))
            {
                Debug.LogError("Selected file must be inside Assets folder.");
                return null;
            }

            string relativePath = "Assets" + path[Application.dataPath.Length..];

            return AssetDatabase.LoadAssetAtPath<DSGraphSO>(relativePath);
        }

        private static void SaveNodeSO(DSNodeSO nodeSO, DSDialogueNodeLegacy node)
        {
            nodeSO.DialogueId = node.Id;
            nodeSO.IsStartNode = node.isStartNode;
            nodeSO.DialogueType = node.DialogueType;
            nodeSO.DialogueText = node.Data.Dialogue.Text;
            nodeSO.Position = node.GetPosition().position;
            nodeSO.ActorName = node.Data.Actor.Title;
            nodeSO.AudioClip = node.Data.AudioClip.Clip;
            nodeSO.ActorSprite = node.Data.Actor.Sprite;
            nodeSO.name = node.Id;
        }

        public static void Save(DSGraphViewLegacy graphView, string path, string assetName)
        {
            DSGraphSO graphSO = ScriptableObject.CreateInstance<DSGraphSO>();
            string fullPath = $"{path}/New Dialogue.asset";
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            AssetDatabase.CreateAsset(graphSO, uniquePath);
            AssetDatabase.SaveAssets();

            string assetPath = AssetDatabase.GetAssetPath(graphSO);
            AssetDatabase.RenameAsset(assetPath, assetName);

            foreach (DSDialogueNodeLegacy node in graphView.Nodes)
            {
                DSNodeSO nodeSO = ScriptableObject.CreateInstance<DSNodeSO>();

                SaveNodeSO(nodeSO, node);

                graphSO.Nodes.Add(nodeSO);

                SaveConnections(graphView, graphSO, node, nodeSO);

                AssetDatabase.AddObjectToAsset(nodeSO, graphSO);
            }

            EditorUtility.SetDirty(graphSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void SaveLoadedGraph(DSGraphViewLegacy graphView, DSGraphSO loadedGraphSO, string assetName)
        {
            DSGraphSO graphSO = ScriptableObject.CreateInstance<DSGraphSO>();
            string fullPath = AssetDatabase.GetAssetPath(loadedGraphSO);

            ClearSubAssets<DSNodeSO>(loadedGraphSO);
            Object.DestroyImmediate(loadedGraphSO, true);
            AssetDatabase.DeleteAsset(fullPath);

            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            AssetDatabase.CreateAsset(graphSO, uniquePath);
            AssetDatabase.SaveAssets();

            string assetPath = AssetDatabase.GetAssetPath(graphSO);
            AssetDatabase.RenameAsset(assetPath, assetName);

            foreach (DSDialogueNodeLegacy node in graphView.Nodes)
            {
                DSNodeSO nodeSO = ScriptableObject.CreateInstance<DSNodeSO>();

                SaveNodeSO(nodeSO, node);

                graphSO.Nodes.Add(nodeSO);

                SaveConnections(graphView, graphSO, node, nodeSO);

                AssetDatabase.AddObjectToAsset(nodeSO, graphSO);
            }

            EditorUtility.SetDirty(graphSO);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ClearSubAssets<T>(ScriptableObject parent) where T : ScriptableObject
        {
            string path = AssetDatabase.GetAssetPath(parent);
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (Object asset in assets)
            {
                if (asset is T && asset != parent)
                {
                    UnityEngine.Object.DestroyImmediate(asset, true);
                }
            }
        }

        public static void SaveConnections(DSGraphViewLegacy graphView, DSGraphSO graphSO, DSDialogueNodeLegacy node, DSNodeSO nodeSO)
        {
            foreach (Port port in node.Choices.Keys)
            {
                if (port.connections.Count() == 0)
                {
                    DSPortData portData = (DSPortData)port.userData;
                    portData.InNodeId = "";
                    portData.OutNodeId = node.Id;
                    DSChoice choice = new(portData.PortName, portData.InNodeId);
                    nodeSO.Choices.Add(choice);
                    graphSO.Connections.Add((DSPortData)port.userData);
                }
                else
                {
                    foreach (Edge edge in port.connections)
                    {
                        if (edge.input.node is not DSDialogueNodeLegacy inNode)
                            continue;
                        DSDialogueNodeLegacy outNode = edge.output.node as DSDialogueNodeLegacy;
                        DSPortData portData = (DSPortData)edge.output.userData;
                        portData.InNodeId = inNode.Id;
                        portData.OutNodeId = outNode.Id;
                        DSChoice choice = new(portData.PortName, portData.InNodeId);
                        nodeSO.Choices.Add(choice);
                        graphSO.Connections.Add(portData);
                    }
                }
            }
        }

        public static void Load(DSGraphSO graphSO, DSGraphViewLegacy graphView)
        {
            if (graphSO.Nodes.Count > 0 && graphSO.Nodes != null)
            {
                foreach (DSNodeSO nodeSO in graphSO.Nodes)
                {
                    CreateNode(nodeSO, graphView);
                }
            }

            LoadConnections(graphSO, graphView);
        }

        private static void CreateNode(DSNodeSO nodeSO, DSGraphViewLegacy graphView)
        {
            if (nodeSO.IsStartNode)
            {
                DSDialogueNodeLegacy node = graphView.CreateNode(nodeSO.DialogueType, nodeSO.Position, isStartNode: true, nodeSO.DialogueId, nodeSO.ActorName, nodeSO.ConversantName, nodeSO.AudioClip, nodeSO.ActorSprite, nodeSO.ConversantSprite, nodeSO.DialogueText, isPasting: false, isLoading: true);
                graphView.AddElement(node);
            }
            else
            {
                DSDialogueNodeLegacy node = graphView.CreateNode(nodeSO.DialogueType, nodeSO.Position, isStartNode: false, nodeSO.DialogueId, nodeSO.ActorName, nodeSO.ConversantName, nodeSO.AudioClip, nodeSO.ActorSprite, nodeSO.ConversantSprite, nodeSO.DialogueText, isPasting: false, isLoading: true);
                graphView.AddElement(node);
            }
        }

        public static void LoadConnections(DSGraphSO graph, DSGraphViewLegacy graphView)
        {
            Dictionary<string, DSDialogueNodeLegacy> nodeLookUp = graphView.Nodes.OfType<DSDialogueNodeLegacy>().ToDictionary(n => n.Id, n => n);

            foreach (DSPortData conn in graph.Connections)
            {
                if (!nodeLookUp.TryGetValue(conn.OutNodeId, out DSDialogueNodeLegacy OutputNode)) return;
                if (!nodeLookUp.TryGetValue(conn.InNodeId, out DSDialogueNodeLegacy InputNode))
                {
                    Port port = null;
                    DSPortData portData = new("", OutputNode.Id, conn.PortName);
                    port = OutputNode.CreateChoicePort(conn.PortName, portData);
                    OutputNode.outputContainer.Add(port);
                    OutputNode.RefreshExpandedState();
                    continue;
                }

                Port outputPort = null;
                DSPortData data = new(InputNode.Id, OutputNode.Id, conn.PortName);

                outputPort = OutputNode.CreateChoicePort(conn.PortName, data);
                OutputNode.outputContainer.Add(outputPort);
                Port inputPort = InputNode.InputPort;

                Edge edge = outputPort.ConnectTo(inputPort);
                graphView.AddElement(edge);
                OutputNode.RefreshExpandedState();
            }
        }
    }
}
