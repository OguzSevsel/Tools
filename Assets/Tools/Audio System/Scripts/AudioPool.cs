using System.Collections.Generic;
using UnityEngine;

namespace Tools.AudioSystem
{
    public class AudioPool : MonoBehaviour
    {
        [SerializeField] private int initialSize = 10;
        private Queue<AudioSource> _pool = new();

        public void Init()
        {
            for (int i = 0; i < initialSize; i++)
                _pool.Enqueue(CreateSource());
        }

        private AudioSource CreateSource()
        {
            var go = new GameObject("AudioSource_Pooled");
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            go.SetActive(false);
            return src;
        }

        public AudioSource Get()
        {
            var src = _pool.Count > 0 ? _pool.Dequeue() : CreateSource();
            src.gameObject.SetActive(true);
            return src;
        }

        public void Return(AudioSource src)
        {
            src.Stop();
            src.clip = null;
            src.gameObject.SetActive(false);
            _pool.Enqueue(src);
        }
    } 
}