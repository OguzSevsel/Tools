using System;
using System.Collections.Generic;
using Tools.DialogueSystem.UI.Elements;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSConversation : ScriptableObject, IDSData
    {
        [field: SerializeField] public List<DSNodeData> Nodes { get; set; }
        public event Action OnNewNodeCreated;
        public int NodeCount => Nodes.Count;

        [field: SerializeField] public string Guid { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public string Description { get; set; }

        public void AddToNodes(DSNodeData node)
        {
            this.Nodes.Add(node);
            OnNewNodeCreated?.Invoke();
        }

        public void RemoveFromNodes(DSNodeData node)
        {
            this.Nodes.Remove(node);
        }

        public DSNodeData Get(string guid)
        {
            return Nodes.Find(node => node.Guid == guid);
        }

        public DSNodeData GetStartNode()
        {
            return Nodes.Find(node => node.IsStartNode == true);
        }

        public List<DSNodeData> GetDialogueChoices(string guid)
        {
            var choices = new List<DSNodeData>();

            foreach (var node in Nodes)
            {
                if (node.Guid == guid)
                {
                    foreach (var choiceNodeId in node.OutputIds)
                    {
                        var choiceNode = Nodes.Find(node => node.Guid == choiceNodeId);

                        if (choiceNode != null)
                        {
                            choices.Add(choiceNode);
                        }
                    }
                }
            }

            return choices;
        }

        public void SetName(string name)
        {
            this.Name = name;
        }

        public void SetDescription(string description)
        {
            this.Description = description;
        }
    }
}