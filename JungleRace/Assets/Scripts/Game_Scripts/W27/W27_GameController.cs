using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using JungleRace;
using Lean.Localization;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using W27;
using W27_JungleRace;
using Random = UnityEngine.Random;

public class W27_GameController : MonoBehaviour
{

    public Bridge bridge;
    [SerializeField] private LeanLocalization leanLocalization;
    [SerializeField] private List<W27_LevelSO> levels = new List<W27_LevelSO>();
    public static W27_LevelSO LevelSO;

    public W27_MazeGenerator MazeInstance;
    public W27_MazeGenerator mg;

    public int level = 3;
    private float time;
    public GameObject getReadyIndicator;
    public SpriteRenderer levelCompletedIndicator;
    public Camera cam;
    public TextMeshPro getReadyText;

    public int currentNumberOfLosses = 0;
    public int currentNumberOfWins = 0;

    public W27_Constants W27Constants = new W27_Constants();

    private int tutorialStepCount = 5;
    public Vector3[] indicatorPoses;
    public GameObject joystick;

    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundsText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;

    public int tutorialStepID;
    private int gameTime;
    public bool timerActive;
    private List<int> levelScores = new List<int>();

    //stats
    public int numberOfWinsTotal = 0;
    public int numberOfLossesTotal = 0;

    public int roundsFailed = 0;
    public int roundsCompleted = 0;
    public int roundsPlayed = 0;
    private int maxLevelWKeys;

    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
    {
        {"0-numberOfWinsTotal", "Total number of wins "},
        {"1-numberOfLossesTotal", "Total number of losses "},
    };

    private void Awake()
    {
        // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

        maxLevelWKeys = levels.Count / 2;
        Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

        // LocalizationKeys = W27Constants.LocalizationKeys;

        gameTime = 60;

        // _ = InitAsync();
    }

    // private async Task InitAsync()
    // {
    //     try
    //     {
    //         await initBaseOperations("Jungle_Race");

    //         SetStartLevel(gameScoreViewModel.level);
    //         SetInGameTopbarTimerStatus(false);
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogException(e);
    //         throw;
    //     }
    // }

    void GetIndicatorPositions()
    {
        indicatorPoses = new Vector3[tutorialStepCount];
        indicatorPoses[0] = joystick.transform.localPosition;
    }

    public void createTutorialElement(int index, bool isIndicatorActive = true)
    {
        //if (index == 1)
        //{
        //    indicatorPoses[1] = mg.target.transform.localPosition;
        //}
        Vector3 indicatorPos = indicatorPoses[index];

        if (indicatorPos == Vector3.zero)
        {
            isIndicatorActive = false;
        }

        // var tutorialOptions = new TutorialItemConfig(true, false, isIndicatorActive, true, indicatorPos, false, eTutorialPopUpPositionState.TOP, 500f, false, "");
        // if (isTutorialInitialized)
        // gameTutorialController.createNextPopUpComponent(tutorialOptions, index, cam);
    }

    public void StartFromWebGL(int levelId)
    {
        SetStartLevel(levelId);
        AudioManager.instance.Play(SoundType.BG);
        GetIndicatorPositions();
        StartGame();
    }

    void StartGame()
    {
        AssignLevel();
        // StartCoroutine(startTimer());
        getReadyIndicator.SetActive(false);
        getReadyTween();
    }

    public void AssignLevel()
    {
        level = math.clamp(level, 3, maxLevelWKeys + 3);
        LevelSO = levels[level - 3];

        levelText.text = $"{LeanLocalization.GetTranslationText("Level")} " + (level - 2);
    }

