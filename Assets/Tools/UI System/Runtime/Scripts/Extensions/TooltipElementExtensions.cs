using PrimeTween;
using Utilities;
using Elements;

namespace Extensions
{
    public static class TooltipElementExtensions
    {
        public static AnimSequence FlashColor(
            this TooltipElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<TooltipElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.Color(
                    endValue: endValue,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime))
                .Next(el.Color(
                    endValue: el.AnimationSettings.DefaultColor,
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

        public static AnimSequence FlashContentColor(
            this TooltipElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<TooltipElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.ContentColor(
                    endValue: endValue,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime))
                .Next(el.ContentColor(
                    endValue: el.AnimationSettings.DefaultColor,
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

        public static AnimSequence FlashHeaderColor(
            this TooltipElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<TooltipElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.HeaderColor(
                    endValue: endValue,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime))
                .Next(el.HeaderColor(
                    endValue: el.AnimationSettings.DefaultColor,
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
    }
}


