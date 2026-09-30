using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.Utilities
{
    public static class DSStyleUtility
    {
        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }

        public static VisualElement SetCursor(this VisualElement element, MouseCursor cursor)
        {
            object objCursor = new UnityEngine.UIElements.Cursor();
            PropertyInfo fields = typeof(UnityEngine.UIElements.Cursor).GetProperty("defaultCursorId", BindingFlags.NonPublic | BindingFlags.Instance);
            fields.SetValue(objCursor, (int)cursor);
            element.style.cursor = new StyleCursor((UnityEngine.UIElements.Cursor)objCursor);

            return element;
        }

        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load(styleSheetName);

                element.styleSheets.Add(styleSheet);
            }

            return element;
        }

        public static VisualElement Padding(this VisualElement element, int padding)
        {
            element.style.paddingLeft = padding;
            element.style.paddingRight = padding;
            element.style.paddingTop = padding;
            element.style.paddingBottom = padding;

            return element;
        }

        public static VisualElement Align(this VisualElement element, Align alignment)
        {
            element.style.alignContent = alignment;
            element.style.alignItems = alignment;
            element.style.alignSelf = alignment;

            return element;
        }

        public static VisualElement Margin(this VisualElement element, int margin)
        {
            element.style.marginLeft = margin;
            element.style.marginRight = margin;
            element.style.marginTop = margin;
            element.style.marginBottom = margin;

            return element;
        }

        public static VisualElement TextSettings(this VisualElement element, TextAnchor alignment, FontStyle fontStyle, int fontSize)
        {
            element.style.flexGrow = 1;
            element.style.unityTextAlign = alignment;
            element.style.fontSize = fontSize;
            element.style.unityFontStyleAndWeight = fontStyle;

            return element;
        }
    }
}