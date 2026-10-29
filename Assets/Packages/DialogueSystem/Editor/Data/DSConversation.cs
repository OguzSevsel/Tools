using System;
using System.Collections.Generic;
using Tools.DialogueSystem.UI.Elements;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSConversation : DSData
    {
        //TODO: This is going to be individual graphs of the Conversations and we will show the conversations on database tab.
        [field: SerializeField] public List<DSNodeData> Nodes { get; private set; }
        [field: SerializeField] public List<DSPortData> Connections { get; private set; }

        public event Action OnNewNodeCreated;
        public int NodeCount => Nodes.Count;

        public DSConversation(string title, string description) : base(title, description)
        {
            Nodes = new List<DSNodeData>();
            Connections = new List<DSPortData>();
        }

        public void AddToNodes(DSNodeData node)
        {
            this.Nodes.Add(node);
            OnNewNodeCreated?.Invoke();
        }
    }
}