using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Witmina_PaperCycle
{
    public class PlayerBehaviour : MonoBehaviour
    {
        public event Action<bool> AnswerGiven;
        
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _throwTreshold = 50;
        [SerializeField] private Transform _throwTransform;
        [SerializeField] private RequestUI _requestUI;
        [SerializeField] private FeedbackUI _feedbackUI;
        [SerializeField] private AudioSource _bicycleAudio;

        private List<HouseBehaviour> _houses = new();

        private bool _moving;
        public bool Moving
        {
            get => _moving;
            set
            {
                _moving = value;
                if (_animator)
                    _animator.enabled = _moving;

                if (!_bicycleAudio)
                    return;
                
                if(_moving)
                    _bicycleAudio.Play();
                else
                    _bicycleAudio.Stop();
            }
        }

        private bool _correctMove;
        private bool _evaluated;

        private float _speedMultiplier = 1;

        private RequestData _currentRequest;

        public RequestData CurrentRequest
        {
            get => _currentRequest;
            set
            {
                _currentRequest = value;
                _requestUI.gameObject.SetActive(_currentRequest != null);
                _requestUI.Set(_currentRequest);
            }
        }

        private bool _pressed;
        private bool _thrown;
        private bool _finished;

        private Vector3 _initialPos;
        private Vector3 _initialInputPos;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _initialPos = transform.position;
        }

        public void Initialize()
        {
            transform.position = _initialPos;
            CurrentRequest = null;
            _finished = false;
            _houses.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_finished)
                return;
            
            if (other.TryGetComponent<HouseBehaviour>(out var house)
                && !_houses.Contains(house))
            {
                _houses.Add(house);

                if (_houses.Count == 2 && CurrentRequest == null)
                    GetNewRequest();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_finished)
                return;
            
            if (!other.TryGetComponent<HouseBehaviour>(out var house)
                || !_houses.Contains(house)) return;

            _houses.Remove(house);
            if (_houses.Count == 0 && CurrentRequest != null)
            {
                if (!_evaluated)
                {
                    _evaluated = true;
                    _correctMove = CurrentRequest.Alignment is HouseAlignment.Neither;
                    CurrentRequest = null;
                    AnswerGiven?.Invoke(_correctMove);
                    GameManager.PlayAudioFx(_correctMove ? AudioFxType.Success : AudioFxType.Miss);
                    _feedbackUI.GiveFeedback(HouseAlignment.Neither, _correctMove);
                    //Debug.Log(_correctMove);
                }
            }
            _thrown = false;
        }

        private void EvaluateMove(HouseBehaviour house, HouseAlignment alignment)
        {
            _correctMove = false;
            if (CurrentRequest.Alignment is HouseAlignment.None)
                _correctMove = CurrentRequest.ColorData.Name == house.ColorData.Name;
            else if(CurrentRequest.Alignment is not HouseAlignment.Neither)
                _correctMove = CurrentRequest.Alignment == house.HouseAlignment;

            if (CurrentRequest.Reversed)
                _correctMove = !_correctMove;

            _evaluated = true;
            CurrentRequest = null;
            AnswerGiven?.Invoke(_correctMove);
            
            GameManager.PlayAudioFx(_correctMove ? AudioFxType.Success : AudioFxType.Fail, 0.5f);
            _feedbackUI.GiveFeedback(alignment, _correctMove, 0.5f);
            //Debug.Log(_evaluated);
        }

        private void Update()
        {
            if (!Moving)
                return;
            
            _speedMultiplier = Mathf.Clamp(1f + (GameManager.Instance.PlayerLevel - 1) * 0.1f, 1f, 2.5f);
            
            transform.Translate(_moveSpeed * _speedMultiplier * Time.deltaTime * Vector3.forward);
        }

        public void OnPress(BaseEventData eventData)
        {
            if (eventData is not PointerEventData pData)
                return;
            
            _initialInputPos = pData.position;
            _pressed = true;
        }
        
        public void OnDrag(BaseEventData eventData)
        {
            if (eventData is not PointerEventData pData
                || !_pressed)
                return;

            var difference = pData.position.x - _initialInputPos.x;
            if (Mathf.Abs(difference) > _throwTreshold)
            {
                _pressed = false;
                ThrowPaper(difference);
            }
        }

        public void OnFinish()
        {
            _finished = true;
            CurrentRequest = null;
            
            _houses.Clear();
        }

        public void ThrowPaper(float directionX)
        {
            if (CurrentRequest == null || _houses.Count != 2)
                return;
            
            var alignment = directionX < 0 ? HouseAlignment.Left : HouseAlignment.Right;


            for (int i = 0; i < _houses.Count; i++)
            {
                var house = _houses[i];
                if (!house.GotPaper && house.HouseAlignment == alignment)
                {
                    _thrown = true;
                    var paper = Instantiate(GameManager.PrefabData.PaperPrefab, _throwTransform).transform;
                    paper.localPosition = Vector3.zero;
                    paper.localRotation = Quaternion.identity;

                    house.ThrowPaper(paper);
                    EvaluateMove(house, alignment);
                    GameManager.PlayAudioFx(AudioFxType.Throw);
                    break;
                }
            }
        }
        
        private void GetNewRequest()
        {
            var level = GameManager.Instance.PlayerLevel;
            var request = new RequestData();

            if (level != 2)
            {
                request.ColorData = _houses[Random.Range(0, _houses.Count)].ColorData;
            }

            request.Alignment = level switch
            {
                < 2 => HouseAlignment.None,
                < 3 => (HouseAlignment)Random.Range(1, 3),
                < 5 => (HouseAlignment)Random.Range(0, 3),
                _ => (HouseAlignment)Random.Range(0, 4),
            };

            request.Reversed = request.Alignment == HouseAlignment.None
                               && level >= 3 && Random.Range(0f, 1f) < 0.4f;
            
            CurrentRequest = request;
            _evaluated = false;
        }
    }
}