using UnityEditor;
using UnityEngine.UIElements;
using Tools.UISystem.Settings;

namespace Tools.UISystem.EditorTools
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
