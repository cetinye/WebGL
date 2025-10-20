using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lean.Localization;
using UnityEngine;
using UnityEngine.UI;

namespace W92_Memories_Of_The_Egyptian_Gods
{
    public class GameManager : MonoBehaviour
    {

        public Bridge bridge;
        public static GameManager instance;

        [SerializeField] private LeanLocalization leanLocalization;

        public Camera mainCamera;
        public GameState state;
        public int faceUpCardCount;
        public float timeToWaitBeforeClosingCards;
        public bool tappable = false;
        public List<GameObject> activeCards = new List<GameObject>();
        public List<GameObject> tappedCards = new List<GameObject>();
        public Card card1, card2;
        public GameObject cloudsL, cloudsR;
        public int totalPlayCount = 0;
        public int infoTabletIndex = 0;

        [SerializeField] private float timeBetweenOpeningEachCard;
        [SerializeField] private float timeToShowAllCards;

        private bool isCheckAvailable = true;
        private bool isCloseSelectedCardsRunning = false;
        private bool isStartHideCardsRunning = false;
        private bool isFinishRunning = false;
        private List<int> levelScores = new List<int>();

        [SerializeField] private LevelManager levelManager;

        public Dictionary<string, string> statDescriptions = new Dictionary<string, string>
        {
            { "0-Correct", "Number of correct card matches" },
            { "1-Wrong", "Number of wrong card matches" },
        };

        private void Awake()
        {
            instance = this;

            // leanLocalization.SetCurrentLanguage(LeanLocalization.Instances[0].CurrentLanguage);

            // _ = InitAsync();
        }

        // private async Task InitAsync()
        // {
        //     try
        //     {
        //         await initBaseOperations("Memories_Of_The_Egyptian_Gods");

        //         SetStartLevel(gameScoreViewModel.level);
        //         SetInGameTopbarTimerStatus(false);
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //         throw;
        //     }
        // }

        // Start is called before the first frame update

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

            Debug.Log("Game Manager Start çalıştı");
            SetStartLevel(levelId);
            state = GameState.Idle;
           
        }

        private void Update()
        {
            if (tappedCards.Count == 0)
                isCheckAvailable = true;

            if (tappedCards.Count >= 2 && isCheckAvailable)
            {
                isCheckAvailable = false;
                CheckMatch();
            }
        }

        private void SetStartLevel(int lastLevel)
        { levelManager.levelId = lastLevel;

            LevelManager.instance.AssignLevel();
            UIManager.instance.SetPlayCountText(1);

             
        }

        public void StartLevel()
        {
            UIManager.instance.timeRemaining = 60f;
            totalPlayCount++;
            UIManager.instance.SetPlayCountText();
            activeCards.Clear();
            LevelManager.instance.SpawnCards();
            faceUpCardCount = 0;

            StartCoroutine(OpenAllCards());
        }

        public void CheckMatch()
        {
            card1 = tappedCards[0].GetComponent<Card>();
            card2 = tappedCards[1].GetComponent<Card>();

            if (card1.transform.GetChild(0).GetComponent<Image>().sprite == card2.transform.GetChild(0).GetComponent<Image>().sprite)
            {
                levelManager.correctCount++;
                levelManager.correct++;

                if (!isStartHideCardsRunning)
                {
                    StartHideCards();
                    UIManager.instance.ShowCorrectUI();
                }
            }

            else
            {
                levelManager.wrongCount++;
                levelManager.wrong++;

                if (!isCloseSelectedCardsRunning)
                {
                    StartCloseSelectedCards();
                    UIManager.instance.ShowWrongUI();
                }
            }
        }

        private void CheckEndLevel()
        {
            if (activeCards.Count == 0)
            {
                state = GameState.Idle;

                int downCounter = PlayerPrefs.GetInt("MemoriesOfTheEgyptianGods_DownCounter", 0);

                //increment level index if beaten level less than the required move count
                if (UIManager.instance.moveCounter <= UIManager.instance.numOfAttempts)
                {
                    LevelManager.instance.levelId++;
                    downCounter = 0;
                    PlayerPrefs.SetInt("MemoriesOfTheEgyptianGods_DownCounter", downCounter);

                    if (UIManager.instance.infoTabletLevel)
                    {
                        int indexTablet = PlayerPrefs.GetInt("indexTablet");
                        if (indexTablet + 1 < UIManager.instance.infoTablets.Count)
                        {
                            PlayerPrefs.SetInt("indexTablet", ++indexTablet);
                            UIManager.instance.infoTabletLevel = false;
                        }
                    }
                }
                else
                {
                    downCounter++;
                    if (downCounter >= LevelManager.LevelSO.levelDownCriteria)
                    {
                        LevelManager.instance.levelId--;
                        downCounter = 0;
                    }
                    PlayerPrefs.SetInt("MemoriesOfTheEgyptianGods_DownCounter", downCounter);
                }

                LevelManager.instance.AssignLevel();

                CalculateScore();
                Transition();
            }
        }

