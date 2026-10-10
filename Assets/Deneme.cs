using UnityEngine;

namespace Game
{
    internal enum DenemeEnum
    {
        World,
        Moon,
        Mars,
        Jupiter,
        Mercury,
    }
    public class Deneme : MonoBehaviour
    {
        [SerializeField, EnumButtons] private DenemeEnum deneme;

        private void Awake() => Application.targetFrameRate = 60;
    }
}
