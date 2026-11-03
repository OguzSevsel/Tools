using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using Unity.Properties;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSNode : Node
    {
        public Port InputPort { get; set; }
        public Port OutputPort { get; set; }
        public DSNodeData Data = null;
        public event Action<DSNode> OnNodeSelect;
        public event Action<Edge> OnEdgeDeleted;

        private VisualElement portContainer;
        private VisualElement infoContainer;
        private Label actorLabel;
        private Label conversantLabel;
        private Label dialogueTextLabel;

        public virtual void Initialize(DSNodeData data)
        {
            this.Data = data;
            Data.IsStartNode = data.IsStartNode;

            SetPosition(new Rect(data.Position, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
            Draw();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelect?.Invoke(this);
        }

        public virtual void Draw()
        {
            portContainer = new VisualElement();
            infoContainer = new VisualElement();

            infoContainer.SetFlex(flexDirection: FlexDirection.Column);
            infoContainer.SetAlignment(alignContent: Align.Center, alignItems: Align.Center);

            portContainer.style.flexDirection = FlexDirection.Column;
            portContainer.style.alignItems = Align.Center;

            mainContainer.Insert(0, portContainer);

            if (!Data.IsStartNode)
                InputPort = CreatePort(portContainer, Direction.Input);

            Label label = DSElementUtility.CreateLabel("Node");

            titleContainer.Clear();
            titleContainer.Insert(0, label);

            actorLabel = DSElementUtility.CreateLabel(Data.Actor.Name);
            conversantLabel = DSElementUtility.CreateLabel(Data.Conversant.Name);
            dialogueTextLabel = DSElementUtility.CreateLabel(Data.Dialogue.Text);

            infoContainer.Add(actorLabel);
            infoContainer.Add(conversantLabel);
            infoContainer.Add(dialogueTextLabel);

            actorLabel.SetVisible(false);
            conversantLabel.SetVisible(false);
            dialogueTextLabel.SetVisible(false);

            actorLabel.SetBinding(
                "value",
                new DataBinding
                {
                    dataSource = Data.Actor,
                    dataSourcePath = new PropertyPath(nameof(Data.Actor.Name)),
                    bindingMode = BindingMode.ToTarget
                }
            );

            conversantLabel.SetBinding(
                "value",
                new DataBinding
                {
                    dataSource = Data.Conversant,
                    dataSourcePath = new PropertyPath(nameof(Data.Conversant.Name)),
                    bindingMode = BindingMode.ToTarget
                }
            );

            dialogueTextLabel.SetBinding(
                "value",
                new DataBinding
                {
                    dataSource = Data.Dialogue,
                    dataSourcePath = new PropertyPath(nameof(Data.Dialogue.Text)),
                    bindingMode = BindingMode.ToTarget
                }
            );

            label.SetAlignment(Align.Center, Align.Center, Align.Center, Justify.Center);
            label.SetTextSettings(fontStyle: FontStyle.Bold, alignment: TextAnchor.MiddleCenter);

            portContainer.Add(titleContainer);
            portContainer.Add(infoContainer);

            mainContainer.SetBackgroundColor(Color.red);
            portContainer.SetBackgroundColor(Color.blue);
            titleContainer.SetBackgroundColor(Color.green);

            OutputPort = CreatePort(portContainer, Direction.Output);
            RefreshPorts();
        }

        #region Creation

        private Port CreatePort(VisualElement container, Direction direction)
        {
            Port port = this.CreatePort("", Orientation.Vertical, direction, Port.Capacity.Multi);
            DSPortData data = new DSPortData(this.Data.Guid, "port");
            port.userData = data;
            container.Add(port);
            Label label = port.Q<Label>("type");
            label.SetMargins(0);
            port.SetAlignment(Align.Center, Align.Center, Align.Center, Justify.Center);
            return port;
        }

        internal void SetInfoVisibility(bool showActor, bool showConversant, bool showDialogueText)
        {
            if (showActor)
            {
                actorLabel.SetVisible();
            }

            if (showConversant)
            {
                conversantLabel.SetVisible();
            }

            if (showDialogueText)
            {
                dialogueTextLabel.SetVisible();
            }
        }

        //public TextField CreateTextField(string title = null, string label = null, bool isMultiLine = false, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        //{
        //    TextField textField = Tools.DialogueSystem.Utilities.DSElementUtility.CreateTextField(title, label, isMultiLine, onValueChanged: onValueChanged);

        //    if (isMultiLine)
        //    {
        //        textField.AddClasses("ds-node__text-field",
        //      "ds-node__quote-text-field");
        //    }
        //    else
        //    {
        //        textField.AddClasses("ds-node__text-field",
        //        "ds-node__filename-text-field",
        //        "ds-node__text-field__hidden");
        //    }

        //    customDataContainer?.Add(textField);

        //    return textField;
        //}

        //public Label CreateLabel(string text = null, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        //{
        //    Label label = Tools.DialogueSystem.Utilities.DSElementUtility.CreateLabel(text, onValueChanged);

        //    customDataContainer?.Add(label);

        //    label.AddClasses("ds-node__text-field",
        //      "ds-node__quote-text-field");

        //    return label;
        //}

        //public DropdownField CreateDropdown(List<string> choices, string text = null, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        //{
        //    DropdownField dropdown = Tools.DialogueSystem.Utilities.DSElementUtility.CreateDropdown(text, choices, onValueChanged);

        //    customDataContainer?.Add(dropdown);

        //    return dropdown;
        //}

        //public ObjectField CreateObjectField(string title, string label, Type type, VisualElement customDataContainer, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged)
        //{
        //    ObjectField objectField = Tools.DialogueSystem.Utilities.DSElementUtility.CreateObjectField(title, type, label, onValueChanged);
        //    customDataContainer.Add(objectField);
        //    return objectField;
        //}

        #endregion
    }
}
