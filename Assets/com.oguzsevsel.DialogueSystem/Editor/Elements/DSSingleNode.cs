using UnityEngine;

namespace Tools.DialogueSystem.Elements
{
	public class DSSingleNode : DSNode
	{
		public override void Initialize(Vector2 position, bool isStartNode, string dialogueId, string actorName, AudioClip audioClip, Sprite actorSprite, string dialogueText, bool isPasting = false, bool isLoading = false)
		{
			base.Initialize(position, isStartNode, dialogueId, actorName, audioClip, actorSprite, dialogueText, isPasting, isLoading);

			DialogueType = DialogueType.Single;
        }

		public override void Draw()
		{
			base.Draw();

            RefreshExpandedState();
        }
    } 
}