        public void CalculateScore()
        {
            Debug.Log("Calculating score...");
            int scorePerRound = Mathf.CeilToInt(1000f / LevelManager.LevelSO.totalRounds);
            Debug.Log($"Score per round: {scorePerRound}");
            int scorePerCorrect = Mathf.CeilToInt((float)scorePerRound / (levelManager.cardAmount / 2f));
            Debug.Log($"Score per correct: {scorePerCorrect}");
            int score = scorePerRound - (scorePerCorrect * levelManager.wrong);
            score = Mathf.Clamp(score, 0, 1000);
            Debug.Log($"Score for this round: {score}");
            levelScores.Add(score);
            Debug.Log($"Total scores: {string.Join(", ", levelScores)}");

            levelManager.correct = 0;
            levelManager.wrong = 0;
        }

        private int CalculateTotalScore()
        {
            int total = 0;
            Debug.Log("Calculating total score...");
            foreach (int score in levelScores)
            {
                total += score;
                Debug.Log($"Adding {score} to total score, current total is {total}");
            }
            Debug.Log($"Total score is {total}");
            return total;
        }


        
    [Serializable]
    public class ResultClass
    {
        public int level;
        public int score;

    }

    ResultClass resultObject = new ResultClass();

        public void Finish()
        {
            if (!isFinishRunning)
            {
                isFinishRunning = true;

                levelManager.score = Mathf.Clamp(CalculateTotalScore(), 0, 1000);

                // gameScoreViewModel.score = levelManager.score;
                // gameScoreViewModel.level = levelManager.levelId;
                RecordStats(levelManager.correctCount, levelManager.wrongCount);

                StartCoroutine(GameOverRoutine());
            }
        }

        public void RecordStats(int correctCount, int falseCount)
        {
            Dictionary<string, object> statData = new Dictionary<string, object>();

            statData.Add("Correct", correctCount);
            statData.Add("False", falseCount);

            // WManagers.WB2B.saveStat(gameScoreViewModel.gameId, statData, statDescriptions);


             resultObject.level = levelManager.levelId;
        resultObject.score = levelManager.score;
        // Debug.Log("skor   " + score);
        string json = JsonUtility.ToJson(resultObject);
        bridge.SendToJSJson(json);


            var mainStatKeyWithIndex = statDescriptions.ToList()[0].Key;
            var mainStatKey = string.Concat(mainStatKeyWithIndex.Where(char.IsLetter));

            var mainStatCurrentValue = statData[mainStatKey];
            // gameScoreViewModel.stat = long.Parse(mainStatCurrentValue.ToString());
        }

