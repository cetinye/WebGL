using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Lean.Localization;
using TMPro;
using Unity_CS;
using UnityEngine;
using UnityEngine.UI;
using W56;
using Random = UnityEngine.Random;

public class W56_MainController : MonoBehaviour
{
    public Bridge bridge;

    [SerializeField] private LeanLocalization leanLocalization;
    private int level = 0;
    [SerializeField] private List<W56_LevelSO> levels = new List<W56_LevelSO>();
    [SerializeField] private W56_LevelSO levelSO;
    private int score = 0;

    [Space]
    [SerializeField] private int downCounter;
    [SerializeField] private int correct;
    [SerializeField] private int wrong;
    [SerializeField] private int totalSwipes;
    [Space]

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] Transform leftBinContainer;
    [SerializeField] Transform rightBinContainer;

    [SerializeField] Transform leftThrowDestination;
    [SerializeField] Transform middleThrowDestination;
    [SerializeField] Transform rightThrowDestination;

    [Header("Left and Right Side Bins"), Space(5f)]
    [SerializeField]
    private W56_Bin binPrefab;

    [SerializeField] private List<RectTransform> leftBinPositions = new List<RectTransform>();
    [SerializeField] private List<RectTransform> rightBinPositions = new List<RectTransform>();

    [SerializeField] private List<MATERIAL_TYPE> availableTypes;
    [SerializeField] private List<Sprite> binSprites = new List<Sprite>();
    [SerializeField] private List<W56_Bin> leftSideBins = new List<W56_Bin>();
    [SerializeField] private List<W56_Bin> rightSideBins = new List<W56_Bin>();

    [SerializeField] private GameObject warningPanel;

    [Header("Materials"), Space(5f)] private List<MATERIAL_TYPE> usedTypes = new List<MATERIAL_TYPE>();

    [System.Serializable]
    public class MaterialSpritePair
    {
        public MATERIAL_TYPE type;
        public Sprite[] sprites;
    }

    [Serializable]
    public class ResultClass
    {
        public int level;
        public int score;

    }

    ResultClass resultObject = new ResultClass();

    public MaterialSpritePair[] materialsPairs = new MaterialSpritePair[4];
    public W56_Material materialInstance;
    public List<W56_Material> createdMaterials = new List<W56_Material>();
    public Transform queueContainer;
    private List<W56_Material> objectPool = new List<W56_Material>();

    private int objectPoolSize = 50;
    private Vector3[] queuePositions;

    private List<int> switchIndexes = new List<int>();
    public GameObject switchWarningContainer;
    public TextMeshProUGUI switchWarningCounterText;

    public GameObject leftBinSparkContainer;
    public GameObject rightBinSparkContainer;

    [Header("Game-Flow"), Space(10)]
    private int answeredCount;
    private int streakCounter;
    private int bestStreak;
    private int currentNumberOfCorrectAnswers;
    private int currentNumberOfWrongAnswers;

    private int numberOfCorrectAnswers;
    private int numberOfWrongAnswers;

    [SerializeField] private TextMeshProUGUI streakText;
    public GameObject streakSparkContainer;
    public GameObject streakContainer;
    public Transform targetCircleBG;

    private float gameTime;
    private bool timerActive;

    //Countdown
    private float countdownTime;
    [SerializeField] private Image countdownFill;

    private bool isCountdownOn = false;
    private float countdownTimer = 5f;
    private float flashInterval = 0.5f;
    private bool isFlashable = true;

    [Header("Witmina-Spesific"), Space(10)]
    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
    {
        { "0-bestStreak", "Longest correct answer streak" },
        { "1-correctCount", "Number of correct answers" },
        { "2-successRate", "Success rate" },
    };

    public eW56FxSoundStates eW56PlayerSoundState;
    public W56_Constants W56Constants = new W56_Constants();
    private int maxLevelWKeys;

    private void Awake()
    {
        leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

        maxLevelWKeys = levels.Count;
        Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

        gameTime = 60;
    }

    void AssignLevelVariables()
    {
        level = Mathf.Clamp(level, 1, maxLevelWKeys);
        levelSO = levels[level - 1];

        levelText.text = "Level: " + level;

        countdownTime = levelSO.timePerQuestion;
        countdownTimer = countdownTime;
    }

    private void Update()
    {
        GameTimeCounter();
        Countdown();

#if UNITY_WEBGL

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnRightButtonClicked();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnLeftButtonClicked();
        }

