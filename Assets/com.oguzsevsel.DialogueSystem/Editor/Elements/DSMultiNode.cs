using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Tools.DialogueSystem.Utilities;

namespace Tools.DialogueSystem.Elements
{
    public class DSMultiNode : DSNode
    {
        public override void Initialize(Vector2 position, bool isStartNode, string dialogueId, string actorName, AudioClip audioClip, Sprite actorSprite, string dialogueText, bool isPasting = false, bool isLoading = false)
        {
            base.Initialize(position, isStartNode, dialogueId, actorName, audioClip, actorSprite, dialogueText, isPasting, isLoading);

            DialogueType = DialogueType.Multi;
        }

        public override void Draw()
        {
            base.Draw();

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                Port choicePort = CreateChoicePort("New Choice", new DSPortData("", this.DialogueId, "New Choice"));

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node__button");

            mainContainer.Insert(1, addChoiceButton);

            RefreshExpandedState();
        }
    }
}
