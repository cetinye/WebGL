using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Witmina_MathOfFuel
{
    public class HorizontalServiceButton : ServiceButton
    {
        //[SerializeField] private SpriteMask _tickMask;
        //[SerializeField] private SpriteMask _numberMask;
        [SerializeField] private float _minPos = -0.5f;
        [SerializeField] private float _maxPos = 0.5f;

        private Vector3 _position;
        public override float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0f, 1f);
                _position = GetPosition(_value);
                transform.localPosition = _position;
            }
        }

        private void Awake()
        {
            Interactable = true;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!Interactable)
                return;
            
            var pos = GameManager.MainCamera.ScreenToWorldPoint(eventData.position);
            var localPosX = transform.parent.InverseTransformPoint(pos).x;

            var valueNew = GetValue(localPosX);
            var rounded = Mathf.Round(valueNew * Ticks);
            valueNew = rounded / Ticks;
            Value = valueNew;
        }

        private void OnMouseUp()
        {
            //_tickMask.enabled = false;
            //_numberMask.enabled = false;
        }

        private Vector3 GetPosition(float value)
        {
            var total = (_maxPos - _minPos);
            var p = value * total + _minPos;
            return new Vector3(p, 0f, 0f);
        }

        private float GetValue(float localPosX)
        {
            return (localPosX - _minPos) / (_maxPos - _minPos);
        }
    }
}
