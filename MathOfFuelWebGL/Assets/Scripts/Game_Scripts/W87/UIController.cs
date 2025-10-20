using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_MathOfFuel
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject _introPanel;
        [SerializeField] private GameObject _endGamePanel;
        [SerializeField] private GameObject _skipButton;
        [SerializeField] private Image _blurImage;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text scoreText;

        private void OnDestroy()
        {
            if (_blurImage)
                _blurImage.DOKill();
        }

        public void Initialize()
        {
            _endGamePanel.gameObject.SetActive(false);
            if (!_blurImage)
                return;

            _blurImage.DOKill();
            _blurImage.gameObject.SetActive(false);
            _blurImage.color = new Color(1f, 1f, 1f, 0f);
        }

        public void ActivateEndGamePanel()
        {
            if (!_blurImage)
            {
                _endGamePanel.SetActive(true);
                return;
            }


            _blurImage.gameObject.SetActive(true);
            _blurImage.color = new Color(1f, 1f, 1f, 0f);
            _blurImage.DOFade(1f, 0.25f)
                .OnComplete(() => _endGamePanel.SetActive(true));
        }

        public void ToggleIntroPanel(bool active)
        {
            _introPanel.SetActive(active);
        }

        public void ToggleSkipButton(bool active)
        {
            _skipButton.SetActive(active);
        }

        public void UpdateLevelText(int level)
        {
            levelText.text = $"{LeanLocalization.GetTranslationText("Level")}: {level}";
        }

        public void UpdateScoreText(int score)
        {
            scoreText.text = $"Score: {score}";
        }
    }

}
