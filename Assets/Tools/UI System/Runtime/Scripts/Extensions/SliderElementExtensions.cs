using PrimeTween;
using Utilities;
using Elements;

namespace Extensions
{
    public static class SliderElementExtensions
    {
        public static AnimSequence FlashBGColor(
            this SliderElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<SliderElement> onComplete = null)
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
            this SliderElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<SliderElement> onComplete = null)
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

        public static AnimSequence FlashHandleColor(
            this SliderElement el,
            UnityEngine.Color? endValue = null,
            float? duration = null,
            Ease? ease = null,
            int cycles = 1,
            float startDelay = 0f,
            float endDelay = 0f,
            bool unScaledTime = true,
            System.Action<SliderElement> onComplete = null)
        {
            var seq = el.Animate()
                .Begin(el.HandleColor(
                    endValue: endValue,
                    duration: duration,
                    ease: ease,
                    startDelay: startDelay,
                    endDelay: endDelay,
                    unScaledTime: unScaledTime))
                .Next(el.HandleColor(
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


