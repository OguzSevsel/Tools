using Tools.DialogueSystem;
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
        target.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
    }

    private void OnMouseDown(MouseDownEvent evt)
    {
        if (_panning)
            return;

        if (!CanStartManipulation(evt))
            return;

        // Only start panning if we clicked empty graph space.
        if (!IsEmptyGraphSpace(evt.target as VisualElement))
            return;

        _panning = true;

        if (target is GraphView graphView)
        {
            _lastMousePosition = graphView.ChangeCoordinatesTo(graphView.contentViewContainer, evt.localMousePosition);
        }

        target.CaptureMouse();

        evt.StopPropagation();
    }

    private void OnMouseMove(MouseMoveEvent evt)
    {
        if (!_panning)
            return;

        if (target is GraphView graphView)
        {
            Vector2 vector = graphView.ChangeCoordinatesTo(graphView.contentViewContainer, evt.localMousePosition) - _lastMousePosition;
            Vector3 scale = graphView.contentViewContainer.transform.scale;
            graphView.viewTransform.position += Vector3.Scale((Vector3)vector, scale);

            evt.StopPropagation();
        }
    }

    private void OnMouseUp(MouseUpEvent evt)
    {
        if (!_panning)
            return;

        if (!CanStopManipulation(evt))
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
            // Minimap should completely own its mouse interaction.
            if (element is MiniMap)
                return false;

            // Anything that is a GraphElement belongs to the graph.
            if (element is GraphElement)
                return false;

            // Ports are GraphElements in GraphView, but explicitly
            // keeping this here makes the intention clear.
            if (element is Port)
                return false;

            // Edges are GraphElements too.
            if (element is Edge)
                return false;

            element = element.parent;
        }

        return true;
    }
}