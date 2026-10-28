using System;

namespace Tools.Core
{
    public class Timer
    {
        public float RemainingTime { get; private set; }
        public event Action OnTimerEnd;

        public Timer(float duration)
        {
            RemainingTime = duration;
        }

        public void Tick(float deltaTime)
        {
            if (RemainingTime == 0f) return;

            RemainingTime -= deltaTime;

            CheckForTimerEnd();
        }

        private void CheckForTimerEnd()
        {
            if (RemainingTime > 0f) return;

            RemainingTime = 0f;

            OnTimerEnd?.Invoke();
        }
    }
}