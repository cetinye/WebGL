using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_MarineManagement
{
    public class HUDPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private float flashInterval;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Image _powerUpFill;
        [SerializeField] private List<Button> _powerUpButtons;
        [SerializeField] private List<PowerUpIconData> _powerUpIcons;

        private static readonly float[] _tresholds = { 0.175f, 0.38f, 0.56f, 0.78f };

        private float _powerUpAmount;

        private Tween _powerUpTween;
        private Sequence redFlash;

        private void OnDestroy()
        {
            _powerUpTween.Kill();
        }

        public void Initialize()
        {
            _powerUpAmount = 0f;
            _powerUpFill.fillAmount = 0f;

            foreach (var icon in _powerUpIcons)
            {
                icon.GameObject.SetActive(false);
            }
        }

        public void SetTimerText(float timer)
        {
            _timerText.text = $"{Mathf.CeilToInt(timer)}";
        }

        public void SetScoreText(int score)
        {
            _scoreText.text = $"{score}";
        }

        public void AddPowerUpAmount(float amount)
        {
            _powerUpAmount = Mathf.Clamp(_powerUpAmount + amount, 0f, 1f);
            _powerUpTween.Kill();
            _powerUpTween = _powerUpFill.DOFillAmount(_powerUpAmount, 0.4f * _powerUpAmount);
            for (int i = 0; i < _powerUpButtons.Count && i < _tresholds.Length; i++)
            {
                if (_powerUpAmount > _tresholds[i])
                {
                    if (!_powerUpButtons[i].gameObject.activeSelf)
                    {
                        _powerUpButtons[i].gameObject.SetActive(true);
                        _powerUpButtons[i].transform.DOPunchScale(0.2f * Vector3.one, 0.2f);
                    }
                }
                else
                {
                    _powerUpButtons[i].gameObject.SetActive(false);
                }
            }
        }

        public void TogglePowerUpIcon(PowerUpType powerUpType, bool active)
        {
            var icon = _powerUpIcons.FirstOrDefault(p => p.PowerUpType == powerUpType);
            if (icon == null)
                return;

            icon.GameObject.SetActive(active);
        }

        public void OnPowerUpApplied(PowerUpType powerUpType)
        {
            var index = (int)powerUpType;
            var prevTreshold = index == 0 ? 0 : _tresholds[index - 1];
            var diff = _tresholds[index] - prevTreshold;

            if (powerUpType != PowerUpType.Time)
            {
                AddPowerUpAmount(-diff);
            }

            //Reset the progress if they choose time to avoid playing very long durations
            else
            {
                redFlash.Kill(true);
                // GameManager.Instance.StopFx("Countdown");
                GameManager.Instance.SetIsFlashing(true);
                AddPowerUpAmount(-1);
            }
        }

        public void FlashRed()
        {
            redFlash = DOTween.Sequence();

            redFlash.Append(_timerText.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(_timerText.DOColor(Color.white, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }
    }
}
