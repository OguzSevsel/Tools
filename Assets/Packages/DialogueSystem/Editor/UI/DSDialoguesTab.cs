using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSDialoguesTab : VisualElement
    {
        public DSDialoguesTab()
        {
            this.style.backgroundColor = Color.snow;
            this.style.flexGrow = 1;
            this.style.position = Position.Relative;
        }
    }
}