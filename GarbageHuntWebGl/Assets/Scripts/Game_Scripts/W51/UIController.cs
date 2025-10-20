using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_GarbageHunt
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private RectTransform _inputPanel;
        [SerializeField] private RectTransform _guiPanel;
        [SerializeField] private RectTransform _startingPanel;
        [SerializeField] private RectTransform _endGamePanel;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _garbageText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private Slider _blurFill;
        [SerializeField] private Slider _pollutionSlider;
        [SerializeField] private Slider _timeSlider;
        [SerializeField] private Image _timeFillImage;
        [SerializeField] private TMP_Text plusTimeText;

        private Tweener _blurTween;
        private Tweener _pollutionTween;
        Sequence textSeq;

        private void Awake()
        {
            _inputPanel.gameObject.SetActive(false);
            _guiPanel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _blurTween.Kill();
            _pollutionTween.Kill();
        }

        public void Initialize()
        {
            _inputPanel.gameObject.SetActive(true);
            _guiPanel.gameObject.SetActive(true);
            _endGamePanel.gameObject.SetActive(false);
            ResetBlurAmount();
        }

        public void SetGarbageText(int count)
        {
            _garbageText.text = count.ToString();
        }

        public void SetLevelText(int count)
        {
            _levelText.text = $"Level: {count}";
        }

        public void SetTimerText(float timer, float maxTimer)
        {
            //_timeSlider.value = 60f - timer;
            //_timeSlider.DOValue(60f - Mathf.Ceil(timer), 0.2f);

            _timeFillImage.fillAmount = (maxTimer - timer) / maxTimer;
        }

        public void SetBlurAmount(float blurAmount)
        {
            //_blurTween.Kill();
            //_blurTween = DOTween.To(() => _blurFill.value, x => _blurFill.value = x, blurAmount, 0.2f);
            //_blurFill.value = Mathf.Clamp(blurAmount, 0f, 1f);

            _pollutionTween.Kill();
            _pollutionTween = DOTween.To(() => _pollutionSlider.value, x => _pollutionSlider.value = x, blurAmount, 0.2f);
            //_pollutionSlider.value = Mathf.Clamp(blurAmount, 0f, 1f);
            _pollutionSlider.DOValue(Mathf.Clamp(blurAmount, 0f, 1f), 0.2f);
        }

        public void ResetBlurAmount()
        {
            //_blurTween.Kill();
            //_blurFill.value = 0f;

            _pollutionTween.Kill();
            _pollutionSlider.value = 0f;
        }

        public void DisableStartPanel()
        {
            _startingPanel.gameObject.SetActive(false);
        }

        public void ActivateEndgamePanel()
        {
            _inputPanel.gameObject.SetActive(false);
            _guiPanel.gameObject.SetActive(false);
            _endGamePanel.gameObject.SetActive(true);
        }

        public void ExtraTimeAnim(int extraTime)
        {
            textSeq.Complete();
            plusTimeText.text = "+" + extraTime.ToString();

            textSeq = DOTween.Sequence();

            textSeq.Append(plusTimeText.DOFade(1f, 0.5f));
            textSeq.Join(plusTimeText.rectTransform.DOAnchorPosY(-20f, 0.5f));
            textSeq.Append(plusTimeText.DOFade(0f, 0.5f));
            textSeq.Append(plusTimeText.rectTransform.DOAnchorPosY(-40f, 0f));
        }
    }
}

