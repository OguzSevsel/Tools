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

            SaveConnections(graphView, conversation);
        }

        public static void SaveConnections(DSGraphView graphView, DSConversation conversation)
        {
            foreach (DSNode originNode in graphView.nodes.OfType<DSNode>())
            {
                DSPortData portData = (DSPortData)originNode.OutputPort.userData;
                var dbPortData = conversation.Connections.Find(p => p.NodeId == originNode.Data.Guid);

                if (dbPortData != null)
                {
                    dbPortData.OutputIds.Clear();
                    dbPortData.NodeId = originNode.Data.Guid;

                    foreach (var edge in originNode.OutputPort.connections)
                    {
                        if (edge.input.node is not DSNode connectedNode)
                            continue;
                        dbPortData.OutputIds.Add(connectedNode.Data.Guid);
                    }
                    continue;
                }

                foreach (var edge in originNode.OutputPort.connections)
                {
                    if (edge.input.node is not DSNode connectedNode)
                        continue;
                    portData.OutputIds.Add(connectedNode.Data.Guid);
                }
                conversation.Connections.Add(portData);
            }

            SyncConnections(graphView, conversation);
            static void SyncConnections(DSGraphView graphView, DSConversation conversation)
            {
                List<DSPortData> deletedPortDataList = new List<DSPortData>();

                foreach (var nodeData in conversation.Connections)
                {
                    var node = graphView.nodes.OfType<DSNode>().Where(n => n.Data.Guid == nodeData.NodeId).FirstOrDefault();

                    if (node == null)
                    {
                        deletedPortDataList.Add(nodeData);
                    }
                    else
                    {
                        if (node.OutputPort.connections.Count() == 0)
                        {
                            deletedPortDataList.Add(nodeData);
                        }
                    }
                }

                deletedPortDataList.ForEach((item) => conversation.Connections.Remove(item));
            }
        }

        public static void LoadConnections(DSConversation conversation, DSGraphView graphView)
        {
            Dictionary<string, DSNode> nodeLookUp = graphView.nodes.OfType<DSNode>().ToDictionary(n => n.Data.Guid, n => n);

            foreach (DSPortData conn in conversation.Connections)
            {
                if (!nodeLookUp.TryGetValue(conn.NodeId, out DSNode originNode)) continue;

                Port outputPort = originNode.OutputPort;

                foreach (var nodeId in conn.OutputIds)
                {
                    if (!nodeLookUp.TryGetValue(nodeId, out DSNode connectedNode)) continue;

                    Port inputPort = connectedNode.InputPort;
                    Edge edge = outputPort.ConnectTo(inputPort);
                    graphView.AddElement(edge);
                    connectedNode.RefreshExpandedState();
                }
            }
        }
    }
}
