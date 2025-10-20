using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Color_Clique
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] TMP_Text levelTimeText;
        [SerializeField] TMP_Text correctText;
        [SerializeField] TMP_Text wrongText;
        [SerializeField] TMP_Text stageScoreText;
        [SerializeField] TMP_Text averageScoreText;

        [Header("Flash Variables")]
        [SerializeField] private float flashInterval = 0.5f;
        private Color defaultColor;

        [Space()]
        [SerializeField] private TMP_Text levelIdText;
        [SerializeField] private TMP_Text levelDownText;
        [SerializeField] private TMP_Text levelUpText;

        void Awake()
        {
            defaultColor = levelTimeText.color;
        }

        public void SetTimeText(float time)
        {
            levelTimeText.text = time.ToString("F0");
        }

        public void UpdateStats(int correct, int wrong)
        {
            correctText.text = "Correct: " + correct.ToString();
            wrongText.text = "Wrong: " + wrong.ToString();
        }

        public void SetStageScoreText(int score)
        {
            stageScoreText.text = score.ToString();
        }

        public void SetAverageScoreText(int score)
        {
            averageScoreText.text = score.ToString();
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

        public void SetDebugTexts(int levelId, int levelDownCounter, int levelUpCounter)
        {
            levelIdText.text = "LevelID: " + levelId.ToString();
            levelDownText.text = "LevelDown#: " + levelDownCounter.ToString();
            levelUpText.text = "LevelUp#: " + levelUpCounter.ToString();
        }
    }
}