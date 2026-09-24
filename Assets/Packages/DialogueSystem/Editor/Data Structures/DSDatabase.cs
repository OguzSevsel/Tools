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
    public class DSDatabase : ScriptableObject
    {
        public string Name;
        [SerializeField] private List<DSDialogueNodeData> nodes = new();
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

        public HashSet<AudioClip> GetAudioClips()
        {
            HashSet<AudioClip> audioClips = new HashSet<AudioClip>();

            foreach (var node in nodes)
            {
                audioClips.Add(node.AudioClip);    
            }

            return audioClips;
        }

        public HashSet<string> GetDialogueTexts()
        {
            HashSet<string> dialogueTexts = new HashSet<string>();

            foreach (var node in nodes)
            {
                dialogueTexts.Add(node.DialogueText);
            }

            return dialogueTexts;
        }

        public HashSet<DSActor> GetActors()
        {
            HashSet<DSActor> actors = new HashSet<DSActor>();

            foreach (var node in nodes)
            {
                actors.Add(node.Actor);
            }

            return actors;
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
