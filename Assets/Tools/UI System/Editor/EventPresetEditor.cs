using UnityEditor;
using UnityEngine.UIElements;
using Settings;

namespace Tools
{
    [CustomEditor(typeof(EventSettingsSO))]
    public class EventPresetEditor : Editor
    {
        public VisualTreeAsset visualTreeAsset;
        public VisualElement Root;

        public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
        {
            Root = new VisualElement();

            visualTreeAsset.CloneTree(Root);

            return Root;
        }
    }
}
