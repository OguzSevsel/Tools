using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.SaveSystem
{
    [Serializable]
    public class GameData
    {
        [SerializeField] public Dictionary<string, object> savedObjects = new Dictionary<string, object>();
        [NonSerialized] public SaveMetadata metadata;
    }
}