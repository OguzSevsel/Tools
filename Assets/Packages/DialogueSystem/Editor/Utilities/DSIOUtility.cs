using System.Collections.Generic;
using System.Linq;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Elements;
using Tools.DialogueSystem.UI;
using Tools.DialogueSystem.UI.Elements;
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

        //public static DSGraphSO PromptAndLoad()
        //{
        //    CreateFolderIfNotExists("Assets", "Conversations");

        //    string path = EditorUtility.OpenFilePanel(
        //        "Load Dialogue Graph",
        //        Application.dataPath + "/Conversations",
        //        "asset"
        //    );

        //    if (string.IsNullOrEmpty(path))
        //        return null;

        //    if (!path.StartsWith(Application.dataPath))
        //    {
        //        Debug.LogError("Selected file must be inside Assets folder.");
        //        return null;
        //    }

        //    string relativePath = "Assets" + path[Application.dataPath.Length..];

        //    return AssetDatabase.LoadAssetAtPath<DSGraphSO>(relativePath);
        //}

        //private static void SaveNodeSO(DSNodeSO nodeSO, DSDialogueNodeLegacy node)
        //{
        //    nodeSO.DialogueId = node.Id;
        //    nodeSO.IsStartNode = node.isStartNode;
        //    nodeSO.DialogueType = node.DialogueType;
        //    nodeSO.DialogueText = node.Data.Dialogue.Text;
        //    nodeSO.Position = node.GetPosition().position;
        //    nodeSO.ActorName = node.Data.Actor.Title;
        //    nodeSO.AudioClip = node.Data.AudioClip.Clip;
        //    nodeSO.ActorSprite = node.Data.Actor.Sprite;
        //    nodeSO.name = node.Id;
        //}

        //public static void Save(DSGraphViewLegacy graphView, string path, string assetName)
        //{
        //    DSGraphSO graphSO = ScriptableObject.CreateInstance<DSGraphSO>();
        //    string fullPath = $"{path}/New Dialogue.asset";
        //    string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        //    AssetDatabase.CreateAsset(graphSO, uniquePath);
        //    AssetDatabase.SaveAssets();

        //    string assetPath = AssetDatabase.GetAssetPath(graphSO);
        //    AssetDatabase.RenameAsset(assetPath, assetName);

        //    foreach (DSDialogueNodeLegacy node in graphView.Nodes)
        //    {
        //        DSNodeSO nodeSO = ScriptableObject.CreateInstance<DSNodeSO>();

        //        SaveNodeSO(nodeSO, node);

        //        graphSO.Nodes.Add(nodeSO);

        //        SaveConnections(graphView, graphSO, node, nodeSO);

        //        AssetDatabase.AddObjectToAsset(nodeSO, graphSO);
        //    }

        //    EditorUtility.SetDirty(graphSO);
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        //public static void SaveLoadedGraph(DSGraphViewLegacy graphView, DSGraphSO loadedGraphSO, string assetName)
        //{
        //    DSGraphSO graphSO = ScriptableObject.CreateInstance<DSGraphSO>();
        //    string fullPath = AssetDatabase.GetAssetPath(loadedGraphSO);

        //    ClearSubAssets<DSNodeSO>(loadedGraphSO);
        //    Object.DestroyImmediate(loadedGraphSO, true);
        //    AssetDatabase.DeleteAsset(fullPath);

        //    string uniquePath = AssetDatabase.GenerateUniqueAssetPath(fullPath);

        //    AssetDatabase.CreateAsset(graphSO, uniquePath);
        //    AssetDatabase.SaveAssets();

        //    string assetPath = AssetDatabase.GetAssetPath(graphSO);
        //    AssetDatabase.RenameAsset(assetPath, assetName);

        //    foreach (DSDialogueNodeLegacy node in graphView.Nodes)
        //    {
        //        DSNodeSO nodeSO = ScriptableObject.CreateInstance<DSNodeSO>();

        //        SaveNodeSO(nodeSO, node);

        //        graphSO.Nodes.Add(nodeSO);

        //        SaveConnections(graphView, graphSO, node, nodeSO);

        //        AssetDatabase.AddObjectToAsset(nodeSO, graphSO);
        //    }

        //    EditorUtility.SetDirty(graphSO);
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        //private static void ClearSubAssets<T>(ScriptableObject parent) where T : ScriptableObject
        //{
        //    string path = AssetDatabase.GetAssetPath(parent);
        //    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);

        //    foreach (Object asset in assets)
        //    {
        //        if (asset is T && asset != parent)
        //        {
        //            UnityEngine.Object.DestroyImmediate(asset, true);
        //        }
        //    }
        //}

        //public static void Load(DSConversation conversation, DSGraphViewLegacy graphView)
        //{
        //    if (conversation.NodeCount > 0)
        //    {
        //        foreach (DSNode nodeSO in conversation.GetNodes())
        //        {
        //            var data = nodeSO.Data;
        //            CreateNode(data, graphView);
        //        }
        //    }

        //    LoadConnections(conversation, graphView);
        //}

        //private static void CreateNode(DSDialogueNodeData nodeSO, DSGraphViewLegacy graphView)
        //{
        //    if (nodeSO.IsStartNode)
        //    {
        //        DSDialogueNodeLegacy node = graphView.CreateNode(nodeSO.DialogueType, nodeSO.Position, isStartNode: true, nodeSO.DialogueId, nodeSO.ActorName, nodeSO.ConversantName, nodeSO.AudioClip, nodeSO.ActorSprite, nodeSO.ConversantSprite, nodeSO.DialogueText, isPasting: false, isLoading: true);
        //        graphView.AddElement(node);
        //    }
        //    else
        //    {
        //        DSDialogueNodeLegacy node = graphView.CreateNode(nodeSO.DialogueType, nodeSO.Position, isStartNode: false, nodeSO.DialogueId, nodeSO.ActorName, nodeSO.ConversantName, nodeSO.AudioClip, nodeSO.ActorSprite, nodeSO.ConversantSprite, nodeSO.DialogueText, isPasting: false, isLoading: true);
        //        graphView.AddElement(node);
        //    }
        //}

        public static void SaveConnections(DSGraphView graphView, DSGraphSO graphSO, DSNode node, DSNodeSO nodeSO)
        {
            foreach (Port port in node.OutputPort.Children())
            {
                if (port.connections.Count() == 0)
                {
                    DSPortData portData = (DSPortData)port.userData;
                    portData.InNodeId = "";
                    portData.OutNodeId = node.Data.Guid.ToString();
                    DSChoice choice = new(portData.PortName, portData.InNodeId);
                    nodeSO.Choices.Add(choice);
                    graphSO.Connections.Add((DSPortData)port.userData);
                }
                else
                {
                    foreach (Edge edge in port.connections)
                    {
                        if (edge.input.node is not DSNode inNode)
                            continue;
                        DSNode outNode = edge.output.node as DSNode;
                        DSPortData portData = (DSPortData)edge.output.userData;
                        portData.InNodeId = inNode.Data.Guid;
                        portData.OutNodeId = outNode.Data.Guid;
                        DSChoice choice = new(portData.PortName, portData.InNodeId);
                        nodeSO.Choices.Add(choice);
                        graphSO.Connections.Add(portData);
                    }
                }
            }
        }

        public static void LoadConnections(DSConversation conversation, DSGraphView graphView)
        {
            Dictionary<string, DSNode> nodeLookUp = graphView.CurrentConversation.Nodes.OfType<DSNode>().ToDictionary(n => n.Data.Guid, n => n);

            foreach (DSPortData conn in conversation.Connections)
            {
                if (!nodeLookUp.TryGetValue(conn.OutNodeId, out DSNode OutputNode)) return;
                if (!nodeLookUp.TryGetValue(conn.InNodeId, out DSNode InputNode))
                {
                    Port port = null;
                    DSPortData portData = new("", OutputNode.Data.Guid, conn.PortName);
                    port = OutputNode.CreatePort(conn.PortName);
                    OutputNode.outputContainer.Add(port);
                    OutputNode.RefreshExpandedState();
                    continue;
                }

                Port outputPort = null;
                DSPortData data = new(InputNode.Data.Guid, OutputNode.Data.Guid, conn.PortName);

                outputPort = OutputNode.CreatePort(conn.PortName);
                OutputNode.outputContainer.Add(outputPort);
                Port inputPort = InputNode.InputPort;

                Edge edge = outputPort.ConnectTo(inputPort);
                graphView.AddElement(edge);
                OutputNode.RefreshExpandedState();
            }
        }
    }
}
