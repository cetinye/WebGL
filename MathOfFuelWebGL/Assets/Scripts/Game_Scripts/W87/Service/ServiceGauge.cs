using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public abstract class ServiceGauge : MonoBehaviour
    {
        protected float _value;
        public virtual float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0f, 1f);
            }
        }
    }
}
