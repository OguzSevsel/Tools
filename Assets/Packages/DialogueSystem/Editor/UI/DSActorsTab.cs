using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.UI;
using Tools.DialogueSystem.Utilities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem
{
    public class DSActorsTab : VisualElement
    {
        private ScrollView actorScrollView;
        private VisualElement actorGrid;
        private VisualElement toolbar;
        private Button createNewActorButton;
        private Button deleteActorButton;

        public Dictionary<DSActorElement, DSActor> Actors { get; set; }
        private DSActor selectedActor { get; set; }

        public DSActorsTab()
        {
            this.style.flexGrow = 1;
            this.style.position = Position.Relative;

            actorScrollView = new ScrollView
            {
                mode = ScrollViewMode.Vertical,
                verticalScrollerVisibility = ScrollerVisibility.Hidden,
                horizontalScrollerVisibility = ScrollerVisibility.Hidden
            };

            actorGrid = new VisualElement();
            actorGrid.style.flexDirection = FlexDirection.Row;
            actorGrid.style.flexWrap = Wrap.Wrap;

            actorScrollView.Add(actorGrid);

            toolbar = new VisualElement();
            toolbar.AddClasses("flex-grow-shrink-0", "flex-direction-column", "height-25");

            createNewActorButton = DSElementUtility.CreateButton("create new actor", onClick: CreateActor);
            this.toolbar.Insert(0, createNewActorButton);
            this.contentContainer.Insert(0, toolbar);
            contentContainer.Insert(1, actorScrollView);
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
        private void CreateActor()
        {
            DSActorElement element = new DSActorElement();
            this.actorGrid.Insert(0,element);
        }

        private void DeleteActor()
        {

        }

        private void ChangeActor()
        {

        }
    }
}
