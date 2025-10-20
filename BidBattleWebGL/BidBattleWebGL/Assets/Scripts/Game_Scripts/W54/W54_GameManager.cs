using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using W54;
using Random = UnityEngine.Random;
using System;
using W54_Managers;


public class W54_GameManager : MonoBehaviour
{

    public Bridge bridge;
    private int level = 1;
    private W54_LevelConfig[] levelCFG;
    private W54_EquationGenerator equationGenerator;
    [SerializeField] private W54_UIManager uiManager;
    [SerializeField] private TextMeshProUGUI leftQuestionText, rightQuestionText;
    private int currentCorrectAnswerIndex;

    private int currentCorrectAnswers = 0;
    private int currentWrongAnswers = 0;

    private float gameTime = 60f;
    private bool timerActive;
    private float flashInterval = 0.5f;
    private bool isFlashable = true;

    private int numberOfCorrectAnswers;
    private int numberOfWrongAnswers;
    private int correctAnswerStreak;
    private int successRate;

    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
    {
        { "0-numberOfSoldArts", "Number of sold arts" },
        { "1-longestStreak", "Longest Streak" },
        { "2-successRate", "Success Rate" },
    };

    // public eW54FxSoundStates eW54PlayerSoundState;
    public eW54EnvironmentSoundStates EW54EnvironmentSoundState;
    public W54_Constants W54Constants = new W54_Constants();

    private void Awake()
    {
        // initBaseOperations("Bid_Battle");
        // SetInGameTopbarTimerStatus(true);
        // SetStartLevel(gameScoreViewModel.level);
        //LocalizationKeys = W50_Constants.LocalizationKeys;
    }


    public void StartFromWebGL(int levelId)
    {

        level = levelId;
        Debug.Log("başlayacak level   " + level);
        equationGenerator = new W54_EquationGenerator();
        levelCFG = W54_LevelConfigs.levelConfigs;

        uiManager.OpenCurtains(() =>
       {
           AudioManager.instance.Play(SoundType.W54_CrowdNoise);

           uiManager.ControlQuestionPanelVisibility(true);
           AskQuestion();
           ControlGameTime(true);
       });
    }

    // public void Start()
    // {
    //     level = PlayerPrefs.GetInt("level", 1);
    //     Debug.Log("alınan level   " + level);
    //     equationGenerator = new W54_EquationGenerator();
    //     levelCFG = W54_LevelConfigs.levelConfigs;
    //     // UnityMessageManager.Instance.SendMessageToFlutter("loading_completed");

    //     uiManager.OpenCurtains(() =>
    //     {
    //         playEnvironmentSoundState(SoundType.W54_CrowdNoise, false, true, 0.3f, 1f);

    //         uiManager.ControlQuestionPanelVisibility(true);
    //         AskQuestion();
    //         ControlGameTime(true);
    //     });
    // }

    private void Update()
    {
        GameTimeCounter();
    }

    private void ControlGameTime(bool isActive)
    {
        timerActive = isActive;
    }

    private void GameTimeCounter()
    {
        if (timerActive)
        {
            gameTime -= Time.deltaTime;

            if (gameTime <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // PlayFx("Countdown", 0.7f, 1f);
                // FlashRed();
            }

            // SetTimerText(gameTime.ToString("F0"));

            if (gameTime < 0)
            {
                ControlGameTime(false);
                uiManager.ControlQuestionPanelVisibility(false);
                uiManager.CloseCurtains(() => EndGame());
            }
        }
    }

    private void EndGame()
    {

        //   PlayerPrefs.SetInt("correctCount", correctCount);
        //     PlayerPrefs.SetInt("wrongCount", wrongCount);
        //     PlayerPrefs.SetInt("levelId", levelId);



        // gameScoreViewModel.level = level;
        // gameScoreViewModel.score = levelCFG[level].levelScore;
        RecordStats();

        // transform.parent.gameObject.SetActive(false);
        // SceneManager.LoadSceneAsync("ScoreScene", LoadSceneMode.Additive);
        // onboardingManager = GameObject.Find("OnboardingManager").GetComponent<OnboardingManager>();
        // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
        // GameOver();
    }


