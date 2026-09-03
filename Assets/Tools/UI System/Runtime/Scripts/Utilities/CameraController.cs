using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Elements;

namespace Utilities
{
    public class CameraController : CameraElement
    {
        public static CameraController Instance;

        [Header("Zoom")]
        [SerializeField] private float _zoomMultiplier = 4f;
        [SerializeField] private float _minZoom = 2f;
        [SerializeField] private float _maxZoom = 8f;
        [SerializeField] private float _focusZoom = 2.5f;

        [Header("Movement")]
        [SerializeField] private BoxCollider2D _cameraBounds;

        public event Action OnZoomChange;

        private float _zoom;
        private Vector3 _dragOrigin;
        private bool _dragging;
        private bool _isDragging;

        public override void Awake()
        {
            base.Awake();
            Instance = this;
            _zoom = Camera.orthographicSize;
        }

        private void Update()
        {
            HandleZoom();
            if (_isDragging)
                HandleDrag();
            ClampCamera();
        }

        public void StopDragging()
        {
            _isDragging = false;
        }

        public void StartDragging()
        {
            _isDragging = true;
        }

        public void FocusOn<T>(Vector3 endPos, Vector2 focusOffset, T target, System.Action<T> onComplete) where T : class
        {
            float duration = 0f;
            float distance = Vector3.Distance(Camera.transform.position, endPos);

            duration = distance * 0.1f;

            Animate()
                .Begin(Move(new Vector3(endPos.x + focusOffset.x, endPos.y + focusOffset.y, -1f), duration))
                .Also(Zoom(duration, _focusZoom)).OnComplete(target, onComplete);

            _zoom = _focusZoom;
        }

        private void HandleZoom()
        {
            if (Mouse.current == null)
                return;

            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll != 0)
            {
                _zoom -= scroll * _zoomMultiplier;
                _zoom = Mathf.Clamp(_zoom, _minZoom, _maxZoom);
                Camera.orthographicSize = Mathf.Lerp(Camera.orthographicSize, _zoom,
                    5f * Time.deltaTime);

                OnZoomChange?.Invoke();
            }
        }

        private void HandleDrag()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame)
            {
                _dragOrigin = GetMouseWorldPosition();
                _dragging = true;
            }

            if ((Mouse.current.leftButton.isPressed || Mouse.current.middleButton.isPressed) && _dragging)
            {
                Vector3 difference = _dragOrigin - GetMouseWorldPosition();
                Camera.transform.position += difference;
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame || Mouse.current.middleButton.wasReleasedThisFrame)
            {
                _dragging = false;
            }
        }

        private void ClampCamera()
        {
            if (_cameraBounds == null)
                return;

            Bounds bounds = _cameraBounds.bounds;

            float camHeight = Camera.orthographicSize;
            float camWidth = camHeight * Camera.aspect;

            Vector3 pos = Camera.transform.position;

            pos.x = Mathf.Clamp(
                pos.x,
                bounds.min.x + camWidth,
                bounds.max.x - camWidth
            );

            pos.y = Mathf.Clamp(
                pos.y,
                bounds.min.y + camHeight,
                bounds.max.y - camHeight
            );

            Camera.transform.position = pos;
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            return Camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
        }
    }
}

