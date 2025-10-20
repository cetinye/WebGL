using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;

namespace Witmina_AtomAlchemist
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance = null;

        public Bridge bridge;


        [SerializeField] private LeanLocalization leanLocalization;

        [SerializeField] private LevelBehaviour _level;
        [SerializeField] private Electron _electronPrefab;
        [SerializeField] private SpriteData _spriteData;

        public SpriteData SpriteData => _spriteData;

        public Electron ElectronPrefab => _electronPrefab;

        public float SpeedMultiplier = 1.5f;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-numberOfCorrectHits", "Number of hits at the nucleus" },
            { "1-numberOfFailedHits", "Number of hits at electrons" },
            { "2-numberOfElementsProgressed", "Number of elements the player has progressed" },
        };

        public void Awake()
        {
            Instance = this;

            // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            _level.Initialize();
            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            // try
            // {
            //     await initBaseOperations("Atom_Alchemist");

            //     SetStartLevel(gameScoreViewModel.level);
            //     SetInGameTopbarTimerStatus(false);
            // }
            // catch (Exception e)
            // {
            //     Debug.LogException(e);
            //     throw;
            // }
        }

        public void StartFromWebGL(int levelId)
        {

            SetStartLevel(levelId);
            LoadLevel();

        }

        private void SetStartLevel(int lastLevel)
        {
            _level.levelId = lastLevel;
        }

        // public override void CustomStart()
        // {
        //     LoadLevel();
        // }

        private void Update()
        {
            // SetTimerText(Mathf.CeilToInt(_level.Timer).ToString());
        }

        public void LoadLevel()
        {
            _level.LoadIntro();
        }

        [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();

        public void Finish(int correctCount, int missCount, int atomicNumbersLeft)
        {
            // gameScoreViewModel.level = _level.levelId;
            // gameScoreViewModel.score = _level.CalculateScore();

            var rate = _level.CalculateScore();

            PlayerPrefs.SetInt("score", rate);
            PlayerPrefs.SetInt("level", _level.levelId);

            Debug.Log("score   " + PlayerPrefs.GetInt("score"));
            Debug.Log("level   " + PlayerPrefs.GetInt("level"));

            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("numberOfCorrectHits", correctCount);
            statData.Add("numberOfFailedHits", missCount);
            statData.Add("numberOfElementsProgressed", _level.progress);

            // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());


            resultObject.level = _level.levelId;
            resultObject.score = rate;
            Debug.Log("skor   " + rate);
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);



            // StartCoroutine(GameOverRoutine());
        }

        // private IEnumerator GameOverRoutine()
        // {
        //     yield return new WaitForSeconds(1.5f);
        //     SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
        //     GameOver();
        // }
    }
}