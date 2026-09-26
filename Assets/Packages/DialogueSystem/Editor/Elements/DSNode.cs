using System;
using System.Collections.Generic;
using System.Linq;
using Tools.DialogueSystem;
using Tools.DialogueSystem.Data;
using Tools.DialogueSystem.Elements;
using Tools.DialogueSystem.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSNode : Node
    {
        public string Id { get; set; } = "Node ID";
        public Dictionary<Port, string> Choices { get; set; }
        public Port InputPort { get; set; }

        public bool isStartNode = false;
        public bool isPasting = false;
        public bool isLoading = false;
        public Color defaultBackgroundColor;
        public DialogueType DialogueType { get; set; }

        public event Action<Edge> OnEdgeDeleted;
        public event Action<DSNode, ChangeEvent<string>> OnNodeIdChanged;

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

        public DropdownField CreateDropdown(List<string> choices, string text = null, VisualElement customDataContainer = null,  EventCallback<ChangeEvent<string>> onValueChanged = null)
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

        public Port CreateChoicePort(string choice, DSPortData portData)
        {
            Port choicePort = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            choicePort.userData = portData;
            this.Choices.Add(choicePort, portData.PortName);
            Button deletePortButton = null;

            if (DialogueType != DialogueType.Single)
            {
                deletePortButton = DSElementUtility.CreateButton("X", () =>
                {
                    if (this.Choices.Count > 1)
                    {
                        List<Edge> edges = choicePort.connections.ToList();

                        Choices.Remove(choicePort);
                        outputContainer.Remove(choicePort);

                        foreach (Edge edge in edges)
                        {
                            edge.input?.Disconnect(edge);
                            edge.output?.Disconnect(edge);

                            OnEdgeDeleted?.Invoke(edge);
                        }

                        RefreshExpandedState();
                    }
                });
            }

            TextField choiceTextField = DSElementUtility.CreateTextField(choice, onValueChanged: (evt) =>
            {
                DSPortData data = (DSPortData)choicePort.userData;
                data.PortName = evt.newValue;
            });

            choiceTextField.AddClasses("ds-node__text-field",
                "ds-node__choice-text-field",
                "ds-node__text-field__hidden");

            choicePort.Add(choiceTextField);

            if (deletePortButton != null)
            {
                choicePort.Add(deletePortButton);
            }

            return choicePort;
        }

        public void OnIdChanged(ChangeEvent<string> evt)
        {
            this.Id = evt.newValue;

            var keys = new List<Port>(Choices.Keys);
            foreach (var key in keys)
            {
                Choices[key] = evt.newValue;
            }

            OnNodeIdChanged?.Invoke(this, evt);
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
    }
}
