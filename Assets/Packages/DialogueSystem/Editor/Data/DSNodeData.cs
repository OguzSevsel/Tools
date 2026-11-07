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
                Notify(nameof(DialogueText));
            }
        }

        [CreateProperty]
        public string DialogueText => _dialogue?.Text;

        void OnDialogueChanged(object sender, BindablePropertyChangedEventArgs e)
        {
            if (e.propertyName == nameof(DSDialogueText.Text))
                Notify(nameof(DialogueText));
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

        public DSNodeData(Vector2 position, DialogueType type)
        {
            this.Guid = GUID.Generate().ToString();
            this.Position = position;
            this.DialogueType = type;
        }

        public void SetActor(DSActor actor)
        {
            this.Actor = actor;
        }

        public void SetConversant(DSActor conversant)
        {
            this.Conversant = conversant;
        }

        public void SetAudioClip(DSAudioClip audioClip)
        {
            this.AudioClip = audioClip;
        }

        public void SetDialogue(DSDialogueText dialogue)
        {
            this.Dialogue = dialogue;
        }

        public void SetAllFields(DSActor actor, DSActor conversant, DSAudioClip audioClip, DSDialogueText dialogue)
        {
            this.Actor = actor;
            this.Conversant = conversant;
            this.AudioClip = audioClip;
            this.Dialogue = dialogue;
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
