using System;
using Unity.Properties;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSDialogueText : DSData
    {
        [SerializeField] private string _text;

        [CreateProperty]
        public string Text
        {
            get => _text;
            private set
            {
                if (_text == value)
                {
                    return;
                }

                _text = value;
                Notify();
            }
        }

        public DSDialogueText(string title, string description, string text) : base(title, description)
        {
            this.Text = text;
        }
    }
}