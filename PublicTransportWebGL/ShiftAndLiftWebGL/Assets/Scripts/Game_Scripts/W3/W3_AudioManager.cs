using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace ShiftAndLift
{
    public class W3_AudioManager : MonoBehaviour
    {
        public static W3_AudioManager instance;
        public List<Sound> sounds = new List<Sound>();

        void Awake()
        {
            instance = this;

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.loop = s.loop;
            }
        }

        public void Play(string name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.Play();
        }


        public void PlayOneShot(string name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.PlayOneShot(sound.clip);
        }

        public void Stop(string name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.Stop();
        }

        public void SetVolume(string name, float volume)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.volume = volume;
        }

        public void FadeOutSound(string name, float time)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.DOFade(0, time);
        }
    }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
}