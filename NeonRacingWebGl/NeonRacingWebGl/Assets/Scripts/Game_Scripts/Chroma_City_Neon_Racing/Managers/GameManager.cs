using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Lean.Localization;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chroma_City_Neon_Racing
{
    public class GameManager : MonoBehaviour
    {

        public Bridge bridge;



        public static GameManager instance;

        [SerializeField] private LeanLocalization leanLocalization;

        [Space][SerializeField] private RenderPipelineAsset defaultRenderPipelineAsset;

        [SerializeField] private RenderPipelineAsset overrideRenderPipelineAsset;
        [SerializeField] private UniversalAdditionalCameraData mainCameraAdditionalData;
        private bool isFinishRunning;
        private LevelManager levelManager;

        public Dictionary<string, string> statDescriptions = new()
        {
            { "0-Correct", "Number of corrects" },
            { "1-Wrong", "Number of wrongs" }
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
                return;
            }

            leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            SwitchDefaultRenderPipeline();

            // _ = InitAsync();


        }

        // private async Task InitAsync()
        // {
        //     try
        //     {
        //         await initBaseOperations("Neon_Racing");

        //         SetStartLevel(gameScoreViewModel.level);
        //         SetInGameTopbarTimerStatus(false);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //         throw;
        //     }
        // }

        // private void Start()
        // {
        //     levelManager = LevelManager.instance;

        //     levelManager.StartGame();
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
            levelManager = LevelManager.instance;
            SetStartLevel(levelId);

            levelManager.StartGame();
            if (TrafficLight.instance != null && levelManager != null)
                levelManager.StartTrafficLight();

        }

        // public override void CustomStart()
        // {
        //     if (TrafficLight.instance != null && levelManager != null)
        //         levelManager.StartTrafficLight();
        // }

        //switch render pipeline to enable post processing

        #region RenderPipeline Switch

        private void OnDestroy()
        {
            CancelInvoke();
            StopAllCoroutines();
            DOTween.KillAll();

            GraphicsSettings.defaultRenderPipeline = defaultRenderPipelineAsset;
            QualitySettings.renderPipeline = defaultRenderPipelineAsset;
            mainCameraAdditionalData.SetRenderer(0);
            mainCameraAdditionalData.renderPostProcessing = false;
        }

        private void SwitchDefaultRenderPipeline()
        {
            GraphicsSettings.defaultRenderPipeline = overrideRenderPipelineAsset;
            QualitySettings.renderPipeline = overrideRenderPipelineAsset;

            mainCameraAdditionalData.SetRenderer(0);
            mainCameraAdditionalData.renderPostProcessing = true;
        }

        #endregion

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
                RecordStats(levelManager.GetCorrectCount(), levelManager.GetWrongCount());

                StartCoroutine(GameOverRoutine());
            }
        }

        public void RecordStats(int correctCount, int falseCount)
        {
            var statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("False", falseCount);

            var rate = levelManager.CalculateScore();
            int level = levelManager.levelId;
            // level--;

            level = Mathf.Clamp(level, 1, levelManager.maxLevelWKeys);

            PlayerPrefs.SetInt("score", rate);
            PlayerPrefs.SetInt("level", levelManager.levelId);

            // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());

            resultObject.level = level;
            resultObject.score = rate;
            Debug.Log("skor   " + rate);
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);


        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(5f);
            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
            // GameOver();
        }

        #endregion
    }
}