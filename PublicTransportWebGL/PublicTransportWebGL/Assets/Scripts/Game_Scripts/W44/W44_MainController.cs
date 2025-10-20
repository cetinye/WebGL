using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Lean.Localization;
using Public_Transport;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using W44;
using Random = UnityEngine.Random;

public class W44_MainController : MonoBehaviour
{
    public Bridge bridge;

    [SerializeField] private LeanLocalization leanLocalization;
    [SerializeField] private List<W44_LevelSO> levels;
    [SerializeField] private W44_LevelSO levelSO;

    private int numberOfBuildingsBetweenStations;
    private int totalNumberOfStations;
    private int totalNumberOfPassengers;
    private int maxNumberOfPassengersInaStation;

    public GameObject[] standardBuildings;
    public GameObject[] secondStandardBuildings;
    public GameObject[] stationBuildings;
    public Sprite[] passengerIndicatorSprites;

    private List<Vector2> stationPos = new List<Vector2>();
    private int[] correctAnswers;
    private int[] spawnCounter;
    private int[] stationIDs;

    private float xOffset = 4.86f;
    private float yOffset = 2.81f;
    private float zOffset = 0.00001f;
    private Vector3 nextSpawnPos = new Vector3(0, 0);
    private Vector3 nextSpawnPosOffset = new Vector3(-4.86f, -2.81f);
    private Vector3 nextLevelSpawnPos = new Vector3(0, 0);

    private Vector3 secondStreetSpawnPos = new Vector3(0, 0, 0);
    private Vector3 secondStreetSpawnPosOffset = new Vector3(-10.6f, 4.9f, 0.3f);

    public GameObject Bus; //bus is inside a container in order to get it on the road offset
    public GameObject BusSprite, rimsBack, rimsFront;

    public GameObject Question;
    public GameObject canvas;

    public GameObject buildingContainer, passengerContainer;

    public Text questionText;

    public W44_Passenger passengerInstance;

    public List<W44_Passenger> passengers = new List<W44_Passenger>();
    public List<GameObject> buildings = new List<GameObject>();
    public List<GameObject> streetObjects = new List<GameObject>();
    public List<GameObject> secondStreetBuildings = new List<GameObject>();

    private int currentBusStation = -1;
    private int nextBusStation = -1;

    public int selectedAnswerFromButtons;
    public GameObject selectedButton;
    public Button[] answerButtons;

    private bool isAnswerTrue;

    private float numberOfCorrectAnswers, numberOfWrongAnswers, numberOfTimedOutAnswers;

    public GameObject feedbackIndicator;
    public Sprite[] trueFalseSprites;

    private int randQuestionIndex = -1;
    private bool levelCompleted;
    private bool passFirstStation = true;

    public GameObject getReadyContainer, levelCompletedContainer;
    public TextMeshProUGUI getReadyText, levelCompletedText;

    public GameObject backgroundBlocker;

    public RuntimeAnimatorController[] passengerAnimations;
    public Sprite[] standingSprites;

    private int totalNumberOfBuildingsForSecondStreet;
    private Sequence wheelSequence, busSequence, camSeq;

    public Camera cam;
    private Sequence getOnSequence, getOffSequence;

    public Transform frontDoorPos, backDoorPos, getOffDestination;
    public eW44FxSoundStates EW44FxSoundStates;
    public eW44EnvironmentSoundStates EW44EnvironmentSoundState;
    public W44_Constants W44Constants = new W44_Constants();

    public AudioSource busEngineSoundSource;
    public AudioSource busAmbianceSoundSource;

    private int numberOfPlays, gameOverLimit = 1;

    public GameObject busUI;

    public int tutorialStepID;
    private bool setLevelCalled = false;
    private bool questionAnswered;
    private float getOnBoardTweenDuration = 1;

    private int level = 1;
    public int[] numberOfQuestionsEachLevel = new[] { 0, 6, 6, 6, 6, 6, 8, 8, 8, 8, 8 };
    public float[] answeringTimerForEachLevel = new[] { 0, 7f, 6.8f, 6.6f, 6.4f, 6.2f, 6f, 5.8f, 5.6f, 5.4f, 5.2f };
    public int[] wrongAnswerLimitForEachLevel = new[] { 0, 1, 1, 1, 0, 3, 3, 2, 1, 0 };
    private int[] levelScores = new[] { 0, 200, 240, 310, 375, 510, 625, 750, 820, 935, 1000 };
    private bool timesUp;
    private Coroutine timer;

