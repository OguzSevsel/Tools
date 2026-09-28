using System;
using System.Runtime.CompilerServices;
using Tools.DialogueSystem.Data;
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
        #region Fields

        private TextField actorName;
        private TextField actorBackground;
        private ObjectField actorSprite;
        private Button deleteButton;
        private VisualElement titleContainer;
        private Label title;
        private DSActor actor;
        public event Action<DSActor> OnActorDeleted;

        #endregion

        #region Initialization

        public DSActorElement(DSActor actor)
        {
            this.actor = actor;
            CreateUIElements();
            RegisterEvents();
            AddClasses();
            LoadFields();
        }

        private void CreateUIElements()
        {
            titleContainer = new VisualElement();
            title = DSElementUtility.CreateLabel("");
            deleteButton = DSElementUtility.CreateButton("", OnDeleteButtonClicked);
            actorName = DSElementUtility.CreateTextField("", "Actor Name", false, onValueChanged: OnActorNameChanged);
            actorBackground = DSElementUtility.CreateTextField("", "Actor Background", true, OnActorBackgroundChanged);
            actorSprite = DSElementUtility.CreateObjectField("Actor Sprite", typeof(Sprite), "Actor Sprite", OnActorSpriteChanged);

            Texture2D icon = EditorGUIUtility.Load("Icons/delete.png") as Texture2D;
            deleteButton.style.backgroundImage = icon;
            actorBackground.verticalScrollerVisibility = ScrollerVisibility.Auto;
            titleContainer.style.flexDirection = FlexDirection.Row;

            titleContainer.Add(title);
            titleContainer.Add(deleteButton);

            this.contentContainer.Add(titleContainer);
            this.contentContainer.Add(actorName);
            this.contentContainer.Add(actorBackground);
            this.contentContainer.Add(actorSprite);
        }

        private void AddClasses()
        {
            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSActorStyles.uss", "DialogueSystem/DSGeneralStyles.uss");
            this.contentContainer.AddToClassList("ds-actor-data-container");
            this.titleContainer.AddToClassList("ds-actortitle-data-container");
            deleteButton.AddClasses("ds-actor_deleteButton-field");
            title.AddToClassList("ds-actortitle__text-field");
            actorName.AddClasses("ds-actor__text-field");
            actorBackground.AddClasses("ds-actorbg__text-field");
            actorSprite.AddClasses("ds-actor__text-field");
        }

        private void RegisterEvents()
        {
            //RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            //RegisterCallback<MouseDownEvent>(OnMouseDown);
            //RegisterCallback<MouseOutEvent>(OnMouseExit);
        }

        #endregion

        #region Events

        private void OnDeleteButtonClicked()
        {
            this.OnActorDeleted?.Invoke(actor);
        }

        private void OnActorSpriteChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            actor.Sprite = evt.newValue as Sprite;
        }

        private void OnActorBackgroundChanged(ChangeEvent<string> evt)
        {
            actor.Background = evt.newValue;
        }

        private void OnActorNameChanged(ChangeEvent<string> evt)
        {
            title.text = evt.newValue;
            actor.Name = evt.newValue;
        }

        #endregion

        #region Utils

        private void LoadFields()
        {
            actorName.value = actor.Name;
            actorSprite.value = actor.Sprite;
            actorBackground.value = actor.Background;
        }

        private void AddBorder(VisualElement element, Color color = default)
        {
            if (color == default)
            {
                color = Color.black;
            }

            element.style.borderRightWidth = 4;
            element.style.borderLeftWidth = 4;
            element.style.borderTopWidth = 4;
            element.style.borderBottomWidth = 4;
            element.style.borderRightColor = color;
            element.style.borderLeftColor = color;
            element.style.borderBottomColor = color;
            element.style.borderTopColor = color;
        }

        #endregion
    }
}