using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chefs_Secret_Recipes
{
    public class GameManager : MonoBehaviour
    {
        public Bridge bridge;



        public static GameManager instance;
        [SerializeField] private LeanLocalization leanLocalization;
        [SerializeField] private LevelManager levelManager;
        private bool isFinishRunning = false;

        [Space()]
        [SerializeField] private RenderPipelineAsset defaultRenderPipelineAsset;
        [SerializeField] private RenderPipelineAsset overrideRenderPipelineAsset;
        [SerializeField] private UniversalAdditionalCameraData mainCameraAdditionalData;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of corrects" },
            { "1-Wrong", "Number of wrongs" },
        };

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }

            leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            SwitchDefaultRenderPipeline();

            // _ = InitAsync();

        }

        public void StartFromWebGL(int levelId)
        {

            SetStartLevel(levelId);

        }

        // private async Task InitAsync()
        // {
        //     try
        //     {
        //         await initBaseOperations("Chefs_Secret_Recipes");

        //         SetStartLevel(gameScoreViewModel.level);
        //         SetInGameTopbarTimerStatus(false);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //         throw;
        //     }
        // }

        // public override void CustomStart()
        // {
        //     if (levelManager == null)
        //         return;

        //     levelManager.StartGame();
        // }

        //switch render pipeline to enable post processing
        #region RenderPipeline Switch        

        void OnDestroy()
        {
            CancelInvoke();
            StopAllCoroutines();
            DG.Tweening.DOTween.KillAll();

            GraphicsSettings.defaultRenderPipeline = defaultRenderPipelineAsset;
            QualitySettings.renderPipeline = defaultRenderPipelineAsset;
            mainCameraAdditionalData.SetRenderer(0);
            mainCameraAdditionalData.renderPostProcessing = false;

            instance = null;
        }

        void SwitchDefaultRenderPipeline()
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
            Debug.Log("Start Level : " + lastLevel);
            if (levelManager == null)
            {
                Debug.LogError("Level Manager is null!");
                return;
            }
            levelManager.levelId = lastLevel;
            levelManager.StartGame();

        }

        public void Finish()
        {
            if (!isFinishRunning)
            {
                isFinishRunning = true;


                // gameScoreViewModel.score = levelManager.CalculateTotalScore();
                // gameScoreViewModel.level = levelManager.levelId;
                RecordStats(levelManager.totalCorrectCount, levelManager.totalWrongCount);

                StartCoroutine(GameOverRoutine());
            }
        }

          [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();

        public void RecordStats(int correctCount, int falseCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("False", falseCount);


            int level = levelManager.levelId;
            int score = levelManager.CalculateTotalScore();
            // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());

              Debug.Log("score   " + PlayerPrefs.GetInt("score"));
            Debug.Log("level   " + PlayerPrefs.GetInt("level"));

            resultObject.level = level;
            resultObject.score = score;
            Debug.Log("skor   " + score);
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);
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