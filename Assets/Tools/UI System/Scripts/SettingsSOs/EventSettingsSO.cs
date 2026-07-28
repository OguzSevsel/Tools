using UnityEngine;

namespace Tools.UISystem
{
    [CreateAssetMenu(fileName = "New Event Settings", menuName = "Animations/Event Settings", order = 1)]
    public class EventSettingsSO : ScriptableObject
    {
        public bool Interactable = true;
        public bool Selectable = true;
        public bool OnValueChanged = false;
    } 
}