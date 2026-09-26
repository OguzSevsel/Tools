using Tools.DialogueSystem.Data;
using UnityEngine;

namespace Tools.DialogueSystem.UI
{
	public class DSSingleNode : DSDialogueNode
	{
		public override void Initialize(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSAudioClip audioClip, bool isStartNode, bool isPasting = false, bool isLoading = false)
		{
            DialogueType = DialogueType.Single;

            base.Initialize(position, DialogueType, dialogueText, actor, audioClip, isStartNode, isPasting, isLoading);
        }

		public override void Draw()
		{
			base.Draw();

            RefreshExpandedState();
        }
    } 
}
