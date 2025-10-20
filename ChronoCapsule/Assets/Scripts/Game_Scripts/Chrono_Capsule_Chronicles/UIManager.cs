using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chrono_Capsule_Chronicles
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Image questionTimer;
        [SerializeField] private TMP_Text gameTimer;
        [SerializeField] private TMP_Text correctText;
        [SerializeField] private TMP_Text wrongText;
        [SerializeField] private TMP_Text chosenWordText;
        private float questionTime;
        private float flashInterval = 0.5f;

        public void SetQuestionTime(float val)
        {
            questionTime = val;
        }

        public void UpdateLevelTimer(float val)
        {
            questionTimer.fillAmount = val / questionTime;
        }

        public void UpdateGameTimer(float val)
        {
            gameTimer.text = val.ToString("F0");
        }

        public void SetChosenWordText(string chosenWord)
        {
            chosenWordText.color = Color.white;
            chosenWordText.text = chosenWord;
        }

        public void Correct()
        {
            chosenWordText.color = Color.green;
        }

        public void Wrong()
        {
            chosenWordText.color = Color.red;
        }

        public void UpdateCorrectText(int val)
        {
            correctText.text = val.ToString("F0");
        }

        public void UpdateWrongText(int val)
        {
            wrongText.text = val.ToString("F0");
        }

        public void CloseAllElements()
        {
            questionTimer.gameObject.SetActive(false);
            chosenWordText.gameObject.SetActive(false);
        }

        public void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(gameTimer.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(gameTimer.DOColor(Color.white, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }
    }
}