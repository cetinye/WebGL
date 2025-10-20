using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Lean.Localization;
using ShiftAndLift;
using UnityEngine;
using W3_Scripts;
using Random = UnityEngine.Random;

public class W3_GameManager : MonoBehaviour
{
    public Bridge bridge;
    //FIELD
    [SerializeField] private LeanLocalization leanLocalization;
    [SerializeField] W3_Shape _shape;
    [SerializeField] W3_ShapeCard _shapeCard;
    [SerializeField] W3_UIManager _uiManager;
    [SerializeField] Transform _shapeParent;
    [SerializeField] Transform _shapeCardParent;
    [SerializeField] Canvas canvas;
    [SerializeField] List<GameObject> truckBoxes = new List<GameObject>();
    [SerializeField] int truckBoxIndex = 0;

    private readonly List<W3_Level> levels = new List<W3_Level>();
    public W3_Conveyor conveyor;
    public List<Sprite> shapes = new List<Sprite>();
    public List<W3_ShapeCard> instantiatedShapeCards = new List<W3_ShapeCard>();
    public List<GameObject> resultObjects = new List<GameObject>();
    public List<GameObject> instantiatedResultObjects = new List<GameObject>();
    public List<Transform> shapePositions = new List<Transform>();
    public List<int> tempAppearedIds = new List<int>();
    public List<int> appearedIds = new List<int>();
    public List<int> nonAppearedIds = new List<int>();
    public List<int> idsOnScreen = new List<int>();
    public List<GameObject> cardsToBeShown = new List<GameObject>();

    private readonly int _maxLevel = 10;
    [SerializeField] private int _level = 1;
    private int _score;
    private int _correctsCount, _wrongsCount;
    private int correct, wrong;
    private List<int> scores = new List<int>();
    private int upCounter;
    private int downCounter;
    private bool _isLevelEnd = true;
    public float _generalTime, _levelTime = 7;
    public bool _levelTimerStatus;
    public GameObject _popEffect;
    public Transform _popsParent;
    public Transform _resultObjectParent;
    public int timesPlayed = 1;

    private Vector2 _shapeCardSize;
    private float _shapeCardYDistance;
    private Vector3 _shapeCardStartPos;
    public int _shapeCount;
    private int _wrongShapeCount;
    private Vector3 _shapeCardMidPos;
    public float _moveSpeed;
    public float _productShowTime;
    private int _levelUpCriteria;
    private int _levelDownCriteria;
    private int _playLimit;
    private int _pointsPerCorrect;
    private int _maxInGame;
    private int _minimumScore;
    private int _penaltyPoints;

    [SerializeField] private GameObject backgrounds;
    [SerializeField] private float timeToMoveBG;
    [SerializeField] private GameObject forklift;
    [SerializeField] private GameObject forkliftWheelF, forkliftWheelB;
    [SerializeField] private Transform forkliftSpawnPoint;

    // level change action 
    private event Action<eGameLevelChangeStatus> onChangeLevelEvent;

    public enum eGameLevelChangeStatus
    {
        LEVELUP,
        LEVELDOWN,
    }

    // camera reference
    public Camera cameraReference;
    private int[] levelScores = new[] { 0, 75, 150, 250, 375, 460, 580, 665, 770, 890, 1000 };
    private List<int> roundScores = new List<int>();

    //stats
    public int numberOfCorrectAnswers;
    public int numberOfIncorrectAnswers;
    public int bestStreakCorrectAnswers;
    public int numberOfCorrectAnswersToThreeOptions;
    public int numberOfCorrectAnswersToFourOptions;
    public int numberOfCorrectAnswersToFiveOptions;
    public int numberOfCorrectAnswersToSixOptions;
    public int numberOfCorrectAnswersToSevenOptions;

    [SerializeField] private List<W3_SO> levelList = new List<W3_SO>();
    [SerializeField] private W3_SO levelSO;

    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
    {
        {"0-numberOfCorrectAnswers", "Given correct answers "},
        {"1-numberOfIncorrectAnswers", "Given incorrect answers "},
        {"2-bestStreakCorrectAnswers", "Best streak of correct answers "},
        {"3-numberOfCorrectAnswersToThreeOptions", "Number of correct answers to three option questions"},
        {"4-numberOfCorrectAnswersToFourOptions", "Number of correct answers to four option questions"},
        {"5-numberOfCorrectAnswersToFiveOptions", "Number of correct answers to five option questions"},
        {"6-numberOfCorrectAnswersToSixOptions", "Number of correct answers to six option questions"},
        {"7-numberOfCorrectAnswersToSevenOptions", "Number of correct answers to seven option questions"},
    };

