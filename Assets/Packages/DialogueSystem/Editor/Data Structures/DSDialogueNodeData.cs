using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSDialogueNodeData
    {
        public string Guid;
        public string DialogueText;
        public string ActorName;
        private Vector2 position;
        public DialogueType DialogueType;
        public AudioClip AudioClip;
        public Sprite ActorSprite;
        public bool IsStartNode;
        public List<DSChoice> Choices = new();

        public DSChoice GetTargetById(string choiceTargetId)
        {
            return Choices.Find(choice => choice.TargetNodeId == choiceTargetId);
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }
    }
}
