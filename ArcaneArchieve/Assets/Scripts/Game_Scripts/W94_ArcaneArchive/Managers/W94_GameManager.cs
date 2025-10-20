using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arcane_Archive;
using Lean.Localization;
using UnityEngine;

public class W94_GameManager : MonoBehaviour
{
    

     public Bridge bridge;
    public static W94_GameManager instance;
    [SerializeField] private W94_LevelManager levelManager;
    [SerializeField] private W94_UIManager uiManager;
    [SerializeField] private LeanLocalization leanLocalization;

    [SerializeField] private TMPro.TMP_Text scoreText;

    public GameState state;
    public Camera mainCamera;

    private bool isFinishRunning = false;

    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of books matched" },
            { "1-Wrong", "Number of moves" },
        };

    private void Awake()
    {
        instance = this;

        leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

        uiManager.PlayPauseVideo();
        // _ = InitAsync();
    }

    // private async Task InitAsync()
    // {
    //     try
    //     {
    //         await initBaseOperations("Arcane_Archive");

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
    //     uiManager.StartIntro();
    // }

    public void StartFromWebGL(int levelId)
    {
        SetStartLevel(levelId);
        uiManager.StartIntro();
        state = GameState.idle;
            StartGame();
        // state = GameState.idle;
    }
   
    // private void Start()
    // {
        
    // }

    public void StartGame()
    {
        uiManager.DisableBlackScreen();
        levelManager.StartLevel();
        uiManager.ResetTime();
    }

    public void RemoveFromBooksList(GameObject bookToRemove)
    {
        levelManager.RemoveBook(bookToRemove);
    }

    public void IncreaseTotalMoveCounter()
    {
        levelManager.IncreaseTotalMovesCounter();
    }

    public void CheckEndLevel()
    {
        if (levelManager.GetActiveBookCount() == 0)
        {
            state = GameState.idle;
            W94_AudioManager.instance.PlayOneShot("Success");
            levelManager.StartEndAnim();
        }
    }

    public void CheckStuck()
    {
        if (levelManager.isShelvesFull())
            Finish(false);
    }

    public void ArrangeFrames(Transform parent)
    {
        uiManager.ArrangeFrames(parent);
    }

    private void DecideLevel(bool isSuccess)
    {
        if (isSuccess)
        {
            int upCounter = PlayerPrefs.GetInt("ArcaneArchive_UpCounter", 0);
            if (++upCounter >= 2)
            {
                upCounter = 0;
                levelManager.levelIndex++;
                Debug.LogWarning("Yeni levelIndex: " + levelManager.levelIndex);
            }
            Debug.LogWarning("UpCounter: " + upCounter);
            PlayerPrefs.SetInt("ArcaneArchive_UpCounter", upCounter);
        }
        else
        {
            int downCounter = PlayerPrefs.GetInt("ArcaneArchive_DownCounter", 0);
            if (++downCounter >= W94_LevelManager.levelSO.levelDownCriteria)
            {
                downCounter = 0;
                levelManager.levelIndex--;
                Debug.LogWarning("Yeni levelIndex: " + levelManager.levelIndex);
            }
            Debug.LogWarning("DownCounter: " + downCounter);
            PlayerPrefs.SetInt("ArcaneArchive_DownCounter", downCounter);
        }
    }

    #region BaseGameManager Functions
    private void SetStartLevel(int lastLevel)
    {
        levelManager.levelIndex = lastLevel;
    }

    public void Finish(bool isSuccess)
    {
        if (!isFinishRunning)
        {
            isFinishRunning = true;

            DecideLevel(isSuccess);

            int totalBookAmount = W94_LevelManager.levelSO.backShelfBookAmount + W94_LevelManager.levelSO.frontShelfBookAmount;
            levelManager.score = (levelManager.totalBooksCleared * W94_LevelManager.levelSO.pointsPerCorrect) - ((totalBookAmount - levelManager.totalBooksCleared) * W94_LevelManager.levelSO.penaltyPoints);

            scoreText.text = "Score: " + levelManager.score.ToString();



            // gameScoreViewModel.score = Mathf.Clamp(Mathf.CeilToInt((float)levelManager.score / W94_LevelManager.levelSO.maxInLevel * 1000), 0, 1000);
            // gameScoreViewModel.level = levelManager.levelIndex;
            RecordStats(levelManager.totalBooksCleared, levelManager.totalMoves);

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

        // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];
        // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());

        var score = Mathf.Clamp(Mathf.CeilToInt((float)levelManager.score / W94_LevelManager.levelSO.maxInLevel * 1000), 0, 1000);
        int level = levelManager.levelIndex;
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetInt("level", level);
            
             resultObject.level = level;
            resultObject.score = score;
            Debug.Log("skor   " + score + " level " + level);
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

    public enum GameState
    {
        intro,
        idle,
        playing
    }
}