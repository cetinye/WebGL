using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;
using W57;

public class W57_GameManager : MonoBehaviour
{

    public Bridge bridge;



    [SerializeField] private W57_LevelManager levelManager;
    [SerializeField] private LeanLocalization leanLocalization;

    [Header("Witmina-Spesific"), Space(10)]
    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
    {
        { "0-bestStreak", "Longest streak of birds saved" },
        { "1-birdsSaved", "Number of birds saved" },
        { "2-birdsElectrocuted", "Number of birds electrocuted" },
        { "3-savingRate", "Bird saving success rate" },
    };

    public eW57FxSoundStates eW57PlayerSoundState;
    public W57_Constants W57Constants = new W57_Constants();

    private void Awake()
    {
        // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

        // _ = InitAsync();
    }

    // private async Task InitAsync()
    // {
    //     try
    //     {
    //         await initBaseOperations("Electro_Birds");

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
        levelManager.SetGameTime();
        levelManager.SetRandomBackground();

        levelManager.SetLevel(true);
    }

    [Serializable]
    public class ResultClass
    {
        public int level;
        public int score;

    }

    ResultClass resultObject = new ResultClass();


    public void RecordStats(int bestStreak, int savedCount, int electrocutedCount, int successRate)
    {
        Dictionary<string, object> statData = new Dictionary<string, object>();

        statData.Add("bestStreak", bestStreak);
        statData.Add("birdsSaved", savedCount);
        statData.Add("birdsElectrocuted", electrocutedCount);
        statData.Add("savingRate", successRate);

        // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];
        // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
    }


    public void EndGame(int bestStreak, int savedCount, int electrocutedCount)
    {
        var successRate = (int)(savedCount / (float)(savedCount + electrocutedCount) * 100f);

        // gameScoreViewModel.level = levelManager.levelId;
        int score = Mathf.Clamp(Mathf.CeilToInt((float)levelManager.CalculateScore() / levelManager.levelSO.maxInGame * 1000), 0, 1000);
        int level = levelManager.levelId;

        resultObject.level = level;
        resultObject.score = score;
        Debug.Log("skor   " + score);
        string json = JsonUtility.ToJson(resultObject);
        bridge.SendToJSJson(json);

        // gameScoreViewModel.score = Mathf.Clamp(score, 0, 1000);
        RecordStats(bestStreak, savedCount, electrocutedCount, successRate);
        // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
        // GameOver();
    }

    private void SetStartLevel(int lastLevel)
    {
        levelManager.levelId = lastLevel;
    }

    public void playFxBySoundState(eW57FxSoundStates soundState, float volume = 0.4f, float pitch = 1f)
    {
        eW57PlayerSoundState = soundState;
        var soundFxNameList = W57Constants.FxSoundList;
        AudioManager.instance.PlayOneShot(soundState);
        // PlayFx(soundFxNameList[(int)soundState], volume, pitch);
    }
}