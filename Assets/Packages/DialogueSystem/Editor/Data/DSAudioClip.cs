using System;
using Unity.Properties;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSAudioClip : DSData
    {
        [SerializeField, SerializeReference] private AudioClip _clip;

        [CreateProperty]
        public AudioClip Clip
        {
            get => _clip;
            private set
            {
                if (_clip == value)
                {
                    return;
                }

                _clip = value;
                Notify();
            }
        }

        public DSAudioClip(string title, string description, AudioClip clip) : base(title, description)
        {
            this.Clip = clip;
        }
    }
}