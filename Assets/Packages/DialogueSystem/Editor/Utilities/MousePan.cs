
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MousePan : MouseManipulator
{
    private bool _panning;
    private Vector2 _lastMousePosition;

    public MousePan()
    {
        activators.Add(new ManipulatorActivationFilter
        {
            button = MouseButton.LeftMouse
        });
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(
            OnMouseDown, TrickleDown.TrickleDown);

        target.RegisterCallback<MouseMoveEvent>(
            OnMouseMove, TrickleDown.TrickleDown);

        target.RegisterCallback<MouseUpEvent>(
            OnMouseUp, TrickleDown.TrickleDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(
            OnMouseDown, TrickleDown.TrickleDown);

        target.UnregisterCallback<MouseMoveEvent>(
            OnMouseMove, TrickleDown.TrickleDown);

        target.UnregisterCallback<MouseUpEvent>(
            OnMouseUp, TrickleDown.TrickleDown);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (_panning || !CanStartManipulation(evt))
            return;

        if (!IsEmptyGraphSpace(evt.target as VisualElement))
            return;

        _panning = true;

        _lastMousePosition = evt.localMousePosition;

        target.CaptureMouse();

        evt.StopPropagation();
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (!_panning)
            return;

        if (target is GraphView graphView)
        {
            Vector2 currentMousePosition = evt.localMousePosition;
            Vector2 delta = currentMousePosition - _lastMousePosition;

            var content = graphView.contentViewContainer;

            // Read the current translation.
            Translate currentTranslate = content.resolvedStyle.translate;

            Vector2 position = new Vector2(
                currentTranslate.x.value,
                currentTranslate.y.value
            );

            position += delta;

            // Apply the new translation.
            content.style.translate = new Translate(
                new Length(position.x, LengthUnit.Pixel),
                new Length(position.y, LengthUnit.Pixel),
                currentTranslate.z
            );

            _lastMousePosition = currentMousePosition;

            evt.StopPropagation();
        }
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        if (!_panning || !CanStopManipulation(evt))
            return;

        _panning = false;

        if (target.HasMouseCapture())
            target.ReleaseMouse();

        evt.StopPropagation();
    }

    private static bool IsEmptyGraphSpace(VisualElement element)
    {
        while (element != null)
        {
            if (element is MiniMap)
                return false;

            if (element is GraphElement)
                return false;

            element = element.parent;
        }

        return true;
    }
}
