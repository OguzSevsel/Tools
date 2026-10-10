using Alchemy;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    enum DenemeEnum
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

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}
