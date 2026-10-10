using System;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSDialogueText : DSData
    {
        public string Text;

        public DSDialogueText(string title, string description, string text) : base(title, description)
        {
            Text = text;
        }
    }
}