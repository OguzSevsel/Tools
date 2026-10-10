using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Tools.Core;

namespace Tools.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private SoundLibrary _library;
        [SerializeField] private AudioMixer _masterMixer;
        private AudioPool _pool;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _pool = GetComponent<AudioPool>();
            _pool.Init();
            _library.Init();
        }

        public AudioSource Play(string key, Vector3? position = null)
        {
            var data = _library.Get(key);
            if (data == null) { Debug.LogWarning($"Sound '{key}' not found."); return null; }

            var source = _pool.Get();

            source.spatialBlend = data.spatialSound;
            source.clip = data.clip;
            source.volume = data.volume;
            source.pitch = data.randomizePitch
                               ? data.pitch + Random.Range(-data.pitchVariance, data.pitchVariance)
                               : data.pitch;
            source.loop = data.loop;
            source.outputAudioMixerGroup = data.mixerGroup;

            if (position.HasValue) source.transform.position = position.Value;

            source.Play();
            if (!data.loop) StartCoroutine(ReturnWhenDone(source, data.clip.length / source.pitch));
            return source;
        }

        public void Stop(AudioSource src) => _pool.Return(src);

        public void SetVolume(string group, float normalizedValue)
        {
            float db = normalizedValue > 0.001f
                       ? Mathf.Log10(normalizedValue) * 20f
                       : -80f;
            _masterMixer.SetFloat(group, db);
        }

        private IEnumerator ReturnWhenDone(AudioSource source, float delay)
        {
            yield return Helpers.GetWait(delay);
            if (source.gameObject.activeSelf) _pool.Return(source);
        }
    } 
}