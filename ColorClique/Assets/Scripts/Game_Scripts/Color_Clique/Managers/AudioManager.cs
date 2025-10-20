using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Audio;

namespace Color_Clique
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        public List<Sound> sounds = new List<Sound>();
        public AudioMixerGroup tickMixer;

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

        public void Play(SoundType name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.Play();
        }

        public void PlayOneShot(SoundType name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.PlayOneShot(sound.clip);
        }

        public void PlayIf(SoundType name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            if (!sound.source.isPlaying)
                sound.source.Play();
        }

        public void PlayAt(SoundType name, float startTime)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.time = startTime;
            sound.source.Play();
        }

        public void PlayCorrect(int comboValue)
        {
            SoundType name = (SoundType)comboValue;
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.PlayOneShot(sound.clip);
        }

        public void PlayTickSpeed(float speedMultiplier)
        {
            SoundType name = SoundType.Tick;
            Sound sound = sounds.Find(sound => sound.name == name);

            if (!sound.source.isPlaying)
                sound.source.Stop();

            sound.source.pitch = speedMultiplier;
            sound.source.outputAudioMixerGroup = tickMixer;
            sound.source.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1f / speedMultiplier);
            sound.source.Play();
        }

        public void Stop(SoundType name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.Stop();
        }

        public AudioSource GetSoundSource(SoundType name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            return sound.source;
        }

        public void FadeTo(SoundType name, float target, float time)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            float initialVolume = 1f;
            sound.source.DOFade(target, time).OnComplete(() =>
            {
                sound.source.Stop();
                sound.source.volume = initialVolume;
            });
        }

        public void PlayAfterXSeconds(SoundType name, float timeToWait)
        {
            StartCoroutine(DelayedPlay(name, timeToWait));
        }

        IEnumerator DelayedPlay(SoundType name, float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.Play();
        }
    }

    public enum SoundType
    {
        Background = 9,
        Ambient = 10,
        Tick = 11,
        Clap = 12,
        Shout = 13,
        Wrong = 14,
        CurtainOpen = 15,
        Confetti = 16,
        Combo1 = 1,
        Combo2 = 2,
        Combo3 = 3,
        Combo4 = 4,
        Combo5 = 5,
        Combo6 = 6,
        Combo7 = 7,
        Combo8 = 8,
    }

    [System.Serializable]
    public class Sound
    {
        public SoundType name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
}