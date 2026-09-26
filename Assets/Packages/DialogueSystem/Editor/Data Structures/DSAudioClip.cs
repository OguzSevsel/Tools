using System;
using UnityEditor;
using UnityEngine;

namespace Tools.DialogueSystem.Data
{
    public class DSAudioClip : DSData
    {
        public AudioClip Clip;

        public DSAudioClip(string name, AudioClip clip)
        {
            this.Name = name;
            this.Clip = clip;
        }
    }
}