using Tools.TweenSystem.Elements;
using UnityEngine;

namespace Tools.TweenSystem.TooltipSystem
{
    public class TooltipSystem : MonoBehaviour
    {
        private static TooltipSystem Instance;
        public TooltipElement ToolTip;
        [SerializeField] private float startDelay = 1f;
        [SerializeField] private bool useFade = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                return;
            }

            Destroy(gameObject);
        }

        public static void Show(string content, string header = "")
        {
            Instance.ToolTip.SetText(content, header);

            if (Instance.useFade)
            {
                _ = Instance.ToolTip.FadeIn(startDelay: Instance.startDelay);
                return;
            }

            Instance.ToolTip.SetActive(true);
        }

        public static void Hide()
        {
            if (Instance.useFade)
            {
                _ = Instance.ToolTip.FadeOut();
                return;
            }

            Instance.ToolTip.SetActive(false);
        }
    }
}
