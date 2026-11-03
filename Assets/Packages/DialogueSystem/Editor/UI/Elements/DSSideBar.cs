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

        public DSSideBar(VisualTreeAsset sideBarAsset, VisualTreeAsset conversationsTabAsset, VisualTreeAsset conversationsCardAsset)
        {
            sideBarAsset.CloneTree(this);
            this.style.width = Length.Percent(25);
            resizeElement = this.Q<VisualElement>("ResizeElement");

            InspectorTab = this.Q<Tab>("InspectorTab");

            titleLabel = this.Q<Label>("CurrentConversationLabel");
            tabControl = this.Q<TabView>("TabControl");

            ConversationsTab = new DSConversationsTab(conversationsTabAsset, conversationsCardAsset);
            resizeElement.AddManipulator(new ResizeManipulator(this, 25, 40));

            tabControl.Add(ConversationsTab);
            tabControl.activeTab = ConversationsTab;
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


