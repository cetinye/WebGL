using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Localization;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Witmina_rotf
{
    public class LevelBehaviour : MonoBehaviour
    {
        #region Fields
        [SerializeField] private int _sequenceId;
        [SerializeField] private int _sequenceTake;
        [SerializeField] private bool _recordMode;
        [SerializeField] private bool _debugMode;

        private FeedbackController _feedback;
        private MushroomController2D _mushroomController;
        private BarController _barController;

        private List<SequenceElement> _sequence = new();
        private List<SequenceElement> _playerSequence = new();

        private float _timer;
        private bool _sequencePlaying;
        private bool _completed;
        private bool _timerRunning;
        public bool success;
        public bool perfect;
        private int _currentTake;
        private int _perfectCount;
        private int _correctCount;
        private int _failCount;
        private bool _solving;

        private Coroutine _playRoutine;

        private static readonly float StartDelay = 0.6f;

        //Countdown
        [SerializeField] private float countdownTime = 5f;

        public bool isCountdownOn = false;
        public float countdownTimer = 5f;
        #endregion

        #region Unity Methods

        private void Awake()
        {
            _currentTake = Random.Range(0, 6);
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (_recordMode && Input.GetKeyDown(KeyCode.Space))
                SaveSequence(_sequenceId, _sequenceTake);

            if (Input.GetKeyDown(KeyCode.Q))
                ActivateMushroom(0);
            if (Input.GetKeyDown(KeyCode.W))
                ActivateMushroom(1);
            if (Input.GetKeyDown(KeyCode.E))
                ActivateMushroom(2);
            if (Input.GetKeyDown(KeyCode.R))
                ActivateMushroom(3);

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (_playRoutine != null)
                {
                    StopCoroutine(_playRoutine);
                    _sequencePlaying = false;
                }
                _playRoutine = StartCoroutine(PlaySequence());
            }

            if (Input.GetKeyDown(KeyCode.H) && !_sequencePlaying && !_completed && !_solving)
            {
                StartCoroutine(SolveSequence());
            }
        }
#endif

        private void FixedUpdate()
        {
            if (_timerRunning)
            {
                _timer += Time.fixedDeltaTime;
                if (_barController)
                    _barController.SetTimerLine(_timer);
            }

            Countdown();
        }

        #endregion

        #region Public Methods
        public void LoadLevel(int playerLevel)
        {
            _mushroomController = GetComponentInChildren<MushroomController2D>();
            if (!_mushroomController)
            {
                Debug.LogError($"No mushroom controller found in level {name}");
                return;
            }

            _barController = GetComponentInChildren<BarController>();
            if (!_barController)
            {
                Debug.LogError($"No bar controller found in level {name}");
                return;
            }

            _feedback = GetComponentInChildren<FeedbackController>();
            if (!_feedback)
            {
                Debug.LogError($"No feedback controller found in level {name}");
                return;
            }

            _feedback.Initialize();
            _barController.Clear();
            _completed = false;

            Subscribe();

#if UNITY_EDITOR
            if (_recordMode)
                return;
#endif

            LoadSequence(GameManager.LevelSO.numOfRhythm, GameManager.LevelSO.typeOfRhythm);
        }
        #endregion

        #region Helpers
        private void StartTimer()
        {
            _timer = 0;
            _timerRunning = true;
        }

        private void SaveSequence(int id, int take)
        {
            FileHandler.SaveToJSON(_sequence, $"Sequence_{id}-{take}.json");
        }

        private void LoadSequence(int rhythmCount, int take)
        {
            var file = GameManager.Instance.LevelData.GetLevelData(rhythmCount, take).ToString();
            _sequence = JsonHelper.FromJson<SequenceElement>(file).ToList();

            if (_playRoutine != null)
            {
                StopCoroutine(_playRoutine);
            }
            _sequencePlaying = false;
            _barController.SetBar(_sequence);
            _barController.Active = true;
            _playRoutine = StartCoroutine(PlaySequence());
            perfect = true;

        }

        private void MushroomOnPressed(int index)
        {
            if (_completed || _sequencePlaying)
                return;

            isCountdownOn = false;
            ActivateMushroom(index);
            // Taptic.Light();
        }

        private void ActivateMushroom(int index)
        {
            _mushroomController.PlayMushroom(index);
#if UNITY_EDITOR
            if (_recordMode)
            {
                if (_sequence.Count < 1)
                    StartTimer();

                _sequence.Add(new SequenceElement(index, _timer));
                return;
            }
#endif
            if (_completed)
                return;

            if (_playerSequence.Count < 1)
                StartTimer();

            _playerSequence.Add(new SequenceElement(index, _timer));

            var lastIndex = _playerSequence.Count - 1;

            _barController.AddLine(_timer);

            if (_playerSequence[lastIndex].Index != _sequence[lastIndex].Index)
            {
                success = false;
            }
            else
            {
                var diff = Mathf.Abs(_playerSequence[lastIndex].Time - _sequence[lastIndex].Time);
                if (diff < 0.1f)
                {
                    _barController.ActivateMushroom(lastIndex);
                }
                else if (diff < 0.25f)
                {
                    _barController.ActivateMushroom(lastIndex);
                    perfect = false;
                }
                else
                {
                    _barController.ActivateMushroom(lastIndex, false);
                    success = false;
                    perfect = false;
                }
            }

            if (_playerSequence.Count == _sequence.Count)
            {
                _completed = true;
                countdownTimer = countdownTime;
                StartCoroutine(FinishRoutine());
            }
        }

        private void Countdown()
        {
            if (isCountdownOn)
            {
                //timer continue if game is playing
                if (countdownTimer > 0)
                {
                    countdownTimer -= Time.deltaTime;
                }
                //stop timer if time ran out
                else if (countdownTimer < 0)
                {
                    isCountdownOn = false;
                    countdownTimer = 0;
                    StartCoroutine(FinishRoutine());
                }
            }
        }

        public void ResetCountdown()
        {
            countdownTimer = countdownTime;
        }
        #endregion

        #region Event Functions
        private void Subscribe()
        {
            _mushroomController.MushroomPressed += MushroomOnPressed;
        }

        private void Unsubscribe()
        {
            _mushroomController.MushroomPressed -= MushroomOnPressed;
        }

        public void OnFinish()
        {
            Unsubscribe();
            _sequence.Clear();
            _playerSequence.Clear();
        }

        #endregion

        #region Coroutines

        private IEnumerator PlaySequence()
        {
            _mushroomController.SetPressable(false);
            _sequencePlaying = true;
            success = true;
            yield return new WaitForSeconds(1f);
            _feedback.ShowFeedback(LeanLocalization.GetTranslationText("GetReady"), 1f);
            yield return new WaitForSeconds(2f);
            _barController.ToggleTimerLine(true);
            _timer = 0f;
            _timerRunning = true;

            for (int i = 0; i < _sequence.Count; i++)
            {
                var seq = _sequence[i];
                _mushroomController.PlayMushroom(seq.Index);
                // Taptic.Light();
                _barController.ActivateMushroom(i);
                if (i == _sequence.Count - 1)
                    break;

                var duration = _sequence[i + 1].Time - seq.Time;
                yield return new WaitForSecondsRealtime(duration);
            }

            _timerRunning = false;
            _barController.ToggleTimerLine(false);
            yield return new WaitForSeconds(StartDelay);
            _barController.DisableMushrooms();
            _feedback.ShowYourTurn(1f);
            _sequencePlaying = false;
        }

        private IEnumerator SolveSequence()
        {
            if (_solving)
                yield break;

            _solving = true;
            _mushroomController.SetPressable(false);
            _completed = false;
            _playerSequence.Clear();

            StartTimer();

            for (int i = 0; i < _sequence.Count; i++)
            {
                var seq = _sequence[i];
                if (i > 0)
                {
                    var delay = _sequence[i].Time - _sequence[i - 1].Time;
                    yield return new WaitForSecondsRealtime(delay);
                }
                ActivateMushroom(seq.Index);
            }

            _solving = false;
        }

        private IEnumerator FinishRoutine()
        {
            if (countdownTimer > 0)
                _feedback.ShowFeedback(success ? (perfect ? LeanLocalization.GetTranslationText("Perfect") : LeanLocalization.GetTranslationText("AlmostDone")) : LeanLocalization.GetTranslationText("NotEnough"), 0.8f);

            else
                _feedback.ShowFeedback(LeanLocalization.GetTranslationText("TimesUp"), 0.8f);

            yield return new WaitForSecondsRealtime(1f);
            _barController.Active = false;
            GameManager.Instance.FinishLevel(success, perfect);
        }

        public void SetPressable(bool state)
        {
            _mushroomController.SetPressable(state);
        }

        #endregion
    }
}

