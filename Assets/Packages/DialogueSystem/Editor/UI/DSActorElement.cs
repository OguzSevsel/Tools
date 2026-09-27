using System;
using System.Runtime.CompilerServices;
using Tools.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSActorElement : VisualElement
    {
        private TextField actorName;
        private TextField actorBackground;
        private ObjectField actorSprite;
        private Button deleteButton;
        public event Action OnActorDeleted;

        public DSActorElement()
        {
            CreateUIElements();
            RegisterEvents();
            AddClasses();
            
        }

        private void RegisterEvents()
        {
            //RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            //RegisterCallback<MouseDownEvent>(OnMouseDown);
            //RegisterCallback<MouseOutEvent>(OnMouseExit);
        }

        private void CreateUIElements()
        {
            VisualElement element = new VisualElement();
            element.style.flexGrow = 1;
            element.style.flexDirection = FlexDirection.Row;
            actorName = DSElementUtility.CreateTextField("", "Actor Name", false, onValueChanged: OnActorNameChanged);
            deleteButton = DSElementUtility.CreateButton("", OnDeleteButtonClicked);
            Texture2D icon = EditorGUIUtility.Load("Icons/delete.png") as Texture2D;
            deleteButton.style.backgroundImage = icon;
            actorBackground = DSElementUtility.CreateTextField("", "Actor Background", true, onValueChanged: OnActorBackgroundChanged);
            actorBackground.verticalScrollerVisibility = ScrollerVisibility.Auto;
            actorBackground.style.textOverflow = TextOverflow.Clip;
            actorSprite = DSElementUtility.CreateObjectField("Actor Sprite", typeof(Sprite), "Actor Sprite", onValueChanged: OnActorSpriteChanged);
            element.Add(actorName);
            element.Add(deleteButton);
            this.contentContainer.Add(element);
            this.contentContainer.Add(actorBackground);
            this.contentContainer.Add(actorSprite);
        }

        private void OnDeleteButtonClicked()
        {
            this.OnActorDeleted?.Invoke();
        }

        private void AddClasses()
        {
            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss");
            this.contentContainer.AddToClassList("ds-actor-data-container");
            actorName.AddClasses("ds-actor__text-field");
            actorBackground.AddClasses("ds-actorbg__text-field");
            actorSprite.AddClasses("ds-actor__text-field");
        }

        private void OnActorSpriteChanged(ChangeEvent<UnityEngine.Object> evt)
        {

        }

        private void OnActorBackgroundChanged(ChangeEvent<string> evt)
        {

        }

        private void OnActorNameChanged(ChangeEvent<string> evt)
        {

        }

        public void OnMouseDown(MouseDownEvent eventData)
        {
            Debug.Log("Actor Clicked");
        }

        public void OnMouseEnter(MouseEnterEvent eventData)
        {
            Debug.Log("Actor Pointer Enter");
        }

        public void OnMouseExit(MouseOutEvent eventData)
        {
            Debug.Log("Actor Pointer Exit");
        }
    }
}