    [SerializeField] private Transform busDoorTr;
    private Vector3 busDoorClosePos = new Vector3(0.307999998f, -0.319000006f, 0);
    private Vector3 busDoorOpenPos = new Vector3(0.727999985f, -0.061999999f, 0);

    //stats
    public int numberOfPassengersCarriedTotal;

    public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
    {
        {"0-numberOfCorrectAnswers", "Given correct answers "},
        {"1-numberOfWrongAnswers", "Given incorrect answers "},
        {"2-numberOfTimedOutAnswers", "Timed out answers "},
        {"3-numberOfPassengersCarriedTotal", "Total number of passengers carried "},
    };

    private int maxLevelWKeys;

    private void Awake()
    {
        leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

        maxLevelWKeys = levels.Count;
        Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

        busEngineSoundSource.enabled = false;
        busAmbianceSoundSource.enabled = false;
        Bus.SetActive(false);
        canvas.SetActive(false);
        busUI.SetActive(false);
    }

    private void BusTween()
    {
        busSequence = DOTween.Sequence();
        busSequence.AppendInterval(0.5f);
        busSequence.Append(BusSprite.transform.DOLocalMoveY(0.3f, 0.5f));
        busSequence.Append(BusSprite.transform.DOLocalMoveY(0.2f, 0.5f));
        busSequence.SetLoops(-1);

        wheelSequence = DOTween.Sequence();
        wheelSequence.Append(
            rimsFront.transform.DOLocalRotate(new Vector3(0, 0, -360), 0.1f, RotateMode.FastBeyond360));
        wheelSequence.Join(rimsBack.transform.DOLocalRotate(new Vector3(0, 0, -360), 0.1f, RotateMode.FastBeyond360));
        wheelSequence.SetLoops(-1);
        wheelSequence.SetEase(Ease.Linear);
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
        StartGame();
        Bus.SetActive(true);
        canvas.SetActive(true);
    }

    void StartGame()
    {
        BusTween();
        playGetReadyTweenAndStartLevel();
    }


    private void playGetReadyTweenAndStartLevel()
    {
        setLevel();
        busEngineSoundSource.enabled = false;
        busAmbianceSoundSource.enabled = false;
        backgroundBlocker.SetActive(true);
        feedbackIndicator.SetActive(false);
        Question.SetActive(false);
        levelCompletedContainer.SetActive(false);
        // getReadyText.text = "Level " + level + "\nAre you ready?";
        getReadyText.text = LeanLocalization.GetTranslationText("Level") + level + "\n" +
        LeanLocalization.GetTranslationText("AreYouReady");
        getReadyContainer.SetActive(true);
        AudioManager.instance.PlayOneShot(SoundType.W44_GetReadySound);

        Sequence getReadySequence = DOTween.Sequence();

        getReadyContainer.transform.localPosition = new Vector2(0, -2005);
        getReadySequence.Append(getReadyContainer.transform.DOLocalMoveY(0, 1f));
        getReadySequence.Append(getReadyContainer.transform.DOLocalMoveY(0, 1f));
        // getReadySequence.Join(busUILeftToMiddle());
        getReadySequence.Append(getReadyContainer.transform.DOLocalMoveY(-2005, 0.5f));
        getReadySequence.OnComplete(createLevel);
    }

