using System;
using Tools.DialogueSystem.UI.Inspector;
using Tools.DialogueSystem.Utilities;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSSideBar : VisualElement
    {
        private VisualElement resizeElement;
        private Label titleLabel;
        private TabView tabControl;
        private Tab InspectorTab;
        private DSConversationsTab ConversationsTab;
        private Button CloseButton;

        public static event Action<bool> OnSideBarToggled;

        private bool isVisible;

        public DSSideBar(VisualTreeAsset sideBarAsset, VisualTreeAsset conversationsTabAsset, VisualTreeAsset conversationsCardAsset)
        {
            sideBarAsset.CloneTree(this);
            this.style.width = Length.Percent(25);
            resizeElement = this.Q<VisualElement>("ResizeElement");

            InspectorTab = this.Q<Tab>("InspectorTab");

            titleLabel = this.Q<Label>("CurrentConversationLabel");
            tabControl = this.Q<TabView>("TabControl");
            CloseButton = this.Q<Button>("CloseButton");

            CloseButton.clicked += ToggleSideBar;

            ConversationsTab = new DSConversationsTab(conversationsTabAsset, conversationsCardAsset);
            resizeElement.AddManipulator(new ResizeManipulator(this, 20, 30));

            tabControl.Add(ConversationsTab);
            tabControl.activeTab = ConversationsTab;
            isVisible = false;
            ToggleSideBar();
        }

        public void ToggleSideBar()
        {
            this.SetVisible(isVisible);
            OnSideBarToggled?.Invoke(isVisible);
            isVisible = !isVisible;
        }

        public void AddInspector(VisualElement inspector)
        {
            InspectorTab.Add(inspector);
        }

        public void ChangeActiveTab(bool isConversations = true)
        {
            if (isConversations)
            {
                tabControl.activeTab = ConversationsTab;
                return;
            }

            tabControl.activeTab = InspectorTab;
        }

        public void ChangeTitleLabel(string value)
        {
            titleLabel.text = value;
        }
    }
}


