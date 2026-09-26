using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    public class DSConversationData : DSData
    {
        //TODO: This is going to be individual graphs of the Conversations and we will show the conversations on database tab.
        public List<DSDialogueNodeData> Nodes { get; private set; }



    }
}