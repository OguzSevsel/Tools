using System;
using System.Collections.Generic;
using Tools.DialogueSystem.UI.Elements;
using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSConversationData : DSData
    {
        //TODO: This is going to be individual graphs of the Conversations and we will show the conversations on database tab.
        private List<DSNode> Nodes;

        public DSConversationData(string title, string description) : base(title, description)
        {
            Nodes = new List<DSNode>();      
        }

        public List<DSNode> GetNodes()
        {
            return Nodes;
        }

        public void AddToNodes(DSNode node)
        {
            this.Nodes.Add(node);
        }
    }
}