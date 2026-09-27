using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSConversationsTab : VisualElement
    {
        public DSConversationsTab()
        {
            this.style.backgroundColor = Color.purple;
            this.style.flexGrow = 1;
            this.style.position = Position.Relative;
        }
    }
}