using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chroma_City_Neon_Racing
{

    public class UIManager : MonoBehaviour
    {
        [Header("TMP_Text")]
        [SerializeField] private TMP_Text levelTimerText;
        [SerializeField] private TMP_Text gameStateText;
        [SerializeField] private TMP_Text levelIdText;
        [SerializeField] private TMP_Text pointAmountText;
        [SerializeField] private TMP_Text playerTargetSpeed;
        [SerializeField] private TMP_Text gameScoreText;
        [SerializeField] private TMP_Text witminaScoreText;

        [Header("TMP_Text")]
        [SerializeField] private List<Image> speedFills = new List<Image>();

        [Header("Image Animation Variables")]
        [SerializeField] private Image timeImage;
        [SerializeField] private Image speedImage;
        [SerializeField] private Image shieldImage;
        [SerializeField] private RectTransform timeTargetRect;
        [SerializeField] private RectTransform speedTargetRect;
        [SerializeField] private float timeToMove;
        [SerializeField] private Vector3 scaleTo;
        private Vector3 defaultTimeImagePos, defaultSpeedImagePos, defaultShieldImagePos;
        private Sequence animSeq;
        private Tween move, scale, rotation, fade;

        [Header("Flash Variables")]
        [SerializeField] private float flashInterval = 0.5f;

        [Header("DEBUG Button Variables")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button restartButton;

        void Start()
        {
            GameEvents.instance.timePickedUp += OnTimePickedUp;
            GameEvents.instance.speedPickedUp += OnSpeedPickedUp;
            GameEvents.instance.shieldPickedUp += OnShieldPickedUp;

            defaultTimeImagePos = timeImage.rectTransform.position;
            defaultSpeedImagePos = speedImage.rectTransform.localPosition;
            defaultShieldImagePos = shieldImage.rectTransform.localPosition;
        }

        void OnDisable()
        {
            GameEvents.instance.timePickedUp -= OnTimePickedUp;
            GameEvents.instance.speedPickedUp -= OnSpeedPickedUp;
            GameEvents.instance.shieldPickedUp -= OnShieldPickedUp;
        }

        public void UpdateTimer(float timer)
        {
            levelTimerText.text = timer.ToString("F0");
        }

        public void UpdateDebugTexts(string text, int levelId, int pointAmount, float playerSpeed)
        {
            gameStateText.text = "CurrentGameState: " + text;
            levelIdText.text = "LevelID: " + levelId;
            pointAmountText.text = "PointAmount: " + pointAmount;
            playerTargetSpeed.text = "PlayerSpeed: " + playerSpeed.ToString("F2");
        }

        public void UpdateScoreTexts(int wScore, int gScore)
        {
            gameScoreText.text = "Game Score: " + gScore.ToString("");
            witminaScoreText.text = "Witmina Score: " + wScore.ToString("");
        }

        public void UpdateSpeedMeter(float playerSpeed)
        {
            int amountToShow = Mathf.CeilToInt((playerSpeed * speedFills.Count) / 6f);

            StartCoroutine(SpeedMeterRoutine(amountToShow));
        }

        IEnumerator SpeedMeterRoutine(int amountToShow)
        {
            Tween fadeTween = null;

            for (int i = 0; i < speedFills.Count; i++)
            {
                if (i < amountToShow)
                {
                    fadeTween = speedFills[i].DOFade(1f, 0.005f);
                    yield return fadeTween.WaitForCompletion();
                }
            }

            for (int i = speedFills.Count - 1; i >= 0; i--)
            {
                if (i > amountToShow)
                {
                    fadeTween = speedFills[i].DOFade(0f, 0.005f);
                    yield return fadeTween.WaitForCompletion();
                }
            }
        }

        public void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(levelTimerText.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(levelTimerText.DOColor(Color.white, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }

        #region Power Up Animations

        public void OnTimePickedUp()
        {
            ResetImage(timeImage);
            timeImage.enabled = true;
            AnimateTimeImage();
        }

        public void OnSpeedPickedUp()
        {
            ResetImage(speedImage);
            speedImage.enabled = true;
            AnimateSpeedImage();
        }

        public void OnShieldPickedUp()
        {
            ResetImage(shieldImage);
            shieldImage.enabled = true;
            AnimateShieldImage();
        }

        private void ResetImage(Image imageToReset)
        {
            move?.Kill();
            scale?.Kill();
            rotation?.Kill();
            animSeq?.Kill();

            imageToReset.rectTransform.DOScale(Vector3.one, 0f);

            timeImage.enabled = false;
            speedImage.enabled = false;
            shieldImage.enabled = false;

            timeImage.rectTransform.position = defaultTimeImagePos;
            speedImage.rectTransform.localPosition = defaultSpeedImagePos;
            shieldImage.rectTransform.localPosition = defaultShieldImagePos;

            speedImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            shieldImage.DOFade(0f, 0f);
        }


        private void AnimateTimeImage()
        {
            animSeq = DOTween.Sequence();

            move = timeImage.rectTransform.DOMove(timeTargetRect.position, timeToMove).SetEase(Ease.OutQuad);
            scale = timeImage.rectTransform.DOScale(scaleTo, timeToMove / 2f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);

            animSeq.Append(move);
            animSeq.Join(scale);
            animSeq.OnComplete(() => ResetImage(timeImage));
        }

        private void AnimateSpeedImage()
        {
            animSeq = DOTween.Sequence();

            move = speedImage.rectTransform.DOLocalMove(speedTargetRect.localPosition, timeToMove).SetEase(Ease.OutExpo);
            scale = speedImage.rectTransform.DOScale(scaleTo, timeToMove / 2f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
            rotation = speedImage.rectTransform.DORotate(new Vector3(0f, 360f, 0f), timeToMove - 1f, RotateMode.WorldAxisAdd).SetEase(Ease.InOutCubic);

            animSeq.Append(move);
            animSeq.Join(scale);
            animSeq.Insert(timeToMove - 1.8f, rotation);
            animSeq.OnComplete(() => ResetImage(speedImage));
        }

        private void AnimateShieldImage()
        {
            animSeq = DOTween.Sequence();

            scale = shieldImage.rectTransform.DOScale(scaleTo, timeToMove / 2f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
            fade = shieldImage.DOFade(1f, timeToMove / 2f).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo);

            animSeq.Append(scale);
            animSeq.Join(fade);
            animSeq.OnComplete(() => ResetImage(shieldImage));
        }

        #endregion
    }
}