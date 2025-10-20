using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Witmina_SpaceBurgers
{
    public class ServeButton : MonoBehaviour
    {
        public event Action Pressed;

        [SerializeField] private Sprite _pressedSprite;
        [SerializeField] private Transform _textTransform;
        [SerializeField] private float textRecoilAmount = 0.1f;

        private Sprite _defaultSprite;
        private SpriteRenderer _buttonSprite;
        private Vector3 _textInitPos;
        private void Awake()
        {
            _buttonSprite = GetComponent<SpriteRenderer>();
            _defaultSprite = _buttonSprite.sprite;
            _textInitPos = _textTransform.localPosition;
        }

        public void OnTouchDown(PointerEventData eventData)
        {
            _buttonSprite.sprite = _pressedSprite;
            _textTransform.localPosition = _textInitPos + textRecoilAmount * Vector3.down;
        }

        public void OnTouchUp(PointerEventData eventData)
        {
            _buttonSprite.sprite = _defaultSprite;
            _textTransform.localPosition = _textInitPos;
            Pressed?.Invoke();
        }
    }
}