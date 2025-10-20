using System;
using TMPro;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public class MathTextController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private TMP_Text _operatorText;
        [SerializeField] private MathOperand _op1;
        [SerializeField] private MathOperand _op2;
        [SerializeField] private MathOperand _result;

        private int _denominator;
        public void SetQuestionText(string questionText)
        {
            _questionText.text = questionText;
        }

        public void UpdateResult(float value)
        {
            var fraction = new Fraction(Mathf.RoundToInt(value * _denominator), _denominator);
            _result.SetValue(fraction, _result.CurrentType);
        }

        public void SetText(QuestionData questionData)
        {
            if (questionData.Op1 != null)
            {
                _op1.SetValue(questionData.Op1, questionData.OperandType);
                _op1.gameObject.SetActive(true);
            }
            else
            {
                _op1.gameObject.SetActive(false);
            }
            
            _op2.SetValue(questionData.Op2, questionData.OperandType);
            _result.SetValue(questionData.Result, questionData.OperandType);
            _result.SetAsQuestionMark();

            _denominator = questionData.Result.Denominator;
            
            switch (questionData.QuestionType)
            {
                case QuestionType.Simple:
                    _operatorText.gameObject.SetActive(false);
                    break;
                case QuestionType.Addition:
                    _operatorText.gameObject.SetActive(true);
                    _operatorText.text = "+";
                    break;
                case QuestionType.Subtraction:
                    _operatorText.gameObject.SetActive(true);
                    _operatorText.text = "-";
                    break;
                case QuestionType.Multiplication:
                    _operatorText.gameObject.SetActive(true);
                    _operatorText.text = "x";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

