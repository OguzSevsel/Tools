using System.Collections.Generic;
using UnityEngine;

namespace Tools.AudioSystem
{
    [CreateAssetMenu(fileName = "New Sound Library", menuName = "Audio/Sound Library")]
    public class SoundLibrary : ScriptableObject
    {
        [System.Serializable]
        public class SoundEntry
        {
            public string key;
            public SoundData data;
        }

        public List<SoundEntry> sounds;
        private Dictionary<string, SoundData> _lookup;

        public void Init()
        {
            _lookup = new Dictionary<string, SoundData>();
            foreach (var entry in sounds)
                _lookup[entry.key] = entry.data;
        }

        public SoundData Get(string key)
        {
            _lookup.TryGetValue(key, out var data);
            return data;
        }
    } 
}