using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.SaveSystem
{
    [Serializable]
    public class GameData
    {
        [SerializeField] public Dictionary<string, object> savedObjects = new();
        [NonSerialized] public SaveMetadata metadata;
    }
}