using System;
using Unity.Properties;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSActor : DSData
    {
        [SerializeField, SerializeReference] private string _background = "Background";

        [CreateProperty, TextArea]
        public string Background
        {
            get => _background;
            private set
            {
                if (_background == value)
                {
                    return;
                }

                _background = value;
                Notify();
            }
        }

        [SerializeField, SerializeReference] private Sprite _sprite;

        [CreateProperty]
        public Sprite Sprite
        {
            get => _sprite;
            private set
            {
                if (_sprite == value)
                {
                    return;
                }

                _sprite = value;
                Notify();
            }
        }

        public DSActor(string title, string description, string background, Sprite sprite) : base(title, description)
        {
            this.Background = background;
            this.Sprite = sprite;
        }
    }
}
