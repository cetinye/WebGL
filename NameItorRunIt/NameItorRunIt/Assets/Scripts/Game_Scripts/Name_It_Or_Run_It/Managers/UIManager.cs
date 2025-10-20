using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Name_It_Or_Run_It
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private TMP_Text questionText;
        [SerializeField] private TMP_Text loadingbarText;
        [SerializeField] private TMP_Text hackText;
        [SerializeField] private Image feedbackImage;
        [SerializeField] private Image timeSlider;
        [SerializeField] private RectTransform questionRect;
        [SerializeField] private RectTransform loadingRect;
        [SerializeField] private RectTransform hackRect;
        [SerializeField] private RectTransform positionDownPlaceholder;
        [SerializeField] private RectTransform hackCenterPlaceholder;
        [SerializeField] private RectTransform questionPlaceholder;
        [SerializeField] private RectTransform loadingPlaceholder;
        [SerializeField] private Sprite correct;
        [SerializeField] private Sprite wrong;
        [SerializeField] private float timeToScale;
        [SerializeField] private float scaleTo;
        [SerializeField] private float timeBetweenLetters;
        [SerializeField] private List<GameObject> bars = new List<GameObject>();
        [SerializeField] private float timeForEachQuestion;
        [SerializeField] private float timeIntervalBtwBars;
        private float time;
        private int previousLoadingNumber = 0;
        private bool isTimerOn = false;

        void Start()
        {
            ResetLoadingBar();
        }

        void Update()
        {
            TimeSlider();
        }

        public void SetQuestion(string text)
        {
            AudioManager.instance.PlayOneShot("QuestionPop");
            questionText.text = LeanLocalization.GetTranslationText(text);
        }

        public void ShowFeedback(bool isCorrect)
        {
            if (isCorrect)
            {
                feedbackImage.sprite = correct;
            }
            else
            {
                feedbackImage.sprite = wrong;
            }

            feedbackImage.transform.localScale = Vector3.zero;
            feedbackImage.gameObject.SetActive(true);
            GiveFeedback().Play().OnComplete(() => feedbackImage.gameObject.SetActive(false));
        }

        public void TimeSlider()
        {
            if (!isTimerOn) return;

            time -= Time.deltaTime;
            timeSlider.fillAmount = time / timeForEachQuestion;

            if (time <= 0f)
            {
                levelManager.CheckAnswer(".");
            }
        }

        public int GetRemainingTime()
        {
            return Mathf.CeilToInt(time);
        }

        public void SetAnswerTime(float time)
        {
            timeForEachQuestion = time;
        }

        public void SetTimer(bool state)
        {
            isTimerOn = state;
            time = timeForEachQuestion;
            timeSlider.fillAmount = time;
        }

        private Sequence GiveFeedback()
        {
            Sequence feedback = DOTween.Sequence();

            feedback.Append(feedbackImage.transform.DOScale(Vector3.one * scaleTo, timeToScale));

            return feedback;
        }

        public void UpdateLoadingBarUp(int questionsAnswered)
        {
            StartCoroutine(UpdateLoadingBarUpRoutine(questionsAnswered));
        }

        public void UpdateLoadingBarDown(int questionsAnswered)
        {
            StartCoroutine(UpdateLoadingBarDownRoutine(questionsAnswered));
        }

        public void ResetLoadingBar()
        {
            for (int i = 0; i < bars.Count; i++)
            {
                bars[i].SetActive(false);
                loadingbarText.text = $"{LeanLocalization.GetTranslationText("DecryptionProgress")} 0%";
            }

            previousLoadingNumber = 0;
        }

        public void StartHackAnim(bool isLevelSuccess)
        {
            StartCoroutine(HackAnim(isLevelSuccess));
        }

        private Sequence MoveToDown(RectTransform rectTransform, float tweenTime)
        {
            Sequence moveToDown = DOTween.Sequence();

            moveToDown.Append(rectTransform.DOAnchorPos(positionDownPlaceholder.anchoredPosition, tweenTime / 2));
            moveToDown.Join(rectTransform.DOScale(0f, tweenTime / 2));

            return moveToDown;
        }

        private Sequence MoveToPos(RectTransform rectTransform, RectTransform moveToPos, float tweenTime, float scale)
        {
            Sequence moveToCenter = DOTween.Sequence();

            moveToCenter.Append(rectTransform.DOAnchorPos(moveToPos.anchoredPosition, tweenTime));
            moveToCenter.Join(rectTransform.DOScale(scale, tweenTime));

            return moveToCenter;
        }

        #region Coroutines

        IEnumerator HackAnim(bool isLevelSuccess)
        {
            string tmpHackText;

            MoveToDown(questionRect, 0.5f);
            Sequence move = MoveToDown(loadingRect, 0.5f);
            yield return move.WaitForCompletion();

            hackRect.localScale = Vector3.zero;
            hackRect.gameObject.SetActive(true);
            move = MoveToPos(hackRect, hackCenterPlaceholder, 0.5f, 1f);

            if (isLevelSuccess)
                AudioManager.instance.PlayOneShot("Success");
            else
                AudioManager.instance.PlayOneShot("Fail");

            yield return move.WaitForCompletion();

            if (isLevelSuccess)
                tmpHackText = $"{LeanLocalization.GetTranslationText("AccessGranted")}: {LeanLocalization.GetTranslationText("DecrytionOfLevel")} " + levelManager.currentLevelId + $" {LeanLocalization.GetTranslationText("Successful")}!";
            else
                tmpHackText = $"{LeanLocalization.GetTranslationText("AccessDenied")}: {LeanLocalization.GetTranslationText("DecrytionOfLevel")} " + levelManager.currentLevelId + $" {LeanLocalization.GetTranslationText("Failed")}!";

            string text = "";

            for (int i = 0; i < tmpHackText.Length + 1; i++)
            {
                text = tmpHackText.Substring(0, i);
                hackText.text = text;
                yield return new WaitForSeconds(timeBetweenLetters / 10f);
            }

            yield return new WaitForSeconds(2);

            MoveToDown(hackRect, 0.5f).OnComplete(() =>
            {
                hackRect.gameObject.SetActive(false);
                levelManager.Restart();
            });

            /* MoveToPos(questionRect, questionPlaceholder, 0.5f, questionPlaceholder.localScale.x);
            move = MoveToPos(loadingRect, loadingPlaceholder, 0.5f, loadingPlaceholder.localScale.x);
            yield return move.WaitForCompletion();
            hackText.text = "";
            levelManager.Restart(); */
        }

        IEnumerator UpdateLoadingBarUpRoutine(int questionsAnswered)
        {
            int stepSize = Mathf.CeilToInt(bars.Count / levelManager.itemCountToBeShown);

            if (questionsAnswered == levelManager.itemCountToBeShown)
            {
                StartCoroutine(UpdateLoadingBarUpText(100));

                for (int i = (questionsAnswered - 1) * stepSize; i < 24; i++)
                {
                    bars[i].SetActive(true);
                    yield return new WaitForSeconds(timeIntervalBtwBars);
                }
            }
            else
            {
                StartCoroutine(UpdateLoadingBarUpText(questionsAnswered * ((stepSize * 4) + 1)));

                for (int i = (questionsAnswered - 1) * stepSize; i < questionsAnswered * stepSize; i++)
                {
                    bars[i].SetActive(true);
                    yield return new WaitForSeconds(timeIntervalBtwBars);
                }
            }
        }

        IEnumerator UpdateLoadingBarDownRoutine(int questionsAnswered)
        {
            int stepSize = Mathf.CeilToInt(bars.Count / levelManager.itemCountToBeShown);

            StartCoroutine(UpdateLoadingBarDownText(questionsAnswered * ((stepSize * 4) + 1)));

            for (int i = (questionsAnswered + 1) * stepSize; i >= questionsAnswered * stepSize; i--)
            {
                bars[i].SetActive(false);
                yield return new WaitForSeconds(timeIntervalBtwBars);
            }
        }

        IEnumerator UpdateLoadingBarUpText(int targetVal)
        {
            for (int i = previousLoadingNumber; i <= targetVal; i++)
            {
                loadingbarText.text = $"{LeanLocalization.GetTranslationText("DecryptionProgress")} " + i + "%";
                yield return new WaitForSeconds(timeIntervalBtwBars / 6f);
            }

            previousLoadingNumber = targetVal;
        }

        IEnumerator UpdateLoadingBarDownText(int targetVal)
        {
            for (int i = previousLoadingNumber; i >= targetVal; i--)
            {
                loadingbarText.text = $"{LeanLocalization.GetTranslationText("DecryptionProgress")} " + i + "%";
                yield return new WaitForSeconds(timeIntervalBtwBars / 6f);
            }

            previousLoadingNumber = targetVal;
        }

        #endregion
    }
}