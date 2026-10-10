using PrimeTween;
using Tools.TweenSystem.Utilities;
using Tools.TweenSystem.Elements;

namespace Tools.TweenSystem.Extensions
{
    public static class ProgressBarElementExtensions
    {
        public static AnimSequence FlashBGColor(
            this ProgressBarElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<ProgressBarElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.BGColor(
                    endValue: endValue,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime))
                .Next(el.BGColor(
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

        public static AnimSequence FlashFillColor(
            this ProgressBarElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<ProgressBarElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.FillColor(
                    endValue: endValue,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime))
                .Next(el.FillColor(
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

        public static AnimSequence FlashTextColor(
            this ProgressBarElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<ProgressBarElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.TextColor(
                    endValue: endValue,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime))
                .Next(el.TextColor(
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

