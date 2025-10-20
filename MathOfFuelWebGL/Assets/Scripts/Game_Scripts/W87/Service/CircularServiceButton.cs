using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Witmina_MathOfFuel
{
    public class CircularServiceButton : ServiceButton
    {
        //[SerializeField] private SpriteMask _tickMask;
        //[SerializeField] private SpriteMask _numberMask;
        [SerializeField] private float _minAngle = -135f;
        [SerializeField] private float _maxAngle = 135f;

        private Quaternion _rotation;
        public override float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0f, 1f);
                _rotation = GetRotation(_value);
                transform.rotation = _rotation;
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
            
            var direction = (Vector3)eventData.position - GameManager.MainCamera.WorldToScreenPoint(transform.position);
            var rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized).eulerAngles;

            //transform.rotation = Quaternion.Euler(rotation);
            
            //_tickMask.enabled = true;
            //_numberMask.enabled = Mathf.Abs(rotation.z % 30) < 0.1f;

            var valueNew = GetValue(rotation);
            var rounded = Mathf.Round(valueNew * Ticks);
            valueNew = rounded / Ticks;
            Value = valueNew;
        }

        private Quaternion GetRotation(float value)
        {
            var total = (_maxAngle - _minAngle);
            var angle = NormalizeAngle(value * total + _minAngle);
            return Quaternion.Euler(0f, 0f, -angle);
        }

        private float GetValue(Vector3 rotation)
        {
            var angle = NormalizeAngle(-rotation.z);
            return (angle - _minAngle) / (_maxAngle - _minAngle);
        }

        public bool EvaluateRotation(float value)
        {
            //Debug.Log($"{name}({_type}) value: {Value}, given: {value}");
            var currentValue = Mathf.FloorToInt(Value) % 1;
            var givenValue = Mathf.FloorToInt(value) % 1;
            
            return currentValue == givenValue;
        }

        private float NormalizeAngle(float angle)
        {
            var angleNormalized = angle % 360;
            if (angleNormalized > 180f)
                angleNormalized -= 360f;
            else if (angleNormalized < -180f)
                angleNormalized += 360f;

            return angleNormalized;
        }
    }
}
