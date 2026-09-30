using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.Utilities
{
    public class ResizeManipulator : MouseManipulator
    {
        private VisualElement targetElement;
        private Vector2 startMousePosition;
        private float startWidth;
        private float width;
        private bool isResizeable;

        public ResizeManipulator(VisualElement target)
        {
            targetElement = target;

            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            target.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            target.SetCursor(MouseCursor.Arrow);
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            target.SetCursor(MouseCursor.ResizeHorizontal);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (!CanStartManipulation(evt))
                return;

            isResizeable = true;
            startMousePosition = evt.mousePosition;
            startWidth = targetElement.resolvedStyle.width;
            width = startWidth;

            target.CaptureMouse();
            evt.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (!isResizeable) return;

            float delta = evt.mousePosition.x - startMousePosition.x;

            width = startWidth - delta;
            targetElement.style.width = Mathf.Clamp(width, 200, 400);

            evt.StopPropagation();
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            if (!CanStopManipulation(evt))
                return;

            isResizeable = false;
            targetElement.ReleaseMouse();
            evt.StopPropagation();
        }
    } 
}