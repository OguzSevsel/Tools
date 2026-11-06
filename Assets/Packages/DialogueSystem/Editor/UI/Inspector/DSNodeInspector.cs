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
            RegisterEvents();
        }

        public void LoadFields(DSNodeData data)
        {
            this.data = data;
            dialogueText.ClearBinding("value");

            DSDatabase database = DSDatabaseManager.Instance.Current;

            actorDropdown.SetItems(database.Actors);
            conversantDropdown.SetItems(database.Actors);
            audioDropdown.SetItems(database.AudioClips);
            dialogueDropdown.SetItems(database.DialogueTexts);

            actorDropdown.SetValue(data.Actor);
            conversantDropdown.SetValue(data.Conversant);
            audioDropdown.SetValue(data.AudioClip);
            dialogueDropdown.SetValue(data.Dialogue);

            Debug.Log($"Actor Item Count: {actorDropdown.GetItemCount()}");
            
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
            //PopupField<DSActor> actorDropdownField = DSElementUtility.CreateDropdown<DSActor>("Actor");
            PopupField<DSActor> conversantDropdownField = DSElementUtility.CreateDropdown<DSActor>("Conversant");
            PopupField<DSAudioClip> audioDropdownField = DSElementUtility.CreateDropdown<DSAudioClip>("Audio Clips");
            PopupField<DSDialogueText> dialogueDropdownField = DSElementUtility.CreateDropdown<DSDialogueText>("Dialogue");


            var actorDropdownField = new PopupField<DSActor>();

            actorDropdownField.formatListItemCallback = actor => {
                Debug.Log($"{actor.Name}");
                return actor.Name;
            };

            actorDropdownField.formatSelectedValueCallback = actor => {
                Debug.Log($"{actor.Name}");
                return actor.Name;
            };

            actorDropdownField.choices = DSDatabaseManager.Instance.Current.Actors;
            actorDropdownField.index = 0;

            container.Add(actorDropdownField);

            actorDropdown = new DSDropdown<DSActor>(actorDropdownField, character => character.Name);
            conversantDropdown = new DSDropdown<DSActor>(conversantDropdownField, character => character.Name);
            audioDropdown = new DSDropdown<DSAudioClip>(audioDropdownField, audio => audio.Name);
            dialogueDropdown = new DSDropdown<DSDialogueText>(dialogueDropdownField, dialogue => dialogue.Name);

            actorDropdownField.SetMargins(marginTop: 5);
            conversantDropdownField.SetMargins(marginTop: 5);
            audioDropdownField.SetMargins(marginTop: 5);
            dialogueDropdownField.SetMargins(marginTop: 5);

            dialogueText = DSElementUtility.CreateTextField("Dialogue Text", isMultiLine: true);

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
            DSDatabaseManager.Instance.DatabaseOpened += HandleOnDatabaseOpened;
            DSDatabaseManager.Instance.DatabaseClosed += HandleOnDatabaseClosed;
        }

        private void HandleOnDatabaseClosed(DSDatabase database)
        {
            UnRegisterEvents();
        }

        private void HandleOnDatabaseOpened(DSDatabase database)
        {

        }

        private void UnRegisterEvents()
        {
            actorDropdown.ValueChanged -= OnActorValueChanged;
            conversantDropdown.ValueChanged -= OnConversantValueChanged;
            audioDropdown.ValueChanged -= OnAudioValueChanged;
            dialogueDropdown.ValueChanged -= OnDialogueValueChanged;
            DSDatabaseManager.Instance.DatabaseOpened -= HandleOnDatabaseOpened;
            DSDatabaseManager.Instance.DatabaseClosed -= HandleOnDatabaseClosed;
        }

        private void OnDialogueValueChanged(DSDialogueText dialogue)
        {
            this.data.Dialogue = dialogue;
            this.dialogueText.value = dialogue.Text;
            this.data.Dialogue.SetText(dialogue.Text);
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
        }

        #endregion
    }
}
