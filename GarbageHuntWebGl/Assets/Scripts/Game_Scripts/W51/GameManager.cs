using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    /// <summary>
    /// Manager class for the game. Implemented as singleton behaviour.
    /// Handles loadn/unload and save operations.
    /// Stores all the metrics and communicates with UI and level.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance = null;

        public Bridge bridge;


        [SerializeField] private LeanLocalization leanLocalization;
        [SerializeField] private LevelBehaviour _levelObject;
        [SerializeField] private UIController _uiController;

        private float _maxTimer;
        [SerializeField] private float _comboBonusDuration = 5f;

        public float ComboBonusDuration => _comboBonusDuration;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-numberOfTotalGarbage", "Number of total garbage caught" },
            { "1-numberOfStaticGarbage", "Number of static garbage caught" },
            { "2-numberOfFishCaught", "Number of fish caught" },
            { "3-garbagePercentage", "Percentage of garbage caught" },
        };

        private bool _started;
        private bool _finished;

        #region Data Fields
        private float _timer;

        public float Timer
        {
            get => _timer;
            private set
            {
                _timer = Mathf.Clamp(value, 0f, _maxTimer);
                // SetTimerText(Mathf.Ceil(_timer).ToString("00"));
                // _uiController.SetTimerText(_timer);
            }
        }

        private float _blurAmount;

        public float BlurAmount
        {
            get => _blurAmount;
            private set
            {
                _blurAmount = value;
                _uiController.SetBlurAmount(_blurAmount);
            }
        }

        private int _garbageCount;

        public int GarbageCount
        {
            get => _garbageCount;
            set
            {
                _garbageCount = value;
                if (!_uiController)
                    return;

                _uiController.SetGarbageText(_garbageCount);
            }
        }

        private int _staticGarbageCount;

        public int StaticGarbageCount
        {
            get => _staticGarbageCount;
            set { _staticGarbageCount = value; }
        }

        private int _totalGarbageCount;

        public int TotalGarbageCount
        {
            get => _totalGarbageCount;
            set { _totalGarbageCount = value; }
        }

        private int _fishCount;

        public int FishCount
        {
            get => _fishCount;
            set { _fishCount = value; }
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (!Instance)
                Instance = this;

            // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            _maxTimer = 60f;

            _levelObject.AssignLevel();

            // _ = InitAsync();
        }

        // private async Task InitAsync()
        // {
        //     try
        //     {
        //         await initBaseOperations("Garbage_Hunt");

        //         SetStartLevel(gameScoreViewModel.level);
        //         SetInGameTopbarTimerStatus(false);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //         throw;
        //     }
        // }

        private void SetStartLevel(int lastLevel)
        {
            // _levelObject.levelId = lastLevel;
            _levelObject.levelId = 1;
        }

        public void StartFromWebGL(int levelId)
        {

            SetStartLevel(levelId);
            LoadLevel();

        }



        // public override void CustomStart()
        // {
        //     LoadLevel();
        // }

        private void Update()
        {
            if (!_started)
                return;

            if (Timer <= 0f && !_finished)
            {
                FinishGame(true);
                return;
            }

            Timer -= Time.deltaTime;
            _uiController.SetTimerText(_timer, _maxTimer);
        }

        #endregion

        #region Public Methods

        public void LoadLevel()
        {
            ResetData();
            _uiController.Initialize();
            _levelObject.Load();
            _started = true;
            _finished = false;

            StartLevel();
        }

        public void StartLevel()
        {
            if (!_levelObject)
            {
                throw new Exception("Level is not loaded yet, but tried to start.\n Call LoadLevel before StartLevel");
            }

            _uiController.DisableStartPanel();
            _levelObject.StartLevel();
        }

        public void IncrementStaticGarbageCount()
        {
            StaticGarbageCount++;
            GarbageCount++;
        }

        public void IncrementTotalGarbageCount()
        {
            TotalGarbageCount++;
            GarbageCount++;
        }

        public void DecreaseGarbageCount()
        {
            GarbageCount--;
        }

        public void AddedExtraTime(int time)
        {
            _uiController.ExtraTimeAnim(time);
        }

        public void AddTime(float time)
        {
            Timer += time;
            AddedExtraTime((int)time);
        }

        public void SetTotalGarbageCount(int count)
        {
            TotalGarbageCount = count;
        }

        public void SetBlurAmount(float amount)
        {
            BlurAmount = amount;
        }

        [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();


        public void FinishGame(bool success)
        {
            if (_finished)
                return;

            _finished = true;
            _levelObject.FinishLevel();
            _uiController.ActivateEndgamePanel();
            OnGameOver();
        }

        private void OnGameOver()
        {
            // gameScoreViewModel.score = Mathf.Clamp(_levelObject.CalculateScore(), 0, 1000);
            // gameScoreViewModel.level = _levelObject.levelId;

            RecordStats();

            StartCoroutine(GameOverRoutine());
        }

        public void RecordStats()
        {

            var rate = Mathf.Clamp(_levelObject.CalculateScore(), 0, 1000);
            resultObject.score = rate;
            resultObject.level = _levelObject.levelId;

            PlayerPrefs.SetInt("score", rate);
            PlayerPrefs.SetInt("level", _levelObject.levelId);


            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("numberOfTotalGarbage", TotalGarbageCount);
            statData.Add("numberOfStaticGarbage", StaticGarbageCount);
            statData.Add("numberOfFishCaught", FishCount);
            statData.Add("garbagePercentage", (float)GarbageCount / TotalGarbageCount);

            // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());

              Debug.Log("score   " + PlayerPrefs.GetInt("score"));
            Debug.Log("level   " + PlayerPrefs.GetInt("level"));

            resultObject.level = _levelObject.levelId;
            resultObject.score = rate;
            Debug.Log("skor   " + rate);
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);
        }

        #endregion

        #region Helpers

        private void ResetData()
        {
            _levelObject.levelId = 1;
            GarbageCount = 0;
            TotalGarbageCount = 0;
            StaticGarbageCount = 0;
            FishCount = 0;
            BlurAmount = 0;

            Timer = _maxTimer;
        }

        #endregion

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(0.5f);
            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
            // GameOver();
        }
    }
}