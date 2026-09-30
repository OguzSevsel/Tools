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

        public virtual void Initialize(Vector2 position, DialogueType type, DSDialogueText dialogueText, DSActor actor, DSAudioClip audioClip, bool isStartNode)
        {
            DSDialogueNodeData data = new DSDialogueNodeData(position, type, dialogueText, actor, audioClip);
            this.Data = data;
            Data.IsStartNode = isStartNode;

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
            mainContainer.Clear();
            Draw();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelect?.Invoke(this);
        }

        public virtual void Draw()
        {
            if (!this.Data.IsStartNode)
            {
                CreatePort(inputContainer, InputPort, Direction.Input);
            }

            var label = DSElementUtility.CreateLabel("Node");
            titleContainer.Insert(0, label);
            label.SetAlignment(UnityEngine.UIElements.Align.Center);
            label.SetTextSettings(fontStyle: FontStyle.Bold);
            titleContainer.SetAlignment(UnityEngine.UIElements.Align.Center);
            mainContainer.Add(titleContainer);
            CreatePort(outputContainer, OutputPort, Direction.Output);
        }

        private void CreatePort(VisualElement container, Port port, Direction direction)
        {
            port = this.CreatePort("", Orientation.Vertical, direction, Port.Capacity.Multi);
            container.Add(port);
            mainContainer.Add(container);
            var label = port.Q<Label>("type");
            label.SetMargins(0);
            port.SetAlignment(UnityEngine.UIElements.Align.Center);
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

            if (customDataContainer != null)
            {
                customDataContainer.Add(textField);
            }

            return textField;
        }

        public Label CreateLabel(string text = null, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            Label label = Tools.DialogueSystem.Utilities.DSElementUtility.CreateLabel(text, onValueChanged);

            if (customDataContainer != null)
            {
                customDataContainer.Add(label);
            }

            label.AddClasses("ds-node__text-field",
              "ds-node__quote-text-field");

            return label;
        }

        public DropdownField CreateDropdown(List<string> choices, string text = null, VisualElement customDataContainer = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            DropdownField dropdown = Tools.DialogueSystem.Utilities.DSElementUtility.CreateDropdown(text, choices, onValueChanged);

            if (customDataContainer != null)
            {
                customDataContainer.Add(dropdown);
            }

            return dropdown;
        }

        public ObjectField CreateObjectField(string title, string label, Type type, VisualElement customDataContainer, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged)
        {
            ObjectField objectField = Tools.DialogueSystem.Utilities.DSElementUtility.CreateObjectField(title, type, label, onValueChanged);
            customDataContainer.Add(objectField);
            return objectField;
        }
    }
}
