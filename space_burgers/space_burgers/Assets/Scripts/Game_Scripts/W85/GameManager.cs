using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;
using UnityEngine.Video;

namespace Witmina_SpaceBurgers
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
        [SerializeField] private VideoClip enVideoClip;
        [SerializeField] private VideoClip trVideoClip;
        [SerializeField] private AudioController _audioController;
        [SerializeField] private PrefabData _prefabData;

        private List<int> bonusScores = new List<int>();

        public static PrefabData PrefabData => Instance != null ? Instance._prefabData : null;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            {"0-numberOfCustomersSatisfied", "Number of customers satisfied"},
            {"1-numberOfCustomersUpset", "Number of customers upset"},
        };

        public static void PlayAudioFx(AudioFxType fx, float delay = 0f)
        {
            if (!Instance)
                return;

            Instance._audioController.PlayFx(fx, delay);
        }

        private Coroutine _videoRoutine;
        private bool _videoWatched;
        private bool _started;

        public Camera MainCamera => _mainCamera;

        private void Awake()
        {
            // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            // _videoPlayer.clip = LeanLocalization.Instances[0].CurrentLanguage == "English" ? enVideoClip : trVideoClip;

            _level.AssignLevel();

            _started = false;

            Instance = this;
            _uiController.ToggleIntroPanel(true);
            _videoPlayer.Play();
            _videoPlayer.Pause();

            _videoWatched = PlayerPrefs.GetInt("SpaceBurgers_VideoWatched", 0) != 0;

            _level.DisableTrays();
            // _ = InitAsync();
        }

        // private async Task InitAsync()
        // {
        //     try
        //     {
        //         await initBaseOperations("Space_Burgers");

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
            _level.levelId = lastLevel;
            _level.AssignLevel();
            _level.DisableTrays();
        }

        void Start()
        {
            _uiController.UpdateLevel(_level.levelId);
        }

        private void Update()
        {
            if (_started)
                _uiController.UpdateTime(Mathf.CeilToInt(_level.Timer));
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
            _videoRoutine = StartCoroutine(VideoRoutine());
        }

        public void LoadLevel()
        {
            StartCoroutine(LoadRoutine());
        }

        public void SkipButton()
        {
            _uiController.ToggleSkipButton(false);
            if (_videoRoutine != null)
                StopCoroutine(_videoRoutine);
            StartCoroutine(VideoEndRoutine());
        }

        public void End(int customerSatisfied, int customerUpset)
        {
            // gameScoreViewModel.score = _level.CalculateTotalScore();
            // gameScoreViewModel.level = _level.levelId;
            RecordStats(customerSatisfied, customerUpset);

            StartCoroutine(EndRoutine());
        }

  [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();


        public void RecordStats(int customerSatisfied, int customerUpset)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            resultObject.level = _level.levelId;
            resultObject.score = _level.CalculateTotalScore();

            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);

            statData.Add("numberOfCustomersSatisfied", customerSatisfied);
            statData.Add("numberOfCustomersUpset", customerUpset);

            // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
        }

        private IEnumerator VideoRoutine()
        {
            // _uiController.ToggleSkipButton(false);
            // _videoPlayer.Play();
            // yield return new WaitForSeconds(0.1f);
            // _audioController.PlayIntro();
            // yield return new WaitForSeconds(1f);

            _videoWatched = true;
            if (_videoWatched)
                _uiController.ToggleSkipButton(true);

            // yield return new WaitForSeconds((float)_videoPlayer.clip.length - 1f);

            _videoWatched = true;
            PlayerPrefs.SetInt("SpaceBurgers_VideoWatched", 1);
            _uiController.ToggleSkipButton(false);
            SkipButton();
            yield return null;
        }

        private IEnumerator VideoEndRoutine()
        {
            _uiController.PlayScreenTransitionOutro();
            _audioController.PlayFx(AudioFxType.IntroPanelOpen);
            yield return new WaitForSeconds(1.6f);
            _audioController.StopIntro();
            _videoPlayer.Stop();
            _uiController.ToggleIntroPanel(false);
            LoadLevel();
        }

        private IEnumerator LoadRoutine()
        {
            _uiController.Initialize();
            _uiController.PlayScreenTransitionIntro();
            _audioController.PlayFx(AudioFxType.IntroPanelOpen);
            _level.Load();
            _started = true;
            yield return new WaitForSeconds(1.5f);
            _level.BeginLevel();
        }

        private IEnumerator EndRoutine()
        {
            _uiController.PlayScreenTransitionOutro();
            _audioController.PlayFx(AudioFxType.IntroPanelClose);
            yield return new WaitForSeconds(1.5f);
            _uiController.ActivateEndGamePanel();
            yield return new WaitForSeconds(0.5f);
            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
            // GameOver();
        }
    }
}