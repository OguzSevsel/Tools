using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    public class DSDialogueNodeData
    {
        public string Guid { get; private set; }

        public DSDialogueText Dialogue;
        public DSAudioClip AudioClip;
        public DSActor Actor;
        public DSActor Conversant;

        private Vector2 position;
        public DialogueType DialogueType;
        public bool IsStartNode;

        public DSDialogueNodeData(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSActor conversant, DSAudioClip audioClip)
        {
            this.position = position;
            this.DialogueType = type;
            this.Dialogue = dialogueText;
            this.AudioClip = audioClip;
            this.Actor = actor;
        }

        public void SetGuid(string guid) => this.Guid = guid;

        public Vector2 GetPosition() => position;

        public void SetPosition(Vector2 position) => this.position = position;
    }
}
