using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Witmina_MathOfFuel
{
    public abstract class ServiceButton : MonoBehaviour
    {
        [SerializeField] private GaugeScale _scale;
        [SerializeField] private SpriteRenderer _dotSprite;

        protected bool _clicked;

        private int _ticks = 12;
        public int Ticks
        {
            get => _ticks;
            set
            {
                _ticks = value;
                if(_scale)
                    _scale.SetScale(_ticks);
            }
        }

        private bool _interactable;
        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                if(_dotSprite)
                    _dotSprite.enabled = _interactable;
            }
        }
        
        protected float _value;
        public virtual float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0f, 1f);
            }
        }

        //public abstract void OnPointerDown(PointerEventData eventData);

        //public abstract void OnPointerMove(PointerEventData eventData);
        public abstract void OnDrag(PointerEventData eventData);
    }
}