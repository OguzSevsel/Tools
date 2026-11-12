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

        public DSGraphTab(VisualTreeAsset conversationsTabAsset, VisualTreeAsset conversationsCardAsset, VisualTreeAsset sideBarAsset, VisualTreeAsset nodeInspectorAsset)
        {
            sideBar = new DSSideBar(sideBarAsset, conversationsTabAsset, conversationsCardAsset);
            DSSideBar.OnSideBarToggled += HandleOnSideBarToggled;
            label = "Conversations";

            graphView = new DSGraphView(this, nodeInspectorAsset);
            
            graphView.OnInspectorOpened += HandleInspectorOpened;
            graphView.OnInspectorClosed += HandleInspectorClosed;
            graphView.OnSideBarOpened += HandleSideBar;

            this.contentContainer.SetFlex(flexDirection: FlexDirection.Row);
            graphView.SetFlex(flexGrow: 1, flexShrink: 1);

            this.contentContainer.Add(graphView);
            this.contentContainer.Add(sideBar);
            DSConversationsTab.OnConversationOpened += HandleOnConversationOpen;
            DSEditorView.OnToggleSideBarShortcut += HandleSideBar;
        }
        public void RefreshNodes()
        {
            graphView.RefreshNodes();
        }

        public void SelectAll()
        {
            graphView.SelectAll();
        }

        private void HandleOnSideBarToggled(bool isVisible)
        {
            if (isVisible)
            {
                graphView.OpenSideBarButton.SetVisible(false);
            }
            else
            {
                graphView.OpenSideBarButton.SetVisible(true);
            }
        }

        private void HandleSideBar()
        {
            sideBar.ToggleSideBar();
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
