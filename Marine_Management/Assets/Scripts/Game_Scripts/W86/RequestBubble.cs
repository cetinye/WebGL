using System;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_MarineManagement
{
    public class RequestBubble : MonoBehaviour
    {
        public event Action BubbleSelected;
        [SerializeField] private Image _sprite;
        [SerializeField] private Image _timerFill;
        [SerializeField] private Image _progressFill;
        [SerializeField] private Animator _animator;

        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (!_animator)
                    return;

                _animator.enabled = _selected;
                if (!_selected)
                    transform.localScale = Vector3.one;
            }
        }

        public bool TimerEnabled
        {
            get => _timerFill.gameObject.activeSelf;
            set => _timerFill.gameObject.SetActive(value);
        }

        public Sprite Sprite
        {
            get => _sprite.sprite;
            set => _sprite.sprite = value;
        }

        public float TimerFillAmount
        {
            get => _timerFill.fillAmount;
            set
            {
                _timerFill.fillAmount = Mathf.Clamp(value, 0f, 1f);
                Color.RGBToHSV(_timerFill.color, out var h, out var s, out var v);
                h = _timerFill.fillAmount * 0.39215f;
                _timerFill.color = Color.HSVToRGB(h, s, v);
            }
        }

        public float ProgressFillAmount
        {
            get => _progressFill.fillAmount;
            set => _progressFill.fillAmount = Mathf.Clamp(value, 0f, 1f);
        }

        public void OnClick()
        {
            BubbleSelected?.Invoke();

            Taptic.Success();
        }
    }
}

