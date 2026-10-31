using System;
using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    public class DSDatabaseManager
    {
        public static DSDatabaseManager Instance;
        private DSDatabase _current;

        public DSDatabase Current => _current;

        public event Action<DSDatabase> DatabaseOpened;
        public event Action<DSDatabase> DatabaseClosed;

        public bool HasDatabase =>
            _current != null;

        public DSDatabaseManager()
        {
            Instance = this;
        }

        public void Open(DSDatabase database)
        {
            _current = database;
            DatabaseOpened?.Invoke(database);
        }

        public void Close()
        {
            DatabaseClosed?.Invoke(_current);
            Save();
            _current = null;
        }

        public void Save()
        {
            if (_current == null)
            {
                Debug.LogError("Current database is NULL!");
                return;
            }

            Debug.Log($"Saving database: {_current.name}");
            Debug.Log($"Database path: {AssetDatabase.GetAssetPath(_current)}");

            EditorUtility.SetDirty(_current);
            AssetDatabase.SaveAssets();
        }
    }
}