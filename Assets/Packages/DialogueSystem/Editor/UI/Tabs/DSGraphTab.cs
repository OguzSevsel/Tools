using Tools.DialogueSystem.UI.Elements;
using Tools.DialogueSystem.Utilities;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSGraphTab : Tab
    {
        DSGraphView graphView;
        VisualElement graphContainer;
        VisualElement sideBarContainer;

        public DSSideBar sideBar;
        Button miniMapButton;

        public DSGraphTab(VisualTreeAsset conversationsTabAsset, VisualTreeAsset sideBarAsset)
        {
            sideBar = new DSSideBar(sideBarAsset, conversationsTabAsset);
            label = "Conversations";

            graphView = new DSGraphView(this);

            this.contentContainer.SetFlex(flexDirection: FlexDirection.Row);
            graphView.SetFlex(flexGrow: 1, flexShrink: 1);

            this.contentContainer.Add(graphView);
            this.contentContainer.Add(sideBar);
        }

        private void MiniMapButtonClickHandler()
        {
            graphView.ToggleMiniMap();
        }
    }
}
