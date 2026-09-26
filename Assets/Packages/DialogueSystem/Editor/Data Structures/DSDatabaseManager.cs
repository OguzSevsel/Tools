using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    public class DSDatabaseManager
    {
        private static DSDatabase _current;

        public static DSDatabase Current => _current;

        public static bool HasDatabase =>
            _current != null;

        public static void Open(DSDatabase database)
        {
            _current = database;
        }

        public static void Close()
        {
            _current = null;
        }
    }
}