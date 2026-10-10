using System;
using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    public class DSDatabaseManager
    {
        private static DSDatabase _current;

        public static DSDatabase Current => _current;

        public static event Action<DSDatabase> DatabaseOpened;
        public static event Action<DSDatabase> DatabaseClosed;

        public static bool HasDatabase =>
            _current != null;

        public static void Open(DSDatabase database)
        {
            _current = database;
            DatabaseOpened?.Invoke(database);
        }

        public static void Close()
        {
            DatabaseClosed?.Invoke(_current);
            _current = null;
        }
    }
}