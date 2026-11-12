using UnityEngine;

namespace Tools.DialogueSystem.UI
{
    using System;
    using System.Collections.Generic;
    using Tools.DialogueSystem.Data;
    using Tools.DialogueSystem.Utilities;
    using UnityEngine.UIElements;

    public class DSPopUpField<T> : VisualElement where T : DSData
    {
        private readonly Label _label;
        private readonly Button _dropdownButton;

        private List<T> _choices = new();
        private T _value;

        private VisualElement _popupLayer;

        public T value
        {
            get => _value;
            set
            {
                _value = value;
                UpdateDisplay();
            }
        }

        public List<T> choices
        {
            get => _choices;
            set
            {
                _choices = value ?? new List<T>();
                UpdateDisplay();
            }
        }

        public event Action<T> OnValueChanged;

        public DSPopUpField(string title)
        {
            this.SetFlex(flexDirection: FlexDirection.Row, flexGrow: 1, flexShrink: 1, flexWrap: Wrap.NoWrap);
            this.SetPaddings(allPadding: 0);
            this.SetAlignment(alignItems: Align.Center);

            _label = new Label(title);
            _label.SetWidth(minWidth: 100);

            _dropdownButton = new Button(OpenDropdown)
            {
                text = "Click To Select"
            };

            _dropdownButton.SetFlex(flexGrow: 1);
            _dropdownButton.SetHeight(minHeight: 25);

            Add(_label);
            Add(_dropdownButton);
        }

        public void SetItems(List<T> items)
        {
            choices = items;
        }

        private void UpdateDisplay()
        {
            if (_value != null)
            {
                _dropdownButton.text = _value.Name;
            }
            else
            {
                _dropdownButton.text = _choices.Count > 0
                    ? "Click To Select"
                    : "There are no items in Database.";
            }
        }

        private void OpenDropdown()
        {
            if (panel == null || _choices.Count == 0)
                return;

            CloseDropdown();

            var root = panel.visualTree;

            _popupLayer = new VisualElement();
            _popupLayer.SetAnchor(top: 0, 0, 0, 0);

            _popupLayer.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.target == _popupLayer)
                    CloseDropdown();
            });

            var dropdown = new ScrollView();

            dropdown.SetAnchor(top: _dropdownButton.worldBound.yMax, left: _dropdownButton.worldBound.x);
            dropdown.SetWidth(width: _dropdownButton.worldBound.width);
            dropdown.SetHeight(maxHeight: 250);
            dropdown.SetBackgroundColor(new Color(0.18f, 0.18f, 0.18f));
            dropdown.SetBorder(allBorder: 0);

            for (int i = 0; i < _choices.Count; i++)
            {
                T item = _choices[i];

                var itemButton = new Button(() =>
                {
                    SelectItem(item);
                })
                {
                    text = item != null ? item.Name : "None"
                };

                itemButton.style.unityTextAlign = TextAnchor.MiddleCenter;
                itemButton.SetBorder(allBorder: 0);
                itemButton.style.color = Color.white;

                dropdown.Add(itemButton);
            }

            _popupLayer.Add(dropdown);
            root.Add(_popupLayer);
        }

        private void SelectItem(T item)
        {
            value = item;
            OnValueChanged?.Invoke(item);

            CloseDropdown();
        }

        private void CloseDropdown()
        {
            if (_popupLayer == null)
                return;

            _popupLayer.RemoveFromHierarchy();
            _popupLayer = null;
        }
    }
}
