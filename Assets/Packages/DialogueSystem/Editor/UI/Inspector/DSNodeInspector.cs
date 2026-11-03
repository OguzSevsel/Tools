using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI.Inspector;
using Tools.DialogueSystem.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSNodeInspector : IDSNodeInspector
    {
        private DSNodeData data;
        private VisualElement container;
        private TextField dialogueText;
        private DSDropdown<DSActor> actorDropdown;
        private DSDropdown<DSActor> conversantDropdown;
        private DSDropdown<DSAudioClip> audioDropdown;
        private DSDropdown<DSDialogueText> dialogueDropdown;

        public DSNodeInspector(DSNodeData data)
        {
            this.data = data;
            container = new VisualElement();
            container.SetFlex(flexGrow: 1, flexDirection: FlexDirection.Column, flexWrap: Wrap.NoWrap);
            container.SetAlignment(justifyContent: Justify.Center);
            container.SetPaddings(10);
            container.SetBackgroundColor(Color.blue);
            Create();
        }

        public void Create()
        {
            CreateUIElements();
            LoadFields(this.data);
        }

        public void LoadFields(DSNodeData data)
        {
            this.data = data;
            RegisterEvents();
            dialogueText.ClearBinding("value");

            actorDropdown.SetItems(DSDatabaseManager.Instance.Current.Actors);
            conversantDropdown.SetItems(DSDatabaseManager.Instance.Current.Actors);
            audioDropdown.SetItems(DSDatabaseManager.Instance.Current.AudioClips);
            dialogueDropdown.SetItems(DSDatabaseManager.Instance.Current.DialogueTexts);

            actorDropdown.SetValue(data.Actor);
            conversantDropdown.SetValue(data.Conversant);
            audioDropdown.SetValue(data.AudioClip);
            dialogueDropdown.SetValue(data.Dialogue);
            
            dialogueText.value = this.data.Dialogue.Text;
            dialogueText.SetBinding("value", new DataBinding
            {
                dataSource = this.data,
                dataSourcePath = new Unity.Properties.PropertyPath("Dialogue.Text"),
                bindingMode = BindingMode.ToSource
            });
        }

        #region Creation

        private void CreateUIElements()
        {
            dialogueText = DSElementUtility.CreateTextField("Dialogue Text", isMultiLine: true);
            DropdownField actorDropdownField = DSElementUtility.CreateDropdown("Actor", new List<string>());
            DropdownField conversantDropdownField = DSElementUtility.CreateDropdown("Conversant", new List<string>());
            DropdownField audioDropdownField = DSElementUtility.CreateDropdown("Audio", new List<string>());
            DropdownField dialogueDropdownField = DSElementUtility.CreateDropdown("Dialogue Text", new List<string>());

            actorDropdown = new DSDropdown<DSActor>(actorDropdownField, character => character.Name);
            conversantDropdown = new DSDropdown<DSActor>(conversantDropdownField, character => character.Name);
            audioDropdown = new DSDropdown<DSAudioClip>(audioDropdownField, audio => audio.Name);
            dialogueDropdown = new DSDropdown<DSDialogueText>(dialogueDropdownField, dialogue => dialogue.Name);

            actorDropdownField.SetMargins(marginTop: 5);
            conversantDropdownField.SetMargins(marginTop: 5);
            audioDropdownField.SetMargins(marginTop: 5);
            dialogueDropdownField.SetMargins(marginTop: 5);

            container.Add(actorDropdownField);
            container.Add(conversantDropdownField);
            container.Add(audioDropdownField);
            container.Add(dialogueDropdownField);
            container.Add(dialogueText);
        }

        #endregion

        #region Events

        private void RegisterEvents()
        {
            actorDropdown.ValueChanged += OnActorValueChanged;
            conversantDropdown.ValueChanged += OnConversantValueChanged;
            audioDropdown.ValueChanged += OnAudioValueChanged;
            dialogueDropdown.ValueChanged += OnDialogueValueChanged;
        }

        private void UnRegisterEvents()
        {
            actorDropdown.ValueChanged -= OnActorValueChanged;
            conversantDropdown.ValueChanged -= OnConversantValueChanged;
            audioDropdown.ValueChanged -= OnAudioValueChanged;
            dialogueDropdown.ValueChanged -= OnDialogueValueChanged;
        }

        private void OnDialogueValueChanged(DSDialogueText dialogue)
        {
            this.data.Dialogue = dialogue;
            this.dialogueText.value = dialogue.Text;
        }

        private void OnConversantValueChanged(DSActor conversant) 
        { 
            this.data.Conversant = conversant;
        }

        private void OnAudioValueChanged(DSAudioClip audio) 
        { 
            this.data.AudioClip = audio;
        }

        private void OnActorValueChanged(DSActor actor)
        {
            this.data.Actor = actor;
        }

        public VisualElement GetContainer()
        {
            return container;
        }

        public void Remove()
        {
            container.RemoveFromHierarchy();
            UnRegisterEvents();
        }

        #endregion
    }
}
