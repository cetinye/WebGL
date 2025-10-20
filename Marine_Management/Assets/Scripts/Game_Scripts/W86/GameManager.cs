using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;
using UnityEngine.Video;


namespace Witmina_MarineManagement
{
    public class GameManager : MonoBehaviour
    {


        public Bridge bridge;

        public static GameManager Instance = null;

        [SerializeField] private LeanLocalization leanLocalization;
        [SerializeField] private LevelBehaviour _level;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private UIController _uiController;
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private AudioController _audioController;
        [SerializeField] private PrefabData _prefabData;

        public static PrefabData PrefabData => Instance != null ? Instance._prefabData : null;
        public static Camera MainCamera => Instance != null ? Instance._mainCamera : null;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            {"0-numberOfBoatsSatisfied", "Number of boats satisfied"},
            {"1-numberOfBoatsUpset", "Number of boats upset"},
            {"2-levelTime", "Time to complete the level"},
        };

        private Coroutine _videoRoutine;

        private int _playerLevel;
        public int PlayerLevel;
        public static readonly int MaxLevel = 25;

        public static void PlayAudioFx(AudioFxType audioFxType)
        {
            if (!Instance || !Instance._audioController)
                return;

            Instance._audioController.PlayFx(audioFxType);
        }

        public static void PlayAudioFx(RequestType requestType)
        {
            if (!Instance || !Instance._audioController)
                return;

            Instance._audioController.PlayFx(requestType);
        }

        private void Awake()
        {
            if (!Instance)
                Instance = this;

            leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            _videoPlayer.Play();
            _videoPlayer.Pause();
            _uiController.ToggleIntroPanel(true);
            _uiController.ToggleSkipButton(false);
            _uiController.ScaleHUDPanel();

            // _ = InitAsync();
        }

        // private async Task InitAsync()
        // {
        //     try
        //     {
        //         await initBaseOperations("Marine_Management");

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
            PlayerLevel = lastLevel;
        }

        public void StartFromWebGL(int levelId)
        {
            SetStartLevel(levelId);
            _level.AssignLevel();
            _uiController.SetRequestList(_prefabData.GetRequestData());

            PlayIntro();
        }

        private void Update()
        {
            if (!_level || !_uiController || !LevelBehaviour.LevelSO)
                return;

            _uiController.MaxTier = LevelBehaviour.LevelSO.typesOfServicesMax;
        }

        private void PlayIntro()
        {
            _videoRoutine = StartCoroutine(IntroRoutine());
        }

        public void EndIntro()
        {
            PlayerPrefs.SetInt("MarineManagement_IntroCompleted", 1);

            if (_videoRoutine != null)
                StopCoroutine(_videoRoutine);

            _audioController.StopIntro();
            _uiController.ToggleSkipButton(false);
            _uiController.ToggleIntroPanel(false);

            LoadLevel();
        }

        public void LoadLevel()
        {
            _uiController.Initialize();
            _audioController.PlayAmbient();
            _level.Load();
        }

        public int GetTimer()
        {
            return _level.GetTimer();
        }

        public int GetActiveBoatCount()
        {
            return _level.GetBoatCount();
        }

        public void EndGame()
        {
            _level.Finish();
        }

        public void Finish(int boatSatisfied, int boatUpset)
        {
            _uiController.ActivateEndGamePanel();

            int score = 1000 - (48 * boatUpset);
            score = Mathf.Clamp(score, 0, 1000);


            int level = PlayerLevel;
            resultObject.level = level;
            resultObject.score = score;
            Debug.Log("skor   " + score);
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);



            Debug.LogWarning("Score: " + score + " boatSatisfied: " + boatSatisfied + " boatUpset: " + boatUpset);

            // gameScoreViewModel.score = Mathf.Clamp(Mathf.CeilToInt((float)score / LevelBehaviour.LevelSO.maxInLevel * 1000), 0, 1000);
            // gameScoreViewModel.level = PlayerLevel;
            RecordStats(boatSatisfied, boatUpset);

            StartCoroutine(GameOverRoutine());
        }

        [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();

        public void RecordStats(int boatSatisfied, int boatUpset)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("numberOfBoatsSatisfied", boatSatisfied);
            statData.Add("numberOfBoatsUpset", boatUpset);
            statData.Add("levelTime", _level.Timer);

            // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
        }

        private IEnumerator IntroRoutine()
        {
            // _videoPlayer.Play();
            // yield return new WaitForSeconds(0.05f);
            // _audioController.PlayIntro();
            _uiController.ToggleIntroPanel(true);
            // yield return new WaitForSeconds(1f);
            // PlayerPrefs.SetInt("MarineManagement_IntroCompleted", 1);
            // if (PlayerPrefs.GetInt("MarineManagement_IntroCompleted", 0) == 1)
            //     _uiController.ToggleSkipButton(true);

            // yield return new WaitForSeconds((float)_videoPlayer.clip.length - 1f);
            EndIntro();
            yield return null;
        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(1f);
            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
            // GameOver();
        }

        public void SetIsFlashing(bool state)
        {
            _level.isFlashable = state;
        }
    }
}
