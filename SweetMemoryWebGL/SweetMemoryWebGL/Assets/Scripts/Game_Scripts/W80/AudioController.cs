using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_SweetMemory
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance = null;

        [SerializeField] private AudioSource _backgroundMusic;
        [SerializeField] private AudioSource _backgroundRain;
        [SerializeField] private AudioClip _introMusic;
        [SerializeField] private AudioClip _rain;
        [SerializeField] private List<AudioClip> _sounds;

        private AudioSource _audioSource;
        private bool _started;

        public void Awake()
        {
            Instance = this;
            _audioSource = GetComponent<AudioSource>();
            _started = false;
        }

        public void Update()
        {
            if (!_started)
                return;
            if (!_backgroundMusic.isPlaying)
            {
                _backgroundMusic.PlayOneShot(_backgroundMusic.clip);
                Instance._backgroundRain.PlayOneShot(Instance._rain, 0.6f);
            }
        }

        public static void PlayIntro()
        {
            if (!Instance)
                return;

            Instance._audioSource.PlayOneShot(Instance._introMusic, 0.5f);
            Instance._backgroundRain.PlayOneShot(Instance._rain, 0.4f);
        }

        public static void ToggleRain(bool active)
        {
            if (!Instance)
                return;

            var rain = Instance._backgroundRain;
            if (active)
            {
                if (!rain.isPlaying)
                    rain.PlayOneShot(Instance._rain, 0.4f);
            }
            else
            {
                if (rain.isPlaying)
                    Instance._backgroundRain.Stop();
            }


        }

        public static void PlayBG()
        {
            if (!Instance)
                return;

            Instance._audioSource.Stop();
            Instance._backgroundRain.Stop();
            Instance._backgroundMusic.Stop();
            Instance._backgroundMusic.PlayOneShot(Instance._backgroundMusic.clip, 0.8f);
            Instance._backgroundRain.PlayOneShot(Instance._rain, 0.4f);
            Instance._started = true;
        }

        public static void Play(AudioType audioType, float volume = 1f)
        {
            if (!Instance)
                return;

            Instance._audioSource.PlayOneShot(Instance._sounds[(int)audioType], volume);
        }

        public static void Stop()
        {
            if (!Instance)
                return;

            Instance._audioSource.Stop();
        }
    }
}

