using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Witmina_MarineManagement
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Boat : MonoBehaviour, IPointerDownHandler
    {
        public static event Action<Boat, Port> PortReached;
        public static event Action<Boat> BoatSelected;

        [SerializeField] private BoatType _type;
        [SerializeField] private int _tier = 1;
        [SerializeField] private float _moveSpeed = 1f;
        [SerializeField] private float _rotationSpeed = 0.5f;
        [SerializeField] private float _feedbackDuration = 2f;
        [SerializeField] private GameObject _selection;
        //[SerializeField] public GameObject Parasite;
        [SerializeField] private Image _parasiteFill;

        [HideInInspector] public bool Finished;
        [HideInInspector] public bool Correct;
        [HideInInspector] public bool Left;
        [HideInInspector] public int RequestCount = 1;

        private Transform _feedback;

        public BoatType Type => _type;
        public int Tier => _tier;
        public Port CurrentPort => _currentPort;

        private NavMeshAgent _agent;
        private Vector3 _target;
        private Port _currentPort;
        private float _maxParasiteTimer;
        private float _parasiteTimer;
        private bool _parasiteTimerRunning;

        private bool _destinationReached;
        private Tween _rotateTween;
        private Coroutine _feedbackRoutine;
        public float ParasiteProgressAmount
        {
            get => _parasiteFill.fillAmount;
            set
            {
                _parasiteFill.fillAmount = value;
                _parasiteFill.gameObject.SetActive(_parasiteFill.fillAmount is > 0f and < 1f);
            }
        }

        public bool Selected
        {
            get => _selection.gameObject.activeSelf;
            set => _selection.gameObject.SetActive(value);
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;

            _target = transform.position;

            _destinationReached = false;
            Finished = false;
            Left = false;
            Selected = false;
            //Parasite.SetActive(false);
            _parasiteTimerRunning = false;

            _parasiteTimer = 1f;
            _maxParasiteTimer = 1f;
            ParasiteProgressAmount = 0f;
        }

        private void OnDestroy()
        {
            _rotateTween.Kill();
            _agent.enabled = false;
            _agent.updatePosition = false;
            
            if (GameManager.Instance.GetTimer() <= 0)
                CheckEndGame();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            BoatSelected?.Invoke(this);
        }

        /*private void OnMouseDown()
        {
            BoatSelected?.Invoke(this);
        }*/

        public void OnFinish()
        {
            OnDestroy();
        }

        private void Update()
        {

            /*//Handling Parasite
            {
                if (Parasite.activeSelf && _parasiteTimerRunning)
                {
                    _parasiteFill.transform.localRotation = Quaternion.identity;
                    if (_parasiteTimer > 0)
                    {
                        _parasiteTimer -= Time.deltaTime;
                    }
                    else
                    {
                        _parasiteTimer = 0;
                        _parasiteTimerRunning = false;
                        ParasiteProgressAmount = 1f;
                        Parasite.SetActive(false);
                    }
                    
                    ParasiteProgressAmount = _parasiteTimer / _maxParasiteTimer;
                }
            }*/

            //Handling Feedback
            if (_feedback && _feedback.gameObject.activeSelf)
            {
                _feedback.rotation = Quaternion.identity;
            }

            //Handling of movement and rotation
            Quaternion _targetRotation = transform.rotation;
            var nearPort = Vector3.Distance(transform.position, _target) < 0.05f;

            if (nearPort)
            {
                _agent.updatePosition = false;
                if (_currentPort)
                {
                    _targetRotation = _currentPort.transform.rotation;
                }

                if (!_destinationReached && _currentPort)
                {
                    _destinationReached = true;
                    PortReached?.Invoke(this, _currentPort);
                }
            }
            else
            {
                _agent.updatePosition = true;
                _targetRotation = Quaternion.LookRotation(Vector3.forward, -_agent.velocity.normalized);
            }

            var angle = Quaternion.Angle(transform.rotation, _targetRotation);
            _agent.speed = nearPort ? 0f : (angle < 8f ? _moveSpeed : 0.001f);
            _rotateTween.Kill();
            if (angle > 5f)
                _rotateTween = transform.DORotateQuaternion(_targetRotation, angle / (360 * _rotationSpeed))
                    .SetEase(Ease.Linear);
            else
                transform.rotation = _targetRotation;
        }

        public void SetDestination(Port port)
        {
            _currentPort = port;
            port.CurrentBoat = this;
            _target = port.transform.position;
            _destinationReached = false;
            _agent.SetDestination(_target);
        }

        public void SetTarget(Vector3 position, bool increasePriority)
        {
            if (_currentPort)
            {
                _currentPort.CurrentBoat = null;
            }
            _currentPort = null;
            _target = position;
            _destinationReached = false;
            if (increasePriority)
                _agent.avoidancePriority += 50;
            _agent.SetDestination(position);
        }

        public void GiveFeedback(bool success)
        {
            if (_feedbackRoutine != null)
                StopCoroutine(_feedbackRoutine);

            if (_feedback)
                Destroy(_feedback.gameObject);

            _feedback = Instantiate(GameManager.PrefabData.GetFeedbackSprite(success), transform);
            _feedback.localPosition = Vector3.zero;
            _feedback.rotation = Quaternion.identity;
            _feedbackRoutine = StartCoroutine(FeedbackRoutine());
        }

        private void CheckEndGame()
        {
            Debug.LogWarning("Timer" + GameManager.Instance.GetTimer() + ", GetActiveBoatCount" + GameManager.Instance.GetActiveBoatCount());

            if (GameManager.Instance.GetTimer() <= 0 && GameManager.Instance.GetActiveBoatCount() <= 1)
                GameManager.Instance.EndGame();
        }

        private IEnumerator FeedbackRoutine()
        {
            var fb = _feedback.gameObject;
            fb.SetActive(true);
            yield return new WaitForSeconds(_feedbackDuration);
            fb.SetActive(false);
        }

        public void RemoveParasite(float duration)
        {
            if (_parasiteTimerRunning)
                return;

            _maxParasiteTimer = _parasiteTimer = duration;
            _parasiteTimerRunning = true;
        }
    }
}