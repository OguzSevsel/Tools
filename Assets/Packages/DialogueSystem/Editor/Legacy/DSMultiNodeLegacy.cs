using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.Elements
{
    public class DSMultiNodeLegacy : DSDialogueNodeLegacy
    {
        public override void Initialize(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSActor conversant, DSAudioClip audioClip, bool isStartNode, bool isPasting = false, bool isLoading = false)
        {
            DialogueType = DialogueType.Multi;
            base.Initialize(position, DialogueType, dialogueText, actor, conversant, audioClip, isStartNode, isPasting, isLoading);
        }

        public override void Draw()
        {
            base.Draw();

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                Port choicePort = CreateChoicePort("New Choice", new DSPortData("", Id, "New Choice"));

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node__button");

            mainContainer.Insert(1, addChoiceButton);

            RefreshExpandedState();
        }
    }
}
