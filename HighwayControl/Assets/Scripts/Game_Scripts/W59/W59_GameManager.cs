using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Lean.Localization;
using UnityEngine;
using W59;

public class W59_GameManager : MonoBehaviour
{

            public Bridge bridge;

    [SerializeField] private LeanLocalization leanLocalization;
    [SerializeField] private W59_LevelController levelController;

    [SerializeField] private GameObject light;

    [Header("Witmina-Spesific"), Space(10)]
    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
    {
        { "0-bestPlay", "Most number of correct vehicle passes" },
        { "1-bestStreak", "Longest streak of correct vehicle passes" },
        { "2-successRate", "Success rate" },
    };

    public W59_Enums.eW59FxSoundStates eW59PlayerSoundState;
    public W59_Constants W59Constants = new W59_Constants();
    private void Awake()
    {
        leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

        // _ = InitAsync();
    }

    // private async Task InitAsync()
    // {
    //     try
    //     {
    //         await initBaseOperations("Highway_Control");

    //         SetStartLevel(gameScoreViewModel.level);
    //         SetInGameTopbarTimerStatus(false);
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogException(e);
    //         throw;
    //     }
    // }

    public void StartFromWebGL(int levelId)
    {

         SetStartLevel(levelId);
        levelController.AssignLevelVariables();
        levelController.SetGameTime();

        levelController.StartLevel();
        StartCoroutine(levelController.GameTimeCounter());

        // StartDayNightCycle();
    }

    private void StartDayNightCycle()
    {
        var currentRotation = light.transform.rotation.eulerAngles;
        var finalRotation = new Vector3(300, currentRotation.y, currentRotation.z);
        light.transform.DOLocalRotate(finalRotation, 30f).OnComplete(() =>
        {
            light.transform.DOLocalRotate(currentRotation, 30f);
        });
    }

     [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

        ResultClass resultObject = new ResultClass();


    public void RecordStats(int correctAnswerCount, int wrongAnswerCount, int bestStreak)
    {
        Dictionary<string, object> statData = new Dictionary<string, object>();

        statData.Add("bestPlay", correctAnswerCount);
        statData.Add("bestStreak", bestStreak);
        var rate = (int)(correctAnswerCount / (float)(correctAnswerCount + wrongAnswerCount) * 100f);
        statData.Add("successRate", rate);

        // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];
        // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
    }


    public void EndGame(int correctAnswerCount, int wrongAnswerCount, int bestStreak)
    {
        // gameScoreViewModel.score = levelController.CalculateScore();
        // gameScoreViewModel.level = levelController.levelId;

        resultObject.level = levelController.levelId;
        resultObject.score = levelController.CalculateScore();
      
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);
        RecordStats(correctAnswerCount, wrongAnswerCount, bestStreak);
        // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
        // GameOver();
    }

    private void SetStartLevel(int lastLevel)
    {
        levelController.levelId = lastLevel;
    }

    public void playFxBySoundState(W59_Enums.eW59FxSoundStates soundState, float volume = 0.3f, float pitch = 1f)
    {
        eW59PlayerSoundState = soundState;
        var soundFxNameList = W59Constants.FxSoundList;
        AudioManager.instance.PlayOneShot(soundState);
        // PlayFx(soundFxNameList[(int)soundState], volume, pitch);
    }
}