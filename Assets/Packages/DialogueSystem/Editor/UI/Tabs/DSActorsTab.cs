using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI;
using Tools.DialogueSystem.Utilities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem
{
    public class DSActorsTab : Tab
    {
        private ScrollView actorScrollView;
        private VisualElement actorGrid;
        private VisualElement toolbar;
        private VisualElement actorCard;
        private VisualTreeAsset cardAsset;
        private Button createNewActorButton;
        public Dictionary<DSActor, DSActorElement> Actors { get; set; }

        public DSActorsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            tabAsset.CloneTree(this);
            label = "Actors";
            Actors = new Dictionary<DSActor, DSActorElement>();
            this.cardAsset = cardAsset;

            actorScrollView = this.Q<ScrollView>("ActorsScrollView");
            actorGrid = this.Q<VisualElement>("ActorsCardContainer");
            createNewActorButton = this.Q<Button>("CreateActorButton");
            createNewActorButton.clicked += OnCreateNewActorButtonClicked;
        }

        private void OnCreateNewActorButtonClicked()
        {
            VisualElement cardElement = new VisualElement();
            cardAsset.CloneTree(cardElement);
            cardElement.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            cardElement.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);

            DSActor actor = new DSActor("New Actor", "Actor Description", "Actor Background", default);
            cardElement.dataSource = actor;
            actorGrid.Add(cardElement);
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
