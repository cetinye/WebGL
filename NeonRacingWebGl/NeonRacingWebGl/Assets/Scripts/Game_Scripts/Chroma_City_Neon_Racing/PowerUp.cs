using Chroma_City_Neon_Racing;
using UnityEngine;

namespace Chroma_City_Neon_Racing
{

    public class PowerUp : MonoBehaviour
    {
        [SerializeField] private PowerUps powerUps;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material powerUpMat;
        [SerializeField] private ParticleSystem wrongParticle;
        private bool isCollideable = true;

        void OnTriggerEnter(Collider other)
        {
            if (isCollideable && GameStateManager.GetGameState() == GameState.Racing && other.TryGetComponent<Player>(out Player player))
            {
                Debug.Log("Player PickedUp PowerUp");
                powerUps.Deactivate();

                if (powerUpMat.color == player.GetColor() || player.GetColor() == Color.white)
                {
                    AudioManager.instance.PlayOneShot(SoundType.CorrectPowerUp);
                    player.ChangeSpeed(true);
                }
                else
                {
                    if (!player.GetShieldState())
                    {
                        AudioManager.instance.PlayOneShot(SoundType.WrongPowerUp);
                        player.ChangeSpeed(false);
                        Instantiate(wrongParticle, transform.position, Quaternion.identity, transform);
                    }
                    else
                    {
                        // Shield protected
                    }
                }

                meshRenderer.enabled = false;
            }
        }

        public void SetIsCollideable(bool state)
        {
            isCollideable = state;
        }

        public void Reset()
        {
            isCollideable = true;
            meshRenderer.enabled = true;
        }
    }
}