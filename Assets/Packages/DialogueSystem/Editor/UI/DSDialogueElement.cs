using System;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSDialogueElement : VisualElement
    {
        private TextField dialogueTextField;
        private Button deleteButton;
        private VisualElement titleContainer;
        private DSDialogueText dialogueText;
        public event Action<DSDialogueText> OnDialogueDelete;

        public DSDialogueElement(DSDialogueText dialogueText)
        {
            this.dialogueText = dialogueText;
            CreateUIElements();
            AddClasses();
            LoadFields();
        }

        private void LoadFields()
        {
            dialogueTextField.value = dialogueText.Text;
        }

        private void CreateUIElements()
        {
            titleContainer = new VisualElement();
            titleContainer.style.flexGrow = 1;
            titleContainer.style.flexDirection = FlexDirection.Row;

            deleteButton = DSElementUtility.CreateButton("", OnDialogueDeleteButtonClicked);
            Texture2D icon = EditorGUIUtility.Load("Icons/delete.png") as Texture2D;
            deleteButton.style.backgroundImage = icon;

            dialogueTextField = DSElementUtility.CreateTextField("", "Dialogue", true, OnDialogueTextChanged);

            this.titleContainer.Add(dialogueTextField);
            this.titleContainer.Add(deleteButton);
            this.contentContainer.Add(titleContainer);
        }

        private void OnDialogueTextChanged(ChangeEvent<string> evt)
        {
            dialogueText.Text = evt.newValue;
        }

        private void OnDialogueDeleteButtonClicked()
        {
            OnDialogueDelete?.Invoke(dialogueText);
        }

        private void AddClasses()
        {
            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSDialogueTextStyles.uss", "DialogueSystem/DSGeneralStyles.uss");
            this.contentContainer.AddToClassList("ds-dialogue-data-container");
            this.titleContainer.AddToClassList("ds-dialoguetitle-data-container");
            deleteButton.AddClasses("ds-dialogue_deleteButton-field");
            dialogueTextField.AddToClassList("ds-dialogue__text-field");
        }
    }
}