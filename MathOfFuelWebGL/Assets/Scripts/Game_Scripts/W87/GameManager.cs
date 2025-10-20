using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Witmina_MathOfFuel
{
    public class GameManager : MonoBehaviour
    {

        public Bridge bridge;

        public static GameManager Instance = null;

        // [SerializeField] private RawImage videoImage;

        [SerializeField] private Camera _mainCamera;
        [SerializeField] private LevelBehaviour _level;
        [SerializeField] private UIController _uiController;
        // [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private AudioController _audioController;
        [SerializeField] private W87_PrefabData _prefabData;

        private bool _isGameOverRoutineRunning = false;

        public static W87_PrefabData PrefabData => Instance != null ? Instance._prefabData : null;

        public static Camera MainCamera => Instance != null ? Instance._mainCamera : null;

        private GameState _state = GameState.idle;
        public GameState State
        {
            get => _state;
            set => _state = value;
        }

        public static void PlayAudioFx(AudioFxType audioFxType, float delay = 0f)
        {
            if (!Instance || !Instance._audioController)
                return;

            Instance._audioController.PlayFx(audioFxType, delay);
        }

        private Coroutine _videoRoutine;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            {"0-numberOfCorrectAnswers", "Number of correct answers"},
            {"1-numberOfWrongAnswers", "Number of wrong answers"},
        };

        private void Awake()
        {
            if (!Instance)
                Instance = this;

            // CreateRenderTexture();
            // _videoPlayer.Play();
            // _videoPlayer.Pause();
            _uiController.ToggleIntroPanel(false);
            _uiController.ToggleSkipButton(false);

            _state = GameState.idle;

            // initBaseOperations("Math_Of_Fuel");
            SetStartLevel(PlayerPrefs.GetInt("level", 1));
        }
        private void SetStartLevel(int lastLevel)
        {
            _level.levelId = lastLevel;
            _level.levelId = Mathf.Clamp(_level.levelId, 0, 25);
        }
        /*private void Start()
        {
            PlayIntro();
        }*/

        // public void Start()
        // {
        //     // SetInGameTopbarTimerStatus(false);

        //     // PlayIntro();
        //     SetStartLevel(1);
        //     LoadLevel();
        // }

        public void StartFromWebGL(int levelId)
        {
            SetStartLevel(levelId);
            LoadLevel();
            // PlayIntro();
        }

        private void PlayIntro()
        {
            _videoRoutine = StartCoroutine(IntroRoutine());
        }

        public void EndIntro()
        {
            PlayerPrefs.SetInt("MathOfFuel_IntroCompleted", 1);

            if (_videoRoutine != null)
                StopCoroutine(_videoRoutine);

            // _videoPlayer.Stop();
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

        [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();

        public void Finish(int correctCount, int failCount)
        {
            _uiController.ActivateEndGamePanel();
            var score = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max((2f * correctCount - failCount) * 50 / 3, 0)), 0, 1000);
            // PlayerPrefs.SetInt("score", score);
            // PlayerPrefs.SetInt("level", PlayerLevel);

            resultObject.level = _level.levelId;
            resultObject.score = score;
            Debug.Log("skor   " + score);
            string json = JsonUtility.ToJson(resultObject);
            Debug.LogWarning(json);
            bridge.SendToJSJson(json);

            // gameScoreViewModel.score = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max((2f * correctCount - failCount) * 50 / 3, 0)), 0, 1000);
            // gameScoreViewModel.level = PlayerLevel;
            RecordStats(correctCount, failCount);

            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);

            if (!_isGameOverRoutineRunning)
                StartCoroutine(GameOverRoutine());
        }

        public void RecordStats(int correctCount, int failCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("numberOfCorrectAnswers", correctCount);
            statData.Add("numberOfWrongAnswers", failCount);

            // WManagers.WRDB.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
        }

        private IEnumerator IntroRoutine()
        {
            // _videoPlayer.Play();
            // _videoPlayer.Pause();
            // _videoPlayer.Play();
            yield return new WaitForSeconds(0.05f);
            _audioController.PlayIntro();
            _uiController.ToggleIntroPanel(true);
            yield return new WaitForSeconds(1f);
            if (PlayerPrefs.GetInt("MathOfFuel_IntroCompleted", 0) == 1)
                _uiController.ToggleSkipButton(true);

            // yield return new WaitForSeconds((float)_videoPlayer.clip.length - 1f);
            EndIntro();
            yield return null;
        }

        private IEnumerator GameOverRoutine()
        {
            _isGameOverRoutineRunning = true;
            yield return new WaitForSeconds(1f);
            // GameOver();

            //      transform.parent.gameObject.SetActive(false);
            // SceneManager.LoadSceneAsync("ScoreScene", LoadSceneMode.Additive);
        }

        public enum GameState
        {
            idle,
            playing
        }

        // private void CreateRenderTexture()
        // {
        //     // Create a new RenderTexture with a supported format
        //     RenderTexture newRenderTexture = new RenderTexture(1080, 2160, 24, GetSupportedRenderTextureFormat());
        //     newRenderTexture.Create();

        //     // Assign the RenderTexture to the VideoPlayer
        //     AssignRenderTexture(newRenderTexture);
        // }

        // private RenderTextureFormat GetSupportedRenderTextureFormat()
        // {
        //     // List of formats to check
        //     RenderTextureFormat[] formats = {
        //     RenderTextureFormat.ARGB32,
        //     RenderTextureFormat.Default,
        //     RenderTextureFormat.ARGBHalf,
        //     RenderTextureFormat.ARGBFloat,
        //     RenderTextureFormat.RGB565,
        //     RenderTextureFormat.RGFloat,
        //     RenderTextureFormat.RGHalf,
        //     RenderTextureFormat.RGInt,
        //     RenderTextureFormat.R8,
        //     RenderTextureFormat.ARGB4444,
        //     RenderTextureFormat.ARGB1555,
        //     RenderTextureFormat.ARGB2101010,
        //     RenderTextureFormat.R16,
        //     RenderTextureFormat.RG16,
        //     RenderTextureFormat.RGB111110Float,
        //     RenderTextureFormat.R8,
        //     RenderTextureFormat.RG32,
        //     RenderTextureFormat.ARGB64,
        //     RenderTextureFormat.Depth,
        //     RenderTextureFormat.Shadowmap,
        //     RenderTextureFormat.RGB111110Float,
        //     RenderTextureFormat.RGHalf,
        //     RenderTextureFormat.R8,
        //     RenderTextureFormat.R16,
        //     RenderTextureFormat.RHalf,
        //     RenderTextureFormat.RG32,
        //     RenderTextureFormat.R8,
        //     RenderTextureFormat.ARGB2101010,
        //     RenderTextureFormat.ARGB64,
        //     RenderTextureFormat.ARGBFloat,
        //     RenderTextureFormat.ARGBHalf,
        //     RenderTextureFormat.ARGB4444,
        //     RenderTextureFormat.ARGB1555,
        //     RenderTextureFormat.DefaultHDR,
        //     RenderTextureFormat.RGB111110Float,
        //     RenderTextureFormat.RGB565,
        //     RenderTextureFormat.R8,
        //     RenderTextureFormat.ARGB64,
        //     RenderTextureFormat.ARGB32,
        //     RenderTextureFormat.ARGBFloat,
        //     RenderTextureFormat.RG32,
        //     RenderTextureFormat.RGInt,
        //     RenderTextureFormat.RGHalf,
        //     RenderTextureFormat.ARGB1555,
        //     RenderTextureFormat.RHalf,
        //     RenderTextureFormat.ARGB4444,
        //     RenderTextureFormat.R8,
        //     RenderTextureFormat.R16,
        // };

        //     // Check each format for support
        //     foreach (RenderTextureFormat format in formats)
        //     {
        //         if (SystemInfo.SupportsRenderTextureFormat(format))
        //         {
        //             Debug.Log("Supported RenderTexture format found: " + format);
        //             return format;
        //         }
        //     }

        //     // Return default format if no supported formats found
        //     return RenderTextureFormat.Default;
        // }

        // public void AssignRenderTexture(RenderTexture renderTexture)
        // {
        //     // videoPlayerRenderTexture = renderTexture;
        //     _videoPlayer.targetTexture = renderTexture;
        //     videoImage.texture = renderTexture;
        // }

    }


}