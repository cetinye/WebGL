using System.Collections.Generic;
using UnityEngine;

namespace To_Be_Or_Not_To_Be
{
    public class W42_AudioController : MonoBehaviour
    {
        public static W42_AudioController instance;

        public List<Sound> sounds = new List<Sound>();

        // Start is called before the first frame update
        void Start()
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