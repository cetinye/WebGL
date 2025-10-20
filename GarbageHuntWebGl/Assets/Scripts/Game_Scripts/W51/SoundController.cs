using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    public enum SoundType
    {
        Ambient,
        HookStart,
        HookDuring,
        FishCaught,
        GarbageCaught,
        GarbageCollected
    }
    public class SoundController : MonoBehaviour
    {
        [SerializeField] private AudioSource[] _ambientSounds;
        [SerializeField] private AudioSource _hookStartSound;
        [SerializeField] private AudioSource _hookSound;
        [SerializeField] private AudioSource _fishCaughtSound;
        [SerializeField] private AudioSource _garbageCollectedSound;
        [SerializeField] private AudioSource[] _garbageCaughtSounds;
        [SerializeField] private AudioSource[] _comboSounds;

        public void PlaySound(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Ambient:
                    foreach (var s in _ambientSounds)
                        s.Play();
                    break;
                case SoundType.HookStart:
                    _hookStartSound.Play();
                    break;
                case SoundType.HookDuring:
                    _hookSound.Play();
                    break;
                case SoundType.FishCaught:
                    _fishCaughtSound.Play();
                    break;
                case SoundType.GarbageCaught:
                    var randomIndex = UnityEngine.Random.Range(0, _garbageCaughtSounds.Length);
                    _garbageCaughtSounds[randomIndex].Play();
                    break;
                case SoundType.GarbageCollected:
                    _garbageCollectedSound.Play();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null);
            }
        }

        public void PlayComboSound(int combo)
        {
            switch (combo)
            {
                case 0:
                    break;
                case 1:
                    _garbageCollectedSound.Play();
                    break;
                case >1:
                    _comboSounds[Mathf.Clamp(combo - 2, 0,  _comboSounds.Length-1)].Play();
                    break;
            }
        }
        
        public void StopSound(SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Ambient:
                    foreach (var s in _ambientSounds)
                        s.Stop();
                    break;
                case SoundType.HookStart:
                    _hookStartSound.Stop();
                    break;
                case SoundType.HookDuring:
                    _hookSound.Stop();
                    break;
                case SoundType.FishCaught:
                    _fishCaughtSound.Stop();
                    break;
                case SoundType.GarbageCaught:
                    foreach (var s in _garbageCaughtSounds)
                        s.Stop();
                    break;
                case SoundType.GarbageCollected:
                    _garbageCollectedSound.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(soundType), soundType, null);
            }
        }

        public void StopAllSounds()
        {
            foreach (var s in _ambientSounds)
                s.Stop();
            _hookStartSound.Stop();
            _hookSound.Stop();
            _fishCaughtSound.Stop();
            foreach (var s in _garbageCaughtSounds)
                s.Stop();
            _garbageCollectedSound.Stop();
        }
    }
}
