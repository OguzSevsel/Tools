using PrimeTween;
using UnityEngine;
using Settings;

namespace Elements
{
    [RequireComponent(typeof(Camera))]
    public class CameraElement : WorldElement
    {
        [field: SerializeField] public CameraSettingsSO Preset { get; private set; }

        public Camera Camera { get; private set; }
        public float CameraOrthoSize { get; private set; }

        public override void Awake()
        {
            base.Awake();
            Camera = GetComponent<Camera>();
        }

        public Tween Zoom(
            float? duration = null,
            float? cameraTargetSize = null,
            Ease? ease = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true)
        {
            CameraOrthoSize = cameraTargetSize ?? Preset.TargetOrthoSize;

            return Tween.CameraOrthographicSize(Camera,
                endValue: CameraOrthoSize,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime);
        }

        public Tween ChangeAspect(
            float? aspect = null,
            float? duration = null,
            Ease? ease = null,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true)
        {
            return Tween.CameraAspect(Camera,
                endValue: aspect ?? Preset.AspectRatio,
                duration: duration ?? AnimationSettings.Duration,
                ease: ease ?? AnimationSettings.Ease,
                startDelay: startDelay,
                endDelay: endDelay,
                useUnscaledTime: unScaledTime);
        }
    }
}

