using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    public class DSDialogueText : DSData
    {
        public string Text;

        public DSDialogueText(string name, string text)
        {
            this.Name = name;
            this.Text = text;
        }
    }
}