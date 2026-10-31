using System;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSNodeData
    {
        [field: SerializeField] public string Guid { get; private set; }

        public DSDialogueText Dialogue;
        public DSAudioClip AudioClip;
        public DSActor Actor;
        public DSActor Conversant;
        [field: SerializeField] public Vector2 Position { get; private set; }
        public DialogueType DialogueType;
        public bool IsStartNode;

        public DSNodeData(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSActor conversant, DSAudioClip audioClip)
        {
            this.Guid = GUID.Generate().ToString();
            this.Position = position;
            this.DialogueType = type;
            this.Dialogue = dialogueText;
            this.AudioClip = audioClip;
            this.Actor = actor;
        }

        public void SetPosition(Vector2 position) => this.Position = position;
    }
}
