using System;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSActor : DSData
    {
        [TextArea] public string Background;
        public Sprite Sprite;

        public DSActor(string title, string description, string background, Sprite sprite) : base(title, description)
        {
            Background = background;
            Sprite = sprite;
        }
    }
}
