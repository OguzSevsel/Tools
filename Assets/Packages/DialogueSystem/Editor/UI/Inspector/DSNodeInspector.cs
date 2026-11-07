using System;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Common;
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
        private DSActorPopUpField actorDropdown;
        private DSConversantPopUpField conversantDropdown;
        private DSAudioPopUpField audioDropdown;
        private DSDialoguePopUpField dialogueDropdown;

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
            RegisterEvents();
            LoadFields(this.data);
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

            actorDropdown.SetValueWithoutNotify(data.Actor != null && database.Actors.Contains(data.Actor)? data.Actor : null);

            conversantDropdown.SetValueWithoutNotify(data.Conversant != null && database.Actors.Contains(data.Conversant) ? data.Conversant : null);

            audioDropdown.SetValueWithoutNotify(data.AudioClip != null && database.AudioClips.Contains(data.AudioClip) ? data.AudioClip : null);

            dialogueDropdown.SetValueWithoutNotify(data.Dialogue != null && database.DialogueTexts.Contains(data.Dialogue) ? data.Dialogue : null);

            dialogueText.SetValueWithoutNotify(data.Dialogue?.Text);

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
            actorDropdown = new DSActorPopUpField();
            conversantDropdown = new DSConversantPopUpField();
            audioDropdown = new DSAudioPopUpField();
            dialogueDropdown = new DSDialoguePopUpField();

            dialogueText = DSElementUtility.CreateTextField("", isMultiLine: true);

            container.Add(actorDropdown);
            container.Add(conversantDropdown);
            container.Add(audioDropdown);
            container.Add(dialogueDropdown);
            container.Add(dialogueText);
        }

        #endregion
        
        #region Events

        private void RegisterEvents()
        {
            actorDropdown.RegisterValueChangedCallback<DSActor>(OnActorValueChanged);
            conversantDropdown.RegisterValueChangedCallback<DSActor>(OnConversantValueChanged);
            audioDropdown.RegisterValueChangedCallback<DSAudioClip>(OnAudioValueChanged);
            dialogueDropdown.RegisterValueChangedCallback<DSDialogueText>(OnDialogueValueChanged);
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
            actorDropdown.UnregisterValueChangedCallback<DSActor>(OnActorValueChanged);
            conversantDropdown.UnregisterValueChangedCallback<DSActor>(OnConversantValueChanged);
            audioDropdown.UnregisterValueChangedCallback<DSAudioClip>(OnAudioValueChanged);
            dialogueDropdown.UnregisterValueChangedCallback<DSDialogueText>(OnDialogueValueChanged);
            DSDatabaseManager.Instance.DatabaseOpened -= HandleOnDatabaseOpened;
            DSDatabaseManager.Instance.DatabaseClosed -= HandleOnDatabaseClosed;
        }

        private void OnDialogueValueChanged(ChangeEvent<DSDialogueText> evt)
        {
            this.data.Dialogue = evt?.newValue;
            this.dialogueText.value = evt?.newValue?.Text;
            this.data.Dialogue?.SetText(evt?.newValue?.Text);
        }

        private void OnConversantValueChanged(ChangeEvent<DSActor> evt) 
        { 
            this.data.Conversant = evt?.newValue;
        }

        private void OnAudioValueChanged(ChangeEvent<DSAudioClip> evt) 
        { 
            this.data.AudioClip = evt?.newValue;
        }

        private void OnActorValueChanged(ChangeEvent<DSActor> evt)
        {
            this.data.Actor = evt?.newValue;
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
