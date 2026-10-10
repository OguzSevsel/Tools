using System.Collections.Generic;
using UnityEngine;

namespace Tools.DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Graph")]
    public class DSGraphSO : ScriptableObject
    {
        public List<DSNodeSO> Nodes = new();
        public List<DSPortData> Connections = new();

        public DSNodeSO GetNodeById(string nodeId)
        {
            return Nodes.Find(node => node.DialogueId == nodeId);
        }

        public DSNodeSO GetStartNode()
        {
            return Nodes.Find(node => node.IsStartNode);
        }

        public DSNodeSO GetNextNode(string selectedNodeId)
        {
            return Nodes.Find(node => node.DialogueId == selectedNodeId);
        }
    }
}

