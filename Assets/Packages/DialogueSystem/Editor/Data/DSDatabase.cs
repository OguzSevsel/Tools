using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Database")]
    public class DSDatabase : ScriptableObject
    {
        [field: SerializeField] public List<DSConversation> Conversations { get; private set; } = new();
        [field: SerializeField] public List<DSActor> Actors { get; private set; }
        [field: SerializeField] public List<DSAudioClip> AudioClips { get; private set; } = new();
        [field: SerializeField] public List<DSDialogueText> DialogueTexts { get; private set; } = new();
        private HashSet<string> _guidLookup;

        private void OnEnable()
        {
            Actors ??= new List<DSActor>();
            DialogueTexts ??= new List<DSDialogueText>();
            BuildLookup();
        }

        private void BuildLookup()
        {
            _guidLookup = new HashSet<string>();

            foreach (DSActor actor in Actors)
            {
                if (!string.IsNullOrEmpty(actor.Guid))
                    _ = _guidLookup.Add(actor.Guid);
            }

            foreach (DSAudioClip audio in AudioClips)
            {
                if (!string.IsNullOrEmpty(audio.Guid))
                    _ = _guidLookup.Add(audio.Guid);
            }

            foreach (DSDialogueText dialogue in DialogueTexts)
            {
                if (!string.IsNullOrEmpty(dialogue.Guid))
                    _ = _guidLookup.Add(dialogue.Guid);
            }

            foreach (DSConversation conversation in Conversations)
            {
                if (!string.IsNullOrEmpty(conversation.Guid))
                    _ = _guidLookup.Add(conversation.Guid);
            }
        }

        public bool Contains(string guid) => _guidLookup.Contains(guid);

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
            _ = _guidLookup.Add(guid);

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
            else if (item is DSConversation conversation)
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
                    _ = DialogueTexts.Remove(text);
                    removed = true;
                }
            }
            else if (item is DSActor actor)
            {
                if (Actors.Contains(actor))
                {
                    _ = Actors.Remove(actor);
                    removed = true;
                }
            }
            else if (item is DSAudioClip clip)
            {
                if (AudioClips.Contains(clip))
                {
                    _ = AudioClips.Remove(clip);
                    removed = true;
                }
            }
            else if (item is DSConversation conversation)
            {
                if (Conversations.Contains(conversation))
                {
                    _ = Conversations.Remove(conversation);
                    removed = true;
                }
            }

            if (removed && _guidLookup.Contains(item.Guid))
            {
                _ = _guidLookup.Remove(item.Guid);
                EditorUtility.SetDirty(this);
            }
        }
    }
}
