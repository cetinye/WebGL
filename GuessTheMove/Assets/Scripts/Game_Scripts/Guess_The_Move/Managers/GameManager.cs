using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;

namespace Guess_The_Move
{
    public class GameManager : MonoBehaviour
    {

        public Bridge bridge;
        public static GameManager instance;
        [SerializeField] private LeanLocalization leanLocalization;
        private bool isFinishRunning;
        private LevelManager levelManager;

        public Dictionary<string, string> statDescriptions = new()
        {
            { "0-Correct", "Number of corrects" },
            { "1-Wrong", "Number of wrongs" }
        };

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;

            // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            // _ = InitAsync();
        }

        // private async Task InitAsync()
        // {
        //     try
        //     {
        //         await initBaseOperations("Guess_The_Move");

        //         SetStartLevel(gameScoreViewModel.level);
        //         SetInGameTopbarTimerStatus(false);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //         throw;
        //     }
        // }

        private void Start()
        {
            
        }

        public void StartFromWebGL(int levelId)
        {
            levelManager = LevelManager.instance;
            SetStartLevel(levelId);
            levelManager.StartGame();
        }

        #region BaseGameManager Functions

        private void SetStartLevel(int lastLevel)
        {
            levelManager.levelId = lastLevel;
        }

          [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();

        public void Finish()
        {
            if (!isFinishRunning)
            {
                isFinishRunning = true;

                // gameScoreViewModel.score = levelManager.GetTotalScore();
                // gameScoreViewModel.level = levelManager.levelId;
                RecordStats(levelManager.totalCorrectCount, levelManager.totalWrongCount);

                StartCoroutine(GameOverRoutine());
            }
        }

        public void RecordStats(int correctCount, int wrongCount)
        {
            var statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("Wrong", wrongCount);

            // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            resultObject.level = levelManager.levelId;
            resultObject.score = levelManager.GetTotalScore();

            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(1f);
            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
            // GameOver();
        }

        #endregion
    }
}