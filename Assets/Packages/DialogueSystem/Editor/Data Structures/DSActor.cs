using UnityEngine;

namespace Tools.DialogueSystem.Elements
{
    public class DSActor : ScriptableObject
    {
        [SerializeField] private string Name;
        [TextArea, SerializeField] private string background;
        [SerializeField] private Sprite sprite;
    }
}
