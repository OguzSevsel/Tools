using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSAudioClipsTab : VisualElement
    {
        public DSAudioClipsTab()
        {
            this.style.backgroundColor = Color.rosyBrown;
            this.style.flexGrow = 1;
            this.style.position = Position.Relative;
        }
    }
}