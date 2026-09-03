using UnityEngine;

namespace Utilities
{
    public class FPSCounter : MonoBehaviour
    {
        public static FPSCounter Instance;

        public float updateInterval = 0.5f;

        private float accum;
        private int frames;
        private float timeLeft;
        private GUIStyle fpsStyle;

        public float CurrentFPS { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            timeLeft = updateInterval;
        }

        void Update()
        {
            timeLeft -= Time.unscaledDeltaTime;
            accum += Time.unscaledDeltaTime;
            frames++;

            if (timeLeft <= 0f)
            {
                CurrentFPS = frames / accum;

                timeLeft = updateInterval;
                accum = 0f;
                frames = 0;
            }
        }

        void OnGUI()
        {
            int fps = (int)CurrentFPS;

            if (fpsStyle == null)
            {
                fpsStyle = new GUIStyle(GUI.skin.label);
                fpsStyle.normal.textColor = Color.blueViolet;
                fpsStyle.fontSize = 24;
            }

            GUI.Label(new Rect(10, 10, 150, 50), $"FPS: {fps}", fpsStyle);
        }
    } 
}
