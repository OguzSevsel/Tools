using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Tools.DialogueSystem.Utilities;
using Tools.DialogueSystem.Data;

namespace Tools.DialogueSystem.UI
{
    public class DSMultiNode : DSDialogueNode
    {
        public override void Initialize(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSAudioClip audioClip, bool isStartNode, bool isPasting = false, bool isLoading = false)
        {
            DialogueType = DialogueType.Multi;
            base.Initialize(position, DialogueType, dialogueText, actor, audioClip, isStartNode, isPasting, isLoading);
        }

        public override void Draw()
        {
            base.Draw();

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                Port choicePort = CreateChoicePort("New Choice", new DSPortData("", this.Id, "New Choice"));

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node__button");

            mainContainer.Insert(1, addChoiceButton);

            RefreshExpandedState();
        }
    }
}
