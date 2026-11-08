using System;
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
        private DSGraphView GraphView;
        private VisualElement container;
        private TextField dialogueText;
        private DSPopUpField<DSActor> actorDropdown;
        private DSPopUpField<DSActor> conversantDropdown;
        private DSPopUpField<DSAudioClip> audioDropdown;
        private DSPopUpField<DSDialogueText> dialogueDropdown;

        public DSNodeInspector(DSNodeData data, DSGraphView graphView)
        {
            this.data = data;
            this.GraphView = graphView;
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

            actorDropdown.value = data.Actor;
            conversantDropdown.value = data.Conversant;
            audioDropdown.value = data.AudioClip;
            dialogueDropdown.value = data.Dialogue;

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
            actorDropdown = new DSPopUpField<DSActor>("Actor");
            conversantDropdown = new DSPopUpField<DSActor>("Conversant");
            audioDropdown = new DSPopUpField<DSAudioClip>("Audio");
            dialogueDropdown = new DSPopUpField<DSDialogueText>("Dialogue");

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
            actorDropdown.OnValueChanged += HandleOnActorValueChanged;
            conversantDropdown.OnValueChanged += HandleOnConversantValueChanged;
            audioDropdown.OnValueChanged += HandleOnAudioValueChanged;
            dialogueDropdown.OnValueChanged += HandleOnDialogueValueChanged;

            //actorDropdown.RegisterValueChangedCallback<DSActor>(OnActorValueChanged);
            //conversantDropdown.RegisterValueChangedCallback<DSActor>(OnConversantValueChanged);
            //audioDropdown.RegisterValueChangedCallback<DSAudioClip>(OnAudioValueChanged);
            //dialogueDropdown.RegisterValueChangedCallback<DSDialogueText>(OnDialogueValueChanged);

            DSDatabaseManager.Instance.DatabaseOpened += HandleOnDatabaseOpened;
            DSDatabaseManager.Instance.DatabaseClosed += HandleOnDatabaseClosed;
            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void HandleOnDialogueValueChanged(DSDialogueText dialogue)
        {
            this.data.Dialogue = dialogue;
            this.dialogueText.value = dialogue.Text;
            this.data.Dialogue?.SetText(dialogue.Text);
            GraphView.RefreshNodes();
        }

        private void HandleOnAudioValueChanged(DSAudioClip clip)
        {
            this.data.AudioClip = clip;
            GraphView.RefreshNodes();
        }

        private void HandleOnConversantValueChanged(DSActor conversant)
        {
            this.data.Conversant = conversant;
            GraphView.RefreshNodes();
        }

        private void HandleOnActorValueChanged(DSActor actor)
        {
            this.data.Actor = actor;
            GraphView.RefreshNodes();
        }

        private void HandleOnDatabaseClosed(DSDatabase database)
        {
        }

        private void HandleOnDatabaseOpened(DSDatabase database)
        {
        }

        private void UnRegisterEvents()
        {
            actorDropdown.OnValueChanged -= HandleOnActorValueChanged;
            conversantDropdown.OnValueChanged -= HandleOnConversantValueChanged;
            audioDropdown.OnValueChanged -= HandleOnAudioValueChanged;
            dialogueDropdown.OnValueChanged -= HandleOnDialogueValueChanged;

            //actorDropdown.UnregisterValueChangedCallback<DSActor>(OnActorValueChanged);
            //conversantDropdown.UnregisterValueChangedCallback<DSActor>(OnConversantValueChanged);
            //audioDropdown.UnregisterValueChangedCallback<DSAudioClip>(OnAudioValueChanged);
            //dialogueDropdown.UnregisterValueChangedCallback<DSDialogueText>(OnDialogueValueChanged);
            DSDatabaseManager.Instance.DatabaseOpened -= HandleOnDatabaseOpened;
            DSDatabaseManager.Instance.DatabaseClosed -= HandleOnDatabaseClosed;
            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        //private void OnDialogueValueChanged(ChangeEvent<DSDialogueText> evt)
        //{
        //    this.data.Dialogue = evt?.newValue;
        //    this.dialogueText.value = evt?.newValue?.Text;
        //    this.data.Dialogue?.SetText(evt?.newValue?.Text);
        //    GraphView.RefreshNodes();
        //}

        //private void OnConversantValueChanged(ChangeEvent<DSActor> evt) 
        //{
        //    this.data.Conversant = evt?.newValue;
        //    GraphView.RefreshNodes();
        //}

        //private void OnAudioValueChanged(ChangeEvent<DSAudioClip> evt) 
        //{
        //    this.data.AudioClip = evt?.newValue;
        //    GraphView.RefreshNodes();
        //}

        //private void OnActorValueChanged(ChangeEvent<DSActor> evt)
        //{
        //    this.data.Actor = evt?.newValue;
        //    GraphView.RefreshNodes();
        //}

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
