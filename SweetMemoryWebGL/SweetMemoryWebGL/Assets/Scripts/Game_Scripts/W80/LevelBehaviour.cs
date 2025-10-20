using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

namespace Witmina_SweetMemory
{
    public class LevelBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelIdText;

        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
        [SerializeField] private LevelSO levelSO;
        private float _levelTime;
        private List<int> scores = new List<int>();


        [SerializeField] private Animator _canvasAnimator;
        [SerializeField] private Transform _gamePanel;
        [SerializeField] private Transform _endPanel;
        [SerializeField] private CartController _cart;
        [SerializeField] private EnvironmentController _environment;
        [SerializeField] private QuestionPanel _questionPanel;
        [SerializeField] private Image _background;
        [SerializeField] private Image _feedbackTrue;
        [SerializeField] private Image _feedbackFalse;
        [SerializeField] private List<GameObject> characters = new List<GameObject>();
        [SerializeField] private Transform rightEdge, leftEdge;
        [SerializeField] private float countdownTimer = 5f;
        [SerializeField] private bool isCountdownOn = false;
        [SerializeField] private Image countdownFill;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private float flashInterval;
        //[SerializeField] private TMP_Text _timerText;

        private bool _cartIn;
        private bool _zoomedIn;
        private float _timer;
        private bool _timerRunning;
        private Tween _gamePanelTween;
        private Tween _backgroundTween;
        private Sequence _feedbackSequence;
        private CakeQuestion _currentQuestion;
        private int _tries = 1;
        private int _correctAnswers;
        private int _wrongAnswers;
        private int _totalCorrectAnswers;
        private int _totalWrongAnswers;
        private bool isFlashable = true;
        private int maxLevelWKeys;

        public float Timer
        {
            get => _timer;
            set
            {
                _timer = Mathf.Max(value, 0f);
                //_timerText.text = $"Time: {(int)Mathf.Ceil(_timer)}";
            }
        }

        public Animator CanvasAnimator => _canvasAnimator;

        private int MaxTries;

        private void Awake()
        {
            _cart.gameObject.SetActive(true);
            _questionPanel.gameObject.SetActive(false);
            _levelTime = 90f;
            // _levelTime = WManagers.WRCM.GetGamePlaytime();
            //_timerText.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_timerRunning)
            {
                if (Timer <= 5.2f && isFlashable)
                {
                    isFlashable = false;
                    // GameManager.Instance.PlayFx("Countdown", 0.7f, 1f);
                    FlashRed();
                }

                if (Timer > 0)
                {
                    Timer -= Time.deltaTime;
                    timeText.text = Timer.ToString("F0");
                }
                else
                {
                    _timerRunning = false;
                    EndLevel();
                }
            }

            Countdown();
        }

        private void OnDestroy()
        {
            _cart.MoveInCompleted -= OnMoveInCompleted;
            _cart.CoverCompleted -= OnCoverCompleted;

            _gamePanelTween.Kill();
            _backgroundTween.Kill();
        }

        private void GetQuestion()
        {
            _currentQuestion = GameManager.QuestionData.GetRandomQuestion(_cart.Cake, out var questionText);
            _questionPanel.SetQuestionText(questionText);
            _questionPanel.ToggleQuestionPanel(true);
            countdownTimer = levelSO.timePerQuestion;
            isCountdownOn = true;
            // Debug.LogError(_cart.Cake.EvaluateQuery(_currentQuestion));
        }

        private int CalculateScore()
        {
            int score = Mathf.Clamp((_correctAnswers * 100) - (_wrongAnswers * levelSO.penaltyPoint), 0, levelSO.maxInLevel);
            scores.Add(Mathf.Clamp(Mathf.CeilToInt((float)score / levelSO.maxInLevel * 1000), 0, 1000));
            return score;
        }

        public int CalculateTotalScore()
        {
            int totalScore = 0;

            for (int i = 0; i < scores.Count; i++)
            {
                totalScore += scores[i];
            }

            totalScore /= scores.Count;
            totalScore = Mathf.Clamp(totalScore, 0, 1000);
            return totalScore;
        }

        public void AnswerQuestion(bool trueButton)
        {
            isCountdownOn = false;
            var isTrue = _cart.Cake.EvaluateQuery(_currentQuestion);
            if (!trueButton)
                isTrue = !isTrue;

            if (countdownTimer <= 0)
                isTrue = false;

            GiveFeedback(isTrue);
            if (isTrue)
            {
                _correctAnswers++;
                _totalCorrectAnswers++;
                _questionPanel.TrueScore++;
            }
            else
            {
                _wrongAnswers++;
                _totalWrongAnswers++;
                _questionPanel.FalseScore++;
            }

            Debug.Log("BeforeTries: " + _tries);
            Debug.Log("MaxTries: " + MaxTries);
            if (_tries != MaxTries)
            {
                _tries++;
                _questionPanel.ToggleQuestionPanel(false);
                StartCoroutine(QuestionEndRoutine());
                Debug.Log("AfterTries: " + _tries);
                return;
            }

            int downCounter = PlayerPrefs.GetInt("SweetMemory_DownCounter", 0);
            int upCounter = PlayerPrefs.GetInt("SweetMemory_UpCounter", 0);

            int score = CalculateScore();
            if (score >= GameManager.LevelSO.minRequiredToPass)
            {
                _correctAnswers = 0;
                _wrongAnswers = 0;

                if (++upCounter >= 2)
                {
                    upCounter = 0;
                    downCounter = 0;
                    GameManager.Instance.PlayerLevel++;
                    Debug.Log("Level Up");
                }
                Debug.LogWarning("UpCounter: " + upCounter);
            }
            else
            {
                downCounter++;
                if (downCounter == 2)
                {
                    upCounter = 0;
                    downCounter = 0;
                    _correctAnswers = 0;
                    _wrongAnswers = 0;
                    GameManager.Instance.PlayerLevel--;
                    Debug.Log("Level Down");
                }
            }

            PlayerPrefs.SetInt("SweetMemory_DownCounter", downCounter);
            PlayerPrefs.SetInt("SweetMemory_UpCounter", upCounter);

            scoreText.text = "Score: " + score;

            _tries = 1;
            _questionPanel.ToggleQuestionPanel(false);
            StartCoroutine(QuestionEndRoutine());
        }

