using System.Collections.Generic;
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

        public static void SetDropShadow(this VisualElement element, bool isRemoving = false)
        {
            if (!isRemoving)
            {
                FilterFunction dropShadow = new(
                    FilterFunctionType.DropShadow
                );

                dropShadow.AddParameter(new FilterParameter(2f));
                dropShadow.AddParameter(new FilterParameter(2f));
                dropShadow.AddParameter(new FilterParameter(3f));
                dropShadow.AddParameter(new FilterParameter(
                    new Color(0.212f, 0.212f, 0.169f, 1f)
                ));

                element.style.filter = new List<FilterFunction>
                {
                    dropShadow
                };
            }
            else
            {
                element.style.filter = StyleKeyword.Null;
            }
        }

        public static VisualElement SetPaddings(this VisualElement element, int? allPadding = null, int? paddingLeft = null, int? paddingRight = null, int? paddingTop = null, int? paddingBottom = null)
        {
            element.style.paddingLeft = allPadding ?? paddingLeft ?? element.style.paddingLeft;
            element.style.paddingRight = allPadding ?? paddingRight ?? element.style.paddingRight;
            element.style.paddingTop = allPadding ?? paddingTop ?? element.style.paddingTop;
            element.style.paddingBottom = allPadding ?? paddingBottom ?? element.style.paddingBottom;
            return element;
        }

        public static VisualElement SetAlignment(this VisualElement element, Align? alignContent = null, Align? alignItems = null, Align? alignSelf = null, Justify? justifyContent = null)
        {
            element.style.alignContent = alignContent ?? element.style.alignContent;
            element.style.alignItems = alignItems ?? element.style.alignItems;
            element.style.alignSelf = alignSelf ?? element.style.alignSelf;
            element.style.justifyContent = justifyContent ?? element.style.justifyContent;
            return element;
        }

        public static VisualElement SetMargins(this VisualElement element, int? allMargin = null, int? marginLeft = null, int? marginRight = null, int? marginTop = null, int? marginBottom = null)
        {
            element.style.marginLeft = allMargin ?? marginLeft ?? element.style.marginLeft;
            element.style.marginRight = allMargin ?? marginRight ?? element.style.marginRight;
            element.style.marginTop = allMargin ?? marginTop ?? element.style.marginTop;
            element.style.marginBottom = allMargin ?? marginBottom ?? element.style.marginBottom;
            return element;
        }

        public static VisualElement SetBackgroundColor(this VisualElement element, Color color)
        {
            element.style.backgroundColor = color;
            return element;
        }

        public static VisualElement SetWidth(this VisualElement element, float? width = null, float? maxWidth = null, float? minWidth = null)
        {
            element.style.width = width ?? element.style.width;
            element.style.maxWidth = maxWidth ?? element.style.maxWidth;
            element.style.minWidth = minWidth ?? element.style.minWidth;
            return element;
        }

        public static VisualElement SetHeight(this VisualElement element, float? height = null, float? maxHeight = null, float? minHeight = null)
        {
            element.style.height = height ?? element.style.height;
            element.style.maxHeight = maxHeight ?? element.style.maxHeight;
            element.style.minHeight = minHeight ?? element.style.minHeight;
            return element;
        }

        public static VisualElement SetBorder(this VisualElement element, Color? borderColor = null, float? allBorder = null, float? borderLeft = null, float? borderRight = null, float? borderTop = null, float? borderBottom = null)
        {
            element.style.borderLeftWidth = allBorder ?? borderLeft ?? element.style.borderLeftWidth;
            element.style.borderRightWidth = allBorder ?? borderRight ?? element.style.borderRightWidth;
            element.style.borderTopWidth = allBorder ?? borderTop ?? element.style.borderTopWidth;
            element.style.borderBottomWidth = allBorder ?? borderBottom ?? element.style.borderBottomWidth;

            element.style.borderLeftColor = borderColor ?? element.style.borderLeftColor;
            element.style.borderRightColor = borderColor ?? element.style.borderRightColor;
            element.style.borderTopColor = borderColor ?? element.style.borderTopColor;
            element.style.borderBottomColor = borderColor ?? element.style.borderBottomColor;
            return element;
        }

        public static VisualElement SetFlex(this VisualElement element, int? flexBasis = null, int? flexGrow = null, int? flexShrink = null, FlexDirection? flexDirection = null, Wrap? flexWrap = null)
        {
            element.style.flexGrow = flexGrow ?? element.style.flexGrow;
            element.style.flexBasis = flexBasis ?? element.style.flexBasis;
            element.style.flexDirection = flexDirection ?? element.style.flexDirection;
            element.style.flexShrink = flexShrink ?? element.style.flexShrink;
            element.style.flexWrap = flexWrap ?? element.style.flexWrap;
            return element;
        }

        public static VisualElement SetVisible(this VisualElement element, bool isVisible = true)
        {
            element.style.display = isVisible ? (StyleEnum<DisplayStyle>)DisplayStyle.Flex : (StyleEnum<DisplayStyle>)DisplayStyle.None;

            return element;
        }

        public static VisualElement SetTextSettings(this VisualElement element, TextAnchor? alignment = null, FontStyle? fontStyle = null, int? fontSize = null)
        {
            element.style.unityTextAlign = alignment ?? element.style.unityTextAlign;
            element.style.fontSize = fontSize ?? element.style.fontSize;
            element.style.unityFontStyleAndWeight = fontStyle ?? element.style.unityFontStyleAndWeight;
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
