using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tools.Utilities
{
    public static class Helpers
    {
        public static float Clamp01(float value, float maxValue)
        {
            float ratio = 0f;

            if (value < maxValue)
            {
                ratio = value / maxValue;
            }
            else
            {
                ratio = 1f;
            }

            return ratio;
        }

        public static double Clamp01(float value, double maxValue)
        {
            double ratio = 0f;
            double doubleValue = (double)value;

            if (doubleValue < maxValue)
            {
                ratio = doubleValue / maxValue;
            }
            else
            {
                ratio = 1d;
            }

            return ratio;
        }

        public static float Clamp01(double value, double maxValue)
        {
            double ratio = 0f;

            if (value < maxValue)
            {
                ratio = value / maxValue;
            }
            else
            {
                ratio = 1d;
            }

            return (float)ratio;
        }

        private static Camera _camera;
        public static Camera Camera
        {
            get
            {
                if (_camera == null) { _camera = Camera.main; }
                return _camera;
            }
        }

        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }

        private static PointerEventData _eventDataCurrentPosition;
        private static List<RaycastResult> _results;
        public static bool IsOverUI()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
            return _results.Count > 0;
        }

        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
            return result;
        }

        public static void DeleteChildren(this Transform t)
        {
            foreach (Transform child in t)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    } 
}
