using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Tools.TweenSystem.Interfaces;

namespace Tools.TweenSystem.Elements
{
    public class Draggable : MonoBehaviour, IDraggable
    {
        private Vector2 _dragOffset;
        public RectTransform RectTransform { get; private set; }

        public event Action<PointerEventData> OnDragBegin;
        public event Action<PointerEventData> OnDraggin;
        public event Action<PointerEventData> OnDragEnd;

        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform.parent,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 movePos))
            {
                Vector2 targetPos = movePos - _dragOffset;

                RectTransform parentRect = (RectTransform)transform.parent;
                Rect myRect = RectTransform.rect;
                Rect pRect = parentRect.rect;

                float minX = pRect.xMin + (myRect.width * RectTransform.pivot.x);
                float maxX = pRect.xMax - (myRect.width * (1 - RectTransform.pivot.x));
                float minY = pRect.yMin + (myRect.height * RectTransform.pivot.y);
                float maxY = pRect.yMax - (myRect.height * (1 - RectTransform.pivot.y));

                targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
                targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

                RectTransform.localPosition = targetPos;
                OnDraggin?.Invoke(eventData);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)RectTransform.parent,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 mousePosInParentSpace);

            _dragOffset = mousePosInParentSpace - (Vector2)RectTransform.localPosition;
            OnDragBegin?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnd?.Invoke(eventData);
        }
    } 
}
