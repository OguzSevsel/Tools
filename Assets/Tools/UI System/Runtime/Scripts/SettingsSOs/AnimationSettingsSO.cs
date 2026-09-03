using PrimeTween;
using UnityEngine;
using Utilities;

namespace Settings
{
    [CreateAssetMenu(fileName = "New Animation Settings", menuName = "Animations/Animation Settings", order = 0)]
    public class AnimationSettingsSO : ScriptableObject
    {
        [Header("General")]
        public Ease Ease = Ease.OutBack;
        public float Duration = 0.5f;
        public float FadeInValue = 1f;
        public float FadeOutValue = 0f;

        [Header("Color")]
        public Color DefaultColor = Color.white;
        public Color EndColor = Color.black;

        [Header("Scale")]
        public Vector3 ScaleUpVector = new(1.1f, 1.1f, 0f);
        public Vector3 ScaleDownVector = new(1f, 1f, 0f);

        [Header("Rotation")]
        public Vector3 RotationVector = new(0f, 0f, 90f);

        [Header("Shake")]
        public Vector3 ShakeStrength = new(0f, 0f, 5f);
        public float ShakeFrequency = 5f;

        [Header("Roll")]
        public SlideDirection RollInDirection = SlideDirection.Left;
        public SlideDirection RollOutDirection = SlideDirection.Right;

        [Header("Slide")]
        public SlideDirection SlideInDirection = SlideDirection.Left;
        public SlideDirection SlideOutDirection = SlideDirection.Right;
        public float SlideDistance = 500f;
    } 
}