        private IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(1f);
            // SubmitScore(gameScoreViewModel, gameScoreConfigModel, eGameOverStatus.GAMEOVER);
            // GameOver();
        }

        public void StartHideCards()
        {
            isStartHideCardsRunning = true;
            StartCoroutine(HideCards());
        }

        public void StartCloseSelectedCards()
        {
            isCloseSelectedCardsRunning = true;
            StartCoroutine(CloseSelectedCards());
        }

        public int GetInfoTabletIndex(int level)
        {
            switch (level)
            {
                case 3:
                    infoTabletIndex = 0;
                    break;

                case 4:
                    infoTabletIndex = 1;
                    break;

                case 5:
                    infoTabletIndex = 2;
                    break;

                case 6:
                    infoTabletIndex = 3;
                    break;

                case 9:
                    infoTabletIndex = 4;
                    break;

                case 10:
                    infoTabletIndex = 5;
                    break;

                case 11:
                    infoTabletIndex = 6;
                    break;

                case 12:
                    infoTabletIndex = 7;
                    break;

                case 15:
                    infoTabletIndex = 8;
                    break;

                case 16:
                    infoTabletIndex = 9;
                    break;

                case 17:
                    infoTabletIndex = 10;
                    break;

                case 18:
                    infoTabletIndex = 11;
                    break;

                case 21:
                    infoTabletIndex = 12;
                    break;

                case 22:
                    infoTabletIndex = 13;
                    break;

                case 23:
                    infoTabletIndex = 14;
                    break;

                case 24:
                    infoTabletIndex = 15;
                    break;

                case 27:
                    infoTabletIndex = 16;
                    break;

                case 28:
                    infoTabletIndex = 17;
                    break;

                case 29:
                    infoTabletIndex = 18;
                    break;

                case 30:
                    infoTabletIndex = 19;
                    break;

                case 31:
                    infoTabletIndex = 20;
                    break;

                case 32:
                    infoTabletIndex = 21;
                    break;

                case 33:
                    infoTabletIndex = 22;
                    break;

                case 34:
                    infoTabletIndex = 23;
                    break;

                case 35:
                    infoTabletIndex = 24;
                    break;

                case 36:
                    infoTabletIndex = 25;
                    break;

                case 37:
                    infoTabletIndex = 26;
                    break;

                case 38:
                    infoTabletIndex = 27;
                    break;

                default:
                    infoTabletIndex = 27;
                    break;
            }

            return infoTabletIndex;
        }

        IEnumerator OpenAllCards()
        {
            //disable user tapping any cards
            tappable = false;

            //wait at the beginning of the level
            yield return new WaitForSeconds(0.5f);
            yield return new WaitForEndOfFrame();

            //open cards in order
            for (int i = 0; i < activeCards.Count; i++)
            {
                activeCards[i].GetComponent<Card>().FlipCardRoutine();
                AudioManager.instance.PlayOneShot("CardSpawn");
                yield return new WaitForSeconds(timeBetweenOpeningEachCard);
            }

            //wait for given time for player to recognize cards
            yield return new WaitForSeconds(timeToShowAllCards);

            StartCoroutine(CloseAllCards());
        }

        IEnumerator CloseAllCards()
        {
            for (int i = 0; i < activeCards.Count; i++)
            {
                activeCards[i].GetComponent<Card>().FlipCardRoutine();
                AudioManager.instance.PlayOneShot("CardTapped");
                yield return new WaitForSeconds(timeBetweenOpeningEachCard);
            }

            AnimateCamera.instance.SwitchCameraToMain();
            state = GameState.Playing;
            faceUpCardCount = 0;
            tappable = true;
        }

        IEnumerator CloseSelectedCards()
        {
            if (tappedCards.Count >= 2)
            {
                yield return new WaitForSeconds(timeToWaitBeforeClosingCards);

                card1.Tapped();
                card2.Tapped();

                tappedCards.Remove(card1.gameObject);
                tappedCards.Remove(card2.gameObject);

                yield return new WaitForSeconds(card2.timeToFlip * 2f);

                faceUpCardCount = 0;
                isCheckAvailable = true;
                isCloseSelectedCardsRunning = false;
            }
        }

        //called to hide correctly matched cards
        IEnumerator HideCards()
        {
            if (tappedCards.Count >= 2)
            {
                yield return new WaitForSeconds(timeToWaitBeforeClosingCards);

                card1.transform.GetChild(0).GetComponent<Image>().enabled = false;
                card1.GetComponent<Image>().enabled = false;
                card2.transform.GetChild(0).GetComponent<Image>().enabled = false;
                card2.GetComponent<Image>().enabled = false;

                activeCards.Remove(card1.gameObject);
                activeCards.Remove(card2.gameObject);

                tappedCards.Remove(card1.gameObject);
                tappedCards.Remove(card2.gameObject);

                card1.enabled = false;
                card2.enabled = false;

                card1.GetComponent<BoxCollider2D>().enabled = false;
                card2.GetComponent<BoxCollider2D>().enabled = false;

                faceUpCardCount = 0;
                isCheckAvailable = true;
                isStartHideCardsRunning = false;

                CheckEndLevel();
            }
        }

        public void Transition()
        {
            UIManager.instance.startButton.interactable = false;
            AudioManager.instance.PlayOneShot("Transition");
            AnimateCamera.instance.SwitchCameraToSecond();
            UIManager.instance.StartEnterClouds();
        }

        public enum GameState
        {
            Menu,
            Idle,
            Success,
            Failed,
            Playing
        }
    }
}