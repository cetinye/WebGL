using System.Collections;
using System.Collections.Generic;
using Chroma_City_Neon_Racing;
using DG.Tweening;
using UnityEngine;

namespace Chroma_City_Neon_Racing
{
    public class TrafficLight : MonoBehaviour
    {
        public static TrafficLight instance;

        private int countdownTimer = 3;
        [SerializeField] private float emissionIntensity;
        [SerializeField] private float timeToColor;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material material;
        [SerializeField] private List<Color> colors = new List<Color>();

        private Sequence colorSequence;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Reset the color and countdown state
            InitializeTrafficLight();
        }

        void InitializeTrafficLight()
        {
            countdownTimer = 3;
            material.color = Color.red;
            material.SetColor(Shader.PropertyToID("_EmissionColor"), material.color * emissionIntensity);
        }

        void OnDestroy()
        {
            if (colorSequence != null && colorSequence.IsActive())
            {
                colorSequence.Kill();
            }
            StopAllCoroutines();
            instance = null; // Reset static instance reference
        }

        public void StartCountdown()
        {
            StopAllCoroutines();
            if (colorSequence != null && colorSequence.IsActive())
            {
                colorSequence.Kill();
            }
            countdownTimer = 3;
            StartCoroutine(CountdownRoutine());
        }

        IEnumerator CountdownRoutine()
        {
            yield return new WaitForSeconds(1f);
            AudioManager.instance.PlayAfterXSeconds(SoundType.Countdown, 0.35f);

            int colorIndex = 0;
            countdownTimer = 3;

            while (countdownTimer > 0)
            {
                countdownTimer--;
                colorSequence = ApplyColorSeq(colors[colorIndex]);
                colorIndex++;
                yield return colorSequence.WaitForCompletion();
                yield return new WaitForSeconds(1f);
            }
        }

        Sequence ApplyColorSeq(Color newColor)
        {
            colorSequence = DOTween.Sequence();
            colorSequence.Append(material.DOColor(newColor, timeToColor));
            colorSequence.Join(material.DOColor(newColor * emissionIntensity, Shader.PropertyToID("_EmissionColor"), timeToColor).OnComplete(() =>
            {
                if (countdownTimer == 0)
                {
                    GameStateManager.SetGameState(GameState.Racing);
                }
            }));
            return colorSequence;
        }
    }
}
