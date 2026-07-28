using UnityEngine;
using UnityEngine.Audio;

namespace Tools.AudioSystem
{
    [CreateAssetMenu(fileName = "New Sound", menuName = "Audio/Sound Data")]
    public class SoundData : ScriptableObject
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 2f)] public float pitch = 1f;
        [Range(0f, 1f)] public float spatialSound = 0f;
        public bool loop = false;
        public AudioMixerGroup mixerGroup;
        public bool randomizePitch = false;
        [Range(0f, 0.3f)] public float pitchVariance = 0.1f;
    } 
}