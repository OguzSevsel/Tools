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

            _ = contentContainer.SetFlex(flexDirection: FlexDirection.Row);
            _ = graphView.SetFlex(flexGrow: 1, flexShrink: 1);

            contentContainer.Add(graphView);
            contentContainer.Add(sideBar);
        }

        private void MiniMapButtonClickHandler() => graphView.ToggleMiniMap();
    }
}
