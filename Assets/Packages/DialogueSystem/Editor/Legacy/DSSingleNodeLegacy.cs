using Tools.DialogueSystem.Data;
using UnityEngine;

namespace Tools.DialogueSystem.Elements
{
    public class DSSingleNodeLegacy : DSDialogueNodeLegacy
    {
        public override void Initialize(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSActor conversant, DSAudioClip audioClip, bool isStartNode, bool isPasting = false, bool isLoading = false)
        {
            DialogueType = DialogueType.Single;

            base.Initialize(position, DialogueType, dialogueText, actor, conversant, audioClip, isStartNode, isPasting, isLoading);
        }

        public override void Draw()
        {
            base.Draw();

            RefreshExpandedState();
        }
    }
}
