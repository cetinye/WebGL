using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Witmina_MarineManagement
{
    public class Port : MonoBehaviour
    {
        public static event Action<Port> BubbleSelected;

        [SerializeField] private Transform _craneTransform;
        [SerializeField] private RequestBubble _requestBubble;
        [SerializeField] private GameObject _waitingBubble;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private float _scoreTextMoveAmount = 1f;
        [SerializeField] private float _scoreTextMoveDuration = 0.2f;
        [SerializeField] private Color _scorePosColor;
        [SerializeField] private Color _scoreNegColor;

        public Boat CurrentBoat;
        public bool Occupied => CurrentBoat != null;

        [HideInInspector] public float MaxRequestTimer = 10f;

        [HideInInspector] public PortState State = PortState.Idle;

        private Vector3 _scoreInitPos;
        private Tween _craneTween;
        private Sequence _scoreTween;
        private Request _currentRequest;
        public Request CurrentRequest
        {
            get => _currentRequest;
            set
            {
                _currentRequest = value;
                if (_requestBubble)
                    _requestBubble.gameObject.SetActive(_currentRequest != null);
                if (_currentRequest == null || !GameManager.PrefabData)
                    return;
            }
        }

        public float WorkTimer = 2f;
        public float WorkTimerMax = 2f;
        public float LeaveTimer;
        public float NewRequestTimer;

        private void Awake()
        {
            CurrentRequest = null;
            _scoreInitPos = _scoreText.transform.position;
            _requestBubble.ProgressFillAmount = 0f;
            _requestBubble.BubbleSelected += OnBubbleSelected;
        }


        public void Initialize()
        {
            State = PortState.Idle;
            CurrentRequest = null;
            ToggleBubbleSelection(false);
            ToggleWaitingBubble(false);
            _craneTween.Kill();
            _craneTransform.rotation = transform.rotation;
            WorkTimer = WorkTimerMax = 2f;
            _requestBubble.ProgressFillAmount = 0f;
            _scoreText.transform.position = _scoreInitPos;
            _scoreText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _craneTween.Kill();
            _requestBubble.BubbleSelected -= OnBubbleSelected;
        }

        public void OnBubbleSelected()
        {
            BubbleSelected?.Invoke(this);

            Taptic.Success();
        }

        public void ShowScore(int score)
        {
            _scoreTween.Kill();

            var prefix = score < 0 ? "-" : "+";
            _scoreText.text = $"{prefix}{score}";
            _scoreText.transform.position = _scoreInitPos;
            _scoreText.color = score < 0 ? _scoreNegColor : _scorePosColor;

            _scoreTween = DOTween.Sequence();
            _scoreTween.AppendCallback(() => _scoreText.gameObject.SetActive(true));
            _scoreTween.Insert(0f, _scoreText.transform.DOMove(_scoreInitPos + _scoreTextMoveAmount * Vector3.up, 0.2f));
            _scoreTween.Insert(0f, _scoreText.DOFade(0.3f, _scoreTextMoveDuration));
            _scoreTween.AppendCallback(() => _scoreText.gameObject.SetActive(false));
            _scoreTween.Play();
        }

        private void Update()
        {
            if (_currentRequest == null)
                return;

            _requestBubble.TimerEnabled = State == PortState.WaitingRequest;
            _requestBubble.TimerFillAmount = _currentRequest.Timer / MaxRequestTimer;
            _requestBubble.ProgressFillAmount = 1f - (WorkTimer / WorkTimerMax);
        }

        public void ToggleCrane(bool active)
        {
            if (!_craneTransform)
                return;

            _craneTween.Kill();
            var rotation = active ?
                Quaternion.LookRotation(Vector3.forward, _craneTransform.position - transform.position)
                : transform.rotation;
            _craneTween = _craneTransform.DORotateQuaternion(rotation, 0.5f)
                .SetEase(Ease.Linear);
        }

        public void SetRequestSprite(Sprite sprite)
        {
            _requestBubble.Sprite = sprite;
        }

        public void ToggleBubbleSelection(bool active)
        {
            if (CurrentBoat)
                CurrentBoat.Selected = active;

            _requestBubble.Selected = active;
        }

        public void ToggleWaitingBubble(bool active)
        {
            _waitingBubble.SetActive(active);
        }
    }
}