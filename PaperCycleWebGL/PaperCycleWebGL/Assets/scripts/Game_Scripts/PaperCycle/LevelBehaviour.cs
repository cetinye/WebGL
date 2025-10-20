using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Witmina_PaperCycle
{
    public class LevelBehaviour : MonoBehaviour
    {
        [SerializeField] private PlayerBehaviour _player;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private RoadController _roadController;
        [SerializeField] private float _levelTimer = 60f;
        [SerializeField] private Image _timerFill;
        [SerializeField] private float flashInterval;
        [SerializeField] private TMP_Text _correctAnswerText;
        [SerializeField] private Transform _backgroundTransform;
        
        private bool _finished;
        private bool _timerRunning;
        private Vector3 _cameraOffset;
        private Vector3 _backgroundInitPos;

        private bool isFlashable = true;

        private float _timer;
        public float Timer
        {
            get => _timer;
            private set
            {
                _timer = value;
                //_timerText.text = $"Time: {Mathf.CeilToInt(_timer)}";
                _timerFill.fillAmount = Mathf.Clamp(_timer / _levelTimer, 0f, 1f);
            }
        }
        
        private int _correctAnswers;
        public int CorrectAnswers
        {
            get => _correctAnswers;
            private set
            {
                _correctAnswers = value;
                if (GameManager.Instance.locale.Equals("tr"))
                    _correctAnswerText.text = $"Doğru: {_correctAnswers}";
                else
                    _correctAnswerText.text =  $"Correct: {_correctAnswers}";
            }
        }

        private int _falseAnswers;

        private void Awake()
        {
            _player.Moving = false;
            _cameraOffset = _player.transform.InverseTransformPoint(_cameraTransform.position);
            _backgroundInitPos = _backgroundTransform.localPosition;
        }

        private void Update()
        {
            if (_finished)
                return;
            
            _cameraTransform.position = _player.transform.TransformPoint(_cameraOffset);
            
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
                Finish();
            }

            _backgroundTransform.localPosition += 0.1f * Time.deltaTime * Vector3.up;
        }

        public void Load()
        {
            Subscribe();
            CorrectAnswers = 0;
            _falseAnswers = 0;
            Timer = _levelTimer;
            _player.Initialize();
            _player.Moving = true;
            _roadController.Initialize();
            _roadController.Active = true;
            _timerRunning = true;
            _finished = false;
            _backgroundTransform.localPosition = _backgroundInitPos;
        }

        public void Finish()
        {
            _finished = true;
            _roadController.Active = false;
            _player.OnFinish();
            Unsubscribe();
            GameManager.Instance.Finish(CorrectAnswers, _falseAnswers);
        }

        private void Subscribe()
        {
            _player.AnswerGiven += OnAnswerGiven;
        }

        private void Unsubscribe()
        {
            _player.AnswerGiven -= OnAnswerGiven;
        }
        
        private void OnAnswerGiven(bool correct)
        {
            if (correct)
            {
                CorrectAnswers++;
                if (CorrectAnswers % 6 == 0)
                    GameManager.Instance.PlayerLevel++;
            }
            else
            {
                GameManager.Instance.PlayerLevel--;
                _falseAnswers++;
            }

            GameManager.Instance.PlayerLevel = Mathf.Clamp(GameManager.Instance.PlayerLevel, 1, 25);
            PlayerPrefs.SetInt("level", GameManager.Instance.PlayerLevel);
        }

        private void FlashRed()
        {
            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(_timerFill.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(_timerFill.DOColor(Color.green, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }
    }
}