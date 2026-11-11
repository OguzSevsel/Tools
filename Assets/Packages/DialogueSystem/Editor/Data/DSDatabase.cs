using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
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

        public static event Action<DSData> OnDataRemoved;

        private void OnEnable()
        {
            Actors ??= new List<DSActor>();
            DialogueTexts ??= new List<DSDialogueText>();
            Conversations ??= new List<DSConversation>();
            AudioClips ??= new List<DSAudioClip>();
            BuildLookup();
        }

        private void BuildLookup()
        {
            _guidLookup = new HashSet<string>();

            foreach (DSActor actor in Actors)
            {
                if (!string.IsNullOrEmpty(actor.Guid))
                    _guidLookup.Add(actor.Guid);
            }

            foreach (DSAudioClip audio in AudioClips)
            {
                if (!string.IsNullOrEmpty(audio.Guid))
                    _guidLookup.Add(audio.Guid);
            }

            foreach (DSDialogueText dialogue in DialogueTexts)
            {
                if (!string.IsNullOrEmpty(dialogue.Guid))
                    _guidLookup.Add(dialogue.Guid);
            }

            foreach (DSConversation conversation in Conversations)
            {
                if (!string.IsNullOrEmpty(conversation.Guid))
                    _guidLookup.Add(conversation.Guid);
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

        public T Get<T>(string guid) where T : DSData
        {
            if (!Contains(guid))
                return null;

            var type = typeof(T);

            if (typeof(T) == typeof(DSActor))
            {
                foreach (DSActor item in Actors)
                {
                    if (item.Guid == guid)
                    {
                        return item as T;
                    }
                }
            }

            if (typeof(T) == typeof(DSDialogueText))
            {
                foreach (DSDialogueText item in DialogueTexts)
                {
                    if (item.Guid == guid)
                    {
                        return item as T;
                    }
                }
            }

            if (typeof(T) == typeof(DSAudioClip))
            {
                foreach (DSAudioClip item in AudioClips)
                {
                    if (item.Guid == guid)
                    {
                        return item as T;
                    }
                }
            }

            if (typeof(T) == typeof(DSConversation))
            {
                foreach (DSConversation item in Conversations)
                {
                    if (item.Guid == guid)
                    {
                        return item as T;
                    }
                }
            }

            return null;
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
            else if (item is DSConversation conversation)
            {
                Conversations.Add(conversation);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
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
            else if (item is DSConversation conversation)
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
                OnDataRemoved?.Invoke(item);
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
