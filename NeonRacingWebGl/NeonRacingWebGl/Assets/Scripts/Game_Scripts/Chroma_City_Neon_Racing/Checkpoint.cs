using System.Collections.Generic;
using Lean.Localization;
using UnityEngine;

namespace Chroma_City_Neon_Racing
{
    public class Checkpoint : MonoBehaviour
    {
        public Material colorMat;
        [SerializeField] private float emissionIntensity;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private List<Color> colors = new();

        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private Mesh engMesh;
        [SerializeField] private Mesh trMesh;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && GameStateManager.GetGameState() == GameState.Racing)
            {
                AudioManager.instance.PlayOneShot(SoundType.Checkpoint);
                Debug.Log("Player Passed Checkpoint");
                player.SetColor(colorMat.color);
            }
        }

        public void Initialize()
        {
            colorMat = meshRenderer.materials[1];

            if (LeanLocalization.Instances[0].CurrentLanguage == "Turkish")
            {
                meshFilter.mesh = trMesh != null ? trMesh : engMesh;
            }
            else
            {
                meshFilter.mesh = engMesh;
            }

            Invoke(nameof(DisablePowerUps), 2f);
        }

        public void SetRandomColor()
        {
            colorMat.color = colors[Random.Range(0, colors.Count)];
            meshRenderer.materials[1] = colorMat;
            colorMat.SetColor(Shader.PropertyToID("_EmissionColor"), colorMat.color * emissionIntensity);
        }

        public Color GetRandomColor()
        {
            return colors[Random.Range(0, colors.Count)];
        }

        private void DisablePowerUps()
        {
            var hits = Physics.OverlapBox(transform.localPosition, transform.localScale / 20f, Quaternion.identity,
                Physics.AllLayers, QueryTriggerInteraction.Collide);
            foreach (var hitObject in hits)
                // Debug.LogWarning("Hit:  " + hitObject.gameObject.name);
                if (hitObject.TryGetComponent(out PowerUps powerUp))
                    powerUp.gameObject.SetActive(false);
            // Debug.LogWarning("Disabled: " + powerUp.gameObject.name);
        }
    }
}