    public void AskQuestion()
    {
        var levelConfig = levelCFG[level];

        var firstEquationType = levelConfig.firstEquationType;
        var secondEquationType = levelConfig.secondEquationType;
        var maxVal = levelConfig.answerMaxValue;

        var firstQuestion = equationGenerator.GenerateEquation(firstEquationType, maxVal);
        var secondQuestion = equationGenerator.GenerateEquation(secondEquationType, maxVal);

        if ((int)firstQuestion.equationAnswer == (int)secondQuestion.equationAnswer)
        {
            AskQuestion();
            return;
        }

        if (Mathf.Abs((int)firstQuestion.equationAnswer - (int)secondQuestion.equationAnswer) > 10)
        {
            AskQuestion();
            return;
        }

        var randPos = Random.Range(0, 2);
        if (randPos == 0)
        {
            leftQuestionText.text = firstQuestion.equationString;
            rightQuestionText.text = secondQuestion.equationString;

            currentCorrectAnswerIndex = firstQuestion.equationAnswer > secondQuestion.equationAnswer ? 0 : 1;
        }
        else
        {
            rightQuestionText.text = firstQuestion.equationString;
            leftQuestionText.text = secondQuestion.equationString;
            currentCorrectAnswerIndex = firstQuestion.equationAnswer > secondQuestion.equationAnswer ? 1 : 0;
        }

        uiManager.ResetCountdown();
        uiManager.SetCountdown(true);
    }

    public void QuestionAnswered(int buttonIndex)
    {
        Debug.LogWarning(buttonIndex);

        ControlAnswer(buttonIndex == currentCorrectAnswerIndex);
        bool isCorrect = buttonIndex == currentCorrectAnswerIndex;

        if (uiManager.GetCountdown() <= 0)
            isCorrect = false;

        uiManager.QuestionAnswered(buttonIndex, isCorrect);

        uiManager.SetCountdown(false);

        StartCoroutine(NewQuestionDelay());
    }

    private IEnumerator NewQuestionDelay()
    {
        //ControlGameTime(false);
        uiManager.ControlAnswerButtonInteractivity(false);

        yield return new WaitForSeconds(1);
        uiManager.ControlAnswerButtonInteractivity(true);
        AskQuestion();
        // ControlGameTime(true);
    }

    private void ControlAnswer(bool isAnswerCorrect)
    {
        if (isAnswerCorrect && uiManager.GetCountdown() > 0)
        {
            currentCorrectAnswers++;
            numberOfCorrectAnswers++;
            correctAnswerStreak++;
            if (currentCorrectAnswers >= levelCFG[level].levelUpCriteria * 4)
            {
                ChangeLevel(+1);
                currentCorrectAnswers = 0;
                currentWrongAnswers = 0;
            }
        }
        else
        {
            currentWrongAnswers++;
            numberOfWrongAnswers++;
            correctAnswerStreak = 0;
            if (currentWrongAnswers >= levelCFG[level].levelDownCriteria * 2)
            {
                ChangeLevel(-1);
                currentCorrectAnswers = 0;
                currentWrongAnswers = 0;
            }
        }

    }

    // private void FlashRed()
    // {
    //     // TMP_Text timerText = GetInGameTopbarTimer();

    //     Sequence redFlash = DOTween.Sequence();

    //     redFlash.Append(timerText.DOColor(Color.red, flashInterval))
    //             .SetEase(Ease.Linear)
    //             .Append(timerText.DOColor(Color.white, flashInterval))
    //             .SetEase(Ease.Linear)
    //             .SetLoops(6);

    //     redFlash.Play();
    // }

      [Serializable]
        public class ResultClass
        {
            public int level;
            public int score;

        }

    ResultClass resultObject = new ResultClass();

    public void RecordStats()
    {
        Dictionary<string, object> statData = new Dictionary<string, object>();

        statData.Add("numberOfSoldArts", numberOfCorrectAnswers);
        statData.Add("longestStreak", correctAnswerStreak);

        var rate = (int)(numberOfCorrectAnswers / (float)(numberOfCorrectAnswers + numberOfWrongAnswers) * 100f);
        statData.Add("successRate", rate);
        PlayerPrefs.SetInt("score", rate);
        level = Mathf.Clamp(level, 1, levelCFG.Length / 2);
        PlayerPrefs.SetInt("level", level);

        Debug.Log("score   " + PlayerPrefs.GetInt("score"));
        Debug.Log("level   " + PlayerPrefs.GetInt("level"));

        // WManagers.WRDB.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];


        resultObject.level = level;
        resultObject.score = rate;
        Debug.Log("skor   " + rate);
        string json = JsonUtility.ToJson(resultObject);
        bridge.SendToJSJson(json);



    }

    private void ChangeLevel(int change)
    {
        level += change;
        level = Mathf.Clamp(level, 1, levelCFG.Length / 2);
        PlayerPrefs.SetInt("level", level);
    }

    private void SetStartLevel(int lastLevel)
    {
        level = lastLevel;
    }
}