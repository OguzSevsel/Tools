using System.Collections.Generic;
using UnityEngine;

namespace Tools.SaveSystem
{
    public class SaveableRegistry : MonoBehaviour
    {
        public static SaveableRegistry Instance { get; private set; }
        private List<ISaveable> saveables;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            Instance.saveables = new List<ISaveable>();
        }

        public static void Register(ISaveable saveable)
        {
            if (!Instance.saveables.Contains(saveable))
                Instance.saveables.Add(saveable);
        }

        public static void Unregister(ISaveable saveable)
        {
            Instance.saveables.Remove(saveable);
        }

        public static IEnumerable<ISaveable> GetAllSaveables() => Instance.saveables;
    } 
}
