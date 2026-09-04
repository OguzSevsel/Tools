using PrimeTween;
using UnityEngine;
using Tools.TweenSystem.Elements;
using Tools.TweenSystem.Utilities;

namespace Tools.TweenSystem.Extensions
{
    public static class ElementExtensions
    {
        public static AnimSequence JiggleGrow(
           this Element el,
           Vector3? rotationStrength = null,
           Vector3? scaleUpValue = null,
           float? frequency = null,
           float? duration = null,
           Ease? ease = null,
           int cycles = 1,
           bool unScaledTime = true,
           float startDelay = 0f,
           float endDelay = 0f,
           System.Action<Element> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.ShakeRotation(strength: rotationStrength,
                duration: duration,
                ease: ease,
                frequency: frequency,
                startDelay: startDelay,
                endDelay: endDelay,
                unscaledTime: unScaledTime))
                .Also(el.ScaleUp(endValue: scaleUpValue,
                duration: duration,
                ease: ease,
                startDelay: startDelay,
                endDelay: endDelay,
                unScaledTime: unScaledTime));

            seq.SetLoops(cycles);

            if (onComplete != null)
                seq.OnComplete(el, onComplete);

            return seq;
        }

        public static AnimSequence GrowSlideIn(
            this Element el,
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<Element> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.SlideIn(direction: direction,
                slideDistance: slideDistance,
                duration: duration,
                ease: ease,
                startDelay: startDelay,
                endDelay: endDelay,
                unScaledTime: unScaledTime))
                .Also(el.Grow(duration: duration,
                ease: ease,
                startDelay: startDelay,
                endDelay: endDelay,
                unScaledTime: unScaledTime));

            seq.SetLoops(cycles);

            if (onComplete != null)
                seq.OnComplete(el, onComplete);

            return seq;
        }

        public static AnimSequence ShrinkSlideOut(this Element el,
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<Element> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.SlideOut(direction,
                slideDistance: slideDistance,
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime))
                .Also(el.Shrink(duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime));

            seq.SetLoops(cycles);

            if (onComplete != null)
            {
                seq.OnComplete(el, onComplete);
            }

            return seq;
        }

        public static AnimSequence RotateSlideIn(this Element el,
            Vector3? rotationValue = null,
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<Element> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.SlideIn(direction: direction,
                slideDistance: slideDistance,
                duration: duration,
                ease: ease,
                startDelay: startDelay,
                endDelay: endDelay,
                unScaledTime: unScaledTime))
                .Also(el.Rotation(endValue: rotationValue, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime));

            seq.SetLoops(cycles);

            if (onComplete != null)
            {
                seq.OnComplete(el, onComplete);
            }

            return seq;
        }

        public static AnimSequence RotateSlideOut(this Element el,
            Vector3? rotationValue = null,
            SlideDirection? direction = null,
            float? slideDistance = null,
            float? fadeEndValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            bool unScaledTime = true,
            float startDelay = 0f,
            float endDelay = 0f,
            System.Action<Element> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.SlideOut(direction: direction, 
                slideDistance: slideDistance, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay,
                unScaledTime: unScaledTime))
                .Also(el.Rotation(endValue: rotationValue, 
                duration: duration, 
                ease: ease, 
                startDelay: startDelay, 
                endDelay: endDelay, 
                unScaledTime: unScaledTime));

            seq.SetLoops(cycles);

            if (onComplete != null)
            {
                seq.OnComplete(el, onComplete);
            }

            return seq;
        }
    }
}

