using System;
using System.Collections.Generic;
using Tools.DialogueSystem.UI.Elements;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSConversation : DSData
    {
        //TODO: This is going to be individual graphs of the Conversations and we will show the conversations on database tab.
        private List<DSNode> nodes;
        public event Action OnNewNodeCreated;
        public int NodeCount => nodes.Count;

        public DSConversation(string title, string description) : base(title, description)
        {
            nodes = new List<DSNode>(); 
        }

        public List<DSNode> GetNodes()
        {
            return nodes;
        }

        public void AddToNodes(DSNode node)
        {
            this.nodes.Add(node);
            OnNewNodeCreated?.Invoke();
        }
    }
}