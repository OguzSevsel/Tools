using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSDropdown<T>
    {
        private readonly DropdownField dropdown;

        private List<T> items = new();

        private Func<T, string> displayNameFunc;

        public T Value { get; private set; }

        public event Action<T> ValueChanged;

        public DSDropdown(
            DropdownField dropdown,
            Func<T, string> displayNameFunc)
        {
            this.dropdown = dropdown;
            this.displayNameFunc = displayNameFunc;

            dropdown.RegisterValueChangedCallback(OnDropdownChanged);
        }

        public void SetItems(IEnumerable<T> newItems)
        {
            items = newItems.ToList();

            dropdown.choices = items
                .Select(displayNameFunc)
                .ToList();

            if (items.Count > 0)
            {
                SetValue(items[0]);
            }
            else
            {
                Value = default;
                dropdown.SetValueWithoutNotify("");
            }
        }

        public void SetValue(T value)
        {
            int index = items.IndexOf(value);

            if (index < 0)
                return;

            Value = value;

            dropdown.SetValueWithoutNotify(
                displayNameFunc(value)
            );
        }

        private void OnDropdownChanged(ChangeEvent<string> evt)
        {
            int index = dropdown.choices.IndexOf(evt.newValue);

            if (index < 0 || index >= items.Count)
                return;

            Value = items[index];

            ValueChanged?.Invoke(Value);
        }
    } 
}