using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Guess_The_Move
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        public List<Sound> sounds = new();

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;

            foreach (var s in sounds)
                if (s.clip != null)
                {
                    s.source = gameObject.AddComponent<AudioSource>();
                    s.source.clip = s.clip;
                    s.source.volume = s.volume;
                    s.source.loop = s.loop;
                }
                else
                {
                    Debug.LogWarning("Sound clip is missing for: " + s.name);
                }
        }

        private void OnDestroy()
        {
            instance = null;
        }

        public void Play(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            if (sound != null && sound.source != null)
                sound.source.Play();
            else
                Debug.LogWarning("Sound or AudioSource is null for: " + name);
        }

        public void PlayOneShot(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            if (sound != null && sound.source != null)
                sound.source.PlayOneShot(sound.clip);
            else
                Debug.LogWarning("Sound or AudioSource is null for: " + name);
        }

        public void PlayIf(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            if (!sound.source.isPlaying)
                sound.source.Play();
        }

        public void Stop(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            if (sound != null && sound.source != null)
                sound.source.Stop();
            else
                Debug.LogWarning("Sound or AudioSource is null for: " + name);
        }

        public AudioSource GetSoundSource(SoundType name)
        {
            var sound = sounds.Find(sound => sound.name == name);
            return sound.source;
        }

        public void FadeTo(SoundType name, float target, float time)
        {
            var sound = sounds.Find(sound => sound.name == name);
            var initialVolume = 1f;
            sound.source.DOFade(target, time).OnComplete(() =>
            {
                sound.source.Stop();
                sound.source.volume = initialVolume;
            });
        }
    }

    public enum SoundType
    {
        BackgroundAmbient,
        BackgroundMusic,
        ButtonClick,
        Correct,
        Wrong,
        Slide,
        FootSound,
        CrowdCheer
    }

    [Serializable]
    public class Sound
    {
        public SoundType name;
        public AudioClip clip;

        [Range(0f, 1f)] public float volume;

        public bool loop;

        [HideInInspector] public AudioSource source;
    }
}