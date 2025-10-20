using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chroma_City_Neon_Racing
{

    public class SpecialPowerUp : MonoBehaviour
    {
        [SerializeField] private PowerUpType type;
        [SerializeField] private float rotateAmount;
        private float durationOfPowerups;
        private Player player;
        private float addSpeedAmount;
        private float timeToAdd;

        void Update()
        {
            transform.Rotate(0, 0, rotateAmount * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player>(out Player player) && GameStateManager.GetGameState() == GameState.Racing)
            {
                this.player = player;

                // if picked up speed while already speeding, skip
                if (type == PowerUpType.Speed && player.IsSpeedPowerupActive())
                {
                    return;
                }

                switch (type)
                {
                    case PowerUpType.Shield:
                        player.SetShieldState(true);
                        GameEvents.instance.ShieldPickedUp();
                        Invoke(nameof(FlashShield), durationOfPowerups - 2f);
                        Invoke(nameof(FlashShield), durationOfPowerups - 1f);
                        Invoke(nameof(Disable), durationOfPowerups);
                        break;
                    case PowerUpType.Speed:
                        player.SetSpeedPowerupActiveState(true);
                        player.SetTargetSpeed(player.GetTargetSpeed() + addSpeedAmount);
                        GameEvents.instance.SpeedPickedUp();
                        Invoke(nameof(Disable), durationOfPowerups);
                        break;
                    case PowerUpType.Time:
                        LevelManager.instance.AddTime(timeToAdd);
                        GameEvents.instance.TimePickedUp();
                        Invoke(nameof(Disable), durationOfPowerups);
                        break;
                    default:
                        break;
                }

                gameObject.SetActive(false);
            }
        }

        void Disable()
        {
            switch (type)
            {
                case PowerUpType.Shield:
                    player.SetShieldState(false);
                    break;
                case PowerUpType.Speed:
                    player.SetTargetSpeed(player.GetTargetSpeed() - addSpeedAmount);
                    player.SetSpeedPowerupActiveState(false);
                    break;
                case PowerUpType.Time:
                    break;
                default:
                    break;
            }
        }

        void FlashShield()
        {
            player.FlashShield();
        }

        public void SetDuration(float val)
        {
            durationOfPowerups = val;
        }

        public void SetAddSpeedAmount(float val)
        {
            addSpeedAmount = val;
        }

        public void SetTimeToAdd(float val)
        {
            timeToAdd = val;
        }
    }

    public enum PowerUpType
    {
        Shield,
        Speed,
        Time,
    }
}