using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public sealed class DigitServiceGauge : ServiceGauge
    {
        [SerializeField] private TMP_Text _text;
        public override float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, 0f, 1f);
                _text.text = Mathf.RoundToInt(_value * 100f).ToString();
            }
        }
    }
}
