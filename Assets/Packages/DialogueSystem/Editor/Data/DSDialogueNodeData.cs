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

        public DSDialogueNodeData(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSAudioClip audioClip)
        {
            this.position = position;
            this.DialogueType = type;
            this.Dialogue = dialogueText;
            this.AudioClip = audioClip;
            this.Actor = actor;
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
