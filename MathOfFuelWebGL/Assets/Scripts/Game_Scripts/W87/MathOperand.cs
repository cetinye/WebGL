using System;
using TMPro;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public class MathOperand : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nominator;
        [SerializeField] private TMP_Text _denominator;
        [SerializeField] private GameObject _dash;
        [SerializeField] private TMP_Text _floatNumber;
        
        public QuestionOperandType CurrentType { get; private set; }

        public void SetAsQuestionMark()
        {
            _nominator.text = "?";
            _floatNumber.text = CurrentType is QuestionOperandType.Percentage ? "?%" : "?";
        }
        
        public void SetValue(Fraction fraction, QuestionOperandType opType)
        {
            CurrentType = opType;
            switch (opType)
            {
                case QuestionOperandType.Fraction:
                    _nominator.text = fraction.Nominator.ToString();
                    _denominator.text = fraction.Denominator.ToString();
                    
                    _nominator.gameObject.SetActive(true);
                    _denominator.gameObject.SetActive(true);
                    _dash.gameObject.SetActive(true);
                    _floatNumber.gameObject.SetActive(false);
                    break;
                case QuestionOperandType.Percentage:
                    _floatNumber.text = $"{Mathf.Round(100f * fraction.Float())}%";
                    
                    _nominator.gameObject.SetActive(false);
                    _denominator.gameObject.SetActive(false);
                    _dash.gameObject.SetActive(false);
                    _floatNumber.gameObject.SetActive(true);
                    break;
                case QuestionOperandType.Float:
                    _floatNumber.text = $"{Mathf.Round(100f * fraction.Float()) / 100f}";
                    _nominator.gameObject.SetActive(false);
                    _denominator.gameObject.SetActive(false);
                    _dash.gameObject.SetActive(false);
                    _floatNumber.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(opType), opType, null);
            }
        }
    }
}

