using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public sealed class CircularServiceGauge : ServiceGauge
    {
        [SerializeField] private float _minAngle = -90f;
        [SerializeField] private float _maxAngle = 90f;
        public override float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0f, 1f);
                var total = _maxAngle - _minAngle;
                transform.rotation = Quaternion.Euler(0f, 0f, -(_value * total + _minAngle));
            }
        }
    }
}
