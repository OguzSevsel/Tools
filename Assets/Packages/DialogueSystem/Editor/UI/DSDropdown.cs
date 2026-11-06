using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSDropdown<T>
    {
        private readonly PopupField<T> dropdown;

        private List<T> items = new();

        private readonly Func<T, string> displayNameFunc;

        public T Value { get; private set; }

        public event Action<T> ValueChanged;

        public DSDropdown(PopupField<T> dropdown,Func<T, string> displayNameFunc)
        {
            this.dropdown = dropdown ??
                throw new ArgumentNullException(nameof(dropdown));

            this.displayNameFunc = displayNameFunc ??
                throw new ArgumentNullException(nameof(displayNameFunc));

            dropdown.formatListItemCallback = item =>
                item == null ? string.Empty : displayNameFunc(item);

            dropdown.formatSelectedValueCallback = item =>
                item == null ? string.Empty : displayNameFunc(item);

            dropdown.RegisterValueChangedCallback(OnDropdownChanged);
        }

        public void SetItems(IEnumerable<T> newItems)
        {
            items = newItems.ToList();

            dropdown.choices = items;

            if (items.Count > 0)
            {
                SetValue(items[0]);
            }
            else
            {
                Value = default;
            }
        }

        public void SetValue(T value)
        {
            if (!items.Contains(value))
                return;

            Value = value;

            dropdown.SetValueWithoutNotify(value);
        }

        private void OnDropdownChanged(ChangeEvent<T> evt)
        {
            Value = evt.newValue;

            ValueChanged?.Invoke(Value);
        }

        public int GetItemCount()
        {
            return items.Count;
        }
    }
}