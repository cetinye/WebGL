using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Witmina_Math;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Lean.Localization;

public class W14_GameManager : MonoBehaviour
{
    public Bridge bridge;
    [SerializeField] private LeanLocalization leanLocalization;
    [SerializeField] private List<W14_LevelSO> levels = new List<W14_LevelSO>();
    [SerializeField] private W14_LevelSO levelSO;

    [Space]
    private int playLimit;
    private int playCount = 0;

    public TMP_Text scoreText;
    public TMP_Text playCountText;
    private int upCounter;
    private int downCounter;
    private int correct;
    private int wrong;
    [Space]

    // FIELD
    [SerializeField] W14_UIManager _uiManager;
    [SerializeField] W14_GridManager _gridManager;

    private readonly W14_NumberGenerator _numberGenerator = new W14_NumberGenerator();
    public Transform gridStartPos, gridMidPos, gridExitPos;
    public Transform oPanelStartPos, oPanelMidPos, oPanelExitPos;

    public List<int> firstNumbers, secondNumbers;
    public List<int> numberPool;
    public List<W14_Tile> tiles;
    public List<W14_Tile> selectedTiles;
    public List<W14_Tile> tilesWithNumbers;

    private int _level = 1;

    private char _operationSign;
    private int _result;
    public bool _isTimersOn;
    private int _correctsCount;
    private int _wrongsCount;
    private bool isFlashable = true;

    public float generalTime;
    public float flashInterval = 0.5f;
    public Image lastTick;
    public Image lastSecTick;
    public float levelTime;
    public int questionCount;

    //stats
    public int numberOfCorrectAnswersTotal;
    public int numberOfIncorrectAnswersTotal;
    public int numberOfCorrectAddition;
    public int numberOfCorrectSubtraction;
    public int numberOfCorrectDivision;
    public int numberOfCorrectMultiplication;
    public int numberOfIncorrectAddition;
    public int numberOfIncorrectSubtraction;
    public int numberOfIncorrectDivision;
    public int numberOfIncorrectMultiplication;

    private int score = 0;

    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
    {
        {"0-numberOfCorrectAnswersTotal", "Correct answers"},
        {"1-numberOfIncorrectAnswersTotal", "Incorrect answers"},
        {"2-numberOfCorrectAddition", "Correct additions"},
        {"3-numberOfCorrectSubtraction", "Correct subtractions"},
        {"4-numberOfCorrectDivision", "Correct divisions"},
        {"5-numberOfCorrectMultiplication", "Correct multiplications"},
        {"6-numberOfIncorrectAddition", "Incorrect additions"},
        {"7-numberOfIncorrectSubtraction", "Incorrect subtractions"},
        {"8-numberOfIncorrectDivision", "Incorrect divisions"},
        {"9-numberOfIncorrectMultiplication", "Incorrect multiplications"},
    };

    // level change action
    public event Action onStartGameEvent;

    private int maxLevelWKeys;

    // MAIN
    void Awake()
    {
        leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

        maxLevelWKeys = levels.Count;
        Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

        _numberGenerator._gameManager = this;
        generalTime = 90;
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
        // base.CustomStart();
        onStartGameEvent?.Invoke();
        W14_AudioManager.instance.PlayOneShot("Background");
        StartLevel();
    }

    void Update()
    {
        Countdown();
    }

    /// <summary>
    /// Reload the level after level is over.
    /// </summary>
    private void StartLevel()
    {
        SetVariables();
        ShowTiles();
        RandomOperationSign();
        GetNumbers();
        MakeNumbersInvisible();
        PlaceNumbers();
        ShowNumbers();
    }

    private void SetVariables()
    {
        _level = Mathf.Clamp(_level, 1, maxLevelWKeys);
        levelSO = levels[_level - 1];

        _uiManager.UpdateLevelText(_level);

        levelTime = levelSO.timePerRound;
        playLimit = levelSO.numOfPlays;
        questionCount = levelSO.numOfQuestions;
        _correctsCount = 0;
        _wrongsCount = 0;
    }

