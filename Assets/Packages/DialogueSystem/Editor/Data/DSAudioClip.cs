using System;
using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    [Serializable]
    public class DSAudioClip : DSData
    {
        public AudioClip Clip;

        public DSAudioClip(string title, string description, AudioClip clip) : base(title, description)
        {
            this.Clip = clip;
        }
    }
}