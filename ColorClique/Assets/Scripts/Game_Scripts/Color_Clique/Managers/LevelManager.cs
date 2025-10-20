using System.Collections;
using System.Collections.Generic;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace Color_Clique
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;

        [Header("Level Variables")]
        public int levelId;
        private int maxLevelWKeys;
        [SerializeField] private LevelSO levelSO;
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();

        [Header("Scene Variables")]
        private float timePerQuestion;
        private int numberOfColors;
        private int shapeCount;
        private int wheelSegments;
        private float rotationSpeed;
        private bool isWheelBarReversalEnabled;
        private int minChangeFrequency;
        private int maxChangeFrequency;
        [HideInInspector] public bool IsTimerOn => isTimerOn;
        private bool isTimerOn;
        public float levelTimer;
        private int correctCount = 0;
        private int wrongCount = 0;
        private int comboCounter = 0;
        public int totalCorrect, totalWrong, totalCombo;
        private bool isClickable;
        private int moveLimitForChange;
        private int moveCounter;
        private int levelUpCounter;
        private int levelDownCounter;

        [Header("Scene Components")]
        [SerializeField] UIManager uiManager;
        [SerializeField] Wheel wheel;
        [SerializeField] SpriteRenderer selectedSp;
        [SerializeField] SpriteRenderer selectedSpBG;
        [SerializeField] Slot selectedSlot;
        [SerializeField] Color selectedColor;
        [SerializeField] private Animator crowdAnimator;
        [SerializeField] private Animator curtainAnimator;
        [SerializeField] private ParticleSystem combo;
        [SerializeField] private List<ParticleSystem> confettis = new List<ParticleSystem>();
        [SerializeField] private Camera particleCam;
        [SerializeField] private TMPro.TMP_Text comboText;

        [Header("Flash Interval")]
        [SerializeField] private bool isFlashable = true;

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
        }

        public void StartGame()
        {
            maxLevelWKeys = levels.Count / 2;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            StartCoroutine(StartGameRoutine());
        }

        void Update()
        {
            LevelTimer();

#if UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Clicked();
            }