        private void OnMoveInCompleted()
        {
            GenerateNewCake();
            if (_zoomedIn)
                return;

            _zoomedIn = true;
            _gamePanelTween.Kill();
            _gamePanelTween = _gamePanel.DOScale(1.7f, 0.6f);
            _backgroundTween.Kill();
            _backgroundTween = _background.DOColor(new Color(0.5f, 0.5f, 0.5f, 1f), 0.6f);
            _cartIn = true;
        }

        private void OnCoverCompleted()
        {
            if (!_questionPanel.gameObject.activeSelf)
                _questionPanel.gameObject.SetActive(true);
            GetQuestion();
        }

        private void AssignLevel(int id)
        {
            id = Mathf.Clamp(id, 1, maxLevelWKeys);
            levelSO = levels[id - 1];
            GameManager.LevelSO = levelSO;

            MaxTries = levelSO.levelUpCriteria;
            levelIdText.text = $"{LeanLocalization.GetTranslationText("Level")} {id}";
        }

        public void Load()
        {
            maxLevelWKeys = levels.Count;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            AssignLevel(GameManager.Instance.PlayerLevel);

            _tries = 1;
            _correctAnswers = 0;
            _totalCorrectAnswers = 0;
            _totalWrongAnswers = 0;
            _zoomedIn = false;
            _endPanel.gameObject.SetActive(false);

            _cart.Initialize();
            _cart.gameObject.SetActive(true);
            _cart.MoveInCompleted += OnMoveInCompleted;
            _cart.CoverCompleted += OnCoverCompleted;
            _environment.Initialize();
            _environment.Active = true;
            _questionPanel.gameObject.SetActive(false);
            _questionPanel.Initialize();

            _cart.MoveIn();

            _feedbackTrue.gameObject.SetActive(false);
            _feedbackFalse.gameObject.SetActive(false);

            AudioController.PlayBG();

            _timer = _levelTime;
            //_timerText.gameObject.SetActive(true);
            _timerRunning = true;
        }

        private void EndLevel()
        {
            _timerRunning = false;
            _cart.MoveInCompleted -= OnMoveInCompleted;
            _cart.CoverCompleted -= OnCoverCompleted;
            _cart.gameObject.SetActive(false);

            _feedbackSequence.Kill();
            _feedbackTrue.gameObject.SetActive(false);

            StartCoroutine(EndRoutine());
        }

        private void GiveFeedback(bool correct)
        {
            var feedback = correct ? _feedbackTrue : _feedbackFalse;
            feedback.color = new Color(1f, 1f, 1f, 0f);

            _feedbackSequence.Kill();
            feedback.gameObject.SetActive(true);

            _feedbackSequence = DOTween.Sequence().SetEase(Ease.Linear);
            _feedbackSequence.Append(feedback.DOFade(1f, 0.2f));
            _feedbackSequence.Insert(0.5f, feedback.DOFade(0f, 0.2f));
            _feedbackSequence.OnComplete(() => feedback.gameObject.SetActive(false));

            AudioController.Play(correct ? AudioType.TrueAnswer : AudioType.FalseAnswer);
        }

        private void GenerateNewCake()
        {
            GameManager.Instance.PlayerLevel = Mathf.Clamp(GameManager.Instance.PlayerLevel, 1, maxLevelWKeys);
            AssignLevel(GameManager.Instance.PlayerLevel);

            _cart.SetCake(GameManager.CakeData.GetRandomCake(GameManager.Instance.PlayerLevel));
            _cart.CoverSequence();
        }

        private IEnumerator QuestionEndRoutine()
        {
            yield return new WaitForSeconds(1f);
            GenerateNewCake();
        }

        private IEnumerator EndRoutine()
        {
            _environment.Active = false;
            _environment.OnEnd();
            _cart.OnEnd();
            AudioController.Stop();

            _gamePanelTween.Kill();
            _gamePanelTween = _gamePanel.DOScale(1f, 0.5f);

            _backgroundTween.Kill();
            _backgroundTween = _background.DOColor(Color.white, 0.5f);

            _questionPanel.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            _endPanel.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            GameManager.Instance.End(_totalCorrectAnswers, _totalWrongAnswers);
        }

        private void Countdown()
        {
            if (isCountdownOn)
            {
                //timer continue if game is playing
                if (countdownTimer > 0)
                {
                    countdownTimer -= Time.deltaTime;
                    countdownFill.fillAmount = countdownTimer / levelSO.timePerQuestion;
                }
                //stop timer if time ran out
                else if (countdownTimer < 0)
                {
                    isCountdownOn = false;
                    countdownTimer = 0;
                    AnswerQuestion(false);
                }
            }
        }

        public float GetLeftTime()
        {
            return countdownTimer;
        }

        private void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(timeText.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(timeText.DOColor(new Color(255f, 243f, 173f, 1f), flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }
    }
}