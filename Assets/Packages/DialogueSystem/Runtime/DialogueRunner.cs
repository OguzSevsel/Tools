using System;
using UnityEngine;

namespace Tools.DialogueSystem
{
    public class DialogueRunner
    {
        internal class DialogueInternal
        {
            private event Action<IDialogueEvent> OnDialogueEvent;
            private DSGraphSO conversation;
            private DSNodeSO currentNode;

            public DialogueInternal(System.Action<IDialogueEvent> callback, DSGraphSO conversation)
            {
                OnDialogueEvent = callback;
                this.conversation = conversation;
            }

            public void AdvanceToChoices(string nodeId)
            {
                currentNode = conversation.GetNodeById(nodeId);
                OnDialogueEvent?.Invoke(new ChoiceEvent(this, currentNode.DialogueId, currentNode.ActorName, currentNode.Choices));
            }

            public void NextDialogue(string selectedId)
            {
                currentNode = null;
                currentNode = conversation.GetNextNode(selectedId);

                if (currentNode == null)
                {
                    OnDialogueEvent?.Invoke(new EndEvent(selectedId, string.Empty));
                    return;
                }

                OnDialogueEvent?.Invoke(new MessageEvent(this, currentNode.DialogueId, currentNode.ActorName, currentNode.DialogueText, currentNode.ActorSprite, currentNode.AudioClip));
            }
        }

        private DSNodeSO currentNode;
        private DSGraphSO conversation;
        private DialogueInternal _internal;

        public event Action<IDialogueEvent> OnDialogueEvent;

        public DialogueRunner(DSGraphSO conversation)
        {
            this.conversation = conversation;
            _internal = new DialogueInternal(OnDialogueEvent, conversation);
        }

        public void StartDialogue()
        {
            currentNode = conversation.GetStartNode();

            _internal = new DialogueInternal(OnDialogueEvent, conversation);

            OnDialogueEvent?.Invoke(new MessageEvent(_internal, currentNode.DialogueId, currentNode.ActorName, currentNode.DialogueText, currentNode.ActorSprite, currentNode.AudioClip));
        }
    } 
}
