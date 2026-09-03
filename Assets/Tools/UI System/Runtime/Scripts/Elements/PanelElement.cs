using PrimeTween;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Interfaces;

namespace Elements
{
    public class PanelElement : UIElement, IMouseInteractable
    {
        public Image Image { get; private set; }
        public event Action<PointerEventData> OnMouseEnter;
        public event Action<PointerEventData> OnMouseExit;
        public event Action<PointerEventData> OnMouseClick;
        public event Action<PointerEventData> OnMouseUp;
        public event Action<PointerEventData> OnMouseDown;

        public override void Awake()
        {
            base.Awake();
            Image = GetComponentInChildren<Image>();
        }

        public Tween Color(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(Image,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                cycles: cycles,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            OnMouseEnter?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            OnMouseExit?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            OnMouseClick?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            OnMouseUp?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!EventSettings.Interactable) return;
            OnMouseDown?.Invoke(eventData);
        }
    }
}