#endif
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

        AssignLevelVariables();

        SetStartBins();
        CreateQueuePositions();
        CreateObjectPool();
        CreateQueue();

        timerActive = true;
        ResetCountdown();
        SetCountdown(true);
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
                FlashRed();
            }

            // SetTimerText(gameTime.ToString("F0"));

            if (gameTime <= 0)
            {
                timerActive = false;
                gameTime = 0;
                EndGame();
            }

        }
    }

    private void RollSwitchChance()
    {
        if (switchIndexes.Count != 0) return;

        int roll = Random.Range(1, 101);
        if (roll <= levelSO.switchChance)
        {
            switchIndexes.Clear();
            switchIndexes.Add(4);
        }
    }

    private void CheckSwitch()
    {
        if (switchIndexes.Count == 0) return;
        if (switchIndexes[0] - answeredCount < 4)
        {
            switchWarningContainer.SetActive(true);
            switchWarningCounterText.text = (switchIndexes[0] - answeredCount).ToString();
            switchWarningContainer.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.2f, 1, 0.25f);

            if (switchIndexes[0] != answeredCount) return;

            MoveBins();
            switchIndexes.RemoveAt(0);
            switchWarningContainer.SetActive(false);
        }
    }

    private void CreateQueuePositions()
    {
        queuePositions = new Vector3[objectPoolSize];
        queuePositions[0] = queueContainer.transform.position;

        for (int i = 1; i < objectPoolSize; i++)
        {
            queuePositions[i] = queuePositions[i - 1] + new Vector3(0, 400 - (i * 30), 500 - (i * 10));
        }
    }

    private void CreateObjectPool()
    {
        for (int i = 0; i < objectPoolSize; i++)
        {
            W56_Material obj = Instantiate(materialInstance, Vector3.zero, Quaternion.identity,
                queueContainer.transform);
            obj.gameObject.SetActive(false);
            obj.transform.position = queuePositions[i];
            objectPool.Add(obj);
        }
    }

    private void CreateQueue()
    {
        for (int i = 0; i < 15; i++)
        {
            AddToQueue();
        }
        targetCircleBG.DOLocalRotate(new Vector3(0, 0, 360), 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

    public void AddToQueue()
    {
        var rand = usedTypes[Random.Range(0, usedTypes.Count)];
        foreach (var pair in materialsPairs)
        {
            if (rand == pair.type)
            {
                var selectedSprite = pair.sprites._RandomItem();
                var newMat = GetObjectFromPool();
                newMat.type = rand;
                newMat.SetSprite(selectedSprite);
                newMat.transform.SetAsFirstSibling();
                newMat.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15f, 16f));
                newMat.transform.localPosition = queuePositions[createdMaterials.Count];
                newMat.gameObject.SetActive(true);
                createdMaterials.Add(newMat);
            }
        }
    }

    W56_Material GetObjectFromPool()
    {
        foreach (W56_Material obj in objectPool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                return obj;
            }
        }

        W56_Material newObj = Instantiate(materialInstance, Vector3.zero, Quaternion.identity,
            queueContainer.transform);
        newObj.gameObject.SetActive(false);
        objectPool.Add(newObj);

        return newObj;
    }

    private void SetStartBins()
    {
        leftSideBins.Add(CreateNewBin(GetAvailableBin(), SIDE.LEFT, leftBinPositions[0], 0));
        rightSideBins.Add(CreateNewBin(GetAvailableBin(), SIDE.RIGHT, rightBinPositions[0], 0));

        var numberOfTypes = levelSO.numOfTypes - 2;
        for (var i = 0; i < numberOfTypes; i++)
        {
            AddBin();
        }
    }

    private MATERIAL_TYPE GetAvailableBin()
    {
        var index = Random.Range(0, availableTypes.Count);
        var bin = availableTypes[index];
        usedTypes.Add(bin);
        availableTypes.RemoveAt(index);

        return bin;
    }

    private W56_Bin CreateNewBin(MATERIAL_TYPE type, SIDE side, RectTransform rectTransform, int posIndex)
    {
        var parent = side == SIDE.LEFT ? leftBinContainer : rightBinContainer;
        W56_Bin bin = Instantiate(binPrefab, parent.position, Quaternion.identity, parent);
        bin.type = type;
        bin.SetRecttransform(rectTransform);
        bin.SetPosIndex(posIndex);
        bin.SetSprite(binSprites[(int)type]);

        return bin;
    }

    public void AddBin()
    {
        SIDE randSide;

        if (leftSideBins.Count == rightSideBins.Count)
        {
            randSide = (SIDE)Random.Range(0, 2);
        }
        else
        {
            randSide = (leftSideBins.Count < rightSideBins.Count) ? SIDE.LEFT : SIDE.RIGHT;
        }

        if (randSide == SIDE.LEFT)
        {
            leftSideBins.Add(CreateNewBin(GetAvailableBin(), SIDE.LEFT, leftBinPositions[1], 1));
        }
        else
        {
            rightSideBins.Add(CreateNewBin(GetAvailableBin(), SIDE.RIGHT, rightBinPositions[1], 1));
        }
    }

    public void MoveBins()
    {
        SetWarningPanelState(true);

        AudioManager.instance.PlayOneShot(SoundType.Switch);

        W56_Bin randFromLeft = leftSideBins._RandomItem();
        W56_Bin randFromRight = rightSideBins._RandomItem();

        int leftPosIndex = randFromLeft.GetPosIndex();
        int rightPosIndex = randFromRight.GetPosIndex();

        leftSideBins.Remove(randFromLeft);
        leftSideBins.Add(randFromRight);

        rightSideBins.Remove(randFromRight);
        rightSideBins.Add(randFromLeft);

        randFromLeft.transform.SetParent(rightBinContainer, true);
        randFromRight.transform.SetParent(leftBinContainer, true);

        randFromLeft.MoveTo(rightBinPositions[rightPosIndex], 0.2f).OnComplete(() =>
        {
            randFromLeft.SetPosIndex(rightPosIndex);
            randFromLeft.SetRecttransform(rightBinPositions[rightPosIndex]);
        });

        randFromRight.MoveTo(leftBinPositions[leftPosIndex], 0.2f).OnComplete(() =>
        {
            randFromRight.SetPosIndex(leftPosIndex);
            randFromRight.SetRecttransform(leftBinPositions[leftPosIndex]);
            SetWarningPanelState(false);
        });

        Taptic.Warning();
    }

    public void SetWarningPanelState(bool state)
    {
        warningPanel.SetActive(state);
    }

    public void OnLeftButtonClicked()
    {
        if (createdMaterials.Count <= 0) return;

        SetCountdown(false);
        answeredCount++;
        var firstOne = createdMaterials[0];
        createdMaterials.RemoveAt(0);
        firstOne.transform.SetAsLastSibling();

        ThrowToLeft(firstOne);
        MoveQueueForward();
        CheckSwitch();
        ControlLevel();
    }

    public void OnRightButtonClicked()
    {
        if (createdMaterials.Count <= 0) return;

        SetCountdown(false);
        answeredCount++;
        var firstOne = createdMaterials[0];
        createdMaterials.RemoveAt(0);
        firstOne.transform.SetAsLastSibling();

        ThrowToRight(firstOne);
        MoveQueueForward();
        CheckSwitch();
        ControlLevel();
    }

    private void ThrowToLeft(W56_Material firstOne)
    {
        bool isCorrect;
        Vector3 throwDestination;

        if (GetCountdown() > 0)
        {
            isCorrect = CheckAnswer(SIDE.LEFT, firstOne);
            throwDestination = isCorrect ? leftThrowDestination.position : middleThrowDestination.position;
        }
        else
        {
            isCorrect = false;
            throwDestination = middleThrowDestination.position;
            OnAnswerWrong();
        }

        if (isCorrect)
            AudioManager.instance.PlayOneShot(SoundType.Correct);
        else
            AudioManager.instance.PlayOneShot(SoundType.Wrong);

        firstOne.transform.DOScale(0.6f, 0.65f);
        firstOne.transform.DOMove(throwDestination, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            leftBinSparkContainer.SetActive(isCorrect);
            firstOne.image.DOFade(0f, 0.15f).OnComplete(() =>
            {
                leftBinSparkContainer.SetActive(false);
                firstOne.gameObject.SetActive(false);
                firstOne.image.DOFade(1f, 0f);
                firstOne.transform.DOScale(1f, 0f);
            });
            AddToQueue();
        });
    }

    private void ThrowToRight(W56_Material firstOne)
    {
        var isCorrect = CheckAnswer(SIDE.RIGHT, firstOne);

        if (isCorrect)
            AudioManager.instance.PlayOneShot(SoundType.Correct);
        else
            AudioManager.instance.PlayOneShot(SoundType.Wrong);

        var throwDestination = isCorrect ? rightThrowDestination.position : middleThrowDestination.position;
        firstOne.transform.DOScale(0.6f, 0.65f);
        firstOne.transform.DOMove(throwDestination, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            rightBinSparkContainer.SetActive(isCorrect);
            firstOne.image.DOFade(0f, 0.15f).OnComplete(() =>
            {
                rightBinSparkContainer.SetActive(false);
                firstOne.gameObject.SetActive(false);
                firstOne.image.DOFade(1f, 0f);
                firstOne.transform.DOScale(1f, 0f);
            });
            AddToQueue();
        });
    }

    private void MoveQueueForward()
    {
        for (var i = 0; i < createdMaterials.Count; i++)
        {
            createdMaterials[i].transform.DOLocalMove(queuePositions[i], 0.25f).SetEase(Ease.InOutCubic);
        }

        ResetCountdown();
        SetCountdown(true);
    }

    private bool CheckAnswer(SIDE clickedSide, W56_Material material)
    {
        switch (clickedSide)
        {
            case SIDE.LEFT:
                {
                    if (leftSideBins.Any(leftSideBin => leftSideBin.type == material.type) && GetCountdown() > 0)
                    {
                        leftThrowDestination.position = leftSideBins.Find(leftSideBin => leftSideBin.type == material.type).transform.position;
                        OnAnswerCorrect();
                        return true;
                    }

                    break;
                }
            case SIDE.RIGHT:
                {
                    if (rightSideBins.Any(rightSideBin => rightSideBin.type == material.type) && GetCountdown() > 0)
                    {
                        rightThrowDestination.position = rightSideBins.Find(rightSideBin => rightSideBin.type == material.type).transform.position;
                        OnAnswerCorrect();
                        return true;
                    }

                    break;
                }
        }

        OnAnswerWrong();
        return false;
    }

    private void OnAnswerCorrect()
    {
        Taptic.Success();
        currentNumberOfCorrectAnswers++;
        numberOfCorrectAnswers++;
        streakCounter++;
        correct++;
        totalSwipes++;
        UpdateStreakCounterText();
    }

    private void OnAnswerWrong()
    {
        Taptic.Failure();
        streakCounter = 0;
        currentNumberOfWrongAnswers++;
        numberOfWrongAnswers++;
        wrong++;
        totalSwipes++;
        UpdateStreakCounterText();

        downCounter = PlayerPrefs.GetInt("StockItUp_DownCounter", 0);
        downCounter++;
        PlayerPrefs.SetInt("StockItUp_DownCounter", downCounter);
    }

    private void UpdateStreakCounterText()
    {
        if (streakCounter > bestStreak)
        {
            bestStreak = streakCounter;
        }

        streakContainer.SetActive(streakCounter != 0);
        streakSparkContainer.SetActive(streakCounter > 10);

        streakText.text = "x" + streakCounter;
        var fontSize = Mathf.Clamp(60 + streakCounter, 60, 120);
        streakText.fontSize = fontSize;
    }

    private void ControlLevel()
    {
        downCounter = PlayerPrefs.GetInt("StockItUp_DownCounter", 0);

        var questionCount = levelSO.levelUpCriteria * 2;
        if (answeredCount == questionCount)
        {
            if (CalculateLevelScore() >= levelSO.minReqScore)
            {
                LevelUp();
            }
            else
            {
                downCounter++;
                if (downCounter >= 2)
                {
                    LevelDown();
                }
                PlayerPrefs.SetInt("StockItUp_DownCounter", downCounter);
            }

            var numberOfTypes = levelSO.numOfTypes - usedTypes.Count;
            for (var i = 0; i < numberOfTypes; i++)
            {
                AddBin();
            }

            answeredCount = 0;
            currentNumberOfCorrectAnswers = 0;
            currentNumberOfWrongAnswers = 0;

            AssignLevelVariables();
            RollSwitchChance();

            PlayerPrefs.SetInt("StockItUp_DownCounter", downCounter);
        }
    }

    private void LevelUp()
    {
        level++;
        level = Mathf.Clamp(level, 1, maxLevelWKeys);
        levelSO = levels[level - 1];
        levelText.text = "Level: " + level;

        downCounter = 0;
        wrong = 0;
        correct = 0;
        PlayerPrefs.SetInt("StockItUp_DownCounter", downCounter);
    }

    private void LevelDown()
    {
        level--;
        level = Mathf.Clamp(level, 1, maxLevelWKeys);
        levelSO = levels[level - 1];
        levelText.text = "Level: " + level;

        downCounter = 0;
        wrong = 0;
        correct = 0;
        PlayerPrefs.SetInt("StockItUp_DownCounter", downCounter);
    }

    public int CalculateLevelScore()
    {
        int levelScore = Mathf.CeilToInt((correct * 100) - (wrong * levelSO.penaltyPoints));
        levelScore = Mathf.Clamp(levelScore, 0, levelSO.maxInLevel);
        scoreText.text = $"Score: {levelScore}";

        return levelScore;
    }

    public int CalculateTotalScore()
    {
        int maxInGame = totalSwipes * 100;
        score = Mathf.CeilToInt((numberOfCorrectAnswers * 100) - (numberOfWrongAnswers * levelSO.penaltyPoints));
        score = Mathf.Clamp(Mathf.CeilToInt((float)score / maxInGame * 1000), 0, 1000);

        return score;
    }

    public void RecordStats()
    {
        Dictionary<string, object> statData = new Dictionary<string, object>();

        statData.Add("bestStreak", bestStreak);
        statData.Add("correctCount", numberOfCorrectAnswers);
        var rate = (int)(numberOfCorrectAnswers / (float)(numberOfCorrectAnswers + numberOfWrongAnswers) * 100f);
        statData.Add("successRate", rate);

        // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];
        // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
    }

    private void EndGame()
    {
        resultObject.level = level;
        resultObject.score = CalculateTotalScore();
        string json = JsonUtility.ToJson(resultObject);
        Debug.LogWarning(json);
        bridge.SendToJSJson(json);

        RecordStats();
    }

    private void SetStartLevel(int lastLevel)
    {
        level = lastLevel;
    }

    private void Countdown()
    {
        if (isCountdownOn)
        {
            //timer continue if game is playing
            if (countdownTimer > 0)
            {
                countdownTimer -= Time.deltaTime;
                countdownFill.fillAmount = countdownTimer / countdownTime;
            }
            //stop timer if time ran out
            else if (countdownTimer < 0)
            {
                isCountdownOn = false;
                countdownTimer = 0;
                OnLeftButtonClicked();
            }
        }
    }

    public void ResetCountdown()
    {
        countdownTimer = countdownTime;
        countdownFill.fillAmount = 1;
    }

    public void SetCountdown(bool state)
    {
        isCountdownOn = state;
    }

    public float GetCountdown()
    {
        return countdownTimer;
    }

    private void FlashRed()
    {
        // TMP_Text timerText = GetInGameTopbarTimer();

        // Sequence redFlash = DOTween.Sequence();
        //
        // redFlash.Append(timerText.DOColor(Color.red, flashInterval))
        //         .SetEase(Ease.Linear)
        //         .Append(timerText.DOColor(Color.white, flashInterval))
        //         .SetEase(Ease.Linear)
        //         .SetLoops(6);
        //
        // redFlash.Play();
    }
}