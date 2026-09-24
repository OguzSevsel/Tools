using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Elements;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSDialogueNodeData
    {
        public string Guid;
        public string DialogueText;
        private Vector2 position;
        public DialogueType DialogueType;
        public AudioClip AudioClip;
        public DSActor Actor;

        public bool IsStartNode;
        public List<DSChoice> Choices = new();

        public DSDialogueNodeData(string guid, string dialogueText, Vector2 position, DialogueType type, AudioClip audioClip, DSActor actor, List<DSChoice> choices)
        {
            this.Guid = guid;
            this.DialogueText = dialogueText;
            this.position = position;
            this.DialogueType = type;
            this.AudioClip = audioClip;
            this.Actor = actor;
        }

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
