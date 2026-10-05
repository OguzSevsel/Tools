using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using Unity.Properties;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSDialoguesTab : Tab
    {
        private Dictionary<DSDialogueText, DSDialogueElement> dialogues;

        private VisualTreeAsset cardAsset;

        private VisualElement dialogueCardContainer;
        private ScrollView dialogueScrollView;
        private Button createDialogueButton;

        public DSDialoguesTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            dialogues = new Dictionary<DSDialogueText, DSDialogueElement>();
            tabAsset.CloneTree(this);
            label = "Dialogues";
            this.cardAsset = cardAsset;

            createDialogueButton = this.Q<Button>("CreateDialogueButton");
            dialogueCardContainer = this.Q<VisualElement>("DialogueCardContainer");
            dialogueScrollView = this.Q<ScrollView>("DialogueScrollView");

            createDialogueButton.clicked += OnCreateDialogueButtonClicked;
        }

        private void OnCreateDialogueButtonClicked()
        {
            VisualElement cardElement = new VisualElement();
            cardAsset.CloneTree(cardElement);
            cardElement.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            cardElement.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            DSDialogueText textObject = new DSDialogueText("New Dialogue", "Dialogue Description", "Dialogue Text");
            cardElement.dataSource = textObject;
            dialogueCardContainer.Add(cardElement);
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            if (evt.currentTarget is VisualElement element)
            {
                element.SetDropShadow(true);
            }
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            if (evt.currentTarget is VisualElement element)
            {
                element.SetDropShadow();
            }
        }
    }
}