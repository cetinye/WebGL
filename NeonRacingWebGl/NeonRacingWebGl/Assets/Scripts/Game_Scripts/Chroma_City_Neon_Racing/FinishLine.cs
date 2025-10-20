using System.Collections;
using System.Collections.Generic;
using Chroma_City_Neon_Racing;
using DG.Tweening;
using Lean.Localization;
using UnityEngine;

namespace Chroma_City_Neon_Racing
{

    public class FinishLine : MonoBehaviour
    {
        [SerializeField] Material material;
        [SerializeField] float timeToColor;
        [SerializeField] float emissionIntensity;

        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private Mesh engMesh;
        [SerializeField] private Mesh trMesh;

        void OnEnable()
        {
            if (LeanLocalization.Instances[0].CurrentLanguage == "Turkish")
            {
                meshFilter.mesh = trMesh;
            }
            else
            {
                meshFilter.mesh = engMesh;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player>(out Player player) && GameStateManager.GetGameState() == GameState.Racing)
            {
                AudioManager.instance.PlayOneShot(SoundType.Finish);
                GameStateManager.SetGameState(GameState.Success);
                LevelManager.instance.LevelFinished();
            }
        }

        public void StartLightChange()
        {
            LightChange(material, timeToColor, emissionIntensity);
        }

        Sequence LightChange(Material material, float timeToColor, float emissionIntensity)
        {
            Sequence seq = DOTween.Sequence();

            Color[] colors = new Color[] { Color.black };
            int colorIndex = 0;

            seq.SetLoops(-1, LoopType.Yoyo);

            seq.Append(material.DOColor(colors[colorIndex], timeToColor).OnComplete(() =>
            {
                colorIndex = (colorIndex + 1) % colors.Length;
            }));
            seq.Join(material.DOColor(colors[colorIndex] * emissionIntensity, Shader.PropertyToID("_EmissionColor"), timeToColor));

            return seq;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable()
        {
            material.color = Color.white;
            material.SetColor(Shader.PropertyToID("_EmissionColor"), material.color * emissionIntensity);
        }

    }
}