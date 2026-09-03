using UnityEngine;

namespace Utilities
{
    public static class RectTransformExtensions
    {
        public static Vector3 SnapBelow(this RectTransform target, RectTransform reference)
        {
            Vector3 refBottomWorld = reference.position - new Vector3(0, reference.rect.height * reference.pivot.y, 0);

            Vector3 targetPivotOffset = new Vector3(0, target.rect.height * (1 - target.pivot.y), 0);

            target.position = refBottomWorld - targetPivotOffset;

            return refBottomWorld - targetPivotOffset;
        }

        public static void SetHeight(this RectTransform rt, float height)
        {
            Vector2 size = rt.sizeDelta;
            size.y = height;
            rt.sizeDelta = size;
        }

        public static void SetWidth(this RectTransform rt, float width)
        {
            Vector2 size = rt.sizeDelta;
            size.x = width;
            rt.sizeDelta = size;
        }
    }
}
