using System;
using UnityEngine;
using Tools.AutoTagSystem;
using TMPro;

namespace Game
{
    public class Deneme : MonoBehaviour
    {
        private string WordDeneme { get; set; }
        
        private void Awake()
        {
            var textComp = GetComponent<TextMeshProUGUI>();
            var word = AutoTag.Instance.SetAutoTags(textComp.text);
            textComp.text = word;
            textComp.fontSize = 20;
        }
        
        private void DenemeMethod()
        {
            WordDeneme = "deneme";
            var textComp = GetComponent<TextMeshProUGUI>();
            textComp.text = WordDeneme;
        }
        
    }
}