    /// <summary>
    /// Show the tiles when the game reload.
    /// </summary>
    //private void ShowTiles()
    //{
    //    PlayFxBySoundState(eW14FxSoundStates.TRANSITION_ENTRY);
    //    _gridManager.tileParent.transform.position = gridStartPos.position;
    //    _gridManager.tileParent.SetActive(true);
    //    _uiManager.operationPanel.transform.position = oPanelStartPos.position;

    //    Sequence seq = GridMoveSeq(gridMidPos.position, oPanelMidPos.position, 1f);
    //    seq
    //     .OnComplete(() => { _isTimersOn = true; seq.Kill(true); });
    //}

    private void MakeNumbersInvisible()
    {
        for (int i = 0; i < _gridManager._tiles.Count; i++)
        {
            _gridManager._tiles[i].txtTileNumber.color = new Color(237f, 204f, 150f, 0f);
        }
    }

    private void ShowTiles()
    {
        _gridManager.tileParent.transform.position = gridMidPos.position;
        _gridManager.tileParent.SetActive(true);
        _uiManager.operationPanel.transform.position = oPanelStartPos.position;
    }

    private void ShowNumbers()
    {
        StartCoroutine(ShowNumbersRoutine());
    }

    IEnumerator ShowNumbersRoutine()
    {
        W14_AudioManager.instance.Play("NumbersSpawn");
        for (int i = 0; i < tilesWithNumbers.Count; i++)
        {

            tilesWithNumbers[i].txtTileNumber.DOFade(1, 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        //W14_AudioManager.instance.Stop("NumbersSpawn");

        Sequence seq = GridMoveSeq(gridMidPos.position, oPanelMidPos.position, 1f);
        seq
         .OnComplete(() =>
         {
             _isTimersOn = true; seq.Kill(true);
             _uiManager.StartGameTimer();
         });
    }

    /// <summary>
    /// Get operation sign randomly.
    /// </summary>
    private void RandomOperationSign()
    {
        int range;
        range = Random.Range(1, levelSO.maxMathOperation + 1);

        _operationSign = range switch
        {
            1 => '+',
            2 => '-',
            3 => ':',
            4 => 'x',
            _ => _operationSign
        };
        _uiManager.SetOperationSign(_operationSign);
    }

    /// <summary>
    /// Get all numbers that created randomly to list (result,first number, second number) and merge the list in numberPool list.
    /// </summary>
    private void GetNumbers()
    {
        int max = 0;
        int min = 1;

        switch (_operationSign)
        {
            case '+':
                min = _level;
                max = _level * 5 + 6;
                break;
            case 'x':
                max = 11;
                break;
            case ':':
                max = (_level - 1) * 5 + 24;
                break;
        }

        _result = _numberGenerator.GetRandomResult(_operationSign, _level, max);
        _uiManager.txtResult.text = _result.ToString();
        firstNumbers = _numberGenerator.GetFirstNumbers(min, max, _level, _result, _operationSign);
        secondNumbers = _numberGenerator.GetSecondNumbers(firstNumbers, _result, _operationSign);
        numberPool = firstNumbers.Concat(secondNumbers).ToList();
    }

    /// <summary>
    /// Place the numbers from numberPool to the grid.
    /// </summary>
    private void PlaceNumbers()
    {
        tilesWithNumbers.Clear();

        tiles = _gridManager._tiles;
        int _howManyNumber = questionCount * 2;

        for (int i = 0; i < _howManyNumber; i++)
        {
            int tilePosition = Random.Range(0, tiles.Count);

            if (tiles[tilePosition].txtTileNumber.text == "")
            {
                tiles[tilePosition].txtTileNumber.text = numberPool[i].ToString();
                tilesWithNumbers.Add(tiles[tilePosition]);
            }
            else
            {
                i--;
            }
        }
    }

    /// <summary>
    /// Timers count down.
    /// </summary>
    private void Countdown()
    {
        // if (!_uiManager.isGeneralTimerActive) return;
        // generalTime -= Time.deltaTime;

        // if (generalTime <= 5.2f && isFlashable)
        // {
        //     isFlashable = false;
        //     PlayFx("Countdown", 0.7f, 1f);
        //     FlashRed();
        // }

        if (!_isTimersOn) return;
        levelTime -= Time.deltaTime;

        CheckTimers();
    }

    /// <summary>
    /// Control the timers for end game.
    /// </summary>
    private void CheckTimers()
    {
        if (levelTime < 0)
        {
            _isTimersOn = false;

            W14_AudioManager.instance.PlayOneShot("Timeout");
            levelTime = 0;
            downCounter++;
            PlayerPrefs.SetInt("Math_DownCounter", downCounter);
            LevelUp(false);
        }
    }

    private void CheckPlayLimit()
    {
        playCount++;
        playCountText.text = "Play Count " + playCount;
        if (playCount >= playLimit)
        {
            GameOver();
        }
    }

    /// <summary>
    /// Button method for solving the math operation.
    /// If result is true correctAnswer++, else the numbers return to grid.
    /// </summary>
    public void Submit()
    {
        ButtonClickSeq();
        int result = 0;

        if (_uiManager.txtNumber.text != "...." && _uiManager.txtNumber2.text != "....")
        {
            int number1 = Convert.ToInt16(_uiManager.txtNumber.text);
            int number2 = Convert.ToInt16(_uiManager.txtNumber2.text);

            switch (_operationSign)
            {
                case '+':
                    result = number1 + number2;
                    break;
                case '-':
                    result = number1 - number2;
                    break;
                case 'x':
                    result = number1 * number2;
                    break;
                case ':':
                    if (number1 % number2 == 0)
                    {
                        result = number1 / number2;
                    }
                    else
                    {
                        result = -1;
                    }

                    break;
            }

            if (result == _result)
            {
                ResultSeq(true);
                ButtonResultSeq(true);
                IncDecScore(true);
                for (int i = 0; i < selectedTiles.Count; i++)
                {
                    selectedTiles[i].txtTileNumber.text = "";
                    selectedTiles[i].txtTileNumber.gameObject.SetActive(true);
                }

                selectedTiles.Clear();

                if (_correctsCount >= questionCount)
                {
                    _isTimersOn = false;
                    upCounter++;
                    PlayerPrefs.SetInt("Math_UpCounter", upCounter);
                    LevelUp(true);
                }

                switch (_operationSign)
                {
                    case '+':
                        numberOfCorrectAddition++;
                        break;
                    case '-':
                        numberOfCorrectSubtraction++;
                        break;
                    case 'x':
                        numberOfCorrectMultiplication++;
                        break;
                    case ':':
                        numberOfCorrectDivision++;
                        break;
                }
            }
            else
            {
                Wrong();
            }
        }
        else
        {
            Wrong();
        }
    }

    private void Wrong()
    {
        ResultSeq(false);
        ButtonResultSeq(false);
        IncDecScore(false);
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            selectedTiles[i]._sequence.Complete();
            selectedTiles[i]._isEnable = true;
            selectedTiles[i].txtTileNumber.gameObject.SetActive(true);
        }

        selectedTiles.Clear();

        switch (_operationSign)
        {
            case '+':
                numberOfIncorrectAddition++;
                break;
            case '-':
                numberOfIncorrectSubtraction++;
                break;
            case 'x':
                numberOfIncorrectMultiplication++;
                break;
            case ':':
                numberOfIncorrectDivision++;
                break;
        }
    }

    private void ResultSeq(bool isCorrect)
    {
        Color color = _uiManager.resultColor;
        color.a = 100;

        if (isCorrect)
        {
            DOTween.Sequence()
                 .Append(_uiManager.txtResult.DOScale(1.3f, 0.125f))
                 .Join(_uiManager.txtResult.DOColor(Color.green, 0.125f))
                 .Append(_uiManager.txtResult.DOScale(1f, 0.125f))
                 .Join(_uiManager.txtResult.DOColor(color, 0.125f))
                 .OnComplete(() =>
                 {
                     _uiManager.txtNumber.text = "....";
                     _uiManager.txtNumber2.text = "....";
                 })
                 .SetAutoKill(true);
        }
        else
        {
            DOTween.Sequence()
                 .Append(_uiManager.txtResult.DOScale(1.3f, 0.125f))
                 .Join(_uiManager.txtResult.DOColor(Color.red, 0.125f))
                 .Append(_uiManager.txtResult.DOScale(1f, 0.125f))
                 .Join(_uiManager.txtResult.DOColor(color, 0.125f))
                 .OnComplete(() =>
                 {
                     _uiManager.txtNumber.text = "....";
                     _uiManager.txtNumber2.text = "....";
                 })
                 .SetAutoKill(true);
        }
    }

    private void LevelUp(bool isLevelUp)
    {
        if (isLevelUp)
        {
            //if (_wrongsCount <= 0)
            //{
            //    BonusPoint();
            //}
            // onChangeLevelEvent(eGameLevelChangeStatus.LEVELUP);
        }
        else
        {
            LevelOver();
            // onChangeLevelEvent(eGameLevelChangeStatus.LEVELDOWN);
        }
        DecideLevel();
        GridMoveSeq(gridExitPos.position, oPanelExitPos.position, 0.8f);
    }

    /// <summary>
    /// Clears number lists.
    /// </summary>
    private void ClearTheLists()
    {
        firstNumbers.Clear();
        secondNumbers.Clear();
        numberPool.Clear();
        selectedTiles.Clear();

        _uiManager.txtNumber.text = "....";
        _uiManager.txtNumber2.text = "....";
        _uiManager.txtResult.text = "....";
    }

    /// <summary>
    /// Clears tiles' text contains the numbers for new level after the level end.
    /// </summary>
    private void ClearTilesTexts()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].txtTileNumber.gameObject.SetActive(true);
            tiles[i]._isEnable = true;
            tiles[i].txtTileNumber.text = "";
        }
    }

    /// <summary>
    /// Set texts' values after the level fails.
    /// </summary>
    private void LevelOver()
    {
        _uiManager.txtNumber.text = "....";
        _uiManager.txtNumber2.text = "....";
        _uiManager.txtResult.text = "....";
        _uiManager.txtOpSign.gameObject.SetActive(false);
        _uiManager.submitBtn.interactable = false;
    }

    /// <summary>
    /// Increases the score and number of correctAnswer.
    /// </summary>
    private void IncDecScore(bool isIncreased)
    {
        if (isIncreased)
        {
            W14_AudioManager.instance.PlayOneShot("Correct");
            _correctsCount++;
            correct++;
            numberOfCorrectAnswersTotal++;
        }
        else
        {
            W14_AudioManager.instance.PlayOneShot("Wrong");
            _wrongsCount++;
            wrong++;
            numberOfIncorrectAnswersTotal++;
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
        resultObject.level = _level;
        resultObject.score = Mathf.Clamp(Mathf.CeilToInt((float)score / levelSO.maxInLevel * 1000), 0, 1000);
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

        statData.Add("numberOfCorrectAnswersTotal", numberOfCorrectAnswersTotal);
        statData.Add("numberOfIncorrectAnswersTotal", numberOfIncorrectAnswersTotal);
        statData.Add("numberOfCorrectAddition", numberOfCorrectAddition);
        statData.Add("numberOfCorrectSubtraction", numberOfCorrectSubtraction);
        statData.Add("numberOfCorrectDivision", numberOfCorrectDivision);
        statData.Add("numberOfCorrectMultiplication", numberOfCorrectMultiplication);
        statData.Add("numberOfIncorrectAddition", numberOfIncorrectAddition);
        statData.Add("numberOfIncorrectSubtraction", numberOfIncorrectSubtraction);
        statData.Add("numberOfIncorrectDivision", numberOfIncorrectDivision);
        statData.Add("numberOfIncorrectMultiplication", numberOfIncorrectMultiplication);

        // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];
        // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());

    }

    private Sequence GridMoveSeq(Vector3 gridTarget, Vector3 oPanelTarget, float duration)
    {
        W14_AudioManager.instance.PlayOneShot("PaperTransition");

        return DOTween.Sequence()
               //.Append(_gridManager.tileParent.transform.DOMove(gridTarget, duration))
               .Join(_uiManager.operationPanel.transform.DOMove(oPanelTarget, duration))
               .OnComplete(() => Reload())
               .SetAutoKill(true);
    }

    /// <summary>
    /// Reload the game by level.
    /// </summary>
    private void Reload()
    {
        // CalculateLevelScore();
        ClearTheLists();
        ClearTilesTexts();
        _uiManager.submitBtn.interactable = true;
        CheckPlayLimit();
        StartLevel();
    }

    public void TryAgainBtn()
    {
        //PlayFxBySoundState(eW14FxSoundStates.BUTTON_CLICK);
        Reload();
    }

    private void ButtonClickSeq()
    {
        DOTween.Sequence()
            .Append(_uiManager.submitBtn.transform.DOScale(1.1f, 0.1f))
            .Append(_uiManager.submitBtn.transform.DOScale(1f, 0.1f))
            .SetAutoKill(true);
    }

    public void FlashRed()
    {
        Sequence redFlash = DOTween.Sequence();

        redFlash.Append(lastTick.DOColor(Color.red, flashInterval))
                .Join(lastSecTick.DOColor(Color.red, flashInterval))
                .SetEase(Ease.Linear)
                .Append(lastTick.DOColor(new Color(0.1176471f, 0.8431373f, 0.9098039f, 1f), flashInterval))
                .Join(lastSecTick.DOColor(new Color(0.1176471f, 0.8431373f, 0.9098039f, 1f), flashInterval))
                .SetEase(Ease.Linear)
                .SetLoops(6);

        redFlash.Play();
    }

    /// <summary>
    /// Button animation works by the result.
    /// </summary>
    /// <param name="isCorrect"></param>
    private void ButtonResultSeq(bool isCorrect) // Vibration will added.
    {
        Sequence seq = DOTween.Sequence();
        //_uiManager.ShowResult(isCorrect);

        if (isCorrect)
        {
            seq
            .Append(_uiManager.submitBtn.transform.DOScale(1.2f, 0.3f))
            .Append(_uiManager.submitBtn.transform.DOScale(1f, 0.2f));
        }
        else
        {
            float zAxis = -20f;

            for (int i = 0; i < 6; i++)
            {
                seq.Append(_uiManager.submitBtn.transform.DORotate(new Vector3(0, 0, zAxis), 0.05f));
                zAxis *= -1;
            }
            seq.Append(_uiManager.submitBtn.transform.DORotate(new Vector3(0, 0, 0), 0.05f));
        }
        seq.onComplete = () => { _uiManager.resultImg.gameObject.SetActive(false); seq.Kill(true); };
    }

    private void SetStartLevel(int lastLevel)
    {
        _level = lastLevel;
    }

    #region Level

    private void DecideLevel()
    {
        score = CalculateLevelScore();
        upCounter = PlayerPrefs.GetInt("Math_UpCounter", 0);
        downCounter = PlayerPrefs.GetInt("Math_DownCounter", 0);

        if (upCounter >= levelSO.levelUpCriteria * 2)
        {
            if (score >= levelSO.minScore)
            {
                LevelUp();
            }
            else
            {
                downCounter++;
                PlayerPrefs.SetInt("Math_DownCounter", downCounter);
            }
        }
        else if (downCounter >= 2)
        {
            LevelDown();
        }

        PlayerPrefs.SetInt("Math_UpCounter", upCounter);
        PlayerPrefs.SetInt("Math_DownCounter", downCounter);
    }

    private void LevelUp()
    {
        _uiManager.BlastConfetti();
        _level++;
        _level = Mathf.Clamp(_level, 1, maxLevelWKeys);
        levelSO = levels[_level - 1];
        _uiManager.UpdateLevelText(_level);
        _uiManager.LevelAnimation(true);

        upCounter = 0;
        downCounter = 0;
        wrong = 0;
        correct = 0;
    }

    private void LevelDown()
    {
        _level--;
        _level = Mathf.Clamp(_level, 1, maxLevelWKeys);
        levelSO = levels[_level - 1];
        _uiManager.UpdateLevelText(_level);
        _uiManager.LevelAnimation(false);

        upCounter = 0;
        downCounter = 0;
        wrong = 0;
        correct = 0;
    }

    public int CalculateLevelScore()
    {
        int levelScore = Mathf.CeilToInt((correct * 100) - (wrong * levelSO.penaltyPoints));
        levelScore = Mathf.Clamp(levelScore, 0, levelSO.maxInLevel);

        scoreText.text = $"Score: {levelScore}";

        return levelScore;
    }

    #endregion
}