using UnityEditor;
using UnityEngine.UIElements;
using Tools.TweenSystem.Settings;

namespace Tools.TweenSystem.EditorTools
{
    [CustomEditor(typeof(CameraSettingsSO))]
    public class CameraPresetEditor : Editor
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
