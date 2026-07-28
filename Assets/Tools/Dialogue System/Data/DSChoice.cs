using System;

namespace Tools.DialogueSystem
{
    [Serializable]
    public class DSChoice
    {
        public DSChoice(string title, string targetNodeId)
        {
            this.Title = title;
            this.TargetNodeId = targetNodeId;
        }

        public string Title;
        public string TargetNodeId;
    }
}