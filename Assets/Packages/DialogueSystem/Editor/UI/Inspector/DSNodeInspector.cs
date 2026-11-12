using System;
using System.Linq;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI.Inspector;
using Tools.DialogueSystem.Utilities;
using Unity.Plastic.Antlr3.Runtime;
using UnityEditor.Search;
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

        private Label titleLabel;
        private TextField nodeTitleTextField;

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
            Create();
        }

        public void LoadFields(DSNodeData data)
        {
            this.data = data;
            this.dataSource = this.data;

            if (this.data.IsStartNode)
            {
                nodeTitleTextField.SetVisible(false);
                titleLabel.text = "Start Node";
            }
            else
            {
                nodeTitleTextField.SetVisible(true);
                titleLabel.text = "Node Title";
            }

            DSDatabase database = DSDatabaseManager.Instance.Current;

            actorDropdown.SetItems(database.Actors);
            conversantDropdown.SetItems(database.Actors);
            audioDropdown.SetItems(database.AudioClips);
            dialogueDropdown.SetItems(database.DialogueTexts);

            actorDropdown.value = database.Get<DSActor>(data.ActorId);
            conversantDropdown.value = database.Get<DSActor>(data.ConversantId);
            audioDropdown.value = database.Get<DSAudioClip>(data.AudioClipId);
            dialogueDropdown.value = database.Get<DSDialogueText>(data.DialogueId);

            this.actorFieldContainer.dataSource = database.Get<DSActor>(data.ActorId);
            this.conversantFieldContainer.dataSource = database.Get<DSActor>(data.ConversantId);
            this.audioClipFieldContainer.dataSource = database.Get<DSAudioClip>(data.AudioClipId);
            this.dialogueFieldContainer.dataSource = database.Get<DSDialogueText>(data.DialogueId);
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
            actorContainer = this.Q<VisualElement>("ActorDropdownContainer");
            conversantContainer = this.Q<VisualElement>("ConversantDropdownContainer");
            audioClipContainer = this.Q<VisualElement>("AudioClipDropdownContainer");
            dialogueContainer = this.Q<VisualElement>("DialogueDropdownContainer");

            titleLabel = this.Q<Label>("NodeTitleLabel");
            nodeTitleTextField = this.Q<TextField>("NodeTitleTextField");

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

            FieldVisible(actorFieldContainer, actorButton, false);
            FieldVisible(conversantFieldContainer, conversantButton, false);
            FieldVisible(audioClipFieldContainer, audioClipButton, false);
            FieldVisible(dialogueFieldContainer, dialogueButton, false);
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
            ToggleFieldVisibility(dialogueFieldContainer, dialogueButton);
        }

        private void HandleAudioClipButtonClicked()
        {
            ToggleFieldVisibility(audioClipFieldContainer, audioClipButton);
        }

        private void HandleConversantButtonClicked()
        {
            ToggleFieldVisibility(conversantFieldContainer, conversantButton);
        }

        private void HandleActorButtonClicked()
        {
            ToggleFieldVisibility(actorFieldContainer, actorButton);
        }

        private void HandleOnDialogueValueChanged(DSDialogueText dialogue)
        {
            this.data.SetDialogueId(dialogue.Guid);
            ChangeSelectionData<DSDialogueText>(dialogue);
            this.dialogueFieldContainer.dataSource = DSDatabaseManager.Instance.Current.Get<DSDialogueText>(this.data.DialogueId);
            GraphView.RefreshNodes();
        }

        private void HandleOnAudioValueChanged(DSAudioClip clip)
        {
            this.data.SetAudioClipId(clip.Guid);
            ChangeSelectionData<DSAudioClip>(clip);
            this.audioClipFieldContainer.dataSource = DSDatabaseManager.Instance.Current.Get<DSAudioClip>(this.data.AudioClipId);
            GraphView.RefreshNodes();
        }

        private void HandleOnConversantValueChanged(DSActor conversant)
        {
            this.data.SetConversantId(conversant.Guid);
            ChangeSelectionData<DSActor>(conversant, false);
            this.conversantFieldContainer.dataSource = DSDatabaseManager.Instance.Current.Get<DSActor>(this.data.ConversantId);
            GraphView.RefreshNodes();
        }

        private void HandleOnActorValueChanged(DSActor actor)
        {
            this.data.SetActorId(actor.Guid);
            ChangeSelectionData<DSActor>(actor);
            this.actorFieldContainer.dataSource = DSDatabaseManager.Instance.Current.Get<DSActor>(this.data.ActorId);
            GraphView.RefreshNodes();
        }

        #endregion

        #region UTILS

        public VisualElement GetVisualElement()
        {
            return this;
        }

        private void FieldVisible(VisualElement element, Button button, bool isVisible = true)
        {
            element.SetVisible(isVisible);

            if (isVisible)
            {
                button.Rotate(90);
                return;
            }
            button.Rotate(0);
        }

        private void ToggleFieldVisibility(VisualElement element, Button button)
        {
            if (element.IsVisible())
            {
                element.SetVisible(false);
                button.Rotate(90);
                return;
            }
            element.SetVisible(true);
            button.Rotate(0);
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
                        node.Data.SetAudioClipId(clip.Guid);
                    }
                    return;
                }

                if (item is DSActor actor && isActor)
                {
                    foreach (var node in selectionList)
                    {
                        node.Data.SetActorId(actor.Guid);
                    }
                    return;
                }

                if (item is DSActor conversant && !isActor)
                {
                    foreach (var node in selectionList)
                    {
                        node.Data.SetConversantId(conversant.Guid);
                    }
                    return;
                }

                if (item is DSDialogueText dialogue)
                {
                    foreach (var node in selectionList)
                    {
                        node.Data.SetDialogueId(dialogue.Guid);
                    }
                    return;
                }
            }
        }

        #endregion
    }
}
