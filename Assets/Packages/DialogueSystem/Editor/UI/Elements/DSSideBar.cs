using Tools.DialogueSystem.Utilities;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSSideBar : VisualElement
    {
        private VisualElement resizeElement;
        private Label currentConversationLabel;
        private TabView tabControl;
        private Tab InspectorTab;
        private DSConversationsTab ConversationsTab;

        public DSSideBar(VisualTreeAsset sideBarAsset, VisualTreeAsset conversationsTabAsset, VisualTreeAsset conversationsCardAsset)
        {
            sideBarAsset.CloneTree(this);
            this.style.width = Length.Percent(25);
            resizeElement = this.Q<VisualElement>("ResizeElement");

            currentConversationLabel = this.Q<Label>("CurrentConversationLabel");
            tabControl = this.Q<TabView>("TabControl");

            ConversationsTab = new DSConversationsTab(conversationsTabAsset, conversationsCardAsset);
            resizeElement.AddManipulator(new ResizeManipulator(this, 15, 35));

            tabControl.Add(ConversationsTab);
            tabControl.activeTab = ConversationsTab;
        }
    }
}