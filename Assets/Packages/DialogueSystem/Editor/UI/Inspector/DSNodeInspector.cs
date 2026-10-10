using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI.Inspector;
using Tools.DialogueSystem.Utilities;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSNodeInspector : VisualElement, IDSNodeInspector
    {
        private DSDialogueNodeData data;
        private TextField dialogueText;
        private DSDropdown<DSActor> actorDropdown;
        private DSDropdown<DSActor> conversantDropdown;
        private DSDropdown<DSAudioClip> audioDropdown;

        public DSNodeInspector(DSDialogueNodeData data)
        {
            this.data = data;
            this.SetFlex(flexGrow: 1, flexDirection: FlexDirection.Column, flexWrap: Wrap.NoWrap);
            Create();
        }

        public void Create()
        {
            CreateUIElements();
            RegisterEvents();
            LoadFields(this.data);
        }

        public void LoadFields(DSDialogueNodeData data)
        {
            this.data = data;
            actorDropdown.SetItems(DSDatabaseManager.Current.Actors);
            conversantDropdown.SetItems(DSDatabaseManager.Current.Actors);
            audioDropdown.SetItems(DSDatabaseManager.Current.AudioClips);

            actorDropdown.SetValue(data.Actor);
            conversantDropdown.SetValue(data.Conversant);
            audioDropdown.SetValue(data.AudioClip);

            dialogueText.value = data.Dialogue.Text;
        }

        #region Creation

        private void CreateUIElements()
        {
            dialogueText = DSElementUtility.CreateTextField("Dialogue Text", "Dialogue Text", true, OnDialogueTextChanged);
            DropdownField actorDropdownField = DSElementUtility.CreateDropdown("Actor", new List<string>());
            DropdownField conversantDropdownField = DSElementUtility.CreateDropdown("Conversant", new List<string>());
            DropdownField audioDropdownField = DSElementUtility.CreateDropdown("Audio", new List<string>());

            actorDropdown = new DSDropdown<DSActor>(actorDropdownField, character => character.Title);
            conversantDropdown = new DSDropdown<DSActor>(conversantDropdownField, character => character.Title);
            audioDropdown = new DSDropdown<DSAudioClip>(audioDropdownField, audio => audio.Title);

            this.contentContainer.Add(actorDropdownField);
            this.contentContainer.Add(conversantDropdownField);
            this.contentContainer.Add(audioDropdownField);
            this.contentContainer.Add(dialogueText);
        }

        #endregion

        #region Events

        private void RegisterEvents()
        {
            actorDropdown.ValueChanged += OnActorValueChanged;
            conversantDropdown.ValueChanged += OnConversantValueChanged;
            audioDropdown.ValueChanged += OnAudioValueChanged;
        }

        private void OnConversantValueChanged(DSActor conversant) => this.data.Conversant = conversant;

        private void OnDialogueTextChanged(ChangeEvent<string> dialogueText) => this.data.Dialogue.Text = dialogueText.newValue;

        private void OnAudioValueChanged(DSAudioClip audio) => this.data.AudioClip = audio;

        private void OnActorValueChanged(DSActor actor) => this.data.Actor = actor;

        #endregion
    }
}
