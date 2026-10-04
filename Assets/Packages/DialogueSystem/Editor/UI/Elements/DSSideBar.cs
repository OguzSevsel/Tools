using Tools.DialogueSystem.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSSideBar : VisualElement
    {
        private VisualElement resizeElement;
        private Label currentConversationLabel;
        private TabView tabControl;
        private Tab InspectorTab;
        private Tab ConversationsTab;

        public DSSideBar(VisualTreeAsset sideBarAsset, VisualTreeAsset conversationsTabAsset)
        {
            sideBarAsset.CloneTree(this);
            this.style.width = Length.Percent(25);
            resizeElement = this.Q<VisualElement>("ResizeElement");
            currentConversationLabel = this.Q<Label>("InspectorLabel");
            tabControl = this.Q<TabView>("SideBarTabView");
            InspectorTab = this.Q<Tab>("InspectorTab");
            ConversationsTab = this.Q<Tab>("ConversationsSideBarTab");
            conversationsTabAsset.CloneTree(ConversationsTab);
            resizeElement.AddManipulator(new ResizeManipulator(this, 15, 35));
        }
    }
}