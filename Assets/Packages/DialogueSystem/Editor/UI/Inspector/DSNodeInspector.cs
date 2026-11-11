using System;
using System.Linq;
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
        private DSNodeData data;
        private DSGraphView GraphView;
        private DSPopUpField<DSActor> actorDropdown;
        private DSPopUpField<DSActor> conversantDropdown;
        private DSPopUpField<DSAudioClip> audioDropdown;
        private DSPopUpField<DSDialogueText> dialogueDropdown;

        private VisualElement actorContainer;
        private VisualElement actorFieldContainer;
        private VisualElement conversantContainer;
        private VisualElement conversantFieldContainer;
        private VisualElement audioClipContainer;
        private VisualElement audioClipFieldContainer;
        private VisualElement dialogueContainer;
        private VisualElement dialogueFieldContainer;

        private Button actorButton;
        private Button conversantButton;
        private Button audioClipButton;
        private Button dialogueButton;

        public DSNodeInspector(DSNodeData data, DSGraphView graphView, VisualTreeAsset nodeInspectorAsset)
        {
            this.data = data;
            this.GraphView = graphView;
            nodeInspectorAsset.CloneTree(this);
            this.SetBackgroundColor(Color.blue);
            Create();
        }

        public void LoadFields(DSNodeData data)
        {
            this.data = data;
            this.dataSource = this.data;

            DSDatabase database = DSDatabaseManager.Instance.Current;

            actorDropdown.SetItems(database.Actors);
            conversantDropdown.SetItems(database.Actors);
            audioDropdown.SetItems(database.AudioClips);
            dialogueDropdown.SetItems(database.DialogueTexts);

            actorDropdown.value = data.Actor;
            conversantDropdown.value = data.Conversant;
            audioDropdown.value = data.AudioClip;
            dialogueDropdown.value = data.Dialogue;

            this.actorFieldContainer.dataSource = this.data.Actor;
            this.conversantFieldContainer.dataSource = this.data.Conversant;
            this.audioClipFieldContainer.dataSource = this.data.AudioClip;
            this.dialogueFieldContainer.dataSource = this.data.Dialogue;
        }

        #region INITIALIZATION

        public void Create()
        {
            CreateUIElements();
            RegisterEvents();
            LoadFields(this.data);
        }

        private void CreateUIElements()
        {
            actorContainer = this.Q<VisualElement>("ActorContainer");
            conversantContainer = this.Q<VisualElement>("ConversantContainer");
            audioClipContainer = this.Q<VisualElement>("AudioClipContainer");
            dialogueContainer = this.Q<VisualElement>("DialogueContainer");

            actorFieldContainer = this.Q<VisualElement>("ActorFieldContainer");
            conversantFieldContainer = this.Q<VisualElement>("ConversantFieldContainer");
            audioClipFieldContainer = this.Q<VisualElement>("AudioClipFieldContainer");
            dialogueFieldContainer = this.Q<VisualElement>("DialogueFieldContainer");

            actorButton = this.Q<Button>("ActorButton");
            conversantButton = this.Q<Button>("ConversantButton");
            audioClipButton = this.Q<Button>("AudioClipButton");
            dialogueButton = this.Q<Button>("DialogueButton");

            actorDropdown = new DSPopUpField<DSActor>("Actor");
            conversantDropdown = new DSPopUpField<DSActor>("Conversant");
            audioDropdown = new DSPopUpField<DSAudioClip>("Audio");
            dialogueDropdown = new DSPopUpField<DSDialogueText>("Dialogue");

            actorContainer.Insert(0, actorDropdown);
            conversantContainer.Insert(0, conversantDropdown);
            audioClipContainer.Insert(0, audioDropdown);
            dialogueContainer.Insert(0, dialogueDropdown);

            ToggleVisibility(actorFieldContainer);
            ToggleVisibility(conversantFieldContainer);
            ToggleVisibility(audioClipFieldContainer);
            ToggleVisibility(dialogueFieldContainer);
        }

        #endregion

        #region EVENT REGISTRY

        private void RegisterEvents()
        {
            actorDropdown.OnValueChanged += HandleOnActorValueChanged;
            conversantDropdown.OnValueChanged += HandleOnConversantValueChanged;
            audioDropdown.OnValueChanged += HandleOnAudioValueChanged;
            dialogueDropdown.OnValueChanged += HandleOnDialogueValueChanged;

            actorButton.clicked += HandleActorButtonClicked;
            conversantButton.clicked += HandleConversantButtonClicked;
            audioClipButton.clicked += HandleAudioClipButtonClicked;
            dialogueButton.clicked += HandleDialogueButtonClicked;

            DSEditorView.OnWindowCloses += UnRegisterEvents;
        }

        private void UnRegisterEvents()
        {
            actorDropdown.OnValueChanged -= HandleOnActorValueChanged;
            conversantDropdown.OnValueChanged -= HandleOnConversantValueChanged;
            audioDropdown.OnValueChanged -= HandleOnAudioValueChanged;
            dialogueDropdown.OnValueChanged -= HandleOnDialogueValueChanged;

            actorButton.clicked -= HandleActorButtonClicked;
            conversantButton.clicked -= HandleConversantButtonClicked;
            audioClipButton.clicked -= HandleAudioClipButtonClicked;
            dialogueButton.clicked -= HandleDialogueButtonClicked;

            DSEditorView.OnWindowCloses -= UnRegisterEvents;
        }

        #endregion

        #region EVENT HANDLERS

        private void HandleDialogueButtonClicked()
        {
            ToggleVisibility(dialogueFieldContainer);
        }

        private void HandleAudioClipButtonClicked()
        {
            ToggleVisibility(audioClipFieldContainer);
        }

        private void HandleConversantButtonClicked()
        {
            ToggleVisibility(conversantFieldContainer);
        }

        private void HandleActorButtonClicked()
        {
            ToggleVisibility(actorFieldContainer);
        }

        private void HandleOnDialogueValueChanged(DSDialogueText dialogue)
        {
            this.data.Dialogue = dialogue;
            ChangeSelectionData<DSDialogueText>(dialogue);
            GraphView.RefreshNodes();
            this.dialogueFieldContainer.dataSource = this.data.Dialogue;
        }

        private void HandleOnAudioValueChanged(DSAudioClip clip)
        {
            this.data.AudioClip = clip;
            ChangeSelectionData<DSAudioClip>(clip);
            GraphView.RefreshNodes();
            this.audioClipFieldContainer.dataSource = this.data.AudioClip;
        }

        private void HandleOnConversantValueChanged(DSActor conversant)
        {
            this.data.Conversant = conversant;
            ChangeSelectionData<DSActor>(conversant, false);
            GraphView.RefreshNodes();
            this.conversantFieldContainer.dataSource = this.data.Conversant;
        }

        private void HandleOnActorValueChanged(DSActor actor)
        {
            this.data.Actor = actor;
            ChangeSelectionData<DSActor>(actor);
            GraphView.RefreshNodes();
            this.actorFieldContainer.dataSource = this.data.Actor;
        }

        #endregion

        #region UTILS

        public VisualElement GetVisualElement()
        {
            return this;
        }

        private void ToggleVisibility(VisualElement element)
        {
            if (element.IsVisible())
            {
                element.SetVisible(false);
                return;
            }
            element.SetVisible(true);
        }

        private void ChangeSelectionData<T>(T item, bool isActor = true) where T : DSData
        {
            var selectionList = GraphView.selection.OfType<DSNode>().ToList();

            if (selectionList.Count > 1)
            {
                if (item is DSAudioClip clip)
                {
                    foreach (var node in selectionList)
                    {
                        node.Data.AudioClip = clip;
                    }
                    return;
                }

                if (item is DSActor actor && isActor)
                {
                    foreach (var node in selectionList)
                    {
                        node.Data.Actor = actor;
                    }
                    return;
                }

                if (item is DSActor conversant && !isActor)
                {
                    foreach (var node in selectionList)
                    {
                        node.Data.Conversant = conversant;
                    }
                    return;
                }

                if (item is DSDialogueText dialogue)
                {
                    foreach (var node in selectionList)
                    {
                        node.Data.Dialogue = dialogue;
                    }
                    return;
                }
            }
        }

        #endregion
    }
}
