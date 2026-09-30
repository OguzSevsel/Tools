using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI.Inspector;
using Tools.DialogueSystem.Utilities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSNodeInspector : VisualElement, IDSNodeInspector
    {
        private DSDialogueNodeData data;
        private TextField dialogueText;
        private DropdownField actorDropdownField;
        private DropdownField audioDropdownField;
        private DSDropdown<DSActor> actorDropdown;
        private DSDropdown<DSAudioClip> audioDropdown;


        public DSNodeInspector(DSDialogueNodeData data)
        {
            this.data = data;
            Create();
        }

        public void Create()
        {
            actorDropdownField = DSElementUtility.CreateDropdown("Actors", new List<string>(), OnActorValueChanged);
            audioDropdownField = DSElementUtility.CreateDropdown("Audio", new List<string>(), OnAudioValueChanged);
            dialogueText = DSElementUtility.CreateTextField("Dialogue Text", "Dialogue Text", true, OnDialogueTextChanged);
            actorDropdown = new DSDropdown<DSActor>(actorDropdownField, character => character.Name);
            audioDropdown = new DSDropdown<DSAudioClip>(audioDropdownField, audio => audio.Name);

            this.contentContainer.Add(actorDropdownField);
            this.contentContainer.Add(audioDropdownField);
            this.contentContainer.Add(dialogueText);
        }

        private void OnDialogueTextChanged(ChangeEvent<string> evt)
        {
            throw new NotImplementedException();
        }

        private void OnAudioValueChanged(ChangeEvent<string> evt)
        {
            throw new NotImplementedException();
        }

        private void OnActorValueChanged(ChangeEvent<string> evt)
        {
            throw new NotImplementedException();
        }
    }
}