    private Tween busUILeftToMiddle()
    {
        var busUIPos = busUI.transform.position;

        var leftPos = cam.ViewportToScreenPoint(new Vector3(-0.3f, 0.3f, busUIPos.z));
        var middlePos = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.75f, busUIPos.z));

        busUI.transform.position = leftPos;

        return busUI.transform.DOMove(middlePos, 1.5f).OnStart(() => busUI.SetActive(true))
            .OnComplete(() => busUI.SetActive(false));
    }

    private Tween busUIMiddleToRight()
    {
        var busUIPos = busUI.transform.position;

        var middlePos = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.3f, busUIPos.z));
        var rightPos = cam.ViewportToScreenPoint(new Vector3(1.3f, 0.75f, busUIPos.z));

        busUI.transform.position = middlePos;
        return busUI.transform.DOMove(rightPos, 2f).OnComplete(() => busUI.SetActive(false));
    }

    private void setLevel()
    {
        int levelDownCounter = PlayerPrefs.GetInt("PT_LevelDownCounter", 0);
        int levelUpCounter = PlayerPrefs.GetInt("PT_LevelUpCounter", 0);

        if (!setLevelCalled)
        {
            setLevelCalled = true;
            return;
        }

        numberOfPlays++;

        if (numberOfWrongAnswers + numberOfTimedOutAnswers <= levelSO.wrongAnswerLimit)
        {
            levelUpCounter++;
            if (levelUpCounter >= 2)
            {
                level++;
                level = Mathf.Clamp(level, 1, maxLevelWKeys);
                levelDownCounter = 0;
                levelUpCounter = 0;
            }
        }
        else if (levelDownCounter == 1)
        {
            level--;
            level = Mathf.Clamp(level, 1, maxLevelWKeys);
            levelDownCounter = 0;
            levelUpCounter = 0;
        }
        else
        {
            levelDownCounter++;
        }

        PlayerPrefs.SetInt("PT_LevelDownCounter", levelDownCounter);
        PlayerPrefs.SetInt("PT_LevelUpCounter", levelUpCounter);
        Debug.LogWarning("levelUpCounter: " + levelUpCounter);

        if (numberOfPlays == gameOverLimit)
        {
            int score = Mathf.Clamp(Mathf.CeilToInt((float)CalculateScore() / levelSO.maxInGame * 1000), 0, 1000);

            resultObject.level = level;
            resultObject.score = score;
            string json = JsonUtility.ToJson(resultObject);
            bridge.SendToJSJson(json);

            RecordStats();
        }

    }

    [Serializable]
    public class ResultClass
    {
        public int level;
        public int score;

    }

    ResultClass resultObject = new ResultClass();

    private int CalculateScore()
    {
        int maxScore = levelSO.maxInGame;
        return (int)Mathf.Clamp((numberOfCorrectAnswers * levelSO.pointsPerCorrectAnswer) - ((numberOfWrongAnswers + numberOfTimedOutAnswers) * 50), 0, maxScore);
    }

    public void RecordStats()
    {
        Dictionary<string, object> statData = new Dictionary<string, object>();

        statData.Add("numberOfCorrectAnswers", numberOfCorrectAnswers);
        statData.Add("numberOfWrongAnswers", numberOfWrongAnswers);
        statData.Add("numberOfTimedOutAnswers", numberOfTimedOutAnswers);
        statData.Add("numberOfPassengersCarriedTotal", numberOfPassengersCarriedTotal);


        var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
        var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

        var mainStatCurrentValue = statData[mainStatKey];
    }

    private void setLevelParameters()
    {
        level = Mathf.Clamp(level, 1, maxLevelWKeys);
        levelSO = levels[level - 1];

        numberOfBuildingsBetweenStations = Random.Range(3, 5);

        totalNumberOfStations = levelSO.numOfQuestions;
        totalNumberOfStations = Mathf.Min(totalNumberOfStations, 10);

        totalNumberOfBuildingsForSecondStreet =
            numberOfBuildingsBetweenStations * (totalNumberOfStations + 2) + totalNumberOfStations;

        totalNumberOfPassengers = level + Random.Range(3, 6);
        maxNumberOfPassengersInaStation = Random.Range(3, 6);
    }

    private void createLevel()
    {
        setLevelParameters();

        backgroundBlocker.SetActive(true);
        getReadyContainer.SetActive(false);
        feedbackIndicator.SetActive(false);
        Question.SetActive(false);
        ControlButtonInteractivity(true);

        correctAnswers = new int[totalNumberOfStations];
        spawnCounter = new int[totalNumberOfStations];
        stationIDs = new int[totalNumberOfStations];

        passFirstStation = true;
        levelCompleted = false;
        isAnswerTrue = false;
        levelCompleted = false;
        timesUp = false;

        selectedAnswerFromButtons = 0;
        nextBusStation = -1;
        numberOfCorrectAnswers = 0;
        numberOfWrongAnswers = 0;

        nextLevelSpawnPos += nextSpawnPosOffset * 3;
        nextSpawnPos = nextLevelSpawnPos;

        secondStreetSpawnPos = nextSpawnPos;
        secondStreetSpawnPos += secondStreetSpawnPosOffset;

        foreach (var passenger in passengers)
        {
            Destroy(passenger.gameObject);
        }

        foreach (var building in buildings)
        {
            Destroy(building.gameObject);
        }

        foreach (var building in secondStreetBuildings)
        {
            Destroy(building.gameObject);
        }

        passengers.Clear();
        stationPos.Clear();

        int currentTotalNumberOfStations = totalNumberOfStations;

        spawnBetweenStations(false);
        while (currentTotalNumberOfStations > 0)
        {
            spawnBetweenStations(false);
            spawnStations();
            currentTotalNumberOfStations--;
        }

        spawnBetweenStations(true);
        spawnPassengers();
        spawnSecondStreetBuildings();

        foreach (var pass in passengers)
        {
            for (int i = 0; i < stationIDs.Length; i++)
            {
                if (stationIDs[i] == stationIDs[pass.getEndingStationID()] && i > pass.getStartingStationID())
                {
                    pass.acceptableEndingStationIndexes.Add(i);
                    correctAnswers[i]++;
                }
            }
        }

        backgroundBlocker.SetActive(false);
        busTravel();
    }

    private void Update()
    {
        if (levelCompleted)
        {
            playLevelCompletedTween();
        }
    }

    private void busTravel()
    {
        busEngineSoundSource.enabled = true;
        busAmbianceSoundSource.enabled = true;

        BusDoorState(false);
        nextBusStation++;
        busSequence.Play();
        wheelSequence.Play();
        Bus.transform.DOMove(stationPos[nextBusStation], 4f).OnComplete(busArrived);
        cameraZoomOut();
    }

    private void cameraZoomIn()
    {
        cam.transform.DOLocalMove(new Vector3(0f, 0f, -10), 2f);
        cam.DOOrthoSize(7.5f, 1f);
    }

    private void cameraZoomOut()
    {
        cam.transform.DOLocalMove(new Vector3(1f, 3f, -10), 2f);
        cam.DOOrthoSize(8.5f, 1f);
    }


    private void busArrived()
    {
        AudioManager.instance.PlayOneShot(SoundType.W44_DoorSound);

        wheelSequence.Pause();
        busSequence.Pause();
        busEngineSoundSource.enabled = false;

        var currentRotation = BusSprite.transform.rotation.eulerAngles;
        BusSprite.transform.DOLocalRotate(new Vector3(currentRotation.x, currentRotation.y, -5f), 1f).OnComplete(
            () => { BusSprite.transform.DOLocalRotate(new Vector3(currentRotation.x, currentRotation.y, 0f), 0.3f); });

        BusDoorState(true);
        selectQuestion();
        askQuestion(randQuestionIndex);
    }

    private void BusDoorState(bool state)
    {
        if (state)
        {
            busDoorTr.DOLocalMove(busDoorOpenPos, 0.5f);
        }
        else
        {
            busDoorTr.DOLocalMove(busDoorClosePos, 0.5f);
        }
    }

    private void passengersGetOnboard()
    {
        cameraZoomIn();

        getOnSequence = DOTween.Sequence();

        var getOnBoardDurationOffset = 0f;

        int numberOfPlayers = spawnCounter[nextBusStation];
        for (int i = 0; i < passengers.Count; i++)
        {
            var passenger = passengers[i];
            if (passenger.getStartingStationID() == nextBusStation)
            {
                passenger.playAnim();
                getOnSequence.Join(passenger.transform
                    .DOMove(frontDoorPos.position, getOnBoardTweenDuration + getOnBoardDurationOffset)
                    .SetEase(Ease.Linear).OnComplete(() =>
                    {
                        passenger.stopAnim();

                        passenger.currentlyOnBoard = true;
                        passenger.collider.enabled = false;
                        numberOfPlayers--;

                        passenger.Indicator_sp.DOFade(0f, 1f);
                        passenger.sp.DOFade(0f, 1f).OnComplete(() =>
                        {
                            passenger.sp.DOFade(1f, 0f);
                            passenger.makeInvisible();
                        });
                    }));
                getOnBoardDurationOffset += 1.45f;
            }
        }

        getOnSequence.AppendInterval(0.5f);
    }

    private void passengersGetOffBoard()
    {
        cameraZoomIn();

        getOffSequence = DOTween.Sequence();

        for (int i = 0; i < passengers.Count; i++)
        {
            var passenger = passengers[i];
            if (passenger.currentlyOnBoard && passenger.getStartingStationID() != nextBusStation &&
                passenger.acceptableEndingStationIndexes.Contains(nextBusStation))
            {
                passenger.setSpriteRotation(1);
                passenger.currentlyOnBoard = false;
                passenger.transform.position = backDoorPos.position;

                Tween passTween = passenger.transform.DOMove(getOffDestination.position, 1.5f).SetEase(Ease.Linear);
                passTween.OnStart(() =>
                {
                    passenger.makeVisible();
                    passenger.playAnim();
                }).OnComplete(() =>
                {
                    passenger.stopAnim();
                    passenger.sp.DOFade(0f, 1f);
                });
                getOffSequence.Append(passTween);

                foreach (var count in passenger.acceptableEndingStationIndexes)
                {
                    correctAnswers[count]--;
                }
            }
        }

        getOffSequence.AppendInterval(0.5f);
    }

    private void selectQuestion()
    {
        randQuestionIndex = Random.Range(0, 2);
    }

    private bool checkIfFirstStation()
    {
        if (passFirstStation && nextBusStation == 0)
        {
            passengersGetOnboard();
            getOnSequence.OnComplete(busTravel);
            passFirstStation = false;
            return true;
        }

        return false;
    }

    private void ControlButtonInteractivity(bool isActive)
    {
        foreach (var button in answerButtons)
        {
            button.interactable = isActive;
        }
    }

    public void checkGivenAnswerCorrect()
    {
        ControlButtonInteractivity(false);

        StopCoroutine(timer);
        if (timesUp)
        {
            numberOfTimedOutAnswers++;
            feedbackTween(1, timesUp: true);
        }
        else if (isAnswerTrue && selectedAnswerFromButtons == 1)
        {
            numberOfCorrectAnswers++;
            feedbackTween(0);
        }
        else if (!isAnswerTrue && selectedAnswerFromButtons == 2)
        {
            numberOfCorrectAnswers++;
            feedbackTween(0);
        }
        else
        {
            numberOfWrongAnswers++;
            feedbackTween(1);
        }

        passengersGetOffBoard();

        getOffSequence.OnComplete(() =>
        {
            if (nextBusStation != stationPos.Count - 1)
            {
                var currentRotation = BusSprite.transform.rotation.eulerAngles;
                BusSprite.transform.DOLocalRotate(new Vector3(currentRotation.x, currentRotation.y, 5f), 1f)
                    .OnComplete(
                        () =>
                        {
                            BusSprite.transform.DOLocalRotate(new Vector3(currentRotation.x, currentRotation.y, 0f),
                                0.3f);
                        });
                busTravel();
            }
            else
            {
                //Debug.Log("Game over");
                playLevelCompletedTween();
            }
        });
    }

    private void askQuestion(int randomQuestionType)
    {
        if (checkIfFirstStation()) return;

        AudioManager.instance.PlayOneShot(SoundType.W44_AskQuestionSound);

        if (randomQuestionType == 0)
        {
            passengersGetOnboard();
            getOnSequence.OnComplete(() =>
            {
                timer = StartCoroutine(questionTimer());
                ControlButtonInteractivity(true);
                Question.SetActive(true);
                int askedNumberOfPassengers = Random.Range(0, correctAnswers[nextBusStation] + 3);
                questionText.text =
                    LeanLocalization.GetTranslationText("NumOfPassengersToGetOff") +
                    "\n" + askedNumberOfPassengers + " ?";
                isAnswerTrue = askedNumberOfPassengers == correctAnswers[nextBusStation];
            });
        }
        else if (randomQuestionType == 1)
        {
            var getOnAndOff = DOTween.Sequence();
            passengersGetOnboard();
            passengersGetOffBoard();
            getOnAndOff.Join(getOnSequence);
            getOnAndOff.Join(getOffSequence);

            getOnAndOff.OnComplete(() =>
            {
                timer = StartCoroutine(questionTimer());
                ControlButtonInteractivity(true);
                Question.SetActive(true);
                int currentNumberOfPassengers = 0;

                foreach (var passenger in passengers)
                {
                    if (passenger.currentlyOnBoard)
                    {
                        currentNumberOfPassengers++;
                    }
                }

                var questionAnswer = Random.Range(currentNumberOfPassengers - 3, currentNumberOfPassengers + 3);
                questionAnswer = Math.Max(0, questionAnswer);
                //questionText.text = "Aractaki toplam yolcu sayisi = " + questionAnswer + " ?";
                questionText.text =
                    LeanLocalization.GetTranslationText("NumOfTotalPassengers") +
                    questionAnswer + " ?";
                isAnswerTrue = questionAnswer == currentNumberOfPassengers;
            });
        }
    }

    private IEnumerator questionTimer()
    {
        timesUp = false;

        var time = 0f;
        while (time < levelSO.answerTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        timesUp = true;
        checkGivenAnswerCorrect();
    }

    private void levelDoneWait()
    {
        backgroundBlocker.SetActive(true);
        Question.SetActive(false);
        getReadyContainer.SetActive(false);
        //levelCompletedText.text = "Level " + (level) + "\ncompleted";
        levelCompletedText.text = LeanLocalization.GetTranslationText("Level") + (level) +
                                  "\n" + LeanLocalization.GetTranslationText("Completed");
        levelCompletedContainer.SetActive(true);
        AudioManager.instance.PlayOneShot(SoundType.W44_LevelCompleted);

        busUI.SetActive(true);
    }

    private void playLevelCompletedTween()
    {
        AudioManager.instance.PlayOneShot(SoundType.W44_LevelCompleted);

        Sequence levelCompletedSequence = DOTween.Sequence();

        levelCompletedContainer.transform.localPosition = new Vector2(0, 2000);
        levelCompletedSequence.AppendInterval(1.5f);
        levelCompletedSequence.AppendCallback(levelDoneWait);
        levelCompletedSequence.Append(levelCompletedContainer.transform.DOLocalMoveY(0, 1f));
        levelCompletedSequence.Append(levelCompletedContainer.transform.DOLocalMoveY(0, 1f));
        levelCompletedSequence.Join(busUIMiddleToRight());
        levelCompletedSequence.Append(levelCompletedContainer.transform.DOLocalMoveY(2000, 0.5f));
        levelCompletedSequence.OnComplete(() => setLevel());
        //levelCompletedSequence.OnComplete(playGetReadyTweenAndStartLevel);

        // gameScoreViewModel.score = levelScores[level];
        // setLevel();
        //gameScoreViewModel.level = level;
        //RecordStats();
        //SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
        //GameOver();
    }

    private void feedbackTween(int isCorrect, bool timesUp = false)
    {
        if (isCorrect == 0)
        {
            AudioManager.instance.PlayOneShot(SoundType.W44_CorrectAnswer);
        }
        else
        {
            AudioManager.instance.PlayOneShot(SoundType.W44_WrongAnswer);
        }

        if (!timesUp)
        {
            selectedButton.transform.DOScale(0.65f, 0.1f).SetEase(Ease.Linear).OnComplete
            (() =>
            {
                selectedButton.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce).OnComplete(
                    () =>
                    {
                        ControlButtonInteractivity(true);
                        Question.SetActive(false);
                    }
                );
            });
        }
        else
        {
            Question.SetActive(false);
        }

        AudioManager.instance.PlayOneShot(SoundType.W44_AskQuestionSound);

        feedbackIndicator.SetActive(true);
        feedbackIndicator.GetComponent<Image>().sprite = trueFalseSprites[isCorrect];
        feedbackIndicator.transform.DOScale(new Vector3(1.15f, 1.15f, 1f), 1.75f).OnComplete(moveToNextStation);
    }

    private void moveToNextStation()
    {
        feedbackIndicator.SetActive(false);
        feedbackIndicator.transform.localScale = new Vector3(1, 1, 1);
    }

    private void spawnBetweenStations(bool lastBuildings)
    {
        if (lastBuildings)
        {
            nextLevelSpawnPos = nextSpawnPos;
        }

        for (var i = 0; i < numberOfBuildingsBetweenStations; i++)
        {
            var temp = Random.Range(0, standardBuildings.Length);
            GameObject n_building = Instantiate(standardBuildings[temp], nextSpawnPos, quaternion.identity,
                buildingContainer.transform);
            buildings.Add(n_building);
            spawnStreetObject(n_building);
            updateSpawnPos();
        }
    }

    private void spawnStreetObject(GameObject building)
    {
        if (Random.Range(0, 100) > 60) return;

        var randomObjectIndex = Random.Range(0, streetObjects.Count);
        GameObject streetObject = Instantiate(streetObjects[randomObjectIndex], Vector3.zero, quaternion.identity,
            building.transform);
        Vector3 randomObjectPos = new Vector3(-5.33f, 4.76f, 0.1f);
        streetObject.transform.localPosition = randomObjectPos;
    }

    private void spawnSecondStreetBuildings()
    {
        for (var i = 0; i < totalNumberOfBuildingsForSecondStreet; i++)
        {
            var temp = Random.Range(0, secondStandardBuildings.Length);
            GameObject n_building = Instantiate(secondStandardBuildings[temp], secondStreetSpawnPos,
                quaternion.identity, buildingContainer.transform);
            secondStreetBuildings.Add(n_building);
            secondStreetSpawnPos = new Vector3(secondStreetSpawnPos.x + xOffset, secondStreetSpawnPos.y + yOffset,
                secondStreetSpawnPos.z + zOffset);
        }
    }

    private void spawnStations()
    {
        var temp = Random.Range(0, stationBuildings.Length);
        GameObject n_station = Instantiate(stationBuildings[temp], nextSpawnPos, quaternion.identity,
            buildingContainer.transform);
        stationPos.Add(nextSpawnPos);
        stationIDs[stationPos.Count - 1] = temp;
        buildings.Add(n_station);
        updateSpawnPos();
    }

    private void updateSpawnPos()
    {
        nextSpawnPos = new Vector3(nextSpawnPos.x + xOffset, nextSpawnPos.y + yOffset, nextSpawnPos.z + zOffset);
    }

    private void spawnPassengers()
    {
        numberOfPassengersCarriedTotal += totalNumberOfPassengers;
        for (int i = 0; i < totalNumberOfPassengers; i++)
        {
            int startStationIndex = Random.Range(0, stationPos.Count - 1);

            int endingStationIndex = Random.Range(startStationIndex + 1, stationPos.Count);

            while (spawnCounter[startStationIndex] >= maxNumberOfPassengersInaStation)
            {
                startStationIndex = Random.Range(0, stationPos.Count - 1);
                endingStationIndex = Random.Range(startStationIndex + 1, stationPos.Count);
            }

            List<int> possibleDestinationIDs = new List<int>();
            for (int j = startStationIndex + 1; j < stationPos.Count; j++)
            {
                possibleDestinationIDs.Add(stationIDs[j]);
            }

            if (possibleDestinationIDs.Distinct().Count() > 1)
            {
                while (stationIDs[startStationIndex] == stationIDs[endingStationIndex])
                {
                    startStationIndex = Random.Range(0, stationPos.Count - 1);
                    endingStationIndex = Random.Range(startStationIndex + 1, stationPos.Count);
                }
            }

            var passengerSpawnPos =
                new Vector3(stationPos[startStationIndex].x + 2.3f + (i * 0.1f),
                    stationPos[startStationIndex].y - 0.2f - (i * 0.1f),
                    i * 0.00001f);

            W44_Passenger new_Passenger = Instantiate(passengerInstance, passengerSpawnPos,
                quaternion.identity, passengerContainer.transform);
            passengers.Add(new_Passenger);

            int randomPassengerIndex = Random.Range(0, passengerAnimations.Length);
            new_Passenger.setStandingSprite(standingSprites[randomPassengerIndex]);
            new_Passenger.setSpriteRotation(-1);
            new_Passenger.setAnimation(passengerAnimations[randomPassengerIndex]);
            new_Passenger.setIndicatorSprite(passengerIndicatorSprites[stationIDs[endingStationIndex]]);
            new_Passenger.setStartingStationID(startStationIndex);
            new_Passenger.setEndingStationID(endingStationIndex);

            spawnCounter[startStationIndex] += 1;
        }
    }

    private void SetStartLevel(int lastLevel)
    {
        level = lastLevel;
        level = Mathf.Clamp(level, 1, maxLevelWKeys);
    }
}