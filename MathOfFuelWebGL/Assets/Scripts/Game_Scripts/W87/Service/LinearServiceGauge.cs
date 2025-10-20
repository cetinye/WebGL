using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public sealed class LinearServiceGauge : ServiceGauge
    {
        [SerializeField] private Transform _spriteTransform;
        [SerializeField] private Vector3 _minOffset;
        [SerializeField] private Vector3 _maxOffset;
        public override float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0f, 1f);
                var movementVector = (_maxOffset - _minOffset);
                _spriteTransform.localPosition = _minOffset + _value * movementVector;
            }
        }
    }
}
