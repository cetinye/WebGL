using System;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public class ServiceBehaviour : MonoBehaviour
    {
        public static event Action<float> ValueChanged; 

        [SerializeField] private ServiceType _type;
        [SerializeField] private QuestionOperandType _operandType;
        [SerializeField] private string _serviceText;
        [SerializeField] private ServiceButton _serviceButton;
        [SerializeField] private ServiceGauge _serviceGauge;
        public ServiceType Type => _type;
        public QuestionOperandType OperandType => _operandType;
        public string Text => _serviceText;

        private float _targetValue;
        private float _currentValue;

        private ServiceButton _currentButton;

        public bool Interactable
        {
            get => _serviceButton.Interactable;
            set => _serviceButton.Interactable = value;
        }

        private void Awake()
        {
            Interactable = false;
        }

        private void OnButtonValueChanged()
        {
            ValueChanged?.Invoke(_serviceButton.Value);
        }

        private void Update()
        {

            if (_serviceButton && Math.Abs(_currentValue - _serviceButton.Value) > 0.005f && GameManager.Instance.State == GameManager.GameState.playing)
            {
                _currentValue = _serviceButton.Value;
                ValueChanged?.Invoke(_currentValue);
            }
            
            if(_serviceGauge)
                _serviceGauge.Value = _currentValue;
        }

        public void SetService(int denominator, int initNominator, int targetNominator)
        {
            _serviceButton.Ticks = denominator;
            _serviceButton.Value = (float)initNominator / denominator;
            _currentValue = _serviceButton.Value;
            _targetValue = (float)targetNominator / denominator;
        }

        public bool Evaluate()
        {
            return Math.Abs(_targetValue - _serviceButton.Value) < (0.5f / _serviceButton.Ticks);
        }
    }
}

