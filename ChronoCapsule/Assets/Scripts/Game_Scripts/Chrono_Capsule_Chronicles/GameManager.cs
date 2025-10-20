using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;

namespace Chrono_Capsule_Chronicles
{
    public class GameManager : MonoBehaviour
    {
        public Bridge bridge;

        public static GameManager instance;
        [SerializeField] private LeanLocalization leanLocalization;
        private LevelManager levelManager;
        private bool isFinishRunning = false;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of corrects" },
            { "1-Wrong", "Number of wrongs" },
        };

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            // _ = InitAsync();
        }

        // private async Task InitAsync()
        // {
        //     try
        //     {
        //         await initBaseOperations("Chrono_Capsule_Chronicles");

        //         SetStartLevel(gameScoreViewModel.level);
        //         SetInGameTopbarTimerStatus(false);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //         throw;
        
        //     }
        // }

        // void Start()
        // {

        // }

        public void StartFromWebGL(int levelId)
        {
            levelManager = LevelManager.instance;

            levelManager.gameTime = 60f;
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

                // gameScoreViewModel.score = levelManager.CalculateScore();
                // gameScoreViewModel.level = levelManager.levelId;
                RecordStats(levelManager.totalCorrect, levelManager.totalWrong);

                StartCoroutine(GameOverRoutine());
            }
        }

        public void RecordStats(int correctCount, int wrongCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("Wrong", wrongCount);

            resultObject.level = levelManager.levelId;
            resultObject.score = levelManager.CalculateScore();
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);



            // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

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