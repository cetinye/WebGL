using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using W42;
using UnityEngine.Video;
using System.Threading.Tasks;
using To_Be_Or_Not_To_Be;
using Lean.Localization;

public class W42_GameController : MonoBehaviour
{
    public Bridge bridge;

    [SerializeField] private LeanLocalization leanLocalization;
    [SerializeField] ParticleSystem _destroyFx;
    [SerializeField] TextMeshProUGUI _timeUpTxt;
    [SerializeField] SpriteRenderer topLight;
    [SerializeField] Sprite defaultLight;
    [SerializeField] Sprite correctLight;
    [SerializeField] Sprite wrongLight;

    [SerializeField] private W42_LevelSO levelSO;
    [SerializeField] private List<W42_LevelSO> levelList = new List<W42_LevelSO>();
    public Dictionary<string, Color> colorsWithName = new Dictionary<string, Color>(5);
    public Color[] colors;
    public List<Color> valueList;
    public List<W42_ModelVariables> models;
    public List<Transform> spawns = new List<Transform>();
    public List<W42_Model> generatedModels = new List<W42_Model>();
    public Sprite[] wrongOrTrueSprites;

    W42_Model _chosenModel;
    Sequence _sequence;
    public W42_Model refModel;
    public Transform targetPos;
    public Transform modelParent;
    public Button noButton, yesButton;
    public Slider timerSlider;
    public GameObject buttons;
    public GameObject textArea;
    public TextMeshProUGUI questionText;
    public SpriteRenderer resultSpr;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelText;
    public Animator levelTextAnimator;
    public float flashInterval;

    bool isFlashable = true;
    readonly int _maxLevel = 10;
    int _wrongQuestionCount;
    int _correctQuestionCount;
    int _choosingValue;
    bool _isClickable;
    bool _isAnyButtonClicked;
    int _score;
    bool _isTimerOn;
    float _timeRemaining;
    bool _isModelShown;
    float _modelShowDuration;
    int _sortingOrderNo = -3;

    public Vector3 outPos = new Vector3(0f, 2.193f, 1f);
    public float moveDuration;
    public float showDuration;

    public int levelUpCriteria;
    public int levelDownCriteria;
    public int upCounter;
    public int downCounter;
    private List<int> scores = new List<int>();

    public int corrects, wrongs;
    public int questionCount;
    public int level = 1;

    public bool isTrickQuestion;

    public W42_Constants W42Constants = new W42_Constants();

    //stats
    public int numberOfCorrectAnswers;
    public int numberOfIncorrectAnswers;
    public int numberOfCorrectAnswersToNotQuestions;
    public int numberOfCorrectAnswersToStandardQuestions;
    public int bestStreakCorrectAnswers;
    public int currentStreakCorrectAnswers;

    [SerializeField] W42_Lights rightLight;
    [SerializeField] W42_Lights leftLight;
    [SerializeField] Image windowFrame;
    [SerializeField] Transform machineTr;
    [SerializeField] Transform modelsTr;

    private List<int> bonusScores = new List<int>();

    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
    {
        { "0-numberOfCorrectAnswers", "Correct Answers " },
        { "1-numberOfIncorrectAnswers", "Incorrect Answers " },
        { "2-numberOfCorrectAnswersToNotQuestions", "Correct answers to \"not\" questions " },
        { "3-numberOfCorrectAnswersToStandardQuestions", "Correct answers to standard questions " },
        { "4-bestStreakCorrectAnswers", "Best streak " },
    };

    // level change action 
    event Action<eGameLevelChangeStatus> onChangeLevelEvent;

    private int[] levelScores = new[] { 0, 100, 150, 250, 375, 500, 625, 750, 850, 935, 1000 };

    private int maxLevelWKeys;

    public enum eGameLevelChangeStatus
    {
        LEVELUP,
        LEVELDOWN,
    }

    // MAIN
    void Awake()
    {
        maxLevelWKeys = levelList.Count;
        Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

        AssignLevelVariables(level);

        onChangeLevelEvent += ChangeLevelByAction;
        refModel.gameController = this;

        _timeRemaining = 60f;

        SetColorsWithName();
        // GenerateLevels();
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
        // base.CustomStart();

        SetStartLevel(levelId);

        GenerateModels();
        DisplayTime(_timeRemaining);
        StartCoroutine(StartLevelCor());
    }

