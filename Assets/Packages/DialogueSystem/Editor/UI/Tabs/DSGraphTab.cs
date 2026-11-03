using System;
using Tools.DialogueSystem.Data;
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
            graphView.OnInspectorOpened += HandleInspectorOpened;
            graphView.OnInspectorClosed += HandleInspectorClosed;
            this.contentContainer.SetFlex(flexDirection: FlexDirection.Row);
            graphView.SetFlex(flexGrow: 1, flexShrink: 1);

            this.contentContainer.Add(graphView);
            this.contentContainer.Add(sideBar);
            DSConversationsTab.OnConversationOpened += HandleOnConversationOpen;
        }

        private void HandleInspectorClosed()
        {
            sideBar.ChangeActiveTab(true);
        }

        private void HandleInspectorOpened(VisualElement inspector, DSNode node)
        {
            sideBar.AddInspector(inspector);
            sideBar.ChangeActiveTab(false);
        }

        private void HandleOnConversationOpen(Data.DSConversation conversation)
        {
            graphView.Load(conversation);
            sideBar.ChangeTitleLabel(conversation.Name);
        }

        private void MiniMapButtonClickHandler() => graphView.ToggleMiniMap();
    }
}
