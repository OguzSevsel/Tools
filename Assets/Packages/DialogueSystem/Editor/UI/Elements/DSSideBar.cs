using Tools.DialogueSystem.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSSideBar : VisualElement
    {
        private VisualElement mainContainer;
        private VisualElement labelContainer;
        private VisualElement inspectorContainer;
        private VisualElement conversationContainer;
        private VisualElement resizeElement;
        private VisualElement toolbar;

        private Button conversationsTabButton;
        private Button inspectorTabButton;
        
        private Label currentConversationLabel;

        public DSSideBar()
        {
            CreateUIElements();
        }

        #region UI Elements

        private void CreateUIElements()
        {
            AddSideBar();
            AddToolbar();
            AddStyles();
        }

        private void AddToolbar()
        {
            inspectorTabButton = DSElementUtility.CreateButton("Inspector", OnInspectorTabButtonClicked);
            conversationsTabButton = DSElementUtility.CreateButton("Conversations", OnConversationsTabButtonClicked);

            toolbar.Add(inspectorTabButton);
            toolbar.Add(conversationsTabButton);
        }

        private void AddSideBar()
        {
            inspectorContainer = new VisualElement();
            conversationContainer = new VisualElement();
            resizeElement = new VisualElement();
            toolbar = new VisualElement();
            mainContainer = new VisualElement();
            labelContainer = new VisualElement();
            currentConversationLabel = DSElementUtility.CreateLabel("Current Conversation: ");

            this.contentContainer.Add(resizeElement);
            this.contentContainer.Add(mainContainer);

            this.mainContainer.Add(toolbar);
            this.mainContainer.Add(labelContainer);
            this.mainContainer.Add(conversationContainer);
            this.mainContainer.Add(inspectorContainer);
            conversationContainer.SetVisible(false);
            inspectorContainer.SetVisible(false);
            this.labelContainer.Add(currentConversationLabel);
        }

        private void AddStyles()
        {
            resizeElement.SetWidth(10, 10, 10);
            resizeElement.SetFlex(flexGrow: 1);

            mainContainer.SetFlex(flexGrow: 1, flexWrap: Wrap.Wrap);
            mainContainer.SetBackgroundColor(Color.blue);
            mainContainer.SetAlignment(alignItems: Align.Stretch);

            toolbar.SetAlignment(alignItems: Align.Stretch, justifyContent: Justify.Center);
            toolbar.SetHeight(30, 30, 30);
            toolbar.SetFlex(flexGrow: 1, flexDirection: FlexDirection.Row, flexWrap: Wrap.NoWrap);

            inspectorTabButton.SetFlex(flexGrow: 1);
            conversationsTabButton.SetFlex(flexGrow: 1);

            labelContainer.SetPaddings(5);
            inspectorContainer.SetBackgroundColor(Color.beige);
            inspectorContainer.SetFlex(flexGrow: 1);
            inspectorContainer.SetPaddings(5);
            conversationContainer.SetBackgroundColor(Color.black);
            conversationContainer.SetFlex(flexGrow: 1);
            conversationContainer.SetPaddings(5);

            this.SetFlex(flexGrow: 0, flexShrink: 0, flexDirection: FlexDirection.Row);
            this.SetWidth(300, 600, 200);
            this.SetAlignment(alignItems: Align.Stretch);
            this.SetBackgroundColor(Color.red);
            this.SetPaddings(null, 0, 0, 10, 10);
            resizeElement.AddManipulator(new ResizeManipulator(this));
        }

        #endregion

        #region Events

        private void OnConversationsTabButtonClicked()
        {
            conversationContainer.SetVisible(true);
            inspectorContainer.SetVisible(false);
        }

        private void OnInspectorTabButtonClicked()
        {
            inspectorContainer.SetVisible(true);
            conversationContainer.SetVisible(false);
        }

        #endregion

        #region Utils

        public void AddToInspector(VisualElement element)
        {
            this.inspectorContainer.Add(element);
        }

        #endregion

        #region Logic

        //TODO: Conversation loading logic
        //TODO: Conversation deletion logic
        //TODO: Conversation creation logic

        #endregion
    }
}