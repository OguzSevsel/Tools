using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Elements;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    public class DSDialogueNodeData : DSData
    {
        public DSDialogueText Dialogue;
        public DSAudioClip AudioClip;
        public DSActor Actor;

        private Vector2 position;
        public DialogueType DialogueType;
        public bool IsStartNode;
        public List<DSChoice> Choices = new();

        public DSDialogueNodeData(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSAudioClip audioClip, List<DSChoice> choices)
        {
            this.position = position;
            this.DialogueType = type;
            this.Dialogue = dialogueText;
            this.AudioClip = audioClip;
            this.Actor = actor;

            string guid = DSDatabaseManager.Current.Register(this);
            this.Guid = guid;
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
