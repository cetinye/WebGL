using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace Witmina_AtomAlchemist
{
    public class LevelBehaviour : MonoBehaviour
    {
        public int levelId;
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
        public static LevelSO LevelSO;
        [SerializeField] private TMP_Text levelText;
        private List<float> scores = new List<float>();

        [SerializeField] private Atom _atom;
        [SerializeField] private UIController _uiController;
        [SerializeField] private AudioController _audioController;
        [SerializeField] private ProtonGun _protonGun;
        private float _gameTime;
        [SerializeField] private GameObject _blackScreen;

        private Element _currentElement;
        private Element _targetElement;
        public int progress;
        private int levelMaxScore;

        private Animator _animator;

        private float _timer;
        private float flashInterval = 0.5f;
        private bool isFlashable = true;
        private bool retargetFlag = true;

        private bool _active;
        private bool _finished;
        public bool isArrowUp;

        private int _hitsToTransmute;
        private int _currentHits;

        [SerializeField] private int _correctHits;
        [SerializeField] private int _failedHits;
        private int tmpCorrect;
        private int tmpWrong;
        private int CurrentHits
        {
            get => _currentHits;
            set
            {
                _currentHits = value;
            }
        }
        private int maxLevelWKeys;

        public float Timer
        {
            get => _timer;
            private set
            {
                _timer = value;
                _uiController.SetTimerText(_timer);
            }
        }

        public void Initialize()
        {
            maxLevelWKeys = levels.Count / 2;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            AssignLevel();

            _animator = GetComponent<Animator>();
            _blackScreen.SetActive(true);
            _gameTime = 60f;
            Timer = _gameTime;
            _active = false;
        }

        private void AssignLevel()
        {
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            LevelSO = levels[levelId - 1];

            levelText.text = "Level " + levelId.ToString();
        }

        private void Update()
        {
            if (!_active || _finished)
                return;

            if (Timer <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.Instance.PlayFx("Countdown", 0.7f, 1f);
                FlashRed();
            }

            if (Timer <= 0)
            {
                Finish();
                return;
            }

            if (Timer <= _gameTime / 2f && retargetFlag)
            {
                retargetFlag = false;
                CalculateLevelScore();
                _uiController.SetTargetElement(GetTargetElementName());
                SetLevelMaxScore();
                DecideLevel(false);
                _atom.SetOrbitalSpeeds();
            }

            Timer -= Time.deltaTime;

#if UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShootGun();
            }
#endif
        }

        public void ShootGun()
        {
            if (!_protonGun.Shoot())
                return;

            _audioController.PlayOneShot("LaserFire");
        }

        private void Subscribe()
        {
            _atom.NucleusHit += OnNucleusHit;
            _atom.Transmuted += OnTransmute;
            Electron.ElectronHit += OnElectronHit;
        }

        private void Unsubscribe()
        {
            Electron.ElectronHit -= OnElectronHit;
            _atom.Transmuted -= OnTransmute;
            _atom.NucleusHit -= OnNucleusHit;
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        public void LoadIntro()
        {
            StartCoroutine(IntroRoutine());
        }

        public void Load()
        {
            StartCoroutine(LoadRoutine());
        }

        private void Finish()
        {
            _finished = true;
            Unsubscribe();
            _uiController.ActivateEndgamePanel();
            GameManager.Instance.Finish(_correctHits, _failedHits, (int)_targetElement - (int)_currentElement);
        }

        private void OnTransmute()
        {
            if (_atom.Element > _currentElement)
            {
                if (isArrowUp)
                    _audioController.PlayOneShot("AtomChangeSuccess");
                else
                    _audioController.PlayOneShot("AtomChangeFail");

                SetTransmuteHits();
                _currentHits = 0;
            }
            else if (_atom.Element < _currentElement)
            {
                if (isArrowUp)
                    _audioController.PlayOneShot("AtomChangeFail");
                else
                    _audioController.PlayOneShot("AtomChangeSuccess");

                SetTransmuteHits();
                CurrentHits = _hitsToTransmute - 1;
            }

            _currentElement = _atom.Element;
            _uiController.SetCurrentElement(_atom.Element);
            progress++;

            _atom.SetOrbitalSpeeds();

            if (_atom.Element.Equals((Element)GetTargetElementIndex()))
            {
                _audioController.PlayOneShot("TargetReached");
                Debug.LogWarning("TARGET REACHED");

                DecideLevel(true);

                CalculateLevelScore();

                _uiController.SetTargetElement(GetTargetElementName());
                SetLevelMaxScore();
                retargetFlag = false;
            }
        }

        public void DecideLevel(bool isSuccess)
        {
            int upCounter = PlayerPrefs.GetInt("AtomAlchemist_UpCounter", 0);
            int downCounter = PlayerPrefs.GetInt("AtomAlchemist_DownCounter", 0);
            if (isSuccess)
            {
                if (++upCounter >= LevelSO.levelUpCriteria * 2)
                {
                    upCounter = 0;
                    downCounter = 0;

                    ++levelId;
                }
                Debug.LogWarning("Level UP" + levelId + " is " + upCounter + "/" + LevelSO.levelUpCriteria * 2);
            }
            else
            {
                if (++downCounter >= LevelSO.levelDownCriteria * 2)
                {
                    upCounter = 0;
                    downCounter = 0;

                    --levelId;
                }
                Debug.LogWarning("Level DOWN" + levelId + " is " + downCounter + "/" + LevelSO.levelDownCriteria * 2);
            }
            PlayerPrefs.SetInt("AtomAlchemist_UpCounter", upCounter);
            PlayerPrefs.SetInt("AtomAlchemist_DownCounter", downCounter);

            AssignLevel();
        }

        public void CalculateLevelScore()
        {
            int score = (tmpCorrect * 100) - (tmpWrong * LevelSO.penaltyPoints);
            score = Mathf.Clamp(score, 0, levelMaxScore);
            float levelScore = ((float)score / levelMaxScore) * 1000;
            Debug.Log($"Calculated level score: {levelScore} = ({_correctHits} * 100) - ({_failedHits} * {LevelSO.penaltyPoints})");
            scores.Add(levelScore);
        }

        public int CalculateScore()
        {
            float score = 0f;

            for (int i = 0; i < scores.Count; i++)
            {
                score += scores[i];
            }

            int total = Mathf.Clamp(Mathf.CeilToInt((float)score / scores.Count), 0, 1000);

            return total;
        }

        public void SetTimer(bool state)
        {
            _active = state;
        }

        private void SetTransmuteHits()
        {
            _hitsToTransmute = LevelSO.hitsToTransmute;
        }

        private void OnNucleusHit()
        {
            if (isArrowUp)
            {
                _audioController.PlayOneShot("LaserFireSuccessCore");
                _correctHits++;
                tmpCorrect++;
            }
            else
            {
                _audioController.PlayOneShot("LaserFireFail");
                _failedHits++;
                tmpWrong++;
            }

            if (++CurrentHits >= _hitsToTransmute)
            {
                _atom.AddElectron();
            }
        }

        private void OnElectronHit(Electron electron)
        {
            if (isArrowUp)
            {
                _audioController.PlayOneShot("LaserFireFail");
                _failedHits++;
                tmpWrong++;
            }
            else
            {
                _audioController.PlayOneShot("LaserFireSuccessElectron");
                _correctHits++;
                tmpCorrect++;
            }

            if (--CurrentHits >= 0)
            {
                //_audioController.Play(AudioFxType.ElectronHit);
                return;
            }

            _atom.RemoveElectron(electron);
        }

        private string GetTargetElementName()
        {
            var names = GameManager.Instance.SpriteData.ElementNames;
            int randIdx;

            if ((int)_currentElement >= 10)
            {
                isArrowUp = false;
                randIdx = (int)_currentElement - UnityEngine.Random.Range(LevelSO.minRandom, LevelSO.maxRandom);
                _uiController.RotateArrow(false);
            }
            else
            {
                isArrowUp = true;
                randIdx = (int)_currentElement + UnityEngine.Random.Range(LevelSO.minRandom, LevelSO.maxRandom);
                _uiController.RotateArrow(true);
            }

            randIdx = Mathf.Clamp(randIdx, 1, 20);
            _targetElement = (Element)randIdx;
            return names[randIdx];
        }

        public int GetTargetElementIndex()
        {
            return (int)_targetElement;
        }

        public int GetCurrentElementIndex()
        {
            return (int)_currentElement;
        }

        public void SetLevelMaxScore()
        {
            tmpCorrect = 0;
            tmpWrong = 0;

            levelMaxScore = Mathf.Abs(GetTargetElementIndex() - GetCurrentElementIndex()) * 100;

            Debug.Log($"Level Max Score: {levelMaxScore}");
            Debug.Log($"Target Element Index: {GetTargetElementIndex()}");
            Debug.Log($"Current Element Index: {GetCurrentElementIndex()}");
        }

        private void FlashRed()
        {
            TMP_Text timerText = _uiController.GetTimerText();

            Sequence redFlash = DOTween.Sequence();

            redFlash.Append(timerText.DOColor(Color.red, flashInterval))
                    .SetEase(Ease.Linear)
                    .Append(timerText.DOColor(Color.white, flashInterval))
                    .SetEase(Ease.Linear)
                    .SetLoops(6);

            redFlash.Play();
        }

        private IEnumerator IntroRoutine()
        {
            _active = false;
            _animator.enabled = true;
            _uiController.gameObject.SetActive(false);

            _animator.Play("Intro");
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.05f);
            _audioController.PlayOneShot("GunEnter");
            _audioController.Play("BackgroundAmbient");
            _audioController.Play("Background");
            yield return new WaitForSeconds(2f);
            AudioController.instance.PlayOneShot("OrbitWave");

            yield return new WaitForSeconds(2.2f);
            _animator.enabled = false;
            Load();
        }

        private IEnumerator LoadRoutine()
        {
            _currentElement = (Element)Random.Range(7, 12);
            var element = _currentElement;

            _currentHits = 0;
            _correctHits = 0;
            _failedHits = 0;
            _protonGun.Initialize();

            _currentElement = element;

            _uiController.gameObject.SetActive(true);
            _uiController.Initialize();
            _uiController.SetCurrentElement(_currentElement);
            _uiController.SetTargetElement(GetTargetElementName());
            SetLevelMaxScore();
            SetTransmuteHits();
            yield return new WaitForSeconds(0.5f);

            _atom.SetAtom(_currentElement);
            _atom.StartAnimation();
            _atom.SetOrbitalSpeeds();

            yield return new WaitForSeconds(0.4f);

            Timer = _gameTime;

            _finished = false;
            _active = true;

            Subscribe();
        }
    }
}