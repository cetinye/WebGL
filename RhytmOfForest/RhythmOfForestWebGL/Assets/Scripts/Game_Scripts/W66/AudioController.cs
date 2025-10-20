using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_rotf
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance = null;
        
        [SerializeField] private AudioSource _ambientSource;
        [SerializeField] private List<AudioClip> _mushroomNotes;

        private AudioSource _audioSource;

        private void Awake()
        {
            if(Instance)
                Destroy(Instance);

            Instance = this;
            
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayAmbient()
        {
            _ambientSource.Play();
        }

        public void PlayMushroomSound(int id)
        {
            _audioSource.PlayOneShot(_mushroomNotes[id]);
        }

        public void PlaySound(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Start:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null);
            }
        }
    }

    public enum SoundType
    {
        Start,
    }
}

