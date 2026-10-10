using TMPro;
using UnityEngine;

namespace Tools.AutoTagSystem
{
    public class AutoTag : MonoBehaviour
    {
        [SerializeField] private TMP_StyleSheet styleSheet;
        [SerializeField] private KeywordsToTag keywordsToTag;

        public static AutoTag Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public string SetAutoTags(string textBoxText)
        {
            foreach (var keyword in keywordsToTag.Keywords)
            {
                if (styleSheet.GetStyle(keyword) == null)
                {
                    Debug.Log($"Text style needed for keyword {keyword}");
                }

                if (textBoxText.Contains(keyword))
                {
                    return textBoxText.Replace($"{keyword}", $"<style=\"{keyword}\">{keyword}</style>");
                }

                string lowercaseText = textBoxText.ToLower();
                string lowerKeyword = keyword.ToLower();

                if (lowercaseText.Contains(lowerKeyword))
                {
                    return textBoxText.Replace($"{lowerKeyword}", $"<style=\"{keyword}\">{keyword}</style>");
                }
            }

            return textBoxText;
        }
    }
}