using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chefs_Secret_Recipes
{
    public class Equation : MonoBehaviour
    {
        [Header("Equation Variables")]
        public int minVal;
        public int maxVal;
        [SerializeField] private EqnType eqnType;
        [SerializeField] private TMP_Text leftMultiplier;
        [SerializeField] private Image leftImage;
        [SerializeField] private TMP_Text operatorText;
        [SerializeField] private TMP_Text rightMultiplier;
        [SerializeField] private Image rightImage;
        [SerializeField] private TMP_Text rightMissingText;
        [SerializeField] private TMP_Text equalText;
        [SerializeField] private TMP_Text answerText;
        [SerializeField] private Image bgImage;
        [SerializeField] private float timeToShow;
        private Sequence showSeq;
        private Hint leftHint;
        private Hint rightHint;
        private int leftMult, rightMult;
        private string operand;
        private QuestionPanel questionPanel;

        void Start()
        {
            // questionPanel = LevelManager.instance.GetQuestionPanel();
        }

        public void GenerateEquation()
        {
            questionPanel = LevelManager.instance.GetQuestionPanel();

            leftHint = questionPanel.GetIngredient();
            rightHint = questionPanel.GetIngredient();

            SetLeftImage(leftHint.GetSprite());

            if (eqnType != EqnType.MissingDefault && eqnType != EqnType.MissingMultipliers)
                SetRightImage(rightHint.GetSprite());

            SetOperand();

            if (eqnType == EqnType.Multipliers || eqnType == EqnType.MissingMultipliers)
            {
                leftMult = UnityEngine.Random.Range(2, 10);
                rightMult = UnityEngine.Random.Range(2, 10);
                leftMultiplier.text = leftMult.ToString();
                rightMultiplier.text = rightMult.ToString();
            }

            if (eqnType == EqnType.MissingDefault || eqnType == EqnType.MissingMultipliers)
                questionPanel.SpawnAnswers(true);
            else
                questionPanel.SpawnAnswers();

            AudioManager.instance.PlayOneShot(SoundType.Question);
            SetEquationFade(1f);
        }

        public void SetEquationFade(float alpha, bool isInstant = false)
        {
            showSeq = DOTween.Sequence();

            showSeq.Append(leftImage?.DOFade(alpha, timeToShow).SetEase(Ease.Linear));
            showSeq.Join(rightImage?.DOFade(alpha, timeToShow).SetEase(Ease.Linear));
            showSeq.Join(operatorText?.DOFade(alpha, timeToShow).SetEase(Ease.Linear));
            showSeq.Join(equalText?.DOFade(alpha, timeToShow).SetEase(Ease.Linear));
            showSeq.Join(answerText?.DOFade(alpha, timeToShow).SetEase(Ease.Linear));
            showSeq.Join(leftMultiplier?.DOFade(alpha, timeToShow).SetEase(Ease.Linear));
            showSeq.Join(rightMultiplier?.DOFade(alpha, timeToShow).SetEase(Ease.Linear));
            showSeq.Join(rightMissingText?.DOFade(alpha, timeToShow).SetEase(Ease.Linear));
            showSeq.Join(bgImage?.DOFade(alpha, timeToShow).SetEase(Ease.Linear));
            showSeq.Join(questionPanel?.FadeMeal(alpha, timeToShow).SetEase(Ease.Linear));

            showSeq.OnComplete(() => GameStateManager.SetGameState(GameState.Question));

            if (isInstant)
                showSeq.Complete();
        }

        public void SetLeftImage(Sprite ingredient)
        {
            leftImage.sprite = ingredient;
        }

        public void SetRightImage(Sprite ingredient)
        {
            rightImage.sprite = ingredient;
        }

        public void SetLeftMultiplier(int value)
        {
            leftMultiplier.text = value.ToString("F0");
            leftMult = value;
        }

        public void SetRightMultiplier(int value)
        {
            rightMultiplier.text = value.ToString("F0");
            rightMult = value;
        }

        public void SetOperand()
        {
            List<string> operators = new List<string>();

            if (LevelManager.instance.levelSO.isAdditionOn)
                operators.Add("+");
            if (LevelManager.instance.levelSO.isSubtractionOn)
                operators.Add("-");
            if (LevelManager.instance.levelSO.isMultiplicationOn && eqnType != EqnType.Multipliers && eqnType != EqnType.MissingMultipliers)
                operators.Add("x");

            int operatorIndex = Random.Range(0, operators.Count);
            operatorText.text = operators[operatorIndex];
            operand = operators[operatorIndex];
        }

        public void SetAnswerText(int value)
        {
            answerText.text = value.ToString("F0");
        }

        public int GetCorrectAnswer()
        {
            int leftVal = leftHint.GetValue();
            int rightVal = rightHint.GetValue();

            if (eqnType == EqnType.Default)
            {
                if (operand == "+")
                    return leftVal + rightVal;

                else if (operand == "-")
                    return leftVal - rightVal;

                else if (operand == "x")
                    return leftVal * rightVal;
            }
            else if (eqnType == EqnType.Multipliers)
            {
                if (operand == "+")
                    return (leftVal * leftMult) + (rightVal * rightMult);

                else if (operand == "-")
                    return (leftVal * leftMult) - (rightVal * rightMult);

                else if (operand == "x")
                    return (leftVal * leftMult) * (rightVal * rightMult);
            }
            else if (eqnType == EqnType.MissingDefault)
            {
                if (operand == "+")
                {
                    SetAnswerText(leftVal + rightVal);
                    return rightVal;
                }
                else if (operand == "-")
                {
                    SetAnswerText(leftVal - rightVal);
                    return rightVal;
                }

                else if (operand == "x")
                {
                    SetAnswerText(leftVal * rightVal);
                    return rightVal;
                }
            }
            else if (eqnType == EqnType.MissingMultipliers)
            {
                if (operand == "+")
                {
                    SetAnswerText((leftVal * leftMult) + (rightVal * rightMult));
                    return rightVal;
                }
                else if (operand == "-")
                {
                    SetAnswerText((leftVal * leftMult) - (rightVal * rightMult));
                    return rightVal;
                }

                else if (operand == "x")
                {
                    SetAnswerText((leftVal * leftMult) * (rightVal * rightMult));
                    return rightVal;
                }
            }

            return -1;
        }
    }

    public enum EqnType
    {
        Default,
        Multipliers,
        MissingDefault,
        MissingMultipliers
    }
}