    void Update()
    {
        CountDown();
    }

    void AssignLevelVariables(int levelId)
    {
        levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);

        levelSO = levelList[levelId - 1];

        questionCount = (int)levelSO.requiredMinQuestionsToSolve;

        showDuration = levelSO.remainingTimeToAnswer;
        moveDuration = levelSO.questionLoadTime / 2f;
        timerSlider.maxValue = showDuration;

        levelUpCriteria = levelSO.levelUpCriteria;
        levelDownCriteria = levelSO.levelDownCriteria;

        levelText.text = $"{LeanLocalization.GetTranslationText("Level")} {levelId}";
    }

    // FUNC
    private void SetColorsWithName()
    {
        colorsWithName.Add("red", colors[0]);
        colorsWithName.Add("blue", colors[1]);
        colorsWithName.Add("green", colors[2]);
        colorsWithName.Add("orange", colors[3]);
        colorsWithName.Add("black", colors[4]);

        valueList = new List<Color>(colorsWithName.Values);
    }

    private void GenerateModels()
    {
        for (int i = 0; i < spawns.Count; i++)
        {
            W42_Model newModel = Instantiate(refModel, modelParent);
            newModel.SetModel(LeanLocalization.GetTranslationText(W42Constants.LocalizationModelKeys[i]), models[i]._sprite);

            Vector3 rotation = new Vector3(0, 0, Random.Range(0, 181));
            newModel.tr.rotation = Quaternion.Euler(rotation);
            Vector3 pos = spawns[i].position;
            pos.z = 1;
            newModel.tr.position = pos;
            generatedModels.Add(newModel);

            AssignRandomSortingOrder(newModel);

            newModel.ShowTheModel();
        }
    }

    private void AssignRandomSortingOrder(W42_Model model)
    {
        model.baseSpr.sortingOrder = ++_sortingOrderNo;
        model.oldSpriteOrder = _sortingOrderNo;
        model.spriteSpr.sortingOrder = ++_sortingOrderNo;
    }

    private void StartLevel()
    {
        AssignLevelVariables(level);
        AskQuestion();
    }

    private IEnumerator StartLevelCor()
    {
        AssignLevelVariables(level);
        yield return new WaitForSeconds(1f);
        W42_AudioController.instance.PlayOneShot("Coin");
        W42_AudioController.instance.Play("Background");

        // reposition
        Vector3 machineTargetPos = new Vector3(0.0599999987f, 5.23000002f, 1f);
        Vector3 machineTargetScale = new Vector3(0.560465693f, 0.560465693f, 0.560465693f);
        Vector3 modelsTargetPosition = new Vector3(-0.00300000003f, 1.42499995f, 0f);
        Vector3 modelsTargetScale = new Vector3(0.933228016f, 0.933228016f, 0.933228016f);

        Sequence repositionSeq = DOTween.Sequence();
        repositionSeq.Append(machineTr.DOMove(machineTargetPos, 1f));
        repositionSeq.Join(machineTr.DOScale(machineTargetScale, 1f));
        repositionSeq.Join(modelsTr.DOMove(modelsTargetPosition, 1f));
        repositionSeq.Join(modelsTr.DOScale(modelsTargetScale, 1f));

        yield return repositionSeq.WaitForCompletion();
        _isTimerOn = true;

        AskQuestion();
    }

    private void AskQuestion()
    {
        if (questionCount > 0)
        {
            topLight.sprite = defaultLight;
            _isClickable = true;
            _isAnyButtonClicked = false;
            _chosenModel = generatedModels[Random.Range(0, generatedModels.Count)];
            SetQuestText(_chosenModel);
            W42_AudioController.instance.PlayOneShot("Move");
            MoveSequence(true);
            //_sequence.onComplete = () => MoveSequence(false);
            questionCount--;
        }
        else
        {
            GetResultToPassLevel();
        }
    }

    private void MoveSequence(bool isComing)
    {
        _sequence = DOTween.Sequence();
        if (isComing)
        {
            rightLight.Light(true);
            leftLight.Light(true);

            _chosenModel.baseSpr.sortingOrder = 101;
            _chosenModel.spriteSpr.sortingOrder = 102;
            _chosenModel.transform.position = outPos;
            _chosenModel.transform.localScale = Vector3.zero;

            _sequence
                .Append(_chosenModel.transform.DOMoveX(0, moveDuration).SetEase(Ease.OutBounce))
                .Append(_chosenModel.transform.DOMove(targetPos.position, moveDuration).SetEase(Ease.OutBounce))
                .Join(_chosenModel.transform.DOScale(0.46f, moveDuration - 0.3f))
                .Join(_chosenModel.transform.GetChild(0).DOScale(0.59f, moveDuration - 0.3f))
                .Join(_chosenModel.transform.DORotate(new Vector3(0, 0, 0), moveDuration))
                .Append(ActivateUIElements(true))
                .AppendInterval(showDuration);

            _sequence.OnComplete(() =>
            {
                CheckButtonClickEvent();
            });

            for (int i = 0; i < generatedModels.Count; i++)
            {
                generatedModels[i].tr.DOShakePosition(1, 0.1f);
                generatedModels[i].tr.DOShakeRotation(1, 0.1f).OnComplete(() =>
                {
                    rightLight.Light(false);
                    leftLight.Light(false);
                });
            }
        }
        else
        {
            _destroyFx.Play();
            _sequence
                .Append(_chosenModel.tr.DOScale(0.22f, 0.2f))
                .Append(_chosenModel.transform.DOMove(_chosenModel.startPosition, 0))
                .Join(_chosenModel.transform.DOScale(0.22f, 0))
                .Join(_chosenModel.transform.DORotate(_chosenModel.startRotation.eulerAngles, 0))
                .Join(ActivateUIElements(false))
                .AppendCallback(AskQuestion);

            _chosenModel.baseSpr.sortingOrder = _chosenModel.oldSpriteOrder;
            _chosenModel.spriteSpr.sortingOrder = ++_chosenModel.oldSpriteOrder;
        }

        timerSlider.value = 0;
        _sequence.SetAutoKill(true);
    }

    private Sequence ActivateUIElements(bool isActive)
    {
        Sequence seq = DOTween.Sequence();

        if (isActive)
        {
            seq.AppendCallback(() =>
            {
                buttons.SetActive(true);
                textArea.SetActive(true);
                timerSlider.value = 0;
                timerSlider.gameObject.SetActive(true);
                _isModelShown = true;
            });
        }
        else
        {
            seq.AppendCallback(() =>
            {
                buttons.SetActive(false);
                textArea.SetActive(false);
                _isModelShown = false;
            });
        }


        return seq;
    }

    private void CheckButtonClickEvent()
    {
        if (!_isAnyButtonClicked)
        {
            NoButtonClicked();
        }
    }

    private void NoButtonClicked()
    {
        _isClickable = false;

        ResetModelShowTimer();

        numberOfIncorrectAnswers++;
        IncDecScore(false);
        //resultSpr.sprite = wrongOrTrueSprites[0];
        _timeUpTxt.gameObject.SetActive(true);
        questionText.text = "";
        ShowResult();
    }

    /// <summary>
    /// Set chooseTrueOrWrong value for generate random question
    /// if equal 0 then Right Answer questions
    /// if equal 1 then Wrong Answer questions
    /// </summary>
    private void SetQuestText(W42_Model model)
    {
        int random = Random.Range(0, 2);

        if (random == 0)
        {
            QuestionWithWrongAnswer(model);
            _wrongQuestionCount--;
            _choosingValue = 0;
        }
        else if (random == 1)
        {
            QuestionWithRightAnswer(model);
            _correctQuestionCount--;
            _choosingValue = 1;
        }
    }

    /// <summary>
    /// set random question for right possibility
    /// set text and for right answer
    /// </summary>
    private void QuestionWithRightAnswer(W42_Model model)
    {
        int random = Random.Range(0, 3);
        {
            if (random == 1 && level >= 2)
            {
                //_questionText.text = "This is not a " + GenerateAnotherColorName(model._colorName) + " " + model._name;
                questionText.text = LeanLocalization.GetTranslationText("thisIs") +
                                    LeanLocalization.GetTranslationText("not") +
                                    LeanLocalization.GetTranslationText("a") +
                                    LeanLocalization.GetTranslationText(GenerateAnotherColorName(model.colorName)) + " " + model._modelName;
                isTrickQuestion = true;
            }
            if (random == 2 && level >= 2)
            {
                //_questionText.text = "This is not a " + GenerateAnotherColorName(model._colorName) + " " +
                //                     models[Random.Range(0, models.Count)].name;

                questionText.text = LeanLocalization.GetTranslationText("thisIs") +
                                    LeanLocalization.GetTranslationText("not") +
                                    LeanLocalization.GetTranslationText("a") +
                                    model.colorName + " " + GetAnotherModelName(model._modelName);
                isTrickQuestion = true;
            }
            else
            {
                //_questionText.text = "This is a " + model._colorName + " " + model._name;
                questionText.text = LeanLocalization.GetTranslationText("thisIs") +
                                    LeanLocalization.GetTranslationText("a") +
                                    model.colorName + " " + model._modelName;
                isTrickQuestion = false;
            }
        }
    }

    private void QuestionWithWrongAnswer(W42_Model model)
    {
        int random = Random.Range(0, 3);
        {
            if (random == 0 && level >= 2)
            {
                //_questionText.text = "This is not a " + model._colorName + " " + model._name;
                questionText.text = LeanLocalization.GetTranslationText("thisIs") +
                                    LeanLocalization.GetTranslationText("not") +
                                    LeanLocalization.GetTranslationText("a") +
                                    model.colorName + " " + model._modelName;
                isTrickQuestion = true;
            }
            else if (random == 1)
            {
                //_questionText.text = "This is a " + model._colorName + " " + GetAnotherModelName(model._name);
                questionText.text = LeanLocalization.GetTranslationText("thisIs") +
                                    LeanLocalization.GetTranslationText("a") +
                                    model.colorName + " " + GetAnotherModelName(model._modelName);
                isTrickQuestion = false;
            }
            else
            {
                //_questionText.text = "This is a " + GenerateAnotherColorName(model._colorName) + " " + model._name;
                questionText.text = LeanLocalization.GetTranslationText("thisIs") +
                                    LeanLocalization.GetTranslationText("a") +
                                    LeanLocalization.GetTranslationText(GenerateAnotherColorName(model.colorName)) + " " + model._modelName;
                isTrickQuestion = false;
            }
        }
    }

    private string GetAnotherModelName(string name)
    {
        string otherName;

        do
        {
            int random = Random.Range(0, models.Count);
            otherName = LeanLocalization.GetTranslationText(W42Constants.LocalizationModelKeys[random]);
        } while (otherName.Equals(name));

        return otherName;
    }

    private string GenerateAnotherColorName(string colorName)
    {
        string otherName;

        do
        {
            int random = Random.Range(0, colors.Length);
            otherName = colorsWithName.FirstOrDefault(x => x.Value == colors[random]).Key;
        } while (otherName.Equals(colorName));

        return otherName;
    }

    public void YesButton()
    {
        if (!_isClickable) return;

        _isAnyButtonClicked = true;

        ResetModelShowTimer();

        Sequence seq = ButtonScaleSeq(yesButton.transform);
        switch (_choosingValue)
        {
            case 0:
                IncDecScore(false);
                resultSpr.sprite = wrongOrTrueSprites[0];
                numberOfIncorrectAnswers++;

                if (currentStreakCorrectAnswers > bestStreakCorrectAnswers)
                {
                    bestStreakCorrectAnswers = currentStreakCorrectAnswers;
                }

                currentStreakCorrectAnswers = 0;

                break;
            case 1:
                IncDecScore(true);
                resultSpr.sprite = wrongOrTrueSprites[1];

                if (isTrickQuestion)
                {
                    numberOfCorrectAnswersToNotQuestions++;
                }
                else
                {
                    numberOfCorrectAnswersToStandardQuestions++;
                }

                currentStreakCorrectAnswers++;
                numberOfCorrectAnswers++;

                break;
        }

        seq.onComplete = () => buttons.SetActive(false);
        questionText.text = "";
        ShowResult();
        _isClickable = false;
    }

    public void NoButton()
    {
        if (!_isClickable) return;
        _isAnyButtonClicked = true;

        ResetModelShowTimer();

        Sequence seq = ButtonScaleSeq(noButton.transform);

        switch (_choosingValue)
        {
            case 0:
                IncDecScore(true);
                resultSpr.sprite = wrongOrTrueSprites[1];

                if (isTrickQuestion)
                {
                    numberOfCorrectAnswersToNotQuestions++;
                }
                else
                {
                    numberOfCorrectAnswersToStandardQuestions++;
                }

                currentStreakCorrectAnswers++;
                numberOfCorrectAnswers++;
                break;
            case 1:
                IncDecScore(false);
                resultSpr.sprite = wrongOrTrueSprites[0];
                numberOfIncorrectAnswers++;
                if (currentStreakCorrectAnswers > bestStreakCorrectAnswers)
                {
                    bestStreakCorrectAnswers = currentStreakCorrectAnswers;
                }

                currentStreakCorrectAnswers = 0;

                break;
        }

        seq.onComplete = () => buttons.SetActive(false);
        questionText.text = "";
        ShowResult();
        _isClickable = false;
    }

    void StartFlashWindow(Color color)
    {
        StartCoroutine(FlashWindowColor(color));
    }

    IEnumerator FlashWindowColor(Color color)
    {
        windowFrame.enabled = true;

        for (int i = 0; i < 2; i++)
        {
            windowFrame.color = color;
            yield return new WaitForSeconds(0.2f);
            windowFrame.enabled = false;
            yield return new WaitForSeconds(0.2f);
            windowFrame.enabled = true;
        }

        windowFrame.enabled = false;
    }

    private Sequence ButtonScaleSeq(Transform btnTransform)
    {
        Sequence seq = DOTween.Sequence()
            .Append(btnTransform.DOScale(1.2f, 0.2f)).SetEase(Ease.InFlash)
            .Append(btnTransform.DOScale(1f, 0.2f)).SetEase(Ease.InFlash)
            .SetAutoKill();

        return seq;
    }

    private void IncDecScore(bool isIncreased)
    {
        if (isIncreased)
        {
            W42_AudioController.instance.PlayOneShot("Correct");
            topLight.sprite = correctLight;
            StartFlashWindow(Color.green);
            corrects++;
        }
        else
        {
            W42_AudioController.instance.PlayOneShot("Wrong");
            topLight.sprite = wrongLight;
            StartFlashWindow(Color.red);
            wrongs++;
        }

        ResetModelShowTimer();
    }

    /// <summary>
    /// Checks wrongs and corrects counts.Then calls LevelUp function by the result.
    /// </summary>
    private void GetResultToPassLevel()
    {
        int score = CalculateScore();
        Debug.LogWarning("Score: " + score);

        if (score >= levelSO.minRequiredScore)
        {
            LevelUp(true);
        }
        else
        {
            LevelUp(false);
        }

        StartLevel();
    }

    private int CalculateScore()
    {
        int score = Mathf.Clamp((corrects * 100) - (wrongs * 50), 0, levelSO.maxInLevel);
        scores.Add(Mathf.Clamp(Mathf.CeilToInt((float)score / levelSO.maxInLevel * 1000), 0, 1000));
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

    private void ShowResult()
    {
        timerSlider.gameObject.SetActive(false);
        Sequence seq = DOTween.Sequence();

        if (_isAnyButtonClicked)
        {
            seq
                .Append(resultSpr.transform.DOScale(new Vector3(0.8f, 0.8f, 0), 0.4f)).SetEase(Ease.Flash)
                .AppendInterval(0.1f)
                .Append(resultSpr.transform.DOScale(new Vector3(0, 0, 0), 0.2f)).SetEase(Ease.Flash)
                .OnComplete(() =>
                {
                    _sequence.Kill();
                    MoveSequence(false);
                });
        }
        else
        {
            seq
                .Append(_timeUpTxt.transform.DOScale(1.5f, 0.4f)).SetEase(Ease.Flash)
                .AppendInterval(0.1f)
                .Append(_timeUpTxt.transform.DOScale(1, 0.2f)).SetEase(Ease.Flash)
                .OnComplete(() =>
                {
                    _timeUpTxt.gameObject.SetActive(false);

                    MoveSequence(false);
                });
        }
    }

    private void ModelShowTimer()
    {
        if (_isModelShown)
        {
            _modelShowDuration += Time.deltaTime;
        }

        timerSlider.value = _modelShowDuration;
    }

    private void ResetModelShowTimer()
    {
        _isModelShown = false;
        _modelShowDuration = 0;
        timerSlider.value = _modelShowDuration;
    }

    private void CountDown()
    {
        if (!_isTimerOn) return;
        _timeRemaining -= Time.deltaTime;

        if (_timeRemaining <= 5.2f && isFlashable)
        {
            isFlashable = false;
            // PlayFx("Countdown", 0.7f, 1f);
            FlashRed();
        }

        if (_timeRemaining <= 0)
        {
            _isTimerOn = false;
            _timeRemaining = 0;

            GameOver();
        }

        DisplayTime(_timeRemaining);
        ModelShowTimer();
    }

    private void LevelAnimation(bool isLevelUp)
    {
        if (!isLevelUp)
        {
            levelText.DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
        }

        levelTextAnimator.SetTrigger("LevelAnim");
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

    private void DisplayTime(float time)
    {
        if (time >= 0)
        {
            timeText.text = time.ToString("F0");

            //SetTimerText($"{minutes:00}:{seconds:00}");
        }
        else
            timeText.text = "0";

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
        int totalScore = Mathf.Clamp(CalculateTotalScore(), 0, 1000);
        Debug.Log("Total Score: " + totalScore);

        resultObject.level = level;
        resultObject.score = totalScore;
        string json = JsonUtility.ToJson(resultObject);
        Debug.LogWarning(json);
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
        statData.Add("numberOfCorrectAnswersToNotQuestions", numberOfCorrectAnswersToNotQuestions);
        statData.Add("numberOfCorrectAnswersToStandardQuestions", numberOfCorrectAnswersToStandardQuestions);
        statData.Add("bestStreakCorrectAnswers", bestStreakCorrectAnswers);

        // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);

        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];
        // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
    }

    private void SetStartLevel(int lastLevel)
    {
        level = lastLevel;
        Debug.Log($"Start Level: {level}");
    }

    #region Level

    private void ChangeLevelByAction(eGameLevelChangeStatus levelChangeStatus)
    {
        upCounter = PlayerPrefs.GetInt("TBONTB_UpCounter", 0);
        downCounter = PlayerPrefs.GetInt("TBONTB_DownCounter", 0);

        if (levelChangeStatus == eGameLevelChangeStatus.LEVELUP)
        {
            upCounter++;

            if (upCounter >= levelUpCriteria * 2)
            {
                upCounter = 0;
                downCounter = 0;
                corrects = 0;
                wrongs = 0;

                level++;
                level = Mathf.Clamp(level, 1, levelList.Count);
                Debug.Log($"Level changed to : {level}");

                LevelAnimation(true);
            }
        }

        if (levelChangeStatus == eGameLevelChangeStatus.LEVELDOWN)
        {
            downCounter++;

            if (downCounter >= levelDownCriteria)
            {
                downCounter = 0;
                upCounter = 0;
                corrects = 0;
                wrongs = 0;

                level--;
                level = Mathf.Clamp(level, 1, levelList.Count);
                Debug.Log($"Level changed to : {level}");

                LevelAnimation(false);
            }
        }

        PlayerPrefs.SetInt("TBONTB_UpCounter", upCounter);
        PlayerPrefs.SetInt("TBONTB_DownCounter", downCounter);
    }

    #endregion
}