using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_PaperCycle
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource _themeSource;
        [SerializeField] private AudioSource _introSource;
        [SerializeField] private AudioSource _fxSource;
        [SerializeField] private List<AudioFxData> _audioFxData;

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
        
        public void PlayIntro()
        {
            _introSource.Play();
        }

        public void PlayTheme()
        {
            _themeSource.Play();
        }

        public void StopIntro()
        {
            _introSource.Stop();
        }

        public void PlayFx(AudioFxType fx, float delay = 0f)
        {
            var audios = _audioFxData.Where(a => a.Type == fx).ToList();
            if (audios.Count < 1)
            {
                Debug.LogError($"audio fx type {fx} could not be found in AudioController");
                return;
            }

            var audioData = audios[Random.Range(0, audios.Count)];

            if (audioData.Clip == null)
                return;

            if (delay == 0f)
                _fxSource.PlayOneShot(audioData.Clip);
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
