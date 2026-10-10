using System;
using System.Collections.Generic;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI.Elements
{
    public class DSNode : Node
    {
        public event Action<Edge> OnEdgeDeleted;
        public Port InputPort { get; set; }
        public Port OutputPort { get; set; }
        public DSDialogueNodeData Data = null;
        public event Action<DSNode> OnNodeSelect;
        private VisualElement portContainer;

        public virtual void Initialize(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSActor conversant, DSAudioClip audioClip, bool isStartNode)
        {
            DSDialogueNodeData data = new(position, type, dialogueText, actor, conversant, audioClip);
            this.Data = data;
            Data.IsStartNode = isStartNode;

            SetPosition(new Rect(position, Vector2.zero));

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

            portContainer.style.flexDirection = FlexDirection.Column;
            portContainer.style.alignItems = Align.Center;

            mainContainer.Insert(0, portContainer);

            if (!Data.IsStartNode)
                CreatePort(portContainer, InputPort, Direction.Input);

            Label label = DSElementUtility.CreateLabel("Node");

            titleContainer.Clear();
            titleContainer.Insert(0, label);

            label.SetAlignment(Align.Center, Align.Center, Align.Center, Justify.Center);
            label.SetTextSettings(fontStyle: FontStyle.Bold, alignment: TextAnchor.MiddleCenter);

            portContainer.Add(titleContainer);

            CreatePort(portContainer, OutputPort, Direction.Output);
            RefreshPorts();
        }

        #region Creation

        private void CreatePort(VisualElement container, Port port, Direction direction)
        {
            port = this.CreatePort("", Orientation.Vertical, direction, Port.Capacity.Multi);
            container.Add(port);
            Label label = port.Q<Label>("type");
            label.SetMargins(0);
            port.SetAlignment(Align.Center, Align.Center, Align.Center, Justify.Center);
        }

        public TextField CreateTextField(string title = null, string label = null, bool isMultiLine = false, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = Tools.DialogueSystem.Utilities.DSElementUtility.CreateTextField(title, label, isMultiLine, onValueChanged: onValueChanged);

            if (isMultiLine)
            {
                textField.AddClasses("ds-node__text-field",
              "ds-node__quote-text-field");
            }
            else
            {
                textField.AddClasses("ds-node__text-field",
                "ds-node__filename-text-field",
                "ds-node__text-field__hidden");
            }

            customDataContainer?.Add(textField);

            return textField;
        }

        public Label CreateLabel(string text = null, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            Label label = Tools.DialogueSystem.Utilities.DSElementUtility.CreateLabel(text, onValueChanged);

            customDataContainer?.Add(label);

            label.AddClasses("ds-node__text-field",
              "ds-node__quote-text-field");

            return label;
        }

        public DropdownField CreateDropdown(List<string> choices, string text = null, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            DropdownField dropdown = Tools.DialogueSystem.Utilities.DSElementUtility.CreateDropdown(text, choices, onValueChanged);

            customDataContainer?.Add(dropdown);

            return dropdown;
        }

        public ObjectField CreateObjectField(string title, string label, Type type, VisualElement customDataContainer, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged)
        {
            ObjectField objectField = Tools.DialogueSystem.Utilities.DSElementUtility.CreateObjectField(title, type, label, onValueChanged);
            customDataContainer.Add(objectField);
            return objectField;
        }

        #endregion
    }
}
