using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using W57;

namespace Game_Scripts.W57
{
    public class W57_Cable : MonoBehaviour
    {
        [SerializeField] private Image warningLight;
        [SerializeField] private GameObject electricFlow;
        [SerializeField] private GameObject sparkExplosion;
        [SerializeField] private W57_LevelManager levelManager;

        public List<LandingPosition> landingPositions = new List<LandingPosition>();
        public List<Transform> cablePos = new List<Transform>();
        public List<W57_Bird> birdsOnCable = new List<W57_Bird>();

        private bool isOverflowing;
        private float overflowDelayAfterWarning = 0.5f;

        [Serializable]
        public class LandingPosition
        {
            public W57_Cable parentCable;
            public Transform landingTransform;
            public bool isOccupied;
        }

        private Sequence flicker;

        private void Start()
        {
            InitFlickerLightSequence();
        }

        public void StartOverflowSequence(float duration)
        {
            if (isOverflowing) return;

            isOverflowing = true;
            levelManager.PlaySound(eW57FxSoundStates.ELECTRIC);

            sparkExplosion.SetActive(true);
            flicker.Restart();
            flicker.Play();
            flicker.OnComplete(() =>
            {
                SendElectric(duration);
                flicker.Rewind();
                sparkExplosion.SetActive(false);
            });
        }

        private void SendElectric(float duration)
        {
            var posses = UpdateCablePath().ToArray();

            electricFlow.SetActive(false);
            electricFlow.transform.position = posses[0];
            electricFlow.SetActive(true);

            electricFlow.transform.DOPath(posses, duration, PathType.Linear, PathMode.Sidescroller2D)
                .OnStart(() =>
                {
                    foreach (var bird in birdsOnCable)
                    {
                        bird.OverflowingStarted();
                    }
                })
                .OnComplete(() =>
                {
                    foreach (var bird in birdsOnCable)
                    {
                        bird.OverflowingCompleted();
                    }
                    isOverflowing = false;
                });
        }

        private List<Vector3> UpdateCablePath()
        {
            List<Vector3> cablePath = new List<Vector3>();

            for (int i = 0; i < cablePos.Count; i++)
            {
                cablePath.Add(cablePos[i].position);
            }

            cablePath.Reverse();
            return cablePath;
        }

        private void InitFlickerLightSequence()
        {
            flicker = DOTween.Sequence();
            flicker.Pause();
            flicker.Append(warningLight.DOColor(Color.red, 0.1f));
            flicker.Append(warningLight.DOColor(Color.white, 0.05f));
            flicker.Append(warningLight.DOColor(Color.red, 0.15f));
            flicker.Join(warningLight.DOFade(0.5f, 0.1f));
            flicker.Append(warningLight.DOFade(1f, 0.1f));
            flicker.AppendInterval(overflowDelayAfterWarning);
        }
    }
}