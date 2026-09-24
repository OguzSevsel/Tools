using System;
using UnityEngine;

namespace Tools.DialogueSystem.Elements
{
    [Serializable]
    public class DSActor
    {
        public string Name;
        [TextArea] public string background;
        public Sprite sprite;

        public DSActor(string name, string background, Sprite sprite)
        {
            this.Name = name;
            this.background = background;
            this.sprite = sprite;
        }
    }
}
