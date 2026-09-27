using System;
using System.Runtime.CompilerServices;
using Tools.DialogueSystem.Utilities;
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

        public DSActorElement()
        {
            actorName = DSElementUtility.CreateTextField("", "Actor Name", false, onValueChanged: OnActorNameChanged);
            actorBackground = DSElementUtility.CreateTextField("", "Actor Background", true, onValueChanged: OnActorBackgroundChanged);
            actorSprite = DSElementUtility.CreateObjectField("Actor Sprite", typeof(Sprite), "Actor Sprite", onValueChanged: OnActorSpriteChanged);

            this.contentContainer.Add(actorName);
            this.contentContainer.Add(actorBackground);
            this.contentContainer.Add(actorSprite);
            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseOutEvent>(OnMouseExit);

            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss");

            this.contentContainer.AddToClassList("ds-actor-data-container");

            actorName.AddClasses("ds-actor__text-field");
            actorBackground.AddClasses("ds-actorbg__text-field");
            actorBackground.verticalScrollerVisibility = ScrollerVisibility.Auto;
            actorBackground.style.textOverflow = TextOverflow.Clip;
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