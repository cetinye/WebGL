using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;

namespace Witmina_rotf
{
    public class GameManager : MonoBehaviour
    {
        public Bridge bridge;
        public static GameManager Instance = null;

        [SerializeField] private LeanLocalization leanLocalization;

        public TMPro.TMP_Text scoreText;
        public TMPro.TMP_Text levelIdText;

        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
        public static LevelSO LevelSO;

        [SerializeField] private UIController _uiController;
        [SerializeField] private LevelData _levelData;
        [SerializeField] private LevelBehaviour _level;
        [SerializeField] private int _maxTries = 5;
        public int TrialCount => _trialCount;
        public int MaxTries => _maxTries;

        public LevelData LevelData => _levelData;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            {"0-numberOfCorrectAnswers", "Number of correct answers"},
            {"1-numberOfPerfectAnswers", "Number of perfect answers"},
            {"2-numberOfWrongAnswers", "Number of wrong answers"},
        };

        private int _trialCount = 0;
        private int _perfectCount = 0;
        private int _correctCount = 0;
        private int _failCount = 0;

        private int upCounter;
        private int downCounter;
        private int perfect;
        private int correct;
        private int wrong;

        public int PlayerLevel;

        private int maxLevelWKeys;

        private void Awake()
        {
            if (Instance)
                Destroy(Instance);

            Instance = this;

            leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            maxLevelWKeys = levels.Count;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);
        }

        private void SetStartLevel(int lastLevel)
        {
            PlayerLevel = lastLevel;
        }

        public void SetLanguage(string languageCode)
        {
            switch (languageCode)
            {
                case "en":
                    LeanLocalization.Instances[0].SetCurrentLanguage("English");
                    break;

                case "tr":
                    LeanLocalization.Instances[0].SetCurrentLanguage("Turkish");
                    break;

                default:
                    LeanLocalization.Instances[0].SetCurrentLanguage("English");
                    break;
            }

            Debug.Log("Current Language set to: " + LeanLocalization.Instances[0].CurrentLanguage);
        }

        public void StartFromWebGL(int levelId)
        {
            SetStartLevel(levelId);

            ResetData();

            AudioController.Instance.PlayAmbient();

            StartCoroutine(LoadRoutine());
        }

        public void Load()
        {
            PlayerLevel = Mathf.Clamp(PlayerLevel, 1, maxLevelWKeys);
            LevelSO = levels[PlayerLevel - 1];

            levelIdText.text = $"{LeanLocalization.GetTranslationText("Level")} " + PlayerLevel;

            _level.LoadLevel(PlayerLevel);
        }

        private void ResetData()
        {
            _trialCount = 0;
            _perfectCount = 0;
            _correctCount = 0;
            _failCount = 0;
        }

        public void FinishLevel(bool success, bool isPerfect)
        {
            _trialCount++;
            _uiController.UpdateTrialCount();
            _level.OnFinish();

            if (success && _level.countdownTimer > 0)
            {
                upCounter++;

                if (isPerfect)
                {
                    _perfectCount++;
                    perfect++;
                }
                else
                {
                    _correctCount++;
                    correct++;
                }
            }
            else
            {
                downCounter++;
                wrong++;
                _failCount++;
            }

            PlayerPrefs.SetInt("RhythmOfForest_UpCounter", upCounter);
            PlayerPrefs.SetInt("RhythmOfForest_DownCounter", downCounter);

            DecideLevel();

            if (_trialCount >= _maxTries)
            {
                EndGame();
                return;
            }

            StartCoroutine(ReloadRoutine());
        }

        private void DecideLevel()
        {
            upCounter = PlayerPrefs.GetInt("RhythmOfForest_UpCounter", 0);
            downCounter = PlayerPrefs.GetInt("RhythmOfForest_DownCounter", 0);

            if (upCounter >= LevelSO.levelUpCriteria * 2)
            {
                LevelUp();
            }
            else if (downCounter >= LevelSO.levelDownCriteria)
            {
                LevelDown();
            }

            PlayerPrefs.SetInt("RhythmOfForest_UpCounter", upCounter);
            PlayerPrefs.SetInt("RhythmOfForest_DownCounter", downCounter);
        }

        private void LevelUp()
        {
            PlayerLevel++;
            PlayerLevel = Mathf.Clamp(PlayerLevel, 1, maxLevelWKeys);
            levelIdText.text = "Level " + PlayerLevel;
            Debug.Log($"Level changed to : {PlayerLevel}");

            downCounter = 0;
            upCounter = 0;
            wrong = 0;
            correct = 0;
            perfect = 0;
        }

        private void LevelDown()
        {
            PlayerLevel--;
            PlayerLevel = Mathf.Clamp(PlayerLevel, 1, maxLevelWKeys);
            levelIdText.text = "Level " + PlayerLevel;
            Debug.Log($"Level changed to : {PlayerLevel}");

            downCounter = 0;
            upCounter = 0;
            wrong = 0;
            correct = 0;
            perfect = 0;
        }

        private int CalculateLevelScore()
        {
            return Mathf.Clamp((_correctCount * 100) - (_failCount * 25) + (_perfectCount * 200), 0, 1000);
        }

        [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();

        private void EndGame()
        {
            _uiController.ShowEndGamePanel(true);

            int score = CalculateLevelScore();
            scoreText.text = "Score: " + score;

            resultObject.level = PlayerLevel;
            resultObject.score = score;
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);

            RecordStats();

            StartCoroutine(GameOverRoutine());
        }

        public void RecordStats()
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("numberOfCorrectAnswers", _correctCount + _perfectCount);
            statData.Add("numberOfWrongAnswers", _failCount);


            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
        }

        #region Coroutines

        private IEnumerator ReloadRoutine()
        {
            _uiController.Transition();
            yield return new WaitForSeconds(2.5f);
            Load();
        }

        private IEnumerator LoadRoutine()
        {
            yield return new WaitForEndOfFrame();
            _uiController.PlayStartAnimation();
            yield return new WaitForSeconds(1f);
            Load();
        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(1f);
        }

        #endregion
    }
}