    private int maxLevelWKeys;

    public int currentStreakCorrectAnswers;
    // MAIN
    void Awake()
    {
        leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

        _generalTime = 60;

        maxLevelWKeys = levelList.Count;
        Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);
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

        StartLevel();

        W3_AudioManager.instance.Play("Background");
    }

    public void Start()
    {
        onChangeLevelEvent += ChangeLevelByAction;
    }

    void Update()
    {
        Countdown();
    }

    private Vector2 GetShapeCardSize(int shapeCount)
    {
        Vector3 shapeCardStartPos = new Vector3(0f, -6f, -1f);
        Vector3 shapeCardMidPos = new Vector3(0f, 3f, -1f);

        Vector2 shapeCardSize = new Vector2(3f, 1.4f);

        switch (shapeCount)
        {
            case 5:
                shapeCardMidPos.y = 4.138f;
                break;
            case 6:
                shapeCardMidPos.y = 4.338f;
                break;
            case 7:
                shapeCardMidPos.y = 4.478f;
                shapeCardSize.x = 2.5f;
                shapeCardSize.y = 1f;
                break;
        }

        return shapeCardSize;
    }

    public void StartLevel()
    {
        AssignLevelVariables();
        _uiManager.UpdateAttemptTxt();
        ClearLists();
        SetVariables();
        StartCoroutine(GetShapes());
    }

    public bool IsPlayLimitReached()
    {
        timesPlayed++;
        if (timesPlayed < _playLimit + 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void AssignLevelVariables()
    {
        _level = Mathf.Clamp(_level, 1, maxLevelWKeys);
        levelSO = levelList[_level - 1];

        _uiManager.SetLeveLText(_level);
    }

    private void SetVariables()
    {
        _correctsCount = 0;
        _wrongsCount = 0;
        _shapeCount = levelSO.totalBoxesShown;
        _wrongShapeCount = levelSO.totalDisplayedFakeBoxes;
        _shapeCardSize = GetShapeCardSize(_shapeCount);
        _moveSpeed = levelSO.beltSpeed;
        _levelTime = levelSO.timeLimitPerQuestionSolve;
        _productShowTime = levelSO.displayTimeOfEachProduct;
        _levelUpCriteria = levelSO.levelUpCriteria;
        _levelDownCriteria = levelSO.levelDownCriteria;
        _playLimit = levelSO.totalRoundsToPlay;
        _pointsPerCorrect = levelSO.pointsPerCorrect;
        _maxInGame = levelSO.maxInGame;
        _minimumScore = levelSO.minimumScore;
        _penaltyPoints = levelSO.penaltyPoints;

        forklift.SetActive(false);
        CloseTruckBoxes();
        conveyor.gameObject.SetActive(true);
    }

    private void DestroyGameObjects()
    {
        if (instantiatedShapeCards.Count > 0)
        {
            for (int i = 0; i < instantiatedShapeCards.Count; i++)
            {
                Destroy(instantiatedShapeCards[i].gameObject);
            }

            instantiatedShapeCards.Clear();
        }

        if (instantiatedResultObjects.Count > 0)
        {
            for (int i = 0; i < instantiatedResultObjects.Count; i++)
            {
                Destroy(instantiatedResultObjects[i]);
            }

            instantiatedResultObjects.Clear();
        }
    }

    private void ClearLists()
    {
        appearedIds.Clear();
        nonAppearedIds.Clear();
        idsOnScreen.Clear();
    }

    /// <summary>
    /// Instantiate the shapes that will come one after the other.
    /// </summary>
    private IEnumerator GetShapes()
    {
        W3_AudioManager.instance.PlayOneShot("Conveyor");

        _uiManager.ChangeTimerVisibility(false);
        yield return new WaitForSeconds(1f);
        DestroyGameObjects();

        for (int i = 0; i < _shapeCount; i++)
        {
            W3_Shape shape = Instantiate(_shape, _shapeParent);
            shape._gameManager = this;
            int range;
            // Aynı shape gelmemesi için
            do
            {
                range = Random.Range(0, shapes.Count);
            } while (tempAppearedIds.Contains(range));

            shape.SetSprite(shapes[range]);
            shape._id = range;
            tempAppearedIds.Add(shape._id);

            yield return new WaitUntil(() => shape._isModelExit == true);
        }

        W3_AudioManager.instance.FadeOutSound("Conveyor", 1f);
        yield return new WaitForSeconds(1f);
        cardsToBeShown.Clear();
        StartCoroutine(ChangeSceneToTruck());
    }

    /// <summary>
    /// Instantiates the shapeCards
    /// </summary>
    /// <change>
    /// change on 23.11.2023 to fix timer starting while cards are still in animation
    /// commented out TimerOn(True) added sequence.onComplete = () => TimerOn(true);
    /// </change>
    private void GetShapeCards()
    {
        for (int i = 0; i < _shapeCount; i++)
        {
            W3_ShapeCard shapeCard = Instantiate(_shapeCard, _shapeCardStartPos, Quaternion.identity, forklift.transform);
            shapeCard.transform.localScale = Vector3.one;
            instantiatedShapeCards.Add(shapeCard);
            shapeCard._gameManager = this;
            SetShapeCard(shapeCard);
        }

        cardsToBeShown.Shuffle();
        _shapeCardMidPos = forkliftSpawnPoint.localPosition;

        for (int i = 0; i < cardsToBeShown.Count; i++)
        {
            cardsToBeShown[i].transform.localPosition = new Vector3(cardsToBeShown[i].transform.localPosition.x, _shapeCardMidPos.y, -1f);
            cardsToBeShown[i].GetComponent<W3_ShapeCard>()._basePosition = cardsToBeShown[i].transform.localPosition;
            _shapeCardMidPos.y += 1.8f;
        }

        tempAppearedIds.Clear();
    }

    /// <summary>
    /// Sets shapeCards' id,sprite,spriteSize.
    /// </summary>
    private void SetShapeCard(W3_ShapeCard shapeCard)
    {
        int value, shapeId;

        if (_wrongShapeCount > 0)
        {
            value = 2;
            _wrongShapeCount--;
        }
        else
        {
            value = 1;
        }

        // 1 ise görülen, 2 ise görülmeyen şekiller arasından seçilir.
        if (value == 1)
        {
            value = Random.Range(0, tempAppearedIds.Count);

            shapeId = tempAppearedIds[value];
            appearedIds.Add(shapeId);
            tempAppearedIds.Remove(shapeId);
        }
        else
        {
            do
            {
                value = Random.Range(0, shapes.Count);
            } while (nonAppearedIds.Contains(value) || appearedIds.Contains(value) ||
                     tempAppearedIds.Contains(value));

            shapeId = value;
            nonAppearedIds.Add(shapeId);
        }

        idsOnScreen.Add(shapeId);
        shapeCard.SetShapeCard(shapeId, shapes[shapeId], _shapeCardSize);
        cardsToBeShown.Add(shapeCard.gameObject);
    }

    /// <summary>
    /// Increases or decreases the score by given answer.
    /// </summary>
    public void IncDecScore(bool isIncreased)
    {
        if (isIncreased)
        {
            W3_AudioManager.instance.PlayOneShot("Correct");
            _uiManager.ShowFeedback(true);
            _correctsCount++;
            numberOfCorrectAnswers++;
            currentStreakCorrectAnswers++;
            correct++;

            switch (_shapeCount)
            {
                case 3:
                    numberOfCorrectAnswersToThreeOptions++;
                    break;
                case 4:
                    numberOfCorrectAnswersToFourOptions++;
                    break;
                case 5:
                    numberOfCorrectAnswersToFiveOptions++;
                    break;
                case 6:
                    numberOfCorrectAnswersToSixOptions++;
                    break;
                case 7:
                    numberOfCorrectAnswersToSevenOptions++;
                    break;
                default:
                    break;
            }
        }
        else
        {
            W3_AudioManager.instance.PlayOneShot("Wrong");
            _uiManager.ShowFeedback(false);
            _wrongsCount++;
            numberOfIncorrectAnswers++;
            wrong++;

            if (currentStreakCorrectAnswers > bestStreakCorrectAnswers)
            {
                bestStreakCorrectAnswers = currentStreakCorrectAnswers;
            }
            currentStreakCorrectAnswers = 0;
        }
    }

    private int CalculateScore()
    {
        int score = Mathf.Clamp((correct * levelSO.pointsPerCorrect) - (wrong * levelSO.penaltyPoints), 0, levelSO.maxInGame);
        scores.Add(Mathf.Clamp(Mathf.CeilToInt((float)score / levelSO.maxInGame * 1000), 0, 1000));
        return score;
    }

    private int CalculateTotalScore()
    {
        int totalScore = 0;

        for (int i = 0; i < scores.Count; i++)
        {
            totalScore += scores[i];
        }

        totalScore /= scores.Count;
        totalScore = Mathf.Clamp(totalScore, 0, 1000);
        return totalScore;
    }

    private void CheckResult()
    {
        int score = CalculateScore();
        Debug.LogWarning("Score: " + score);

        if (score >= levelSO.minimumScore)
        {
            LevelUp(true);
        }
        else
        {
            int downCounter = PlayerPrefs.GetInt("W3_DownCounter", 0);
            if (++downCounter >= levelSO.levelDownCriteria)
            {
                LevelUp(false);
                downCounter = 0;
            }
            PlayerPrefs.SetInt("W3_DownCounter", downCounter);
        }

        correct = 0;
        wrong = 0;

        _isLevelEnd = true;
    }

    /// <summary>
    /// Timers start to countdown.
    /// </summary>
    private void Countdown()
    {
        if (!_levelTimerStatus) return;
        _levelTime -= Time.deltaTime;

        CheckTimers();
    }

    /// <summary>
    /// Check the timers to end level or game.
    /// </summary>
    private void CheckTimers()
    {
        if (_levelTime <= 0)
        {
            _levelTimerStatus = false;
            W3_AudioManager.instance.PlayOneShot("TimesUp");
            LevelUp(false);
            _isLevelEnd = true;
            ChangeSceneToStorage();
        }
    }

    private void LevelUp(bool isUp)
    {
        if (isUp)
        {
            onChangeLevelEvent(eGameLevelChangeStatus.LEVELUP);
        }
        else
        {
            onChangeLevelEvent(eGameLevelChangeStatus.LEVELDOWN);
        }
    }

    [Serializable]
    public class ResultClass
    {
        public int level;
        public int score;

    }

    ResultClass resultObject = new ResultClass();

    private void GameOver()
    {
        int score = CalculateTotalScore();

        resultObject.level = _level;
        resultObject.score = Mathf.Clamp(score, 0, 1000);
        Debug.Log("skor   " + score);
        string json = JsonUtility.ToJson(resultObject);
        bridge.SendToJSJson(json);

        RecordStats();

        // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
        // base.GameOver();
    }

    public void RecordStats()
    {
        Dictionary<string, object> statData = new Dictionary<string, object>();

        statData.Add("numberOfCorrectAnswers", numberOfCorrectAnswers);
        statData.Add("numberOfIncorrectAnswers", numberOfIncorrectAnswers);
        statData.Add("bestStreakCorrectAnswers", bestStreakCorrectAnswers);
        statData.Add("numberOfCorrectAnswersToThreeOptions", numberOfCorrectAnswersToThreeOptions);
        statData.Add("numberOfCorrectAnswersToFourOptions", numberOfCorrectAnswersToFourOptions);
        statData.Add("numberOfCorrectAnswersToFiveOptions", numberOfCorrectAnswersToFiveOptions);
        statData.Add("numberOfCorrectAnswersToSixOptions", numberOfCorrectAnswersToSixOptions);
        statData.Add("numberOfCorrectAnswersToSevenOptions", numberOfCorrectAnswersToSevenOptions);

        // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];
        // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());

    }

    public void ChangeSceneToStorage()
    {
        StartCoroutine(ChangeSceneToStorageRoutine());
    }

    IEnumerator ChangeSceneToStorageRoutine()
    {
        _uiManager.ChangeTimerVisibility(false);

        MoveForkliftsWheels(false);

        Tween liftMove = forklift.transform.DOMoveX(-7f, 2f);
        yield return liftMove.WaitForCompletion();

        W3_AudioManager.instance.FadeOutSound("Truck", timeToMoveBG);
        Tween moveBg = backgrounds.GetComponent<RectTransform>().DOAnchorPosX(0f, timeToMoveBG);
        yield return moveBg.WaitForCompletion();
        W3_AudioManager.instance.Stop("Truck");

        CheckResult();

        if (IsPlayLimitReached())
            GameOver();

        StartLevel();
    }

    IEnumerator ChangeSceneToTruck()
    {
        forklift.SetActive(true);

        W3_AudioManager.instance.SetVolume("Truck", 0.8f);
        W3_AudioManager.instance.Play("Truck");
        Tween moveBg = backgrounds.GetComponent<RectTransform>().DOAnchorPosX(1500f, timeToMoveBG);
        yield return moveBg.WaitForCompletion();

        GetShapeCards();

        forklift.transform.position = new Vector3(-7f, -1.69f, 1f);

        //start from 1 to skip wheels
        for (int i = 1; i < forklift.transform.childCount; i++)
        {
            Transform child = forklift.transform.GetChild(i);

            child.localPosition = new Vector3(4f, child.localPosition.y, child.localPosition.z);
        }

        MoveForkliftsWheels(true);

        Tween liftMove = forklift.transform.DOMoveX(-2f, 2f);
        yield return liftMove.WaitForCompletion();
        _uiManager.ChangeTimerVisibility(true);
    }

    private void MoveForkliftsWheels(bool isGoingForward)
    {
        W3_AudioManager.instance.PlayOneShot("Forklift");
        Quaternion rotationVectorF, rotationVectorB;

        if (isGoingForward)
        {
            rotationVectorF = new Quaternion(0f, 0f, -180f, 1f);
            rotationVectorB = new Quaternion(0f, 0f, -225f, 1f);
        }
        else
        {
            rotationVectorF = new Quaternion(0f, 0f, 180f, 1f);
            rotationVectorB = new Quaternion(0f, 0f, 225f, 1f);
        }

        forkliftWheelF.transform.rotation = Quaternion.identity;
        forkliftWheelB.transform.rotation = Quaternion.identity;
        forkliftWheelF.transform.DORotate(rotationVectorF.eulerAngles, 3f);
        forkliftWheelB.transform.DORotate(rotationVectorB.eulerAngles, 3f);
    }

    public void IncreaseTruckBox()
    {
        truckBoxIndex++;

        for (int i = 0; i < truckBoxIndex; i++)
        {
            truckBoxes[i].SetActive(true);
        }
    }

    private void CloseTruckBoxes()
    {
        truckBoxIndex = 0;

        for (int i = 0; i < truckBoxes.Count; i++)
        {
            truckBoxes[i].SetActive(false);
        }
    }

    private void SetStartLevel(int lastLevel)
    {
        _level = lastLevel;
    }

    #region Level

    private void ChangeLevelByAction(eGameLevelChangeStatus levelChangeStatus)
    {
        upCounter = PlayerPrefs.GetInt("SAL_UpCounter", 0);
        downCounter = PlayerPrefs.GetInt("SAL_DownCounter", 0);

        if (levelChangeStatus == eGameLevelChangeStatus.LEVELUP)
        {
            upCounter++;

            if (upCounter >= _levelUpCriteria * 2)
            {
                upCounter = 0;
                downCounter = 0;
                _correctsCount = 0;
                _wrongsCount = 0;

                _level++;
                _level = Mathf.Clamp(_level, 1, maxLevelWKeys);
                Debug.Log($"Level changed to : {_level}");
                // gameScoreViewModel.level = _level;

                _uiManager.LevelAnimation(true);
            }
        }

        if (levelChangeStatus == eGameLevelChangeStatus.LEVELDOWN)
        {
            downCounter++;

            if (downCounter >= _levelDownCriteria)
            {
                downCounter = 0;
                upCounter = 0;
                _correctsCount = 0;
                _wrongsCount = 0;

                _level--;
                _level = Mathf.Clamp(_level, 1, maxLevelWKeys);
                Debug.Log($"Level changed to : {_level}");
                // gameScoreViewModel.level = _level;

                _uiManager.LevelAnimation(false);
            }
        }

        PlayerPrefs.SetInt("SAL_UpCounter", upCounter);
        PlayerPrefs.SetInt("SAL_DownCounter", downCounter);
    }

    #endregion
}

#region Shuffle List

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}

#endregion