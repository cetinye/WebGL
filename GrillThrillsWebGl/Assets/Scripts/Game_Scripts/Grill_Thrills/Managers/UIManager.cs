using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;

namespace Grill_Thrills
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelTimerText;
        [SerializeField] private TMP_Text correctText;
        [SerializeField] private TMP_Text wrongText;
        [SerializeField] private TMP_Text levelText;

        [Header("Slider Variables")]
        [SerializeField]
        private RectTransform sliderRectTransform;

        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 endPos;
        [SerializeField] private float lerpFactor;
        public Vector3 target;

        [Header("Flash Variables")]
        [SerializeField]
        private float flashInterval = 0.5f;

        private Color defaultColor;
        private LevelManager levelManager;
        private float stepAmount;
        private Tween levelTextTween;

        private void Start()
        {


            defaultColor = levelTimerText.color;
            sliderRectTransform.localPosition = startPos;
        }

        private void Update()
        {
            // set target slider position
            target = new Vector3(startPos.x + levelManager.GetCorrectCount() * stepAmount,
                sliderRectTransform.localPosition.y, sliderRectTransform.localPosition.z);

            // prevent slider from going out of bounds
            if (target.x > endPos.x) target = endPos;

            // lerp slider fill
            sliderRectTransform.localPosition =
                Vector3.Lerp(sliderRectTransform.localPosition, target, Time.deltaTime * lerpFactor);
        }

        void OnDestroy()
        {
            DOTween.Kill(levelTextTween);
        }

        public void ShowLevelId(int levelId)
        {
            if (levelText == null) return;

            levelTextTween = levelText.DOFade(0, 0);
            levelText.text = $"{LeanLocalization.GetTranslationText("Level")} " + levelId;
            levelTextTween = levelText.DOFade(1, 0.5f).SetLoops(2, LoopType.Yoyo);

            if (levelText != null && levelText.transform != null)
                levelTextTween = levelText.transform.DOScale(1.2f, 1f).OnComplete(() =>
                {
                    if (levelText != null && levelText.transform != null)
                    {
                        levelTextTween = levelText.DOFade(0, 0f);
                        levelTextTween = levelText.transform.DOScale(1f, 0f);
                    }
                });
        }

        public void UpdateTimer(float time)
        {
            levelTimerText.text = time.ToString("F0");
        }

        public void UpdateCorrectText(int correctCount)
        {
            correctText.text = "Correct: " + correctCount;
        }

        public void UpdateWrongText(int wrongCount)
        {
            wrongText.text = "Wrong: " + wrongCount;
        }

        public void CalculateStepAmount()
        {
            levelManager = LevelManager.instance;
            stepAmount = (endPos.x - startPos.x) / levelManager.GetLevelSO().levelUpCriteria;
        }

        public void FlashRed()
        {
            var redFlash = DOTween.Sequence();

            redFlash.Append(levelTimerText.DOColor(Color.red, flashInterval))
                .SetEase(Ease.Linear)
                .Append(levelTimerText.DOColor(defaultColor, flashInterval))
                .SetEase(Ease.Linear)
                .SetLoops(6);

            redFlash.Play();
        }
    }
}