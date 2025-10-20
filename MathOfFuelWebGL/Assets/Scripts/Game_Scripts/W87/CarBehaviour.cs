using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public class CarBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _feedbackSpriteRenderer;
        [SerializeField] private Sprite _happySprite;
        [SerializeField] private Sprite _angrySprite;

        private bool _moving;

        public bool Moving
        {
            get => _moving;
            set
            {
                _moving = value;
                _animator.speed = _moving ? 1f : 0f;
            }
        }

        private void Awake()
        {
            _feedbackSpriteRenderer.gameObject.SetActive(false);
            Moving = false;
        }

        public void GiveFeedback(bool success)
        {
            _feedbackSpriteRenderer.sprite = success ? _happySprite : _angrySprite;
            _feedbackSpriteRenderer.gameObject.SetActive(true);
        }
    }
}

