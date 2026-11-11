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

        //public static void Save(DSGraphView graphView, DSConversation conversation)
        //{
        //    List<DSNode> nodeElementList =
        //        graphView.nodes.OfType<DSNode>().ToList();

        //    foreach (var node in nodeElementList)
        //    {
        //        // Clear previous connections before saving current ones.
        //        node.Data.OutputIds.Clear();

        //        var nodeData = conversation.Nodes.Find(
        //            n => n.Guid == node.Data.Guid
        //        );

        //        if (nodeData == null)
        //        {
        //            conversation.AddToNodes(node.Data);
        //        }

        //        foreach (var edge in node.OutputPort.connections)
        //        {
        //            if (edge.input.node is not DSNode connectedNode)
        //                continue;

        //            node.Data.OutputIds.Add(connectedNode.Data.Guid);
        //        }
        //    }

        //    List<DSNodeData> deletedNodeList = new();

        //    foreach (var nodeData in conversation.Nodes)
        //    {
        //        var node = nodeElementList.Find(
        //            n => n.Data.Guid == nodeData.Guid
        //        );

        //        if (node == null)
        //            deletedNodeList.Add(nodeData);
        //    }

        //    deletedNodeList.ForEach(conversation.RemoveFromNodes);
        //}

        public static void Save(DSGraphView graphView, DSConversation conversation)
        {
            List<DSNode> nodeElementList = graphView.nodes.OfType<DSNode>().ToList();

            if (nodeElementList.Count > 0)
            {
                foreach (var node in nodeElementList)
                {
                    var nodeData = conversation.Nodes.Find(n => n.Guid == node.Data.Guid);

                    if (nodeData == null)
                    {
                        conversation.AddToNodes(node.Data);
                    }

                    node.Data.OutputIds.Clear();

                    foreach (var edge in node.OutputPort.connections)
                    {
                        if (edge.input.node is not DSNode connectedNode)
                            continue;
                        node.Data.OutputIds.Add(connectedNode.Data.Guid);
                    }
                }

                List<DSNodeData> deletedNodeList = new List<DSNodeData>();

                foreach (var nodeData in conversation.Nodes)
                {
                    var node = nodeElementList.Find(n => n.Data.Guid == nodeData.Guid);

                    if (node == null)
                    {
                        deletedNodeList.Add(nodeData);
                    }
                }

                deletedNodeList.ForEach((item) => conversation.RemoveFromNodes(item));
            }
        }

        public static void CreateConnection(DSGraphView graphView, DSNode originNode, DSNode connectedNode)
        {
            Port outputPort = originNode.OutputPort;
            Port inputPort = connectedNode.InputPort;

            Edge edge = outputPort.ConnectTo(inputPort);
            graphView.AddElement(edge);
            connectedNode.RefreshExpandedState();
        }

        public static void LoadConnections(DSConversation conversation, DSGraphView graphView)
        {
            Dictionary<string, DSNode> nodeLookUp = graphView.nodes.OfType<DSNode>().ToDictionary(n => n.Data.Guid, n => n);

            foreach (DSNodeData nodeData in conversation.Nodes)
            {
                if (!nodeLookUp.TryGetValue(nodeData.Guid, out DSNode originNode)) continue;

                Port outputPort = originNode.OutputPort;

                foreach (var nodeId in nodeData.OutputIds)
                {
                    if (!nodeLookUp.TryGetValue(nodeId, out DSNode connectedNode)) continue;

                    CreateConnection(graphView, originNode, connectedNode);
                }
            }

            //foreach (DSPortData conn in conversation.Connections)
            //{
            //    if (!nodeLookUp.TryGetValue(conn.NodeId, out DSNode originNode)) continue;

            //    Port outputPort = originNode.OutputPort;

            //    foreach (var nodeId in conn.OutputIds)
            //    {
            //        if (!nodeLookUp.TryGetValue(nodeId, out DSNode connectedNode)) continue;

            //        Port inputPort = connectedNode.InputPort;
            //        Edge edge = outputPort.ConnectTo(inputPort);
            //        graphView.AddElement(edge);
            //        connectedNode.RefreshExpandedState();
            //    }
            //}
        }
    }
}
