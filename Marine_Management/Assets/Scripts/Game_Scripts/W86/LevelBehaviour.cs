using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Witmina_MarineManagement
{
    public class LevelBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private List<LevelSO> levels;
        public static LevelSO LevelSO;
        private int maxLevelWKeys;

        private int upCounter;
        private int downCounter;
        private int correct;
        private int wrong;

        [SerializeField] private List<Port> _ports;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _leavePoint;
        [SerializeField] private Transform _boatsParent;
        [SerializeField] private HUDPanel _hudPanel;
        [SerializeField] private int _scoreGain = 100;
        //[SerializeField] private int _parasiteMinLevel = 4;
        private float _levelTimer;
        [SerializeField] private float _requestTimer = 10f;
        [SerializeField] private float _serviceTimer = 1.5f;
        //[SerializeField] private float _parasiteTimer = 1f;
        [SerializeField] private float _powerUpTimer = 5f;
        [SerializeField] private float _requestTimerVip = 6f;
        [SerializeField] private float _requestDelay = 1f;
        [SerializeField] private float _spawnInterval = 1.5f;
        [SerializeField] private float _boatLeaveTimer = 3f;
        //[SerializeField] [Range(0f,1f)] private float _boatParasiteChance = 0.2f;
        [SerializeField][Range(0f, 1f)] private float _boatDoubleRequestChance = 0.4f;
        [SerializeField] private int _boatEventMinRequests = 3;
        [SerializeField] private int _boatEventMaxRequests = 5;

        private Boat _lastBoat;
        private Port _selectedPort;
        private List<Boat> _boats = new();
        private List<Request> _requests = new();
        private Dictionary<PowerUpType, Coroutine> _powerUpRoutines = new();

        private int _correctCount;
        private int _boatsSatisfied;
        private int _boatsUpset;

        private int _maxBoats;
        private bool _spawning;
        private bool _finished;
        private bool _serviceBoatNext;
        public bool isFlashable = true;

        private float _workSpeedMultiplier;
        private float _scoreMultiplier;

        private static readonly int MaxScore = 100000;
        public int MaxTier { get; private set; }

        private float _timer;
        public float Timer
        {
            get => _timer;
            private set
            {
                _timer = value;
                if (!_hudPanel)
                    return;

                _hudPanel.SetTimerText(_timer);
            }
        }

        private int _score;
        public int Score
        {
            get => _score;
            private set
            {
                _score = Math.Clamp(value, 0, MaxScore);
                if (!_hudPanel)
                    return;

                _hudPanel.SetScoreText(_score);
            }
        }

        private void Awake()
        {
            _levelTimer = 60f;
            // _levelTimer = 90;
            //var aspect = 2f * Screen.width / Screen.height;
            //transform.localScale = new Vector3(aspect, 1f, 1f);
            _finished = true;
        }

        public void AssignLevel()
        {
            maxLevelWKeys = levels.Count;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            GameManager.Instance.PlayerLevel = Mathf.Clamp(GameManager.Instance.PlayerLevel, 1, maxLevelWKeys);
            LevelSO = levels[GameManager.Instance.PlayerLevel - 1];
        }

        public void OnDestroy()
        {
            Unsubscribe();
            StopAllCoroutines();
        }

        public void Update()
        {
            levelText.text = $"Level {GameManager.Instance.PlayerLevel}";

            if (_finished)
                return;

            var reqTier = _requests.Count == 0 ? 1
                : _requests.Max(r => r.Tier);
            MaxTier = Math.Max(GameManager.Instance.PlayerLevel, reqTier);


            if (Timer <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.Instance.PlayFx("Countdown", 0.7f, 1f);
                _hudPanel.FlashRed();
            }

            if (Timer > 0)
            {
                Timer -= Time.deltaTime;
            }
            else
            {
                Timer = 0;
            }

            foreach (var p in _ports)
            {
                if (!p || !p.CurrentBoat)
                    continue;

                switch (p.State)
                {
                    case PortState.Idle:
                        break;
                    case PortState.WaitingRequest:
                        if (p.CurrentRequest != null)
                        {
                            if (p.CurrentRequest.Timer > 0)
                                p.CurrentRequest.Timer -= Time.deltaTime;
                            else
                            {
                                p.CurrentRequest = null;
                                p.CurrentBoat.Correct = false;
                                p.State = PortState.Idle;
                                LeaveBoat(p);
                            }
                        }
                        break;
                    case PortState.Working:
                        if (p.CurrentRequest != null)
                        {
                            if (p.WorkTimer > 0)
                                p.WorkTimer -= Time.deltaTime * _workSpeedMultiplier;
                            else
                            {
                                p.WorkTimer = 0;
                                p.CurrentBoat.GiveFeedback(p.CurrentBoat.Correct);
                                p.CurrentRequest = null;
                                if (--p.CurrentBoat.RequestCount <= 0)
                                {
                                    p.CurrentBoat.Finished = true;
                                    p.LeaveTimer = _boatLeaveTimer;
                                    p.ToggleCrane(false);
                                    p.State = PortState.WaitingLeave;
                                    p.ToggleWaitingBubble(true);
                                }
                                else
                                {
                                    Score += Mathf.FloorToInt(_scoreGain * _scoreMultiplier);
                                    p.State = PortState.Idle;
                                    StartCoroutine(MakeRequestRoutine(p.CurrentBoat, p));
                                }
                            }
                        }
                        break;
                    case PortState.WaitingLeave:
                        if (p.CurrentBoat.Finished && !p.CurrentBoat.Left)
                        {
                            if (p.LeaveTimer > 0)
                                p.LeaveTimer -= Time.deltaTime;
                            else
                            {
                                p.LeaveTimer = 0;
                                p.CurrentBoat.Correct = false;
                                p.State = PortState.Idle;
                                LeaveBoat(p);
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            for (var i = 0; i < _boats.Count; i++)
            {
                var boat = _boats[i];
                if (boat && boat.Left && boat.transform.position.y < -6f)
                {
                    _boats.Remove(boat);
                    Destroy(boat.gameObject);
                }
            }
        }

        public void Load()
        {
            _hudPanel.Initialize();

            foreach (var p in _ports)
            {
                p.Initialize();
                p.MaxRequestTimer = _requestTimer;
            }

            for (int i = 0; i < _boats.Count; i++)
            {
                var boat = _boats[i];
                Destroy(boat.gameObject);
            }
            _boats.Clear();
            _requests.Clear();

            Score = 0;
            _correctCount = 0;
            _boatsSatisfied = 0;
            _boatsUpset = 0;
            _workSpeedMultiplier = 1f;
            _scoreMultiplier = 1f;
            Timer = _levelTimer;
            _maxBoats = _ports.Count;
            Subscribe();
            StartCoroutine(SpawnRoutine());
            _spawning = true;
            _finished = false;
        }

        public void Finish()
        {
            if (_finished) return;

            _finished = true;
            Unsubscribe();
            StopAllCoroutines();
            foreach (var b in _boats)
            {
                b.OnFinish();
            }
            foreach (var p in _ports)
            {
                p.CurrentRequest = null;
            }
            GameManager.Instance.Finish(_boatsSatisfied, _boatsUpset);
        }

        public int GetTimer()
        {
            return Mathf.CeilToInt(Timer);
        }

        public int GetBoatCount()
        {
            return _boats.Count;
        }

        private void Subscribe()
        {
            Boat.PortReached += OnPortReached;
            Boat.BoatSelected += OnBoatSelected;
            Port.BubbleSelected += OnBubbleSelected;
            UIServiceElement.ServiceSelected += OnServiceSelected;

            //InputController.Instance.Pressed += OnPressed;
        }

        private void Unsubscribe()
        {
            Boat.PortReached -= OnPortReached;
            Boat.BoatSelected -= OnBoatSelected;
            Port.BubbleSelected -= OnBubbleSelected;
            UIServiceElement.ServiceSelected -= OnServiceSelected;

            //InputController.Instance.Pressed -= OnPressed;
        }

        /*private void OnPressed(PointerEventData pData)
        {
            var origin = GameManager.MainCamera.ScreenToWorldPoint(pData.position);
            var direction = GameManager.MainCamera.transform.forward;
            var hit = Physics2D.Raycast(origin, direction,
                Single.MaxValue, Physics.AllLayers);

            if (!hit)
            {
                return;
            }
            
            if(hit.collider.TryGetComponent<IPointerDownHandler>(out var pDown))
                pDown.OnPointerDown(pData);
        }*/

        private void OnServiceSelected(RequestType requestType)
        {
            if (!_selectedPort || _selectedPort.CurrentRequest == null
                || _selectedPort.State != PortState.WaitingRequest)
                return;

            /*if (_selectedPort.CurrentBoat && _selectedPort.CurrentBoat.Parasite.activeSelf)
            {
                if(requestType is RequestType.Cleaning)
                    _selectedPort.CurrentBoat.RemoveParasite(_parasiteTimer);
                return;
            }*/

            var success = _selectedPort.CurrentRequest.Type == requestType;

            _selectedPort.LeaveTimer = _boatLeaveTimer;
            _selectedPort.WorkTimer = _serviceTimer;
            _selectedPort.WorkTimerMax = _serviceTimer;
            _selectedPort.ToggleBubbleSelection(false);

            var boat = _selectedPort.CurrentBoat.Correct = success;
            if (success)
            {
                correct++;
                upCounter++;
                PlayerPrefs.SetInt("MarineManagement_UpCounter", upCounter);
                _selectedPort.State = PortState.Working;
            }
            else
            {
                LeaveBoat(_selectedPort);
            }

            _selectedPort = null;
            GameManager.PlayAudioFx(requestType);

            Taptic.Success();

            DecideLevel();
        }

        private void DecideLevel()
        {
            upCounter = PlayerPrefs.GetInt("MarineManagement_UpCounter", 0);
            downCounter = PlayerPrefs.GetInt("MarineManagement_DownCounter", 0);

            if (upCounter >= LevelSO.levelUpCriteria * 2)
            {
                LevelUp();
            }
            else if (downCounter >= LevelSO.levelDownCriteria * 2)
            {
                LevelDown();
            }

            PlayerPrefs.SetInt("MarineManagement_UpCounter", upCounter);
            PlayerPrefs.SetInt("MarineManagement_DownCounter", downCounter);
        }

        private void LevelUp()
        {
            GameManager.Instance.PlayerLevel++;
            GameManager.Instance.PlayerLevel = Mathf.Clamp(GameManager.Instance.PlayerLevel, 1, maxLevelWKeys);
            LevelSO = levels[GameManager.Instance.PlayerLevel - 1];
            levelText.text = "Level: " + GameManager.Instance.PlayerLevel;

            upCounter = 0;
            downCounter = 0;
            wrong = 0;
            correct = 0;
        }

        private void LevelDown()
        {
            GameManager.Instance.PlayerLevel--;
            GameManager.Instance.PlayerLevel = Mathf.Clamp(GameManager.Instance.PlayerLevel, 1, maxLevelWKeys);
            LevelSO = levels[GameManager.Instance.PlayerLevel - 1];
            levelText.text = "Level: " + GameManager.Instance.PlayerLevel;

            upCounter = 0;
            downCounter = 0;
            wrong = 0;
            correct = 0;
        }

        public int CalculateLevelScore()
        {
            int levelScore = Mathf.CeilToInt((correct * 100) - (wrong * LevelSO.penaltyPoints));
            levelScore = Mathf.Clamp(levelScore, 0, LevelSO.maxInLevel);

            scoreText.text = $"Score: {levelScore}";

            return levelScore;
        }

        private void OnBubbleSelected(Port port)
        {
            if (port.State == PortState.WaitingLeave)
            {
                Taptic.Success();
                LeaveBoat(port);
                return;
            }

            if (_selectedPort == port || port.State != PortState.WaitingRequest)
                return;

            if (_selectedPort)
                _selectedPort.ToggleBubbleSelection(false);

            _selectedPort = port;
            _selectedPort.ToggleBubbleSelection(true);

            Taptic.Success();
        }

        private void OnBoatSelected(Boat boat)
        {
            var port = _ports.FirstOrDefault(p => p && p.CurrentBoat == boat);
            if (!port)
                return;

            if (!boat.Finished)
            {
                if (port.CurrentRequest != null)
                    OnBubbleSelected(port);
            }
            else
                LeaveBoat(port);
        }

        private void OnPortReached(Boat boat, Port port)
        {
            if (!port || !_boats.Contains(boat) || !_ports.Contains(port))
                return;

            GameManager.PlayAudioFx(AudioFxType.ShipNear);
            StartCoroutine(MakeRequestRoutine(boat, port));
        }

        private void SpawnBoat()
        {
            var emptyPort = _ports.FirstOrDefault(p => !p.Occupied);

            if (!emptyPort || _boats.Count >= _maxBoats)
                return;

            GameManager.Instance.PlayerLevel = Mathf.Clamp(GameManager.Instance.PlayerLevel, 1, maxLevelWKeys);
            var level = GameManager.Instance.PlayerLevel;
            Boat boatPrefab;
            if (_serviceBoatNext)
            {
                boatPrefab = GameManager.PrefabData.GetRandomBoat(BoatType.Event);
            }
            else
            {
                boatPrefab = GameManager.PrefabData.GetRandomBoat(GameManager.Instance.PlayerLevel, _lastBoat);
            }
            _lastBoat = boatPrefab;

            var boat = Instantiate(boatPrefab, _spawnPoint.position, Quaternion.identity, _boatsParent);
            if (boat.Type is BoatType.Event)
                boat.RequestCount = Random.Range(_boatEventMinRequests, _boatEventMaxRequests);
            else
                boat.RequestCount = (LevelSO.boatDensity == 2 &&
                                     Random.Range(0f, 1f) < _boatDoubleRequestChance) ? 2 : 1;

            /*var parasiteRoll = GameManager.Instance.PlayerLevel >= _parasiteMinLevel
                                    && Random.Range(0f, 1f) < _boatParasiteChance;
            if(parasiteRoll && boat.Parasite)
                boat.Parasite.SetActive(true);*/

            _boats.Add(boat);

            boat.SetDestination(emptyPort);
            _serviceBoatNext = false;
        }

        private void LeaveBoat(Port port)
        {
            Taptic.Success();

            if (!port || !port.CurrentBoat)
                return;

            var boat = port.CurrentBoat;

            if (boat.Correct)
            {
                var scoreGain = Mathf.FloorToInt(_scoreGain * _scoreMultiplier);
                Score += scoreGain;
                _hudPanel.AddPowerUpAmount(scoreGain / (12f * _scoreGain));
                if (++_correctCount >= 2)
                {
                    _correctCount = 0;
                }
                GameManager.PlayAudioFx(AudioFxType.CorrectPlay);
                // port.ShowScore(scoreGain);

                _boatsSatisfied++;
            }
            else
            {
                Score -= _scoreGain;
                wrong++;
                downCounter++;
                PlayerPrefs.SetInt("MarineManagement_DownCounter", downCounter);
                GameManager.PlayAudioFx(AudioFxType.FailPlay);
                // port.ShowScore(-_scoreGain);
                _boatsUpset++;
            }

            port.CurrentBoat = null;
            boat.Left = true;
            boat.SetTarget(_leavePoint.position, true);
            boat.GiveFeedback(boat.Correct);

            port.CurrentRequest = null;
            port.ToggleCrane(false);
            port.ToggleWaitingBubble(false);

            if (_selectedPort == port)
            {
                _selectedPort.ToggleBubbleSelection(false);
                _selectedPort = null;
            }
        }

        private void MakeRequest(Boat boat, Port port)
        {
            if (port.CurrentRequest != null)
                return;

            var requestData = GameManager.PrefabData.GetRandomRequestData(LevelSO.typesOfServicesMax);
            var timer = boat.Type is BoatType.Vip ? _requestTimerVip : _requestTimer;
            // var request = new Request(boat, port, requestData.RequestType, requestData.MinLevel, timer);
            var request = new Request(boat, port, requestData.RequestType, timer);
            _requests.Add(request);

            port.MaxRequestTimer = timer;
            port.CurrentRequest = request;
            port.SetRequestSprite(requestData.RequestSprite);
            port.WorkTimerMax = port.WorkTimer = _serviceTimer;
            port.State = PortState.WaitingRequest;
        }

        public void ApplyPowerUp(int typeId)
        {
            var type = (PowerUpType)typeId;
            if (typeId >= Enum.GetValues(typeof(PowerUpType)).Length
                || _powerUpRoutines.ContainsKey(type))
                return;

            _powerUpRoutines.Add(type, StartCoroutine(PowerUpRoutine(type)));
            _hudPanel.OnPowerUpApplied(type);

            Taptic.Success();
        }

        private IEnumerator MakeRequestRoutine(Boat boat, Port port, bool toggleCrane = true)
        {
            if (toggleCrane)
                port.ToggleCrane(true);
            yield return new WaitForSeconds(_requestDelay);
            MakeRequest(boat, port);
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                if (_spawning && !_finished)
                {
                    var level = GameManager.Instance.PlayerLevel;

                    // var maxBoats = level switch
                    // {
                    //     < 3 => 3,
                    //     < 10 => 4,
                    //     < 19 => 5,
                    //     _ => 6,
                    // };
                    var maxBoats = LevelSO.totalNumOfMarinas;
                    if (_boats.Count(b => !b.Left) < maxBoats && Timer > 0)
                    {
                        yield return new WaitForSeconds(_spawnInterval);
                        SpawnBoat();
                    }
                }

                yield return null;
            }
            yield return null;
        }

        private IEnumerator PowerUpRoutine(PowerUpType powerUpType)
        {
            _hudPanel.TogglePowerUpIcon(powerUpType, true);
            switch (powerUpType)
            {
                case PowerUpType.Score:
                    _scoreMultiplier = 2f;
                    yield return new WaitForSeconds(_powerUpTimer);
                    _scoreMultiplier = 1f;
                    break;
                case PowerUpType.Speed:
                    _workSpeedMultiplier = 2f;
                    yield return new WaitForSeconds(_powerUpTimer);
                    _workSpeedMultiplier = 1f;
                    break;
                case PowerUpType.Time:
                    Timer += 10f;
                    yield return new WaitForEndOfFrame();
                    break;
                case PowerUpType.Boat:
                    _serviceBoatNext = true;
                    yield return new WaitForEndOfFrame();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(powerUpType), powerUpType, null);
            }

            _hudPanel.TogglePowerUpIcon(powerUpType, false);
            _powerUpRoutines.Remove(powerUpType);
            yield return null;
        }
    }
}