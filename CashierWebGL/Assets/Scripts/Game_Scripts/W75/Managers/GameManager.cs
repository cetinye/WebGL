using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;

namespace Cashier
{
    public class GameManager : MonoBehaviour
    {
        public Bridge bridge;

        public static GameManager instance;
        [SerializeField] private LeanLocalization leanLocalization;
        [SerializeField] private LevelManager levelManager;
        private bool isFinishRunning = false;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of correct barcode reads" },
            { "1-Wrong", "Number of wrong barcode reads" },
        };

        private void Awake()
        {
            instance = this;

            leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);
        }

        public void StartFromWebGL(int levelId)
        {
            SetStartLevel(levelId);
            levelManager.Initialize();
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

                resultObject.level = levelManager.levelId;
                resultObject.score = levelManager.CalculateScore();
                Debug.Log("skor   " + levelManager.CalculateScore());
                string json = JsonUtility.ToJson(resultObject);
                bridge.SendToJSJson(json);

                RecordStats(levelManager.totalCorrect, levelManager.totalWrong);

                StartCoroutine(GameOverRoutine());
            }
        }

        public void RecordStats(int correctCount, int falseCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("False", falseCount);


            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(1f);
            // GameOver();
        }

        #endregion
    }
}