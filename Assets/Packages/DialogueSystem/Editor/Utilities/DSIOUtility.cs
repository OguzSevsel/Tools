using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Tools.DialogueSystem.Elements;
using Tools.DialogueSystem.UI;

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

            string relativePath = "Assets" + path.Substring(Application.dataPath.Length);

            return AssetDatabase.LoadAssetAtPath<DSGraphSO>(relativePath);
        }

        private static void SaveNodeSO(DSNodeSO nodeSO, DSDialogueNode node)
        {
            nodeSO.DialogueId = node.Id;
            nodeSO.IsStartNode = node.isStartNode;
            nodeSO.DialogueType = node.DialogueType;
            nodeSO.DialogueText = node.Data.DialogueText;
            nodeSO.Position = node.GetPosition().position;
            nodeSO.ActorName = node.Data.Actor.Name;
            nodeSO.AudioClip = node.Data.AudioClip;
            nodeSO.ActorSprite = node.Data.Actor.sprite;
            nodeSO.name = node.Id;
        }

        public static void Save(DSGraphView graphView, string path, string assetName)
        {
            DSGraphSO graphSO = ScriptableObject.CreateInstance<DSGraphSO>();
            string fullPath = $"{path}/New Dialogue.asset";
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

            AssetDatabase.CreateAsset(graphSO, uniquePath);
            AssetDatabase.SaveAssets();

            string assetPath = AssetDatabase.GetAssetPath(graphSO);
            AssetDatabase.RenameAsset(assetPath, assetName);

            foreach (DSDialogueNode node in graphView.Nodes)
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

        public static void SaveLoadedGraph(DSGraphView graphView, DSGraphSO loadedGraphSO, string assetName)
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

            foreach (DSDialogueNode node in graphView.Nodes)
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

        static void ClearSubAssets<T>(ScriptableObject parent) where T : ScriptableObject
        {
            string path = AssetDatabase.GetAssetPath(parent);
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (var asset in assets)
            {
                if (asset is T && asset != parent)
                {
                    UnityEngine.Object.DestroyImmediate(asset, true);
                }
            }
        }

        public static void SaveConnections(DSGraphView graphView, DSGraphSO graphSO, DSDialogueNode node, DSNodeSO nodeSO)
        {
            foreach (var port in node.Choices.Keys)
            {
                if (port.connections.Count() == 0)
                {
                    DSPortData portData = (DSPortData)port.userData;
                    portData.InNodeId = "";
                    portData.OutNodeId = node.Id;
                    DSChoice choice = new DSChoice(portData.PortName, portData.InNodeId);
                    nodeSO.Choices.Add(choice);
                    graphSO.Connections.Add((DSPortData)port.userData);
                }
                else
                {
                    foreach (var edge in port.connections)
                    {
                        DSDialogueNode inNode = edge.input.node as DSDialogueNode;
                        if (inNode == null)
                            continue;
                        DSDialogueNode outNode = edge.output.node as DSDialogueNode;
                        DSPortData portData = (DSPortData)edge.output.userData;
                        portData.InNodeId = inNode.Id;
                        portData.OutNodeId = outNode.Id;
                        DSChoice choice = new DSChoice(portData.PortName, portData.InNodeId);
                        nodeSO.Choices.Add(choice);
                        graphSO.Connections.Add(portData);
                    }
                }
            }
        }

        public static void Load(DSGraphSO graphSO, DSGraphView graphView)
        {
            if (graphSO.Nodes.Count > 0 && graphSO.Nodes != null)
            {
                foreach (var nodeSO in graphSO.Nodes)
                {
                    CreateNode(nodeSO, graphView);
                }
            }

            LoadConnections(graphSO, graphView);
        }

        private static void CreateNode(DSNodeSO nodeSO, DSGraphView graphView)
        {
            if (nodeSO.IsStartNode)
            {
                DSDialogueNode node = graphView.CreateNode(nodeSO.DialogueType, nodeSO.Position, isStartNode: true, nodeSO.DialogueId, nodeSO.ActorName, nodeSO.AudioClip, nodeSO.ActorSprite, nodeSO.DialogueText, isPasting: false, isLoading: true);
                graphView.AddElement(node);
            }
            else
            {
                DSDialogueNode node = graphView.CreateNode(nodeSO.DialogueType, nodeSO.Position, isStartNode: false, nodeSO.DialogueId, nodeSO.ActorName, nodeSO.AudioClip, nodeSO.ActorSprite, nodeSO.DialogueText, isPasting: false, isLoading: true);
                graphView.AddElement(node);
            }
        }

        public static void LoadConnections(DSGraphSO graph, DSGraphView graphView)
        {
            Dictionary<string, DSDialogueNode> nodeLookUp = graphView.Nodes.OfType<DSDialogueNode>().ToDictionary(n => n.Id, n => n);

            foreach (var conn in graph.Connections)
            {
                if (!nodeLookUp.TryGetValue(conn.OutNodeId, out DSDialogueNode OutputNode)) return;
                if (!nodeLookUp.TryGetValue(conn.InNodeId, out DSDialogueNode InputNode))
                {
                    Port port = null;
                    DSPortData portData = new DSPortData("", OutputNode.Id, conn.PortName);
                    port = OutputNode.CreateChoicePort(conn.PortName, portData);
                    OutputNode.outputContainer.Add(port);
                    OutputNode.RefreshExpandedState();
                    continue;
                }

                Port outputPort = null;
                DSPortData data = new DSPortData(InputNode.Id, OutputNode.Id, conn.PortName);

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
