using System;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    public class DSActor : DSData
    {
        [TextArea] public string Background;
        public Sprite Sprite;

        public DSActor(string name, string background, Sprite sprite)
        {
            this.Name = name;
            this.Background = background;
            this.Sprite = sprite;
        }
    }
}
