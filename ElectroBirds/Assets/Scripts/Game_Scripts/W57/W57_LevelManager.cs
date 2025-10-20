using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ElectroBirds;
using Game_Scripts.W57;
using Lean.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using W57;
using Random = UnityEngine.Random;

public class W57_LevelManager : MonoBehaviour
{
    public int levelId;
    [SerializeField] private List<W57_LevelSO> levels = new List<W57_LevelSO>();
    public W57_LevelSO levelSO;

    public bool electricSentToBirds = false;

    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text correctText;
    [SerializeField] private TMP_Text wrongText;

    [SerializeField] private List<W57_Cable> cables = new List<W57_Cable>();
    [SerializeField] private W57_BirdPoolController poolController;
    [SerializeField] private W57_GameManager gameManager;

    [Header("Background Animation"), Space(10)]
    [SerializeField]
    private Sprite[] bgSprites;

    [SerializeField] private Image bgImage;
    [SerializeField] private RectTransform bgRect;
    [SerializeField] private CanvasGroup powerLineContainer;
    [SerializeField] private CanvasGroup birdPoolContainer;
    [SerializeField] private GameObject fxContainer;

    private int numberOfBirds;
    private float flowDuration;
    private float flowInterval;

    private int currentStreak;
    private int bestStreak;
    private int numberOfBirdsSaved;
    private int correctCount;
    private int wrongCount;
    private int numberOfBirdsElectrocuted;

    private float gameTime;
    private bool timerActive;
    private float flashInterval = 0.5f;
    private bool isFlashable = true;
    private int currentNumOfBirds = 0;
    private int maxLevelWKeys;

    public void LevelUp()
    {
        levelId++;
        PlayerPrefs.SetInt("ElectroBirds_Level", levelId);
        SetLevel();
    }

    public void LevelDown()
    {
        levelId--;
        PlayerPrefs.SetInt("ElectroBirds_Level", levelId);
        SetLevel();
    }

    public void SetGameTime()
    {
        // gameTime = WManagers.WRCM.GetGamePlaytime();
        gameTime = 60;
    }

    public void SetRandomBackground()
    {
        var randSprite = bgSprites[Random.Range(0, bgSprites.Length)];
        bgImage.sprite = randSprite;
    }

    private void AssignLevel()
    {
        maxLevelWKeys = levels.Count / 2;
        Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

        levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
        levelSO = levels[levelId - 1];

        levelText.text = $"{LeanLocalization.GetTranslationText("Level")} " + levelId;
    }

    public void SetLevel(bool isFirstLoad = false)
    {
        AssignLevel();

        numberOfBirds = levelSO.numOfBirds;
        flowDuration = levelSO.flowDuration;
        flowInterval = levelSO.flowInterval;

        // StartBackgroundAnimation();

        if (isFirstLoad)
            StartBackgroundAnimation();
        else
            ControlBirds();
    }

    private void StartBackgroundAnimation()
    {
        var introSeq = DOTween.Sequence();
        introSeq.Append(bgRect.DOAnchorPosY(0f, 3f));
        introSeq.InsertCallback(2.5f, () =>
        {
            powerLineContainer.DOFade(1f, 1f);
            fxContainer.SetActive(true);
        });
        introSeq.OnComplete(StartLevel);
    }

    private void StartLevel()
    {
        var flockSeq = DOTween.Sequence();

        for (int i = 0; i < numberOfBirds; i++)
        {
            var bird = poolController.GetObjectFromPool();
            var landingPos = GetAvailableDestination(bird);

            bird.gameObject.SetActive(true);
            landingPos.parentCable.birdsOnCable.Add(bird);
            flockSeq.Join(bird.GoToDestination(landingPos.landingTransform));

            currentNumOfBirds++;
        }

        flockSeq.AppendInterval(1f);
        flockSeq.OnComplete(() =>
        {
            timerActive = true;
            StartCoroutine(SendOverflows());
        });
    }

