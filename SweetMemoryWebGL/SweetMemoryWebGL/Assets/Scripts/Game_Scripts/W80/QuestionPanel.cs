using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Witmina_SweetMemory
{
    public class QuestionPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _questionPanel;
        [SerializeField] private TMP_Text _questionText;
        [SerializeField] private TMP_Text _trueScoreText;
        [SerializeField] private TMP_Text _falseScoreText;

        private int _trueScore;
        public int TrueScore
        {
            get => _trueScore;
            set
            {
                _trueScore = value;
                _trueScoreText.text = _trueScore.ToString();
            }
        }
        
        private int _falseScore;

        public int FalseScore
        {
            get => _falseScore;
            set
            {
                _falseScore = value;
                _falseScoreText.text = _falseScore.ToString();
            }
        }

        public void Initialize()
        {
            _questionPanel.SetActive(true);
            TrueScore = 0;
            FalseScore = 0;
        }

        public void ToggleQuestionPanel(bool active)
        {
            _questionPanel.SetActive(active);
        }
        public void SetQuestionText(string question)
        {
            _questionText.text = question;
        }
    }
}

