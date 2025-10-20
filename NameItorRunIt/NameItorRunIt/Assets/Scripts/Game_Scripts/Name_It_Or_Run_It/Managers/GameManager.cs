using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;

namespace Name_It_Or_Run_It
{
    public class GameManager : MonoBehaviour
    {

         public Bridge bridge;
        public static GameManager instance;
        [SerializeField] private LeanLocalization leanLocalization;
        [SerializeField] private LevelManager levelManager;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of correct answers"},
            { "1-Wrong", "Number of wrong answers"},
        };

        private void Awake()
        {
            instance = this;

            // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            Taptic.tapticOn = true;

            // _ = InitAsync();
        }

        // private async Task InitAsync()
        // {
        //     try
        //     {
        //         await initBaseOperations("Name_It_Or_Run_It");

        //         SetStartLevel(gameScoreViewModel.level);
        //         SetInGameTopbarTimerStatus(false);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //         throw;
        //     }
        // }

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
            levelManager.StartGame();
        }

        #region BaseGameManager Functions
        private void SetStartLevel(int lastLevel)
        {
            levelManager.levelId = lastLevel;
        }

        public void Finish()
        {
            // gameScoreViewModel.score = levelManager.CalculateScore();
            // gameScoreViewModel.level = levelManager.levelId;
            RecordStats(levelManager.GetCorrectCount(), levelManager.GetWrongCount());
            StartCoroutine(GameOverRoutine());
        }

         [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();

        public void RecordStats(int correctCount, int wrongCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("Wrong", wrongCount);
             
        
            int level = levelManager.levelId;
            resultObject.level = level;
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
            yield return new WaitForSeconds(0.1f);
            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
            // GameOver();
        }
        #endregion
    }
}