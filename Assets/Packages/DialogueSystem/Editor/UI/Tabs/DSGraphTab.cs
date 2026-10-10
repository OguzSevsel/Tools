using Tools.DialogueSystem.UI.Elements;
using Tools.DialogueSystem.Utilities;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSGraphTab : Tab
    {
        private DSGraphView graphView;
        private VisualElement graphContainer;
        private VisualElement sideBarContainer;

        public DSSideBar sideBar;
        private Button miniMapButton;

        public DSGraphTab(VisualTreeAsset conversationsTabAsset, VisualTreeAsset conversationsCardAsset, VisualTreeAsset sideBarAsset)
        {
            sideBar = new DSSideBar(sideBarAsset, conversationsTabAsset, conversationsCardAsset);
            label = "Conversations";

            graphView = new DSGraphView(this);

            this.contentContainer.SetFlex(flexDirection: FlexDirection.Row);
            graphView.SetFlex(flexGrow: 1, flexShrink: 1);

            this.contentContainer.Add(graphView);
            this.contentContainer.Add(sideBar);
        }

        private void MiniMapButtonClickHandler() => graphView.ToggleMiniMap();
    }
}
