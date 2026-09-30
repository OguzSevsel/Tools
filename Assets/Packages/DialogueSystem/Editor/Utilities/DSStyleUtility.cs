using PlasticPipe.PlasticProtocol.Messages;
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

        public static VisualElement AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (string styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load(styleSheetName);

                element.styleSheets.Add(styleSheet);
            }
            return element;
        }

        public static VisualElement SetPaddings(this VisualElement element, int? allPadding = null, int paddingLeft = 0, int paddingRight = 0, int paddingTop = 0, int paddingBottom = 0)
        {
            element.style.paddingLeft = allPadding ?? paddingLeft;
            element.style.paddingRight = allPadding ?? paddingRight;
            element.style.paddingTop = allPadding ?? paddingTop;
            element.style.paddingBottom = allPadding ?? paddingBottom;
            return element;
        }

        public static VisualElement SetAlignment(this VisualElement element, Align alignContent = Align.Auto, Align alignItems = Align.Auto, Align alignSelf = Align.Auto, Justify justifyContent = Justify.Center)
        {
            element.style.alignContent = alignContent;
            element.style.alignItems = alignItems;
            element.style.alignSelf = alignSelf;
            element.style.justifyContent = justifyContent;
            return element;
        }

        public static VisualElement SetMargins(this VisualElement element, int? allMargin = null, int marginLeft = 0, int marginRight = 0, int marginTop = 0, int marginBottom = 0)
        {
            element.style.marginLeft = allMargin ?? marginLeft;
            element.style.marginRight = allMargin ?? marginRight;
            element.style.marginTop = allMargin ?? marginTop;
            element.style.marginBottom = allMargin ?? marginBottom;
            return element;
        }

        public static VisualElement SetBackgroundColor(this VisualElement element, Color color)
        {
            element.style.backgroundColor = color;
            return element;
        }

        public static VisualElement SetWidth(this VisualElement element, int width = 200, int maxWidth = 500, int minWidth = 200)
        {
            element.style.width = width;
            element.style.maxWidth = maxWidth;
            element.style.minWidth = minWidth;
            return element;
        }

        public static VisualElement SetHeight(this VisualElement element, int height = 200, int maxHeight = 500, int minHeight = 200)
        {
            element.style.height = height;
            element.style.maxHeight = maxHeight;
            element.style.minHeight = minHeight;
            return element;
        }

        public static VisualElement SetBorder(this VisualElement element, Color borderColor, int? allBorder = null, int borderLeft = 3, int borderRight = 3, int borderTop = 3, int borderBottom = 3)
        {
            element.style.borderLeftWidth = allBorder ?? borderLeft;
            element.style.borderRightWidth = allBorder ?? borderRight;
            element.style.borderTopWidth = allBorder ?? borderTop;
            element.style.borderBottomWidth = allBorder ?? borderBottom;

            element.style.borderLeftColor = borderColor;
            element.style.borderRightColor = borderColor;
            element.style.borderTopColor = borderColor;
            element.style.borderBottomColor = borderColor;
            return element;
        }

        public static VisualElement SetFlex(this VisualElement element, int flexBasis = 100, int flexGrow = 1, int flexShrink = 1, FlexDirection flexDirection = FlexDirection.Row, Wrap flexWrap = Wrap.Wrap)
        {
            element.style.flexGrow = flexGrow;
            element.style.flexBasis = flexBasis;
            element.style.flexDirection = flexDirection;
            element.style.flexShrink = flexShrink;
            element.style.flexWrap = flexWrap;
            return element;
        }

        public static VisualElement SetTextSettings(this VisualElement element, TextAnchor alignment = TextAnchor.MiddleCenter, FontStyle fontStyle = FontStyle.Normal, int fontSize = 18)
        {
            element.style.unityTextAlign = alignment;
            element.style.fontSize = fontSize;
            element.style.unityFontStyleAndWeight = fontStyle;
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
    }
}