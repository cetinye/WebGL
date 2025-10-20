using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using Lean.Localization;

public class W59_LevelController : MonoBehaviour
{
    public int levelId;
    [SerializeField] private List<W59_LevelSO> levels;
    [SerializeField] private W59_LevelSO levelSO;
    public static W59_LevelSO LevelSO;
    private List<int> scores = new List<int>();

    [Space]
    [SerializeField] private int downCounter;
    [SerializeField] private int upCounter;
    [SerializeField] private int wrong;
    [SerializeField] private int correct;
    [SerializeField] private int vehiclesPassed;
    [SerializeField] private bool isLevelPassedBefore;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Animator levelAnim;
    [Space]

    [SerializeField] private W59_TrafficController trafficController;
    [SerializeField] private W59_LaneController laneController;
    [SerializeField] private W59_GameManager gameManager;

    private List<W59_Enums.VEHICLE_TYPE> availableTypes = new();
    private List<W59_Lane> lanes = new();

    private List<int> timeToSwitch = new List<int>();
    private float gameTime;
    private bool timerActive = true;
    [SerializeField] private TextMeshProUGUI correctPassText;
    [SerializeField] private TextMeshProUGUI wrongPassText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private float flashInterval;

    [SerializeField] private List<W59_AnswerButton> answerButtons = new();

    private int correctPassCount;
    private int wrongPassCount;
    private bool isFlashable = true;

    private int currentStreak;
    private int maxLevelWKeys;
    private int bestStreak;

    public void SetGameTime()
    {
        gameTime = 60f;
    }

    public void AssignLevelVariables()
    {
        maxLevelWKeys =levels.Count / 2;
        Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

        levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
        levelSO = levels[levelId - 1];
        LevelSO = levelSO;

        levelText.text = $"{LeanLocalization.GetTranslationText("Level")}: {levelId}";

        SetAvailableVehicleTypes();
        SelectRandomMomentsToSwitch();

        lanes = laneController.CreateLanes(levelSO, availableTypes);
        ControlButtons(lanes);
    }

    public void StartLevel()
    {
        trafficController.StartTraffic(levelSO, availableTypes, lanes);
    }

    private void ControlButtons(List<W59_Lane> createdLanes)
    {
        for (int i = 0; i < levelSO.totalNumOfLanes; i++)
        {
            answerButtons[i].assignedLane = createdLanes[i];
            answerButtons[i].gameObject.SetActive(true);
        }
    }


    private void SelectRandomMomentsToSwitch()
    {
        for (int i = 0; i < levelSO.gateSwitchCount; i++)
        {
            var randomTime = (int)Random.Range(10, gameTime - 10);

            foreach (var time in timeToSwitch)
            {
                if (Mathf.Abs(randomTime - time) < 10)
                {
                    randomTime = (int)Random.Range(10, gameTime - 10);
                }
            }
            timeToSwitch.Add(randomTime);
        }
    }

    private void SetAvailableVehicleTypes()
    {
        var availableCount = levelSO.totalCarTypes;

        for (int i = 0; i < availableCount; i++)
        {
            var type = RandomType();
            while (availableTypes.Contains(type))
            {
                type = RandomType();
            }
            availableTypes.Add(type);
        }
    }

    private W59_Enums.VEHICLE_TYPE RandomType()
    {
        W59_Enums.VEHICLE_TYPE[] allVehicleTypes =
            (W59_Enums.VEHICLE_TYPE[])Enum.GetValues(typeof(W59_Enums.VEHICLE_TYPE));
        int randomIndex = Random.Range(0, allVehicleTypes.Length);

        return allVehicleTypes[randomIndex];
    }

