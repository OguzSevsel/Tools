using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

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

            var src = _pool.Get();

            src.spatialBlend = data.spatialSound;
            src.clip = data.clip;
            src.volume = data.volume;
            src.pitch = data.randomizePitch
                               ? data.pitch + Random.Range(-data.pitchVariance, data.pitchVariance)
                               : data.pitch;
            src.loop = data.loop;
            src.outputAudioMixerGroup = data.mixerGroup;

            if (position.HasValue) src.transform.position = position.Value;

            src.Play();
            if (!data.loop) StartCoroutine(ReturnWhenDone(src, data.clip.length / src.pitch));
            return src;
        }

        public void Stop(AudioSource src) => _pool.Return(src);

        public void SetVolume(string group, float normalizedValue)
        {
            float db = normalizedValue > 0.001f
                       ? Mathf.Log10(normalizedValue) * 20f
                       : -80f;
            _masterMixer.SetFloat(group, db);
        }

        private IEnumerator ReturnWhenDone(AudioSource src, float delay)
        {
            yield return Tools.Utilities.Helpers.GetWait(delay);
            if (src.gameObject.activeSelf) _pool.Return(src);
        }
    } 
}