using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chefs_Secret_Recipes
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        [SerializeField] private TMP_Text levelTimeText;
        [SerializeField] private Image questionTimeSlider;
        [SerializeField] private TMP_Text gameStateText;
        [SerializeField] private TMP_Text levelIdText;
        [SerializeField] private TMP_Text operatorsText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text correctText;
        [SerializeField] private TMP_Text wrongText;
        [SerializeField] private TMP_Text levelText;

        [Header("Flash Variables")]
        [SerializeField] private float flashInterval = 0.5f;
        private Color defaultColor;

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }

            defaultColor = levelTimeText.color;
        }

        void OnDestroy()
        {
            DOTween.Kill(levelText);
            instance = null;
        }

        public void ShowLevelId(int levelId)
        {

            Debug.Log("Showing Level ID: " + levelId);
            levelText.DOFade(0, 0);
            levelText.text = $"{LeanLocalization.GetTranslationText("Level")} " + levelId;
            levelText.DOFade(1, 0.5f).SetLoops(2, LoopType.Yoyo);

            if (levelText != null && levelText.transform != null)
            {
                levelText.transform.DOScale(1.2f, 1f).OnComplete(() =>
                {
                    if (levelText != null && levelText.transform != null)
                    {
                        levelText.DOFade(0, 0f);
                        levelText.transform.DOScale(1f, 0f);
                    }
                });
            }
        }

        public void SetLevelTimeText(float time)
        {
            levelTimeText.text = time.ToString("F0");
        }

        public void SetQuestionTime(float time)
        {
            questionTimeSlider.fillAmount = time / LevelManager.instance.levelSO.timeLimitPerOperation;
        }

        public void SetSliderVisibility(bool state)
        {
            questionTimeSlider.transform.parent.gameObject.SetActive(state);
        }

        public void UpdateCorrectText(int val)
        {
            correctText.text = val.ToString("F0");
        }

        public void UpdateWrongText(int val)
        {
            wrongText.text = val.ToString("F0");
        }

        public void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(levelTimeText.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(levelTimeText.DOColor(defaultColor, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }
    }
}