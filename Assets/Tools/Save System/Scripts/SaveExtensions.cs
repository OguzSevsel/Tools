using UnityEngine;

namespace Tools.SaveSystem
{
    public static class SaveExtensions
    {
        public static TransformData ToSaveData(this Transform t) => new TransformData(t);

    }
}

