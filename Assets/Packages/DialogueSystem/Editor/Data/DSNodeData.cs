using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    [GeneratePropertyBag]
    public class DSNodeData : INotifyBindablePropertyChanged
    {
        [field: SerializeField] public string Guid { get; private set; }

        [SerializeField] private DSDialogueText _dialogue;
        [CreateProperty]
        public DSDialogueText Dialogue
        {
            get => _dialogue;
            set
            {
                if (_dialogue == value) return;

                if (_dialogue != null) _dialogue.propertyChanged -= OnDialogueChanged;
                _dialogue = value;
                if (_dialogue != null) _dialogue.propertyChanged += OnDialogueChanged;

                Notify(nameof(Dialogue));
                Notify(nameof(DialogueText));   // dropdown switched -> label refreshes
            }
        }

        // Read-only convenience property the label binds to
        [CreateProperty]
        public string DialogueText => _dialogue?.Text;

        void OnDialogueChanged(object sender, BindablePropertyChangedEventArgs e)
        {
            if (e.propertyName == nameof(DSDialogueText.Text))
                Notify(nameof(DialogueText));   // text edited -> label refreshes
        }

        [SerializeField] private DSAudioClip _audioClip;
        [CreateProperty]
        public DSAudioClip AudioClip
        {
            get => _audioClip;
            set
            {
                if (_audioClip == value)
                    return;

                _audioClip = value;
                Notify();
            }
        }

        [SerializeField] private DSActor _actor;

        [CreateProperty]
        public DSActor Actor
        {
            get => _actor;
            set
            {
                if (_actor == value)
                    return;

                _actor = value;
                Notify();
            }
        }

        [SerializeField] private DSActor _conversant;
        [CreateProperty]
        public DSActor Conversant
        {
            get => _conversant;
            set
            {
                if (_conversant == value)
                    return;

                _conversant = value;
                Notify();
            }
        }

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
            this.Conversant = conversant;
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
