using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Guess_The_Move
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text topText;
        [SerializeField] private TMP_Text levelTimerText;
        [SerializeField] private Image questionTimerSlider;
        [SerializeField] private Button yesButton;
        [SerializeField] private Image yesButtonImage;
        [SerializeField] private Sprite yesDefault;
        [SerializeField] private Sprite yesPressed;
        [SerializeField] private Button noButton;
        [SerializeField] private Image noButtonImage;
        [SerializeField] private Sprite noDefault;
        [SerializeField] private Sprite noPressed;
        [SerializeField] private Image feedbackImage;
        [SerializeField] private Sprite correctSp;
        [SerializeField] private Sprite wrongSp;
        [SerializeField] private float feedbackScaleTime;
        [SerializeField] private TMP_Text correctText;
        [SerializeField] private TMP_Text wrongText;
        [SerializeField] private Color correctColor;
        [SerializeField] private Color wrongColor;
        [SerializeField] private Color defaultColor;

        [Header("Flash Variables")]
        [SerializeField]
        private float flashInterval = 0.5f;

        private Color defaultFlashColor;

        private void Awake()
        {
            defaultFlashColor = levelTimerText.color;
        }

        public void SetText(string text)
        {
            topText.text = text;
        }

        public void SetButtonsPressable(bool pressable)
        {
            yesButton.interactable = pressable;
            noButton.interactable = pressable;

            if (pressable)
            {
                yesButtonImage.sprite = yesDefault;
                noButtonImage.sprite = noDefault;
            }
        }

        public void SetTimerText(float passedLevelTime)
        {
            levelTimerText.text = passedLevelTime.ToString("F0");
        }

        public void UpdateQuestionTimer(float passedQuestionTime, float totalQuestionTime)
        {
            questionTimerSlider.fillAmount = passedQuestionTime / totalQuestionTime;
        }

        public void UpdateCorrectWrongUI(int correctCount, int wrongCount)
        {
            correctText.text = correctCount.ToString("F0");
            wrongText.text = wrongCount.ToString("F0");
        }

        public void Feedback(bool isCorrect)
        {
            if (isCorrect)
            {
                AudioManager.instance.PlayOneShot(SoundType.Correct);
                feedbackImage.sprite = correctSp;
                topText.color = correctColor;
                SetText($"{LeanLocalization.GetTranslationText("CORRECT")}");
            }
            else
            {
                AudioManager.instance.PlayOneShot(SoundType.Wrong);
                feedbackImage.sprite = wrongSp;
                topText.color = wrongColor;
                SetText($"{LeanLocalization.GetTranslationText("WRONG")}");
            }

            var feedbackSeq = DOTween.Sequence();

            feedbackSeq.Append(feedbackImage.transform.DOScale(Vector3.one, feedbackScaleTime / 2f));
            feedbackSeq.Append(feedbackImage.transform.DOScale(Vector3.one, 0.2f));
            feedbackSeq.Append(feedbackImage.transform.DOScale(Vector3.zero, feedbackScaleTime / 2f));

            feedbackSeq.OnComplete(() =>
            {
                topText.color = defaultColor;
                LevelManager.instance.NewQuestion();
            });
        }

        public void YesPressed()
        {
            yesButtonImage.sprite = yesPressed;
        }

        public void NoPressed()
        {
            noButtonImage.sprite = noPressed;
        }

        public void FlashRed()
        {
            var redFlash = DOTween.Sequence();

            redFlash.Append(levelTimerText.DOColor(Color.red, flashInterval))
                .SetEase(Ease.Linear)
                .Append(levelTimerText.DOColor(defaultFlashColor, flashInterval))
                .SetEase(Ease.Linear)
                .SetLoops(6);

            redFlash.Play();
        }
    }
}