    private void ControlBirds()
    {
        var flockSeq = DOTween.Sequence();

        if (numberOfBirds > currentNumOfBirds)
        {
            for (int i = 0; i < numberOfBirds - currentNumOfBirds; i++)
            {
                var bird = poolController.GetObjectFromPool();
                var landingPos = GetAvailableDestination(bird);

                bird.gameObject.SetActive(true);
                landingPos.parentCable.birdsOnCable.Add(bird);
                flockSeq.Join(bird.GoToDestination(landingPos.landingTransform));

                currentNumOfBirds++;
            }
        }
        else if (currentNumOfBirds > numberOfBirds)
        {
            for (int i = 0; i < currentNumOfBirds - numberOfBirds; i++)
            {
                var bird = poolController.GetRandomBird();
                var cable = bird.GetStandingCable();

                cable.birdsOnCable.Remove(bird);
                bird.SetLandingPosState(false);

                GameObject destinationTransform = new GameObject();
                destinationTransform.transform.position = new Vector3(50, 50, 100);

                flockSeq.Join(bird.GoToDestination(destinationTransform.transform));
                flockSeq.OnComplete(() => bird.gameObject.SetActive(false));

                currentNumOfBirds--;
            }
        }
    }

    private W57_Cable.LandingPosition GetAvailableDestination(W57_Bird bird)
    {
        var randCable = Random.Range(0, cables.Count);
        var randIndex = Random.Range(0, 2);
        var landingPos = cables[randCable].landingPositions[randIndex];

        while (landingPos.isOccupied)
        {
            randCable = Random.Range(0, cables.Count);
            randIndex = Random.Range(0, 2);
            landingPos = cables[randCable].landingPositions[randIndex];
        }
        landingPos.isOccupied = true;

        bird.SetStandingCable(cables[randCable], landingPos);

        return landingPos;
    }

    private IEnumerator SendOverflows()
    {
        W57_Cable randomCable;

        while (timerActive)
        {
            int r = Random.Range(0, 100);
            if (r <= 100)
            {
                //loop until find a cable with birds
                do
                {
                    randomCable = cables[Random.Range(0, cables.Count)];

                } while (randomCable.birdsOnCable.Count == 0);
            }
            else
                randomCable = cables[Random.Range(0, cables.Count)];

            if (randomCable.birdsOnCable.Count > 0)
                electricSentToBirds = true;
            else
                electricSentToBirds = false;

            randomCable.StartOverflowSequence(flowDuration);

            yield return new WaitForSeconds(flowInterval);
        }
    }

    public void BirdSaved()
    {
        numberOfBirdsSaved++;
        currentStreak++;
        correctCount++;

        if (currentStreak > bestStreak)
        {
            bestStreak = currentStreak;
        }

        if (correctCount >= levelSO.levelUpCriteria * 2)
        {
            correctCount = 0;
            wrongCount = 0;
            LevelUp();
        }

        correctText.text = numberOfBirdsSaved.ToString();
    }

    public void BirdElectrocuted()
    {
        currentStreak = 0;
        numberOfBirdsElectrocuted++;
        wrongCount++;

        if (wrongCount >= levelSO.levelDownCriteria * 2)
        {
            correctCount = 0;
            wrongCount = 0;
            LevelDown();
        }

        wrongText.text = numberOfBirdsElectrocuted.ToString();
    }

    private void Update()
    {
        GameTimeCounter();
    }

    public int CalculateScore()
    {
        return Mathf.Max((numberOfBirdsSaved * levelSO.pointsPerCorrect) - (numberOfBirdsElectrocuted * levelSO.penaltyPoints), 0);
    }

    private void GameTimeCounter()
    {
        if (timerActive)
        {
            gameTime -= Time.deltaTime;

            if (gameTime <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // gameManager.PlayFx("Countdown", 0.7f, 1f);
                FlashRed();
            }

            timeText.text = gameTime.ToString("F0");

            if (!(gameTime < 0)) return;
            timerActive = false;
            gameTime = 0;

            StartOutro()
                .OnComplete(() => gameManager.EndGame(bestStreak, numberOfBirdsSaved, numberOfBirdsElectrocuted));
        }
    }

    private Sequence StartOutro()
    {
        var outroSeq = DOTween.Sequence();
        outroSeq.Join(bgRect.DOAnchorPosY(-3500f, 3f));
        outroSeq.Join(powerLineContainer.DOFade(0f, 0.5f));
        outroSeq.Join(birdPoolContainer.DOFade(0f, 0.5f).OnComplete(() =>
        {
            fxContainer.SetActive(false);
            foreach (var cable in cables)
            {
                cable.gameObject.SetActive(false);
            }
            poolController.SetAllInactive();
        }));

        return outroSeq;
    }

    public void PlaySound(eW57FxSoundStates soundState)
    {
        gameManager.playFxBySoundState(soundState);
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

}