using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using TMPro;
using UnityEngine;

namespace Customs_Scanner
{
    public class GameManager : MonoBehaviour
    {
        public Bridge bridge;
        public static GameManager instance;

        [SerializeField] private LeanLocalization leanLocalization;

        public TMP_Text scoreText;

        [SerializeField] private LevelManager levelManager;
        [SerializeField] private UIManager uiManager;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of detected prohibited items" },
            { "1-Wrong", "Number of undetected prohibited items" },
        };

        private void Awake()
        {
            instance = this;

            leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            Taptic.tapticOn = true;
        }

        public void StartFromWebGL(int levelId)
        {
            SetStartLevel(levelId);
            levelManager.StartGame();
        }

        public void Wrong()
        {
            levelManager.wrongCount++;
            levelManager.wrong++;
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
            Debug.Log("GameManager.Finish() called");
            int score = (levelManager.correctCount * 100) - (levelManager.wrongCount * levelManager.level.penaltyPoints);
            Debug.Log($"Score calculated: {score}");
            int finalScore = Mathf.Clamp(Mathf.CeilToInt((float)score / (levelManager.totalPassedForbiddenProductAmount * 100) * 1000), 0, 1000);
            Debug.Log($"Final score calculated: {finalScore}");
            finalScore = Mathf.Clamp(finalScore, 0, 1000);
            Debug.Log($"Final score clamped: {finalScore}");

            resultObject.level = levelManager.levelId;
            resultObject.score = finalScore;
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);

            Debug.Log($"Submitted score: {score}, level: {levelManager.levelId}");
            RecordStats(levelManager.correctCount, levelManager.wrongCount);

            StartCoroutine(GameOverRoutine());
        }

        public void RecordStats(int correctCount, int wrongCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("Wrong", wrongCount);

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