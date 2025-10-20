using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Localization;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Witmina_InputController;
using Random = UnityEngine.Random;

namespace Witmina_MathOfFuel
{
    public class LevelBehaviour : MonoBehaviour
    {
        public int levelId;
        [SerializeField] private List<W87_LevelSO> levels;
        public static W87_LevelSO LevelSO;
        private List<int> scores = new List<int>();

        [SerializeField] private UIController _uiController;

        private float _levelTimer;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private float flashInterval;
        [SerializeField] private GameObject _submitButton;
        [SerializeField] private MathTextController _mathTextController;
        [SerializeField] private List<ServiceBehaviour> _services;
        [SerializeField] private Transform _carEntry;
        [SerializeField] private Transform _carExit;
        [SerializeField] private List<Transform> _carSlots;
        [SerializeField] private float _carMoveDuration;
        [SerializeField] private Image countdownFill;

        private readonly List<CarBehaviour> _cars = new();
        private bool _timerRunning;
        private int _carCount;
        private int _trueCount;
        private int _falseCount;
        private int _correctCount;
        private int _failCount;
        private ServiceBehaviour _currentService;
        private QuestionData _currentQuestionData;
        private bool _isFinishRunning = false;
        private bool isFlashable = true;

        private ServiceButton _currentButton;

        private Sequence _carSequence;

        private float _timer;
        public float Timer
        {
            get => _timer;
            private set
            {
                _timer = value;
                _timerText.text = $"{LeanLocalization.GetTranslationText("Time")}: {Mathf.CeilToInt(_timer)}";
            }
        }

        private float _requestTimer;
        private float _requestTime;
        private int maxLevelWKeys;

        private void Awake()
        {
            // _levelTimer = WManagers.WRCM.GetGamePlaytime();
            _levelTimer = 90f;
        }

        void Start()
        {
            _uiController.UpdateLevelText(levelId);
        }

        private void Update()
        {
            if (!_timerRunning)
                return;

            if (Timer <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.Instance.PlayFx("Countdown", 0.7f, 1f);
                FlashRed();
            }

            if (Timer > 0)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                Timer = 0;

                GameManager.Instance.State = GameManager.GameState.idle;

                if (!_isFinishRunning)
                    Finish();
            }

            if (_requestTimer > 0)
            {
                _requestTimer -= Time.deltaTime;
                countdownFill.fillAmount = _requestTimer / _requestTime;
            }
            else if (_currentService)
            {
                _requestTimer = 0;
                OnEvaluate(false);
            }
        }

        public void Load()
        {
            maxLevelWKeys = levels.Count;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            _carSequence.Kill();
            for (int i = 0; i < _cars.Count; i++)
            {
                Destroy(_cars[i].gameObject);
            }
            _cars.Clear();
            foreach (var s in _services)
            {
                s.gameObject.SetActive(false);
            }
            _currentService = null;
            _currentQuestionData = null;

            _submitButton.gameObject.SetActive(false);
            _mathTextController.gameObject.SetActive(false);


            _trueCount = 0;
            _falseCount = 0;
            _correctCount = 0;
            _failCount = 0;
            Timer = _levelTimer;
            GenerateCars();
        }

        private void OnPressed(PointerEventData pData)
        {
            var origin = GameManager.MainCamera.ScreenToWorldPoint(pData.position);
            var direction = GameManager.MainCamera.transform.forward;
            var hit = Physics2D.Raycast(origin, direction,
                Single.MaxValue, Physics.AllLayers);

            if (!hit)
            {
                return;
            }

            if (hit.collider.TryGetComponent<ServiceButton>(out var button))
            {
                _currentButton = button;
            }

            else
                _currentButton = null;
        }

        private void OnMoved(PointerEventData pData)
        {
            if (_currentButton)
            {
                _currentButton.OnDrag(pData);
            }
        }

        private void OnReleased(PointerEventData pData)
        {
            _currentButton = null;
        }

        private void AssignLevel()
        {
            levelId = math.clamp(levelId, 1, maxLevelWKeys);
            LevelSO = levels[levelId - 1];

            _uiController.UpdateLevelText(levelId);
        }

