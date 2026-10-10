using System;

namespace Tools.DialogueSystem
{
    [Serializable]
    public class DSChoice
    {
        public DSChoice(string title, string targetNodeId)
        {
            Title = title;
            TargetNodeId = targetNodeId;
        }

        public string Title;
        public string TargetNodeId;
    }
}