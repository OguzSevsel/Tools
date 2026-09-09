using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem
{
    public class DSActorsTab : VisualElement
    {
        public DSActorsTab()
        {
            this.style.backgroundColor = Color.green;
            this.style.flexGrow = 1;
            this.style.position = Position.Relative;
        }
    }
}
