using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    //[GeneratePropertyBag]
    public class DSNodeData : INotifyBindablePropertyChanged
    {
        [field: SerializeField] public string Guid { get; private set; }
        [field: SerializeField] public Vector2 Position { get; private set; }
        [field: SerializeField] public List<string> OutputIds { get; set; }
        public DialogueType DialogueType;
        public bool IsStartNode;

        [field: SerializeField] public string ActorId { get; private set; }
        [field: SerializeField] public string ConversantId { get; private set; }
        [field: SerializeField] public string AudioClipId { get; private set; }
        [field: SerializeField] public string DialogueId { get; private set; }

        [SerializeField, SerializeReference] private string _name;

        [CreateProperty]
        public string Name
        {
            get => _name;
            private set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                Notify();
            }
        }

        public DSNodeData(Vector2 position, DialogueType type, string name, bool isStartNode)
        {
            this.Guid = GUID.Generate().ToString();
            this.Name = name;
            this.IsStartNode = isStartNode;
            this.OutputIds = new List<string>();
            this.Position = position;
            this.DialogueType = type;
        }

        public void SetAllIds(string actorId, string conversantId, string audioClipId, string dialogueId)
        {
            this.ActorId = actorId;
            this.ConversantId = conversantId;
            this.AudioClipId = audioClipId;
            this.DialogueId = dialogueId;
        }

        public void SetAudioClipId(string id)
        {
            this.AudioClipId = id;
        }

        public void SetActorId(string id)
        {
            this.ActorId = id;
        }

        public void SetDialogueId(string id)
        {
            this.DialogueId = id;
        }

        public void SetConversantId(string id)
        {
            this.ConversantId = id;
        }

        public void SetPosition(Vector2 position) => this.Position = position;

        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        protected void Notify(
            [CallerMemberName] string propertyName = null)
        {
            propertyChanged?.Invoke(
                this,
                new BindablePropertyChangedEventArgs(propertyName)
            );
        }
    }
}
