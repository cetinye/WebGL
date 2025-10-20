using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NanoInspector
{
    public class GameManager : MonoBehaviour
    {

        public Bridge bridge;
        public static GameManager instance;

        public GameState state = GameState.Idle;

        [SerializeField] private LevelManager levelManager;
        [SerializeField] private UIManager uiManager;

        private bool isFinishRunning = false;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of correct organisms investigated" },
            { "1-Wrong", "Number of wrong organisms investigated" },
        };

        private void Awake()
        {
            instance = this;

            // SetInGameTopbarTimerStatus(false);
            // initBaseOperations("Nano_Inspector");

        }

        // public void Start()
        // {

        //     uiManager.StartAnimation();
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
            uiManager.StartAnimation();
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

                int totalQuestionAmount = levelManager.correctCount + levelManager.wrongCount;
                float multiplier = (float)levelManager.correctCount / (float)totalQuestionAmount;
                levelManager.score = Mathf.CeilToInt(1000f * multiplier);
                levelManager.score = Mathf.Clamp(levelManager.score, 0, 1000);

                var score = Mathf.CeilToInt(Mathf.Max(levelManager.score, 0));
                // PlayerPrefs.SetInt("score", score);
                // PlayerPrefs.SetInt("level", levelManager.levelId);
                // gameScoreViewModel.score = Mathf.CeilToInt(Mathf.Max(levelManager.score, 0));
                // gameScoreViewModel.level = levelManager.levelId;
                RecordStats(levelManager.correctCount, levelManager.wrongCount);


                resultObject.level = levelManager.levelId;
                resultObject.score = score;
                Debug.Log("skor   " + score);
                string json = JsonUtility.ToJson(resultObject);
                bridge.SendToJSJson(json);

                // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);

                StartCoroutine(GameOverRoutine());
            }
        }

        public void RecordStats(int correctCount, int wrongCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("Wrong", wrongCount);

            // WManagers.WRDB.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(1f);
            // GameOver();

            transform.parent.gameObject.SetActive(false);
        }
        #endregion

        public enum GameState
        {
            Intro,
            Idle,
            Playing
        }
    }
}