    private IEnumerator startTimer()
    {
        while (time < gameTime)
        {
            if (timerActive)
            {
                time += 1;
            }
            String printTime = ((int)time).ToString();
            timerText.text = printTime;
            yield return new WaitForSeconds(1);
        }

        handleGameOver();
    }
    public void nextLevel()
    {
        getReadyIndicator.SetActive(false);
        timerActive = true;

        // var targetDistanceMin = (size * size) / 2;
        // var targetDistanceMax = (size * size) - 1;

        float cameraDistance = level + 13f;

        // if (gameTutorialController != null)
        // if (false)
        // {
        //     if (tutorialStepID == 0)
        //     {
        //         size = 6;
        //         randomNumberOfRotations = 0;
        //         ghostSpeed = 0;
        //         targetDistanceMin = 20;
        //         targetDistanceMax = 20;

        //         createTutorialElement(0);
        //     }
        //     else if (tutorialStepID == 1)
        //     {
        //         size = 6;
        //         randomNumberOfRotations = 0;
        //         ghostSpeed = 0f;
        //         targetDistanceMin = 20;
        //         targetDistanceMax = 20;
        //         createTutorialElement(1);
        //     }
        //     else if (tutorialStepID == 2)
        //     {
        //         size = 6;
        //         randomNumberOfRotations = 0;
        //         ghostSpeed = 1;
        //         targetDistanceMin = 20;
        //         targetDistanceMax = 20;
        //         createTutorialElement(2);
        //     }
        //     else if (tutorialStepID == 3)
        //     {
        //         size = 6;
        //         randomNumberOfRotations = 1;
        //         ghostSpeed = 1f;
        //         targetDistanceMin = 20;
        //         targetDistanceMax = 20;
        //         createTutorialElement(3);
        //     }

        //     level = 2;
        // }
        // else
        // {
        //     // if (level == 3)
        //     // {
        //     //     size = 4;
        //     //     randomNumberOfRotations = 0;
        //     //     ghostSpeed = 1;
        //     // }
        //     // else if (level == 4)
        //     // {
        //     //     size = 4;
        //     //     randomNumberOfRotations = 0;
        //     //     ghostSpeed = 1.1;
        //     // }
        //     // else if (level == 5)
        //     // {
        //     //     size = 5;
        //     //     randomNumberOfRotations = 1;
        //     //     ghostSpeed = 1.3f;
        //     // }
        //     // else if (level == 6)
        //     // {
        //     //     size = 5;
        //     //     randomNumberOfRotations = 2;
        //     //     ghostSpeed = 1.5f;
        //     // }
        //     // else if (level == 7)
        //     // {
        //     //     size = 6;
        //     //     randomNumberOfRotations = 3;
        //     //     ghostSpeed = 2f;
        //     // }
        //     targetDistanceMin = (size * size) / 2;
        //     targetDistanceMax = (size * size) - 1;
        // }

        mg = Instantiate(MazeInstance, new Vector3(0, 0, 0), Quaternion.identity);
        mg.gm = this;
        mg.cam = cam;
        mg.levelCompleteIndicator = levelCompletedIndicator;
        mg.setLevelParameters(LevelSO.mapSize, LevelSO.mapSize, LevelSO.numOfRotations, LevelSO.mapSize * LevelSO.mapSize / 2, (LevelSO.mapSize * LevelSO.mapSize) - 1,
            LevelSO.turtleSpeed, LevelSO.rabbitSpeed, cameraDistance);
    }

    public void getReadyTween()
    {
        // getReadyText.text = LocalizationData[LocalizationKeys[(int)eW27LocalizationDefKey.GET_READY]];
        getReadyText.text = $"{LeanLocalization.GetTranslationText("GetReady")}";
        cam.transform.rotation = new Quaternion(0, 0, 0, 0);
        getReadyIndicator.SetActive(true);
        getReadyIndicator.transform.position = new Vector3(0, -30, 0);
        getReadyIndicator.transform.DOMoveY(0, 2f).OnComplete(nextLevel);

        roundsText.text = roundsPlayed + 1 + " / " + LevelSO.totalRounds;
    }

    public int CalculateLevelScore(bool isSuccess)
    {
        int levelScore = 0;
        if (isSuccess)
            levelScore = LevelSO.maxInLevel;
        else
            levelScore = -LevelSO.penaltyPoints;

        levelScores.Add(levelScore);

        return levelScore;
    }

    private int CalculateTotalScore()
    {
        int total = 0;
        foreach (var score in levelScores)
        {
            total += score;
        }

        return Mathf.Clamp(total, 0, 1000);
    }

    [Serializable]
    public class ResultClass
    {
        public int level;
        public int score;

    }

    ResultClass resultObject = new ResultClass();

    public void handleGameOver()
    {
        int score = CalculateTotalScore();
        scoreText.text = $"{LeanLocalization.GetTranslationText("Score")}: {score}";

        resultObject.score = score;
        resultObject.level = level;

        string json = JsonUtility.ToJson(resultObject);
        bridge.SendToJSJson(json);

        RecordStats();
        // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
        // GameOver();
    }

    public void RecordStats()
    {
        Dictionary<string, object> statData = new Dictionary<string, object>();

        statData.Add("numberOfWinsTotal", numberOfWinsTotal);
        statData.Add("numberOfLossesTotal", numberOfLossesTotal);

        // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];
        // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
    }
    public IEnumerator tutorialNext(int tutorialStepID, bool isIndicatorActive = true)
    {
        yield return new WaitForSeconds(2f);
        createTutorialElement(tutorialStepID, isIndicatorActive: isIndicatorActive);
    }
    private void SetStartLevel(int lastLevel)
    {
        level = lastLevel;
    }
}