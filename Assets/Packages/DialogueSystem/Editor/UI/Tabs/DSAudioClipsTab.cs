using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.DialogueSystem.UI
{
    public class DSAudioClipsTab : Tab
    {
        VisualElement card;

        public DSAudioClipsTab(VisualTreeAsset tabAsset, VisualTreeAsset cardAsset)
        {
            tabAsset.CloneTree(this);
            label = "Audio Clips";
        }
    }
}