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
        private Button createNewActorButton;
        private Label actorCountLabel;
        public Dictionary<DSActor, DSActorElement> Actors { get; set; }

        public DSActorsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            tabAsset.CloneTree(this);
            label = "Actors";
            CreateUIElements();
            RegisterEvents();
            AddClasses();
            Actors = new Dictionary<DSActor, DSActorElement>();
            DSDatabaseManager.DatabaseOpened += OnDatabaseOpened;
            DSDatabaseManager.DatabaseClosed += OnDatabaseClosed;
        }

        private void OnDatabaseOpened(DSDatabase database)
        {
            foreach (var actor in DSDatabaseManager.Current.Actors)
            {
                CreateActorUIElement(actor);
            }
        }

        private void OnDatabaseClosed(DSDatabase database)
        {
            foreach (var actor in DSDatabaseManager.Current.Actors)
            {
                if (Actors.ContainsKey(actor))
                {
                    DSActorElement actorElement = Actors[actor];
                    if (actorGrid.Contains(actorElement))
                    {
                        actorGrid.Remove(actorElement);
                        Actors.Remove(actor);
                    }
                }
            }
        }

        private void CreateActorUIElement(DSActor newActor)
        {
            DSActorElement element = new DSActorElement(newActor);
            element.OnActorDeleted += DeleteActor;
            
            this.actorGrid.Insert(0, element);
            this.Actors.Add(newActor, element);
        }

        private void CreateActor()
        {
            DSActor newActor = new DSActor("", "", "", null);
            CreateActorUIElement(newActor);
            DSDatabaseManager.Current.Register(newActor);
        }

        private void DeleteActor(DSActor actor)
        {
            DSActorElement element = Actors[actor];
            this.actorGrid.Remove(element);
            Actors.Remove(actor);
            element.OnActorDeleted -= DeleteActor;
            DSDatabaseManager.Current.Unregister(actor);
        }

        private void CreateUIElements()
        {
            actorScrollView = new ScrollView
            {
                mode = ScrollViewMode.Vertical,
                verticalScrollerVisibility = ScrollerVisibility.Hidden,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };
            actorGrid = new VisualElement();
            toolbar = new VisualElement();
            createNewActorButton = DSElementUtility.CreateButton("create new actor", onClick: CreateActor);

            actorScrollView.Add(actorGrid);
            this.toolbar.Insert(0, createNewActorButton);
            this.contentContainer.Insert(0, toolbar);
            contentContainer.Insert(1, actorScrollView);
        }

        private void RegisterEvents()
        {
            RegisterCallback<AttachToPanelEvent>(OnAttached);
            RegisterCallback<DetachFromPanelEvent>(OnDetached);
        }

        private void AddClasses()
        {
            this.AddStyleSheets("DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSActorStyles.uss", "DialogueSystem/DSGeneralStyles.uss");
            toolbar.AddClasses("flex-grow-shrink-0", "flex-direction-column", "height-25");
            this.style.flexGrow = 1;
            this.style.position = Position.Relative;
            actorGrid.style.flexDirection = FlexDirection.Row;
            actorGrid.style.flexWrap = Wrap.Wrap;
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

        private void OnDetached(DetachFromPanelEvent evt)
        {
            DSDatabaseManager.DatabaseOpened -= OnDatabaseOpened;
            DSDatabaseManager.DatabaseClosed -= OnDatabaseClosed;
        }

        private void OnAttached(AttachToPanelEvent evt)
        {

        }
    }
}
