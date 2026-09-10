using System;
using UnityEngine;
using UnityEngine.UIElements;

public static class TextFieldExtensions
{
    public static void RegisterSelectionChangedCallback(this TextField textField, Action<string> onSelectionChanged)
    {
        int lastCursorIndex = textField.cursorIndex;
        int lastSelectIndex = textField.selectIndex;

        void CheckSelection()
        {
            if (textField.cursorIndex != lastCursorIndex || textField.selectIndex != lastSelectIndex)
            {
                lastCursorIndex = textField.cursorIndex;
                lastSelectIndex = textField.selectIndex;
                onSelectionChanged?.Invoke(GetSelectedText(textField));
            }
        }

        // selection can change via keyboard (arrows+shift, ctrl+a) or mouse drag
        textField.RegisterCallback<KeyUpEvent>(_ => CheckSelection());
        textField.RegisterCallback<PointerUpEvent>(_ => CheckSelection());
        textField.RegisterCallback<PointerMoveEvent>(_ => CheckSelection()); // catches drag-selection
    }

    public static string GetSelectedText(this TextField textField)
    {
        int start = Mathf.Min(textField.cursorIndex, textField.selectIndex);
        int end = Mathf.Max(textField.cursorIndex, textField.selectIndex);
        return start == end ? string.Empty : textField.text.Substring(start, end - start);
    }
}