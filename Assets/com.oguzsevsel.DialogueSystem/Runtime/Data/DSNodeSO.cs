using System.Collections.Generic;
using UnityEngine;

namespace Tools.DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Node")]
    public class DSNodeSO : ScriptableObject
    {
        public string DialogueId;
        public string DialogueText;
        public string ActorName;
        public Vector2 Position;
        public DialogueType DialogueType;
        public AudioClip AudioClip;
        public Sprite ActorSprite;
        public bool IsStartNode;
        public List<DSChoice> Choices = new();

        public DSChoice GetTargetById(string choiceTargetId)
        {
            return Choices.Find(choice => choice.TargetNodeId == choiceTargetId);
        }
    }
}



