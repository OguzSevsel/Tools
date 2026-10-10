using Tools.TweenSystem.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tools.TweenSystem.EditorTools
{
    [CustomEditor(typeof(TextSettingsSO))]
    public class TextPresetEditor : Editor
    {
        public VisualTreeAsset visualTreeAsset;
        [System.NonSerialized] public VisualElement Root;

        public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
        {
            Root = new VisualElement();

            visualTreeAsset.CloneTree(Root);

            return Root;
        }
    }
}
