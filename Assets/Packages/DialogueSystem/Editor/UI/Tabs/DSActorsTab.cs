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
        private VisualTreeAsset actorCardAsset;
        private Button createNewActorButton;
        public Dictionary<DSActor, DSActorElement> Actors { get; set; }

        public DSActorsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            tabAsset.CloneTree(this);
            label = "Actors";
            Actors = new Dictionary<DSActor, DSActorElement>();
            actorCardAsset = cardAsset;

            actorScrollView = this.Q<ScrollView>("ActorsScrollView");
            actorGrid = this.Q<VisualElement>("ActorsCardContainer");
            createNewActorButton = this.Q<Button>("CreateActorButton");
            createNewActorButton.clicked += OnCreateNewActorButtonClicked;
        }

        private void OnCreateNewActorButtonClicked()
        {
            VisualElement cardElement = new VisualElement();
            actorCardAsset.CloneTree(cardElement);
            cardElement.RegisterCallback<MouseEnterEvent>(OnMouseEnterToActorCard);
            cardElement.RegisterCallback<MouseLeaveEvent>(OnMouseExitFromActorCard);
            actorGrid.Add(cardElement);
        }

        private void OnMouseExitFromActorCard(MouseLeaveEvent evt)
        {
            if (evt.currentTarget is VisualElement element)
            {
                element.SetDropShadow(true);
            }
        }

        private void OnMouseEnterToActorCard(MouseEnterEvent evt)
        {
            if (evt.currentTarget is VisualElement element)
            {
                element.SetDropShadow();
            }
        }
    }
}