#endif
        }

        IEnumerator StartGameRoutine()
        {
            AssignLevelVariables();
            AssignWheelVariables();
            SetMoveLimit();
            wheel.Initialize();
            SelectItem();

            OpenCurtains();

            yield return new WaitForSeconds(1.1f);

            isTimerOn = true;
            isClickable = true;
            wheel.StartTimer(GetTimePerQuestion());

            AudioManager.instance.Play(SoundType.Background);
            AudioManager.instance.Play(SoundType.Ambient);
            AudioManager.instance.PlayTickSpeed(levelSO.spinSpeedMultiplier);
        }

        public void AssignLevelVariables()
        {
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            levelSO = levels[levelId - 1];

            numberOfColors = levelSO.numberOfColors;
            shapeCount = levelSO.shapeCount;
            wheelSegments = levelSO.wheelSegments;
            rotationSpeed = 2 * levelSO.spinSpeedMultiplier;
            timePerQuestion = levelSO.answerTime;
            isWheelBarReversalEnabled = levelSO.isWheelBarReversalEnabled;
            minChangeFrequency = levelSO.minChangeFrequency;
            maxChangeFrequency = levelSO.maxChangeFrequency;

            if (isTimerOn)
                AudioManager.instance.PlayTickSpeed(levelSO.spinSpeedMultiplier);
        }

        private void AssignWheelVariables()
        {
            wheel.AssignWheelVariables(wheelSegments, rotationSpeed);
        }

        private void LevelTimer()
        {
            if (!isTimerOn) return;

            levelTimer -= Time.deltaTime;

            if (levelTimer < 0)
            {
                isTimerOn = false;
                wheel.SetSliderState(false);
                levelTimer = 0;
                CloseCurtains();
                GameManager.instance.Finish();
            }

            if (levelTimer <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
                uiManager.FlashRed();
            }

            uiManager.SetTimeText(levelTimer); ;
        }

        private void SetMoveLimit()
        {
            moveLimitForChange = Random.Range(minChangeFrequency, maxChangeFrequency);
            moveCounter = 0;
        }

        public void Check(Sprite clickedImage, Color clickedColor, bool isTimeOut = false)
        {
            isClickable = false;

            if (!isTimeOut && selectedSp.enabled == true && selectedSp.sprite == clickedImage && selectedColor == clickedColor)
            {
                Correct();
            }
            else if (!isTimeOut && selectedSp.enabled == false && selectedColor == clickedColor)
            {
                Correct();
            }
            else
            {
                Wrong();
            }

            //select new item if move limit reached
            moveCounter++;
            if (moveCounter >= moveLimitForChange)
            {
                SelectItem();
                SetMoveLimit();
            }

            CheckLevel();
            wheel.StartTimer(GetTimePerQuestion());
        }

        private void Correct()
        {
            correctCount++;
            totalCorrect++;
            comboCounter++;
            totalCombo++;

            levelUpCounter++;
            uiManager.UpdateStats(correctCount, wrongCount);
            wheel.SetNeedleColor(Color.green, 0.5f);
            wheel.GiveFeedback(Color.green);
            AudioManager.instance.PlayCorrect(Mathf.Clamp(comboCounter, 0, 7));
            SelectItem();

            if (comboCounter >= 2)
            {
                PlayCombo();
            }
            if (comboCounter >= 10)
            {
                PlayConfettis();
            }
        }

        public void Wrong()
        {
            wrongCount++;
            totalWrong++;
            comboCounter = 0;

            levelDownCounter++;
            uiManager.UpdateStats(correctCount, wrongCount);
            wheel.SetNeedleColor(Color.red, 0.5f);
            wheel.GiveFeedback(Color.red);
            AudioManager.instance.PlayOneShot(SoundType.Confetti);
        }

        private void PlayCombo()
        {
            comboText.text = comboCounter.ToString() + $"x {LeanLocalization.GetTranslationText("Combo")}";
            particleCam.gameObject.SetActive(true);
            combo.Play();
            CancelInvoke(nameof(DisableParticleCam));
            Invoke(nameof(DisableParticleCam), 0.5f);
        }

        private void PlayConfettis()
        {
            AudioManager.instance.PlayOneShot(SoundType.Confetti);

            foreach (ParticleSystem confetti in confettis)
            {
                confetti.Play();
            }
        }

        private void DisableParticleCam()
        {
            particleCam.gameObject.SetActive(false);
        }

        public void Clicked()
        {
            if (!isClickable) return;

            Slot clickedSlot = wheel.GetClickedSlot();
            Check(clickedSlot.GetItemSprite(), clickedSlot.GetSlotColor());

            if (levelSO.isWheelBarReversalEnabled)
            {
                wheel.ReverseNeedle();
            }
        }

        private int CalculateLevelScore()
        {
            int levelScore = Mathf.CeilToInt((correctCount * levelSO.pointsPerQuestion) + (comboCounter * levelSO.comboScore) - (wrongCount * levelSO.penaltyPoints));
            levelScore = Mathf.Max(levelScore, 0);
            return levelScore;
        }

        public int GetTotalScore()
        {
            float totalScore = (totalCorrect * levelSO.pointsPerQuestion) + (totalCombo * levelSO.comboScore) - (totalWrong * levelSO.penaltyPoints);
            float maxInGame = ((totalCorrect + totalWrong) * levelSO.pointsPerQuestion) + (totalCombo * levelSO.comboScore);
            int witScore = Mathf.Clamp(Mathf.CeilToInt(totalScore / maxInGame * 1000f), 0, 1000);
            return witScore;
        }

        private void CheckLevel()
        {
            if (levelUpCounter >= levelSO.levelUpCriteria * 2)
            {
                levelUpCounter = 0;

                SetLevel(++levelId);
                CrowdClap();
            }

            if (levelDownCounter >= levelSO.levelDownCriteria * 2)
            {
                levelDownCounter = 0;

                SetLevel(--levelId);
                CrowdShout();
            }

            uiManager.SetStageScoreText(CalculateLevelScore());
            uiManager.SetAverageScoreText(GetTotalScore());

            // uiManager.SetDebugTexts(levelId, levelDownCounter, levelUpCounter);
        }

        private void SetLevel(int levelId)
        {
            this.levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            AssignLevelVariables();
            AssignWheelVariables();
            wheel.Initialize();
            SelectItem();
        }

        public void SelectItem()
        {
            Slot previousSlot = selectedSlot;

            do
            {
                selectedSlot = wheel.SelectSlot();
                selectedSp.sprite = selectedSlot.GetItemSprite();
                selectedColor = selectedSlot.GetSlotColor();
                selectedSpBG.color = selectedColor;

            } while (previousSlot == selectedSlot);
        }

        public void SetIsClickable(bool state)
        {
            isClickable = state;
        }

        public Wheel GetWheel()
        {
            return wheel;
        }

        public int GetNumberOfColors()
        {
            return numberOfColors;
        }

        public int GetShapeCount()
        {
            return shapeCount;
        }

        public float GetTimePerQuestion()
        {
            return timePerQuestion;
        }

        #region Animations

        public void OpenCurtains()
        {
            AudioManager.instance.PlayAfterXSeconds(SoundType.CurtainOpen, 0.2f);
            curtainAnimator.Play("CurtainOpen", -1, 0.0f);
        }

        public void CloseCurtains()
        {
            AudioManager.instance.PlayAfterXSeconds(SoundType.CurtainOpen, 0.2f);
            curtainAnimator.Play("CurtainClose", -1, 0f);
        }

        public void CrowdClap()
        {
            AudioManager.instance.PlayOneShot(SoundType.Clap);
            crowdAnimator.Play("Clap", -1, 0.0f);
        }

        public void CrowdShout()
        {
            AudioManager.instance.PlayOneShot(SoundType.Shout);
            crowdAnimator.Play("Shout", -1, 0.0f);
        }

        #endregion
    }
}