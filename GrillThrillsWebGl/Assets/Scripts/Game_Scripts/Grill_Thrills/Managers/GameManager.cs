using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Grill_Thrills
{
    public class GameManager : MonoBehaviour
    {


        public Bridge bridge;



        public static GameManager instance;
        [SerializeField] private LeanLocalization leanLocalization;
        private LevelManager levelManager;
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
            if (instance == null)
            {
                instance = this;
                 levelManager = LevelManager.instance;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            // SwitchDefaultRenderPipeline();

            _ = InitAsync();
        }

        private async Task InitAsync()
        {
            // try
            // {
            //     await initBaseOperations("Grill_Thrills");

            //     SetStartLevel(gameScoreViewModel.level);
            //     SetInGameTopbarTimerStatus(false);
            // }
            // catch (Exception e)
            // {
            //     Debug.LogException(e);
            //     throw;
            // }
        }

        void Start()
        {
            levelManager = LevelManager.instance;

        }

        public void StartFromWebGL(int levelId)
        {
            
            Debug.Log("Starting from WebGL");
            levelManager.StartGame();
        }

        // public override void CustomStart()
        // {
        //     Debug.Log("Called");
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
                RecordStats(levelManager.totalCorrectCount + levelManager.totalIdeallyCookedCount, levelManager.totalWrongCount);

                StartCoroutine(GameOverRoutine());
            }
        }

        public void RecordStats(int correctCount, int falseCount)
        {

            var rate = levelManager.CalculateScore();
            int level = levelManager.levelId;

            PlayerPrefs.SetInt("score", rate);
            PlayerPrefs.SetInt("level", level);


            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("False", falseCount);

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
            yield return new WaitForSeconds(1f);
            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
            // GameOver();
        }

        #endregion
    }
}