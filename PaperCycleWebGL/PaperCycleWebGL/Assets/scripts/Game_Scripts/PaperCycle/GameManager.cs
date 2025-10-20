using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Witmina_PaperCycle
{
    public class GameManager : MonoBehaviour
    {
            public Bridge bridge;

        public static GameManager Instance = null;

        [SerializeField] private LevelBehaviour _level;
        [SerializeField] private UIController _uiController;
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private AudioController _audioController;
        [SerializeField] private PrefabData _prefabData;
        [SerializeField] private RequestAssetData _requestAssetData;

        public static PrefabData PrefabData => Instance != null ? Instance._prefabData : null;
        public static RequestAssetData RequestAssetData => Instance != null ? Instance._requestAssetData : null;

        private Coroutine _videoRoutine;

        private int _playerLevel;

        public int PlayerLevel;
        public static readonly int MaxLevel = 25;

        public string locale;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-numberOfCorrectMoves", "Number of correct moves" },
            { "1-numberOfFalseMoves", "Number of false moves" },
        };

        public static void PlayAudioFx(AudioFxType audioFxType, float delay = 0f)
        {
            if (!Instance || !Instance._audioController)
                return;

            Instance._audioController.PlayFx(audioFxType, delay);
        }

        private void Awake()
        {
            if (!Instance)
                Instance = this;

            _videoPlayer.Play();
            _videoPlayer.Pause();
            _uiController.ToggleSkipButton(false);
            _uiController.ToggleIntroPanel(true);

            // SetInGameTopbarTimerStatus(false);
            // initBaseOperations("Paper_Cycle");
             SetStartLevel(PlayerPrefs.GetInt("level", 1));
        }

        private void SetStartLevel(int lastLevel)
        {
            PlayerLevel = lastLevel;
        }

        // public void Start()
        // {
        //             // UnityMessageManager.Instance.SendMessageToFlutter("loading_completed");

        //     PlayIntro();
        // }

        public void SetLanguage(string languageCode)
        {
            switch (languageCode)
            {
                case "en":
                    locale = "en";
                    break;

                case "tr":
                    locale = "tr";
                    break;

                default:
                    locale = "en";
                    break;
            }
        }

        public void StartFromWebGL(int levelId)
        {
            PlayerLevel = levelId;
            // PlayIntro();
            EndIntro();
        }

        private void PlayIntro()
        {
            _videoRoutine = StartCoroutine(IntroRoutine());
        }

        public void EndIntro()
        {
            PlayerPrefs.SetInt("PaperCycle_IntroCompleted", 1);

            if (_videoRoutine != null)
                StopCoroutine(_videoRoutine);

            _videoPlayer.Stop();
            //_audioController.StopIntro();
            _uiController.ToggleSkipButton(false);
            _uiController.ToggleIntroPanel(false);

            LoadLevel();
        }

        public void LoadLevel()
        {
            _uiController.Initialize();
            _audioController.PlayTheme();
            _level.Load();
        }

           [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;
    
        }

    ResultClass resultObject = new ResultClass();

        public void Finish(int correctCount, int falseCount)
        {
            _uiController.ActivateEndGamePanel();

            //var total = (correctCount + falseCount);
            //var score = ((float)correctCount / total) * (50 * total);

            ///<summary>
            ///max level is 25, player can see 66 houses at most in the last level
            ///so maximum allowed score which is 1000, divided by 66 gives 15 point for 
            ///each correct answer
            /// </summary>
            float score = correctCount * 15f;
            score = Mathf.Clamp(score, 0f, 1000f);
            PlayerPrefs.SetInt("score", (int)score);
            PlayerPrefs.SetInt("level", PlayerLevel);

            
        resultObject.level = PlayerLevel;
        resultObject.score = (int)score;
        string json = JsonUtility.ToJson(resultObject);
        bridge.SendToJSJson(json);

            // gameScoreViewModel.score = Mathf.CeilToInt(Mathf.Max(score, 0));
            // gameScoreViewModel.level = PlayerLevel;
            RecordStats(correctCount, falseCount);

            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
                   transform.parent.gameObject.SetActive(false);
        // SceneManager.LoadSceneAsync("ScoreScene", LoadSceneMode.Additive);

            // StartCoroutine(GameOverRoutine());
        }

        public void RecordStats(int correctCount, int falseCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("numberOfCorrectMoves", correctCount);
            statData.Add("numberOfFalseMoves", falseCount);

            // WManagers.WRDB.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
        }

        private IEnumerator IntroRoutine()
        {
            _videoPlayer.Play();
            //yield return new WaitForSeconds(0.05f);
            //_audioController.PlayIntro();
            _uiController.ToggleIntroPanel(true);
            yield return new WaitForSeconds(1f);
            if (PlayerPrefs.GetInt("PaperCycle_IntroCompleted", 0) == 1)
                _uiController.ToggleSkipButton(true);

            yield return new WaitForSeconds((float)_videoPlayer.clip.length - 1f);
            EndIntro();
            yield return null;
        }

        private IEnumerator GameOverRoutine()
        {
                   transform.parent.gameObject.SetActive(false);
        SceneManager.LoadSceneAsync("ScoreScene", LoadSceneMode.Additive);
            yield return new WaitForSeconds(1f);
            // GameOver();
        }
    }
}