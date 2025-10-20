using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace W91_ReflectoGear
{
    public class GameManager : MonoBehaviour
    {
        public Bridge bridge;

        public static GameManager instance;

        [SerializeField] private LevelManager levelManager;
        [SerializeField] private UIManager uiManager;

        public GameState state = GameState.Idle;
        public Camera mainCamera;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of correct gears tapped" },
            { "1-Wrong", "Number of wrong gears tapped" },
        };

        [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();

        private void Awake()
        {
            instance = this;

            // SetInGameTopbarTimerStatus(false);
            // initBaseOperations("Reflecto_Gear");

        }


        // void Start()
        // {
        //         SetStartLevel(0);
        //     state = GameState.Intro;

        //     uiManager.StartIntro();

        // }

        public void SetLanguage(string languageCode)
        {
            switch (languageCode)
            {
                case "en":
                    break;

                case "tr":
                    break;

                default:
                    break;
            }
        }

        public void StartFromWebGL(int levelId)
        {
            Debug.Log("StartFromWebGL   " + levelId);
            SetStartLevel(levelId);
            state = GameState.Intro;

            uiManager.StartIntro();

        }

        private void SetStartLevel(int lastLevel)
        {
            Debug.Log("SetStartLevel   " + lastLevel);
            levelManager.levelId = lastLevel;
        }

        public void Finish()
        {
            var score = levelManager.GetTotalScore();
            PlayerPrefs.SetInt("score", score);
            PlayerPrefs.SetInt("level", levelManager.levelId);
            // gameScoreViewModel.score = levelManager.GetTotalScore();
            // gameScoreViewModel.level = levelManager.levelId;
            RecordStats(levelManager.correctCounter, levelManager.errorCounter);

              resultObject.level = levelManager.levelId;
        resultObject.score = score;
        string json = JsonUtility.ToJson(resultObject);
        bridge.SendToJSJson(json);

            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);

          //  transform.parent.gameObject.SetActive(false);
            // SceneManager.LoadSceneAsync("ScoreScene", LoadSceneMode.Additive);
            // StartCoroutine(GameOverRoutine());
        }

        public void RecordStats(int correctCount, int falseCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("False", falseCount);

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
        }

        public enum GameState
        {
            Intro,
            Idle,
            Success,
            Failed,
            Playing
        }
    }
}