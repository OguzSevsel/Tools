using UnityEditor;
using UnityEngine.UIElements;
using Tools.TweenSystem.Settings;
using System;

namespace Tools.TweenSystem.EditorTools
{
    [CustomEditor(typeof(AnimationSettingsSO))]
    public class AnimationSettingsEditor : Editor
    {
        public VisualTreeAsset visualTreeAsset;
        [System.NonSerialized] public VisualElement Root;
        [System.NonSerialized] public Foldout FadeFoldout;
        [System.NonSerialized] public FloatField FadeInFloatField;
        [System.NonSerialized] public FloatField FadeOutFloatField;

        public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
        {
            Root = new VisualElement();

            visualTreeAsset.CloneTree(Root);

            RegisterFields();

            return Root;
        }

        public void RegisterFields()
        {
            FadeFoldout = Root.Q<Foldout>("FadeFoldout");
            FadeInFloatField = Root.Q<FloatField>("FadeIn");
            FadeOutFloatField = Root.Q<FloatField>("FadeOut");
            FadeInFloatField.RegisterValueChangedCallback(ValueChangedHandler);
            FadeOutFloatField.RegisterValueChangedCallback(ValueChangedHandler);
        }

        private void ValueChangedHandler(ChangeEvent<float> evt)
        {
            if (evt.newValue > 1)
            {
                if (evt.currentTarget == FadeInFloatField)
                {
                    FadeInFloatField.value = 1;
                    return;
                }
                FadeOutFloatField.value = 1;
            }

            if (evt.newValue < 0)
            {
                if (evt.currentTarget == FadeInFloatField)
                {
                    FadeInFloatField.value = 0;
                    return;
                }
                FadeOutFloatField.value = 0;
            }
        }
    }
}



