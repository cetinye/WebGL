using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using TMPro;
using UnityEngine;

namespace Witmina_SweetMemory
{
    public class GameManager : MonoBehaviour
    {
        public Bridge bridge;

        [SerializeField] private LeanLocalization leanLocalization;
        public static GameManager Instance = null;
        public static LevelSO LevelSO;
        public static CakeData CakeData => Instance._cakeData;
        public static QuestionData QuestionData => Instance._questionData;

        [SerializeField] private CakeData _cakeData;
        [SerializeField] private QuestionData _questionData;
        [SerializeField] private LevelBehaviour _level;
        [SerializeField] private TMP_Text _levelText;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            {"0-numberOfCorrectAnswers", "Number of correct answers"},
            {"1-numberOfWrongAnswers", "Number of wrong answers"},
        };

        [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();

        private int _playerLevel;
        public int PlayerLevel;
        
        private void Awake()
        {
            if (!Instance)
                Instance = this;

            leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);
        }

        private void SetStartLevel(int lastLevel)
        {
            PlayerLevel = lastLevel;
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
            StartCoroutine(StartRoutine());
        }

        private void Update()
        {
            // SetTimerText(Mathf.CeilToInt(_level.Timer).ToString());
        }

        public void Load()
        {
            PlayerLevel = Mathf.Clamp(PlayerLevel, 0, 15);
            _level.Load();
        }

        private IEnumerator StartRoutine()
        {
            _level.CanvasAnimator.enabled = true;
            AudioController.PlayIntro();
            yield return new WaitForSeconds(7.5f);
            _level.CanvasAnimator.enabled = false;
            yield return new WaitForEndOfFrame();
            Load();
        }

        public void End(int correctAnswers, int wrongAnswers)
        {
            int score = _level.CalculateTotalScore();
            score = Mathf.Clamp(score, 0, 1000);
            Debug.Log("Score: " + score);

            resultObject.level = PlayerLevel;
            resultObject.score = score;
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);

            RecordStats(correctAnswers, wrongAnswers);

            StartCoroutine(GameOverRoutine());
        }

        public void RecordStats(int correctCount, int wrongCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("numberOfCorrectAnswers", correctCount);
            statData.Add("numberOfWrongAnswers", wrongCount);

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
    }
}