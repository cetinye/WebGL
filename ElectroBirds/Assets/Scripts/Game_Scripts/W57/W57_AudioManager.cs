using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using W57;



public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;
        public List<Sound> sounds = new List<Sound>();
    // public AudioMixerGroup tickMixer;

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

        Debug.Log("AudioManager initialized with " + sounds.Count + " sounds.");

        Debug.Log("Sounds in AudioManager:");
        foreach (var sound in sounds)
        {
            Debug.Log($"Sound Name: {sound.name}, Clip: {sound.clip?.name}, Volume: {sound.volume}, Loop: {sound.loop}");

        }
    }

        public void Play(eW57FxSoundStates name)
    {
        Sound sound = sounds.Find(sound => sound.name == name);
        sound.source.Play();
    }

        public void PlayOneShot(eW57FxSoundStates name)
        {
            Debug.LogError("Playing sound: " + name);
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.PlayOneShot(sound.clip);
        }

        public void PlayIf(eW57FxSoundStates name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            if (!sound.source.isPlaying)
                sound.source.Play();
        }

        public void PlayAt(eW57FxSoundStates name, float startTime)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.time = startTime;
            sound.source.Play();
        }

        public void PlayCorrect(int comboValue)
        {
            eW57FxSoundStates name = (eW57FxSoundStates)comboValue;
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.PlayOneShot(sound.clip);
        }

        // public void PlayTickSpeed(float speedMultiplier)
        // {
        //     SoundType name = SoundType.Tick;
        //     Sound sound = sounds.Find(sound => sound.name == name);

        //     if (!sound.source.isPlaying)
        //         sound.source.Stop();

        //     sound.source.pitch = speedMultiplier;
        //     sound.source.outputAudioMixerGroup = tickMixer;
        //     sound.source.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1f / speedMultiplier);
        //     sound.source.Play();
        // }

        public void Stop(eW57FxSoundStates name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.Stop();
        }

        public AudioSource GetSoundSource(eW57FxSoundStates name)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            return sound.source;
        }

        public void FadeTo(eW57FxSoundStates name, float target, float time)
        {
            Sound sound = sounds.Find(sound => sound.name == name);
            float initialVolume = 1f;
            sound.source.DOFade(target, time).OnComplete(() =>
            {
                sound.source.Stop();
                sound.source.volume = initialVolume;
            });
        }

        // public void PlayAfterXSeconds(eW57FxSoundStates name, float timeToWait)
        // {
        //     StartCoroutine(DelayedPlay(name, timeToWait));
        // }

        IEnumerator DelayedPlay(eW57FxSoundStates name, float timeToWait)
        {
            yield return new WaitForSeconds(timeToWait);
            Sound sound = sounds.Find(sound => sound.name == name);
            sound.source.Play();
        }
    }

   
    [System.Serializable]
    public class Sound
    {
        public eW57FxSoundStates name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }




