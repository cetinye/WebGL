using UnityEngine;
using TMPro;

namespace NanoInspector
{
    public class Question : MonoBehaviour
    {
        public string questionText;
        public QuestionIncludes questionIncludes;
        public float questionTimer;
        public bool isCorrect = false;
        public bool isTrickQuestion = false;
        public bool isTrickQuestionColorEnabled = false;
        public bool isTrickQuestionShapeEnabled = false;
        public bool isTrickQuestionMovementEnabled = false;

        [SerializeField] private TextMeshProUGUI questionTMProText;

        public void SetQuestionText(string text)
        {
            questionTMProText.text = text;
        }

        public enum QuestionIncludes
        {
            Color,
            Shape,
            Movement
        }
    }
}