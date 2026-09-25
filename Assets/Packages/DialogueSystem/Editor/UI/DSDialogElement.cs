using System;
using Tools.DialogueSystem.Utilities;
using UnityEngine;
using UnityEngine.UIElements;


namespace Tools.DialogueSystem.UI
{
    public class DSDialogElement
    {
        private readonly VisualElement root;
        private readonly Label title;
        private readonly Label message;
        private readonly Button confirmButton;
        private readonly Button cancelButton;

        public DSDialogElement(VisualElement root)
        {
            this.root = root;

            root.style.position = Position.Absolute;
            root.style.display = DisplayStyle.Flex;
            root.style.width = 200;
            root.style.height = 300;
            root.style.alignSelf = Align.Center;
            root.style.justifyContent = Justify.Center;
            root.style.backgroundColor = Color.firebrick;
            root.style.flexGrow = 1;

            title = DSElementUtility.CreateLabel("title");
            message = DSElementUtility.CreateLabel("message");

            confirmButton = DSElementUtility.CreateButton("confirm button");
            cancelButton = DSElementUtility.CreateButton("cancel button");

            confirmButton.style.flexGrow = 1;
            cancelButton.style.flexGrow = 1;
            cancelButton.style.fontSize = 15;
            confirmButton.style.fontSize = 15;

            root.Add(title);
            root.Add(message);
            root.Add(confirmButton);
            root.Add(cancelButton);

            root.BringToFront();

            Hide();
        }

        public void Show(
            string titleText,
            string messageText,
            string confirmText,
            string cancelText,
            Action onConfirm,
            Action onCancel = null)
        {
            title.text = titleText;
            message.text = messageText;

            confirmButton.text = confirmText;
            cancelButton.text = cancelText;

            confirmButton.clicked += Confirm;
            cancelButton.clicked += Cancel;

            root.style.display = DisplayStyle.Flex;

            void Confirm()
            {
                Cleanup();
                onConfirm?.Invoke();
            }

            void Cancel()
            {
                Cleanup();
                onCancel?.Invoke();
            }

            void Cleanup()
            {
                confirmButton.clicked -= Confirm;
                cancelButton.clicked -= Cancel;

                Hide();
            }
        }

        public void Hide()
        {
            root.style.display = DisplayStyle.None;
        }
    } 
}