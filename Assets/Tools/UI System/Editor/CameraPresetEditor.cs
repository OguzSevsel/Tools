using UnityEditor;
using UnityEngine.UIElements;

namespace Tools.UISystem
{
    [CustomEditor(typeof(CameraSettingsSO))]
    public class CameraPresetEditor : Editor
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
