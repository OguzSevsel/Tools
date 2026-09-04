using System;
using System.Collections.Generic;

namespace Tools.SaveSystem
{
    [Serializable]
    public class GameData
    {
        public Dictionary<string, object> savedObjects = new Dictionary<string, object>();
        public SaveMetadata metadata;
    }
}