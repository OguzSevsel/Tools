using System.Collections.Generic;
using Tools.DialogueSystem.Elements;
using Tools.DialogueSystem.UI;
using UnityEngine;
using System;
using UnityEditor;
using Tools.DialogueSystem.Data;

namespace Tools.DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Database")]
    public class DialogueDatabase : ScriptableObject
    {
        [SerializeField]
        private List<DSDialogueNodeData> nodes = new();

        private HashSet<string> _guidLookup;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            _guidLookup = new HashSet<string>();

            foreach (var node in nodes)
            {
                if (!string.IsNullOrEmpty(node.Guid))
                    _guidLookup.Add(node.Guid);
            }
        }

        public bool Contains(string guid)
        {
            return _guidLookup.Contains(guid);
        }

        public string GenerateUniqueGuid()
        {
            string guid;

            do
            {
                guid = Guid.NewGuid().ToString("N");
            }
            while (_guidLookup.Contains(guid));

            return guid;
        }

        public void Register(DSDialogueNodeData node)
        {
            if (Contains(node.Guid))
            {
                throw new InvalidOperationException(
                    $"Dialogue node GUID already exists: {node.Guid}"
                );
            }

            nodes.Add(node);
            _guidLookup.Add(node.Guid);

            EditorUtility.SetDirty(this);
        }

        public void Unregister(DSDialogueNodeData node)
        {
            if (!nodes.Remove(node))
                return;

            _guidLookup.Remove(node.Guid);

            EditorUtility.SetDirty(this);
        }
    }
}