    public IEnumerator GameTimeCounter()
    {
        while (timerActive)
        {
            gameTime -= 1;
            CheckGateSwitch();
            timeText.text = gameTime.ToString("00");

            if (gameTime <= 5f && isFlashable)
            {
                isFlashable = false;
                // gameManager.PlayFx("Countdown", 0.7f, 1f);
                FlashRed();
            }

            if (gameTime == 0)
            {
                timerActive = false;
                gameTime = 0;
                gameManager.EndGame(correctPassCount, wrongPassCount, bestStreak);
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void FlashRed()
    {
        Sequence redFlash = DOTween.Sequence();

        redFlash.Append(timeText.DOColor(Color.red, flashInterval))
                .SetEase(Ease.Linear)
                .Append(timeText.DOColor(Color.white, flashInterval))
                .SetEase(Ease.Linear)
                .SetLoops(6);

        redFlash.Play();
    }

    private void UpdateAnswerCounter()
    {
        correctPassText.text = correctPassCount.ToString();
        wrongPassText.text = wrongPassCount.ToString();
    }

    private void DecideLevel()
    {
        downCounter = PlayerPrefs.GetInt("HighwayControl_DownCounter", 0);
        upCounter = PlayerPrefs.GetInt("HighwayControl_UpCounter", 0);

        if (vehiclesPassed >= levelSO.numOfVehiclesReqToPassLevel)
        {
            vehiclesPassed = 0;

            if (CalculateLevelScore() > levelSO.minScore)
            {
                upCounter++;
                if (upCounter >= 2)
                {
                    LevelUp();
                }
                PlayerPrefs.SetInt("HighwayControl_UpCounter", upCounter);
            }
            else
            {
                downCounter++;
                if (downCounter >= 2)
                {
                    LevelDown();
                }
                PlayerPrefs.SetInt("HighwayControl_DownCounter", downCounter);
            }
        }
        PlayerPrefs.SetInt("HighwayControl_DownCounter", downCounter);
    }

    private void LevelUp()
    {
        levelId = Mathf.Clamp(levelId + 1, 1, maxLevelWKeys);

        if (!isLevelPassedBefore)
        {
            isLevelPassedBefore = true;
        }

        downCounter = 0;
        wrong = 0;
        correct = 0;

        PlayerPrefs.SetInt("HighwayControl_DownCounter", downCounter);

        if (LevelSO != levels[levelId])
            LevelAnimation(true);

        LevelSO = levels[levelId];

        levelText.text = $"{LeanLocalization.GetTranslationText("Level")}: {levelId}";
    }

    private void LevelDown()
    {
        levelId = Mathf.Clamp(levelId - 1, 1, maxLevelWKeys);

        if (!isLevelPassedBefore)
        {
            isLevelPassedBefore = true;
        }

        downCounter = 0;
        wrong = 0;
        correct = 0;

        PlayerPrefs.SetInt("HighwayControl_DownCounter", downCounter);

        if (LevelSO != levels[levelId])
            LevelAnimation(false);

        LevelSO = levels[levelId];

        levelText.text = $"{LeanLocalization.GetTranslationText("Level")}: {levelId}";
    }

    public int CalculateLevelScore()
    {
        int levelScore = Mathf.CeilToInt((correct * 100) - (wrong * levelSO.penaltyScore));
        // Debug.Log($"Calculated level score: {levelScore} = ({correct} * 100) - ({wrong} * {LevelSO.penaltyPoints})");

        levelScore = Mathf.Clamp(levelScore, 0, LevelSO.maxInLevel);
        // Debug.Log($"Clamped level score to: {levelScore} (between 0 and {LevelSO.maxInLevel})");

        scoreText.text = $"Score: {levelScore}";
        // Debug.Log("Updated score text to: " + scoreText.text);

        scores.Add(Mathf.Clamp(Mathf.CeilToInt((float)levelScore / levelSO.maxInLevel * 1000), 0, 1000));

        return levelScore;
    }

    public int CalculateScore()
    {
        int numerator = (correctPassCount * 100) - (wrongPassCount * levelSO.penaltyScore);
        Debug.Log($"CalculateScore: correctPassCount = {correctPassCount}, wrongPassCount = {wrongPassCount}, penaltyPoints = {levelSO.penaltyScore}, numerator = {numerator}");
        int denominator = (correctPassCount + wrongPassCount) * 100;
        Debug.Log($"CalculateScore: correctPassCount = {correctPassCount}, wrongPassCount = {wrongPassCount}, denominator = {denominator}");
        float intermediateResult = (float)numerator / denominator;
        Debug.Log($"CalculateScore: numerator = {numerator}, denominator = {denominator}, intermediateResult = {intermediateResult}");
        int score = Mathf.CeilToInt(intermediateResult * 1000);
        Debug.Log($"CalculateScore: score = {score}");
        score = Mathf.Clamp(score, 0, 1000);
        Debug.Log($"CalculateScore: Clamped score to: {score} (between 0 and 1000)");

        return score;
    }

    public void LevelAnimation(bool isLevelUp)
    {
        if (!isLevelUp)
        {
            levelText.DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
        }

        levelAnim.SetTrigger("LevelAnim");
    }

    private void CheckGateSwitch()
    {
        if (timeToSwitch.Contains((int)gameTime))
        {
            laneController.SwitchGateTypes();
        }
    }

    public void CheckAnswer(W59_AnswerButton answerButton)
    {
        answerButton.assignedLane.Explode();

        var vehicle = answerButton.assignedLane.collidingVehicle;
        if (vehicle == null) return;

        if (vehicle.tagScanned) return;

        bool isCorrect;
        vehiclesPassed++;
        if (vehicle.GetType() == answerButton.assignedLane.acceptedType)
        {
            isCorrect = false;
            wrong++;
            wrongPassCount++;
            if (currentStreak > bestStreak)
            {
                bestStreak = currentStreak;
            }
            currentStreak = 0;

            Taptic.Failure();
            gameManager.playFxBySoundState(W59_Enums.eW59FxSoundStates.WRONG, 0.8f);
        }
        else
        {
            isCorrect = true;
            correct++;
            correctPassCount++;
            currentStreak++;
            Taptic.Success();
            gameManager.playFxBySoundState(W59_Enums.eW59FxSoundStates.CORRECT, 0.8f);
        }

        vehicle.SetVehicleAsScanned();
        UpdateAnswerCounter();
        answerButton.ShowTicket(isCorrect);
        gameManager.playFxBySoundState(W59_Enums.eW59FxSoundStates.TICKET, 0.2f);
        DecideLevel();
    }

    public void OnVehiclePass(bool isCorrectPass)
    {
        vehiclesPassed++;
        if (isCorrectPass)
        {
            correct++;
            correctPassCount++;
            Taptic.Success();
        }
        else
        {
            wrong++;
            wrongPassCount++;
            Taptic.Failure();
        }
        UpdateAnswerCounter();
        DecideLevel();
    }
}