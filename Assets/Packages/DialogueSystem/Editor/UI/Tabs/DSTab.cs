using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools
{
    public class DSTab : Tab
    {
        private ScrollView actorScrollView;
        private VisualElement actorContainer;
        private VisualElement buttonContainer;
        private Button deleteAllButton;
        private Button selectAllButton;

        private VisualTreeAsset cardAsset;
        private Button createActorButton;
        public Dictionary<DSActor, VisualElement> Actors { get; set; }
        public List<DSActor> selectedActors { get; }

        public DSTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            
        }

        private void GetUIElements()
        {
            actorScrollView = this.Q<ScrollView>("ActorsScrollView");
            actorContainer = this.Q<VisualElement>("ActorsCardContainer");
            createActorButton = this.Q<Button>("CreateActorButton");
            buttonContainer = this.Q<VisualElement>("ButtonContainer");
            deleteAllButton = this.Q<Button>("DeleteSelectedButton");
            selectAllButton = this.Q<Button>("SelectAllButton");
        }
    }
}
