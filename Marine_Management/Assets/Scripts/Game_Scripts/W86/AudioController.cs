using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_MarineManagement
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource _ambientSource;
        [SerializeField] private AudioSource _themeSource;
        [SerializeField] private AudioSource _introSource;
        [SerializeField] private AudioSource _fxSource;
        [SerializeField] private AudioSource _serviceFxSource;
        [SerializeField] private List<AudioFxData> _audioFxData;
        [SerializeField] private List<ServiceAudioFxData> _serviceAudioFxData;

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
        
        public void PlayIntro()
        {
            _themeSource.Play();
            _introSource.Play();
        }

        public void PlayAmbient()
        {
            _ambientSource.Play();
        }

        public void StopIntro()
        {
            _introSource.Stop();
        }

        public void PlayFx(AudioFxType fx, float delay = 0f)
        {
            var audioData = _audioFxData.FirstOrDefault(a => a.Type == fx);
            if (audioData == null)
            {
                Debug.LogError($"audio fx type {fx} could not be found in AudioController");
                return;
            }

            if (delay == 0f)
                _fxSource.PlayOneShot(audioData.Clip);
            else
                StartCoroutine(PlayDelayedRoutine(audioData.Clip, delay));
        }
        
        public void PlayFx(RequestType rt, float delay = 0f)
        {
            var audioData = _serviceAudioFxData.FirstOrDefault(a => a.Type == rt);
            if (audioData == null)
            {
                Debug.LogError($"audio fx type {rt} could not be found in AudioController");
                return;
            }

            if (delay == 0f)
                _serviceFxSource.PlayOneShot(audioData.Clip);
            else
                StartCoroutine(PlayDelayedRoutine(audioData.Clip, delay));
        }

        private IEnumerator PlayDelayedRoutine(AudioClip audioClip, float delay)
        {
            yield return new WaitForSeconds(delay);
            _fxSource.PlayOneShot(audioClip);
        }
    }
}