        private void GenerateQuestion()
        {
            AssignLevel();

            if (_currentService)
                _currentService.gameObject.SetActive(false);

            var tierData = GameManager.PrefabData.GetQuestionTierData();

            Debug.Log($"Generating question for {tierData.ServiceType} {tierData.QuestionType} {tierData.Difficulty}");

            //Generate Question Data
            _currentQuestionData = new QuestionData
            {
                ServiceType = (ServiceType)Random.Range(0, (int)tierData.ServiceType + 1),
                QuestionType = (QuestionType)Random.Range(0, (int)tierData.QuestionType + 1),
                Difficulty = (QuestionDifficulty)Random.Range(0, (int)tierData.Difficulty + 1)
            };

            var service = _services.FirstOrDefault(s => s.Type == _currentQuestionData.ServiceType);
            if (!service)
            {
                Debug.LogError("No service found for the question");
                return;
            }

            _currentQuestionData.OperandType = service.OperandType;

            //Generate numbers
            var numbers = GameManager.PrefabData.GetNumbersInTier(_currentQuestionData.Difficulty);

            var denominator = numbers[Random.Range(0, numbers.Count)];

            Debug.Log($"Generated {denominator} for the question");

            switch (_currentQuestionData.QuestionType)
            {
                case QuestionType.Simple:
                    _currentQuestionData.Result = new Fraction(Random.Range(1, denominator + 1), denominator);
                    _currentQuestionData.Op2 = _currentQuestionData.Result.Simplified();
                    _currentQuestionData.Op1 = null;
                    break;
                case QuestionType.Addition:
                    _currentQuestionData.Result = new Fraction(Random.Range(1, denominator + 1), denominator);
                    var nom1 = Random.Range(1, _currentQuestionData.Result.Nominator);
                    _currentQuestionData.Op1 = new Fraction(nom1, denominator).Simplified();
                    _currentQuestionData.Op2 = new Fraction(_currentQuestionData.Result.Nominator - nom1, denominator).Simplified();
                    break;
                case QuestionType.Subtraction:
                    _currentQuestionData.Op1 = new Fraction(Random.Range(1, denominator + 1), denominator);
                    var nom2 = Random.Range(1, _currentQuestionData.Op1.Nominator);
                    _currentQuestionData.Result = new Fraction(nom2, denominator).Simplified();
                    _currentQuestionData.Op2 = new Fraction(_currentQuestionData.Op1.Nominator - nom2, denominator).Simplified();
                    break;
                case QuestionType.Multiplication:
                    var dividersR = Fraction.GetDividers(denominator);
                    var nominatorR = dividersR[Random.Range(1, dividersR.Count)];
                    _currentQuestionData.Result = new Fraction(nominatorR, denominator);

                    var dividersN = Fraction.GetDividers(nominatorR);
                    var nominator1 = dividersN[Random.Range(0, dividersN.Count)];
                    var denominator1 = dividersR[Random.Range(1, dividersR.Count)];
                    _currentQuestionData.Op1 = new Fraction(nominator1, denominator1);
                    _currentQuestionData.Op2 = new Fraction(nominatorR / nominator1,
                        denominator / denominator1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.Log($"Generated {_currentQuestionData.Result} for the question");

            _mathTextController.gameObject.SetActive(true);
            _mathTextController.SetQuestionText(LeanLocalization.GetTranslationText(service.Text));
            _mathTextController.SetText(_currentQuestionData);

            _currentService = service;
            _currentService.gameObject.SetActive(true);
            _currentService.Interactable = true;
            _currentService.SetService(_currentQuestionData.Result.Denominator, 0, _currentQuestionData.Result.Nominator);

            _requestTimer = tierData.WaitTime;
            _requestTime = tierData.WaitTime;

            _submitButton.gameObject.SetActive(true);
        }

        private void GenerateCars()
        {
            foreach (var slot in _carSlots)
            {
                SpawnCar(slot);
            }

            _carSequence.Kill();
            _carSequence = DOTween.Sequence().SetEase(Ease.Linear);
            var step = (_carMoveDuration / _cars.Count);
            for (int i = 0; i < _cars.Count; i++)
            {
                var car = _cars[i];
                car.transform.localPosition = 5f * Vector3.right;
                _carSequence.Insert(i * step, car.transform.DOLocalMove(Vector3.zero, _carMoveDuration)
                    .SetEase(Ease.OutQuad));
            }

            _carSequence.OnComplete(OnGenerateCarsComplete);
            _carSequence.Play();
        }

        private void OnGenerateCarsComplete()
        {
            GenerateQuestion();
            _timerRunning = true;
            GameManager.Instance.State = GameManager.GameState.playing;
            Subscribe();
        }


        private void SpawnCar(Transform slot)
        {
            var carPrefab = GameManager.PrefabData.GetRandomCar();
            var car = Instantiate(carPrefab, slot);
            car.transform.localPosition = Vector3.zero;
            _cars.Add(car);
        }

        private void MoveCars()
        {
            var initCar = _cars[0];
            _submitButton.gameObject.SetActive(false);

            _carSequence.Kill();
            _carSequence = DOTween.Sequence().SetEase(Ease.Linear);
            _carSequence.Append(initCar.transform.DOMove(_carExit.position, _carMoveDuration)
                .SetEase(Ease.InQuad))
                .OnStart(() => initCar.Moving = true)
                .OnComplete(() => initCar.Moving = false);

            SpawnCar(_carEntry);
            for (int i = 1; i < _cars.Count; i++)
            {
                var car = _cars[i];
                _carSequence.Insert(i * (_carMoveDuration / _cars.Count),
                    car.transform.DOMove(_carSlots[i - 1].position, _carMoveDuration)
                        .SetEase(Ease.InOutQuad)
                        .OnStart(() => car.Moving = true)
                        .OnComplete(() => car.Moving = false));
            }

            _carSequence.OnComplete(() =>
            {
                Destroy(_cars[0].gameObject);
                _cars.RemoveAt(0);
                GenerateQuestion();
            });
            _carSequence.PrependInterval(0.2f);
            _carSequence.Play();
        }


        public void Evaluate()
        {
            if (!_currentService)
                return;

            GameManager.PlayAudioFx(AudioFxType.Submit);
            OnEvaluate(_currentService.Evaluate());
        }

        private void OnEvaluate(bool success)
        {
            _currentService.Interactable = false;
            _currentService.gameObject.SetActive(false);
            _mathTextController.gameObject.SetActive(false);
            _currentService = null;
            _currentQuestionData = null;

            Debug.Log(success);

            if (success)
            {
                _correctCount++;
                _trueCount++;
                GameManager.PlayAudioFx(AudioFxType.Success, 0.2f);
            }
            else
            {
                _failCount++;
                _falseCount++;
                GameManager.PlayAudioFx(AudioFxType.Fail, 0.2f);
            }

            if (++_carCount >= LevelSO.levelUpCounter * 2)
            {
                if (CalculateLevelScore() >= LevelSO.minScore)
                {
                    levelId++;

                    _carCount = 0;
                    _trueCount = 0;
                    _falseCount = 0;
                }
                else
                {
                    if (++_falseCount >= LevelSO.levelDownCounter)
                    {
                        levelId--;

                        _carCount = 0;
                        _trueCount = 0;
                        _falseCount = 0;
                    }
                }
            }

            AssignLevel();
            _cars[0].GiveFeedback(success);
            MoveCars();
        }

        public int CalculateLevelScore()
        {
            int levelScore = Mathf.CeilToInt((_trueCount * 100) - (_falseCount * LevelSO.penaltyPoints));
            // Debug.Log($"Calculated level score: {levelScore} = ({_trueCount} * 100) - ({_falseCount} * {LevelSO.penaltyPoints})");

            levelScore = Mathf.Clamp(levelScore, 0, LevelSO.maxInLevel);
            // Debug.Log($"Clamped level score to: {levelScore} (between 0 and {LevelSO.maxInLevel})");

            _uiController.UpdateScoreText(levelScore);
            // Debug.Log("Updated score text to: " + levelScore);

            scores.Add(Mathf.Clamp(Mathf.CeilToInt((float)levelScore / LevelSO.maxInLevel * 1000), 0, 1000));

            return levelScore;
        }

        public int CalculateScore()
        {
            int total = 0;

            for (int i = 0; i < scores.Count; i++)
            {
                total += scores[i];
            }

            return Mathf.Clamp(total, 0, 1000);
        }

        public void Finish()
        {
            _isFinishRunning = true;
            _submitButton.gameObject.SetActive(false);
            _carSequence.Kill();
            GameManager.Instance.Finish(_correctCount, _failCount);
            Unsubscribe();
        }

        private void Subscribe()
        {
            ServiceBehaviour.ValueChanged += OnServiceValueChanged;

            InputController.Instance.Pressed += OnPressed;
            InputController.Instance.Moved += OnMoved;
            InputController.Instance.Released += OnReleased;
        }

        private void Unsubscribe()
        {
            ServiceBehaviour.ValueChanged -= OnServiceValueChanged;

            InputController.Instance.Pressed -= OnPressed;
            InputController.Instance.Moved -= OnMoved;
            InputController.Instance.Released -= OnReleased;
        }

        private void OnServiceValueChanged(float value)
        {
            if (_currentQuestionData == null)
                return;

            _mathTextController.UpdateResult(value);
            GameManager.PlayAudioFx(AudioFxType.Tick);
        }

        private void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(_timerText.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(_timerText.DOColor(Color.white, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }
    }
}