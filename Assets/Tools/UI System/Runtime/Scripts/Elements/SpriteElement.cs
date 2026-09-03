using PrimeTween;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Interfaces;
using Settings;

namespace Elements
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteElement : WorldElement, IMouseInteractable
    {
        [field: SerializeField] public EventSettingsSO EventSettings { get; protected set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public Collider2D Collider2D { get; private set; }

        public event Action<PointerEventData> OnMouseEnter;
        public event Action<PointerEventData> OnMouseExit;
        public event Action<PointerEventData> OnMouseClick;
        public event Action<PointerEventData> OnMouseDown;
        public event Action<PointerEventData> OnMouseUp;

        public override void Awake()
        {
            base.Awake();
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Collider2D = GetComponent<Collider2D>();
        }

        #region Events

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (EventSettings.Interactable) OnMouseEnter?.Invoke(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (EventSettings.Interactable) OnMouseExit?.Invoke(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (EventSettings.Interactable) OnMouseClick?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (EventSettings.Interactable) OnMouseUp?.Invoke(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (EventSettings.Interactable) OnMouseDown?.Invoke(eventData);
        }

        #endregion

        #region Animations

        public Tween FadeIn(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<SpriteElement> onComplete = null
            )
        {
            return Tween.Alpha(
            SpriteRenderer,
            endValue: endValue ?? AnimationSettings.FadeInValue,
            duration ?? AnimationSettings.Duration,
            ease ?? AnimationSettings.Ease,
            startDelay: startDelay,
            endDelay: endDelay,
            useUnscaledTime: unScaledTime)
            .OnComplete(this, onComplete);
        }

        public Tween FadeOut(
            float? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<SpriteElement> onComplete = null
            )
        {
            return Tween.Alpha(
            SpriteRenderer,
            endValue: endValue ?? AnimationSettings.FadeOutValue,
            duration ?? AnimationSettings.Duration,
            ease ?? AnimationSettings.Ease,
            startDelay: startDelay,
            endDelay: endDelay,
            useUnscaledTime: unScaledTime)
            .OnComplete(this, onComplete);
        }

        public Tween Color(
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action onComplete = null)
        {
            return Tween.Color(SpriteRenderer,
                endValue: endValue ?? AnimationSettings.EndColor,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime)
                .OnComplete(onComplete);
        }

        #endregion

        #region Utils

        public void Hide()
        {
            SpriteRenderer.enabled = false;
            Collider2D.enabled = false;
        }

        public void Show()
        {
            SpriteRenderer.enabled = true;
            Collider2D.enabled = true;
        }

        #endregion
    }
}

