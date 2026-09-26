using System;
using System.Collections.Generic;
using System.Linq;
using Tools.DialogueSystem.Data;
using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Database")]
    public class DSDatabase : ScriptableObject
    {
        public string Name;
        public HashSet<DSConversationData> Conversations { get; private set; } = new();
        public HashSet<DSActor> Actors { get; private set; } = new();
        public HashSet<DSAudioClip> AudioClips { get; private set; } = new();
        public HashSet<DSDialogueText> DialogueTexts { get; private set; } = new();
        private HashSet<string> _guidLookup;

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            _guidLookup = new HashSet<string>();

            foreach (var actor in Actors)
            {
                if (!string.IsNullOrEmpty(actor.Guid))
                    _guidLookup.Add(actor.Guid);
            }

            foreach (var audio in AudioClips)
            {
                if (!string.IsNullOrEmpty(audio.Guid))
                    _guidLookup.Add(audio.Guid);
            }

            foreach (var dialogue in DialogueTexts)
            {
                if (!string.IsNullOrEmpty(dialogue.Guid))
                    _guidLookup.Add(dialogue.Guid);
            }

            foreach (var conversation in Conversations)
            {
                if (!string.IsNullOrEmpty(conversation.Guid))
                    _guidLookup.Add(conversation.Guid);
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

        public string Register<T>(T item) where T : DSData
        {
            string guid = GenerateUniqueGuid();
            item.Guid = guid;
            _guidLookup.Add(guid);

            if (item is DSActor actor)
            {
                Actors.Add(actor);
            }
            else if (item is DSDialogueText text)
            {
                DialogueTexts.Add(text);
            }
            else if (item is DSAudioClip clip)
            {
                AudioClips.Add(clip);
            }
            else if (item is DSConversationData conversation)
            {
                Conversations.Add(conversation);
            }

            EditorUtility.SetDirty(this);
            return guid;
        }

        public void Unregister<T>(T item) where T : DSData
        {
            bool removed = false;

            if (item is DSDialogueText text)
            {
                if (DialogueTexts.Contains(text))
                {
                    DialogueTexts.Remove(text);
                    removed = true;
                }
            }
            else if (item is DSActor actor)
            {
                if (Actors.Contains(actor))
                {
                    Actors.Remove(actor);
                    removed = true;
                }
            }
            else if (item is DSAudioClip clip)
            {
                if (AudioClips.Contains(clip))
                {
                    AudioClips.Remove(clip);
                    removed = true;
                }
            }
            else if (item is DSConversationData conversation)
            {
                if (Conversations.Contains(conversation))
                {
                    Conversations.Remove(conversation);
                    removed = true;
                }
            }

            if (removed && _guidLookup.Contains(item.Guid))
            {
                _guidLookup.Remove(item.Guid);
                EditorUtility.SetDirty(this);
            }
        }
    }
}
