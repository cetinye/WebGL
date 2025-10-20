using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Localization;
using TMPro;
using UnityEngine;

namespace Name_It_Or_Run_It
{
    public class LevelManager : MonoBehaviour
    {
        public static Action OnGameStateChange;

        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text levelText;

        [SerializeField] private UIManager uiManager;

        public int levelId;
        public LevelSO levelSO;
        public List<LevelSO> levels = new List<LevelSO>();

        [SerializeField] private Item selectedItem;
        [SerializeField] private Window selectedWindow;
        [SerializeField] private int chosenLetterIndex;
        [SerializeField] private int answeredQuestionCount = 0;
        [SerializeField] private int correctCount = 0;
        [SerializeField] private int wrongCount = 0;
        private List<int> levelScores = new List<int>();
        private int downCounter;
        private int upCounter;
        private int correct;
        private int wrong;
        public int currentLevelId;
        [SerializeField] private int questionIndex;
        [SerializeField] private int shownWindowCount;
        private bool isLevelSuccess;
        public int itemCountToBeShown;
        public float timeToShowItem;
        public int score;

        [SerializeField] private List<Sprite> easyItemSprites = new List<Sprite>();
        [SerializeField] private List<Item> easyItems = new List<Item>();
        [SerializeField] private List<Item> easyItemsClone;
        [SerializeField] private List<Sprite> mediumItemSprites = new List<Sprite>();
        [SerializeField] private List<Item> mediumItems = new List<Item>();
        [SerializeField] private List<Item> mediumItemsClone;
        [SerializeField] private List<Sprite> hardItemSprites = new List<Sprite>();
        [SerializeField] private List<Item> hardItems = new List<Item>();
        [SerializeField] private List<Item> hardItemsClone;
        [SerializeField] private List<RectTransform> windowPositions = new List<RectTransform>();
        [SerializeField] private List<Window> windows = new List<Window>();
        [SerializeField] private List<LetterWindow> letterWindows = new List<LetterWindow>();
        [SerializeField] private List<string> questions = new List<string>();
        [SerializeField] private List<Item> chosenList;

        private List<int> bonusScores = new List<int>();

        private string[] engChars = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        private string[] trChars = { "a", "b", "c", "ç", "d", "e", "f", "g", "ğ", "h", "ı", "i", "j", "k", "l", "m", "n", "o", "ö", "p", "q", "r", "s", "ş", "t", "u", "ü", "v", "y", "z" };

        private int maxLevelWKeys;

        void OnEnable()
        {
            GameStateManager.OnGameStateChanged += OnStateChange;
        }

        void OnDestroy()
        {
            GameStateManager.OnGameStateChanged -= OnStateChange;
        }

        public void StartGame()
        {
            maxLevelWKeys = levels.Count / 2;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            AssignLevelVariables();

            levelText.text = $"Level: {levelId}";

            StartCoroutine(StartRoutine());
        }

        IEnumerator StartRoutine()
        {
            CreateItems();
            AudioManager.instance.PlayOneShot("Intro");
            yield return new WaitForSeconds(0.9f);
            AudioManager.instance.Play("Background");
            shownWindowCount = 4;
            ConstructChosenItemsList();

            yield return new WaitForSeconds(0.5f);
            windows[0].Select();
        }

        private void AssignLevelVariables()
        {
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            levelSO = levels[levelId - 1];

            itemCountToBeShown = levelSO.levelUpCriteria;
            timeToShowItem = levelSO.timeToShowItem;
            uiManager.SetAnswerTime(levelSO.answerTime);
            currentLevelId = levelId;
        }

        private void AssignItems()
        {
            foreach (Window window in windows)
            {
                Item randItem = GetRandomItem();
                window.SetItem(randItem);
            }
        }

        private void ConstructChosenItemsList()
        {
            if (!levelSO.isMixed)
            {
                if (levelSO.isEasy)
                {
                    chosenList.AddRange(easyItemsClone);
                }

                if (levelSO.isMedium)
                    chosenList.AddRange(mediumItemsClone);

                if (levelSO.isHard)
                    chosenList.AddRange(hardItemsClone);
            }
            else
            {
                if (levelSO.isEasy && levelSO.isMedium && !levelSO.isHard)
                {
                    chosenList.AddRange(mediumItemsClone);
                    easyItemsClone.Shuffle();
                    float rate = (100 - levelSO.mixRate) / 100;
                    for (int i = 0; i < chosenList.Count * rate; i++)
                    {
                        chosenList.Add(easyItemsClone[i]);
                    }
                }

                if (levelSO.isEasy && levelSO.isMedium && levelSO.isHard)
                {
                    chosenList.AddRange(hardItemsClone);
                    easyItemsClone.Shuffle();
                    mediumItemsClone.Shuffle();
                    float rate = (100 - levelSO.mixRate) / 100;
                    rate /= 2f;
                    for (int i = 0; i < chosenList.Count * rate; i++)
                    {
                        chosenList.Add(easyItemsClone[i]);
                        chosenList.Add(mediumItemsClone[i]);
                    }
                }
            }

            chosenList.Shuffle();
            AssignItems();
        }

        private void GenerateQuestion()
        {
            if (levelSO.questionType1
                && !levelSO.questionType2
                && !levelSO.questionType3
                && !levelSO.questionType4)
            {
                chosenLetterIndex = 0;
                uiManager.SetQuestion(questions[0]);
                questionIndex = 0;
            }

            if (levelSO.questionType2
                && !levelSO.questionType1
                && !levelSO.questionType3
                && !levelSO.questionType4)
            {
                chosenLetterIndex = selectedItem.itemName.Length - 1;
                uiManager.SetQuestion(questions[1]);
                questionIndex = 1;
            }

            if (levelSO.questionType3
                && !levelSO.questionType1
                && !levelSO.questionType2
                && !levelSO.questionType4)
            {
                chosenLetterIndex = 1;
                uiManager.SetQuestion(questions[2]);
                questionIndex = 2;
            }

            if (levelSO.questionType4
                && !levelSO.questionType1
                && !levelSO.questionType2
                && !levelSO.questionType3)
            {
                chosenLetterIndex = UnityEngine.Random.Range(0, selectedItem.itemName.Length);
                uiManager.SetQuestion(questions[3]);
                questionIndex = 3;
            }

            //Choose random if question1 and question2 is enabled
            if (levelSO.questionType1
                && levelSO.questionType2
                && !levelSO.questionType3
                && !levelSO.questionType4)
            {
                int randomChance = UnityEngine.Random.Range(1, 101);

                if (randomChance <= 50)
                {
                    chosenLetterIndex = 0;
                    uiManager.SetQuestion(questions[0]);
                    questionIndex = 0;
                }
                else
                {
                    chosenLetterIndex = selectedItem.itemName.Length - 1;
                    uiManager.SetQuestion(questions[1]);
                    questionIndex = 1;
                }
            }

            //Choose random if question1, question2 and question3 is enabled
            if (levelSO.questionType1
                && levelSO.questionType2
                && levelSO.questionType3
                && !levelSO.questionType4)
            {
                int randomChance = UnityEngine.Random.Range(1, 101);
                if (randomChance <= 33)
                {
                    chosenLetterIndex = 0;
                    uiManager.SetQuestion(questions[0]);
                    questionIndex = 0;
                }
                else if (randomChance > 33 && randomChance <= 66)
                {
                    chosenLetterIndex = selectedItem.itemName.Length - 1;
                    uiManager.SetQuestion(questions[1]);
                    questionIndex = 1;
                }
                else
                {
                    chosenLetterIndex = 1;
                    uiManager.SetQuestion(questions[2]);
                    questionIndex = 2;
                }
            }

            //Choose random if all question types are enabled
            if (levelSO.questionType1
                && levelSO.questionType2
                && levelSO.questionType3
                && levelSO.questionType4)
            {
                int randomChance = UnityEngine.Random.Range(1, 101);

                if (randomChance <= 25)
                {
                    chosenLetterIndex = 0;
                    uiManager.SetQuestion(questions[0]);
                    questionIndex = 0;
                }
                else if (randomChance > 25 && randomChance <= 50)
                {
                    chosenLetterIndex = selectedItem.itemName.Length - 1;
                    uiManager.SetQuestion(questions[1]);
                    questionIndex = 1;
                }
                else if (randomChance > 50 && randomChance <= 75)
                {
                    chosenLetterIndex = 1;
                    uiManager.SetQuestion(questions[2]);
                    questionIndex = 2;
                }
                else
                {
                    chosenLetterIndex = UnityEngine.Random.Range(0, selectedItem.itemName.Length);
                    uiManager.SetQuestion(questions[3]);
                    questionIndex = 3;
                }
            }

            GenerateLetters();
        }

        private void GenerateLetters()
        {
            string randLetter;
            int randLetterIdx;
            letterWindows.Shuffle();
            string[] chars = LeanLocalization.Instances[0].CurrentLanguage.Equals("Turkish") ? trChars : engChars;
            string[] dupChars = (string[])chars.Clone();

            letterWindows[0].SetLetter(selectedItem.itemName[chosenLetterIndex].ToString());

            for (int i = 1; i < letterWindows.Count; i++)
            {
                if (questionIndex != 3)
                {
                    do
                    {
                        randLetterIdx = UnityEngine.Random.Range(0, dupChars.Length);
                        randLetter = dupChars[randLetterIdx];
                        letterWindows[i].SetLetter(randLetter);

                    } while (dupChars[randLetterIdx] == null || randLetter.Equals(selectedItem.itemName[chosenLetterIndex].ToString()));

                    dupChars[randLetterIdx] = null;
                }
                else
                {
                    do
                    {
                        randLetterIdx = UnityEngine.Random.Range(0, dupChars.Length);
                        randLetter = dupChars[randLetterIdx];
                        letterWindows[i].SetLetter(randLetter);

                    } while (dupChars[randLetterIdx] == null || selectedItem.itemName.Contains(randLetter));

                    dupChars[randLetterIdx] = null;
                }
            }

            uiManager.SetTimer(true);
            letterWindows[0].SetClickable(true);
        }

        public void CheckAnswer(string chosenLetter)
        {
            letterWindows[0].SetClickable(false);
            answeredQuestionCount++;

            if (chosenLetter.Equals(selectedItem.itemName[chosenLetterIndex].ToString()))
            {
                AudioManager.instance.PlayOneShot("Correct");
                correctCount++;
                correct++;
                correctCount = Mathf.Clamp(correctCount, 0, 1000);

                uiManager.ShowFeedback(true);
                uiManager.UpdateLoadingBarUp(correctCount);
            }
            else
            {
                AudioManager.instance.PlayOneShot("Wrong");
                // correctCount--;
                wrongCount++;
                wrong++;
                correctCount = Mathf.Clamp(correctCount, 0, 1000);

                uiManager.ShowFeedback(false);
                uiManager.UpdateLoadingBarDown(correctCount);
            }

            uiManager.SetTimer(false);
            GameStateManager.SetGameState(GameStateManager.GameState.WAITING_SELECTION);
        }

        private void OnStateChange()
        {
            switch (GameStateManager.GetGameState())
            {
                case GameStateManager.GameState.WAITING_SELECTION:
                    CloseSelection();
                    break;

                case GameStateManager.GameState.ASK_QUESTION:
                    GenerateQuestion();
                    break;

                default:
                    break;
            }
        }

        public void SetSelectedItem(Item item, Window window)
        {
            selectedItem = item;
            selectedWindow = window;
        }

        private Item GetRandomItem()
        {
            Item chosenItem;
            int randIndex = UnityEngine.Random.Range(0, chosenList.Count);

            chosenItem = chosenList[randIndex];
            chosenList.RemoveAt(randIndex);
            return chosenItem;
        }

        private void CloseSelection()
        {
            StartCoroutine(CloseSelectionRoutine());
        }

        public void CheckEndGame()
        {
            foreach (Window window in windows)
            {
                if (window.gameObject.activeSelf)
                    return;
            }

            CalculateLevelScore();
            DecideLevel();
            StopAllCoroutines();
            uiManager.StartHackAnim(isLevelSuccess);
        }

        public void ResetVariables()
        {
            correctCount = 0;
            wrongCount = 0;
            answeredQuestionCount = 0;
            shownWindowCount = 0;
        }

        public void Restart()
        {
            StartCoroutine(RestartRoutine());
        }

        private void CreateItems()
        {
            foreach (Sprite easySprite in easyItemSprites)
            {
                Item item = new Item()
                {
                    itemSprite = easySprite,
                    itemName = LeanLocalization.GetTranslationText(easySprite.name)
                };

                easyItems.Add(item);
            }

            foreach (Sprite mediumSprite in mediumItemSprites)
            {
                Item item = new Item()
                {
                    itemSprite = mediumSprite,
                    itemName = LeanLocalization.GetTranslationText(mediumSprite.name)
                };

                mediumItems.Add(item);
            }

            foreach (Sprite hardSprite in hardItemSprites)
            {
                Item item = new Item()
                {
                    itemSprite = hardSprite,
                    itemName = LeanLocalization.GetTranslationText(hardSprite.name)
                };

                hardItems.Add(item);
            }

            easyItemsClone = new List<Item>(easyItems);
            mediumItemsClone = new List<Item>(mediumItems);
            hardItemsClone = new List<Item>(hardItems);
        }

        private void DecideLevel()
        {
            downCounter = PlayerPrefs.GetInt("NameItOrRunIt_DownCounter", 0);
            upCounter = PlayerPrefs.GetInt("NameItOrRunIt_UpCounter", 0);

            if (score >= levelSO.minScore)
            {
                isLevelSuccess = true;

                upCounter++;

                if (upCounter >= 2)
                {
                    upCounter = 0;
                    LevelUp();
                }
                PlayerPrefs.SetInt("NameItOrRunIt_UpCounter", upCounter);
            }
            else
            {
                isLevelSuccess = false;

                downCounter++;

                if (downCounter >= levelSO.levelDownCriteria)
                {
                    downCounter = 0;
                    LevelDown();
                }
                PlayerPrefs.SetInt("NameItOrRunIt_DownCounter", downCounter);
            }
        }

        private void LevelUp()
        {
            levelId++;
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            levelText.text = "Level: " + levelId;
        }

        private void LevelDown()
        {
            levelId--;
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            levelText.text = "Level: " + levelId;
        }

        public int CalculateLevelScore()
        {
            int levelScore = Mathf.CeilToInt((correct * 100) - (wrong * levelSO.penaltyPoints));
            levelScore = Mathf.Clamp(levelScore, 0, levelSO.maxInLevel);
            levelScores.Add(levelScore);
            score = levelScore;
            scoreText.text = $"Score: {levelScore}";

            return levelScore;
        }

        public int CalculateScore()
        {
            float inGameScore = (correct * 100) - (wrong * 25);
            Debug.Log("inGameScore: " + inGameScore);
            float maxInGame = correct * 100;
            float witminaScore = inGameScore / maxInGame * 1000f;
            Debug.Log("witminaScore: " + witminaScore);

            return Mathf.Clamp(Mathf.CeilToInt(witminaScore), 0, 1000);
        }

        public void CloseLetters(LetterWindow avoidWindow)
        {
            foreach (LetterWindow letterWindow in letterWindows)
            {
                if (letterWindow != avoidWindow)
                    letterWindow.SetLetter("");
            }
        }

        public int GetCorrectCount()
        {
            return answeredQuestionCount - wrongCount;
        }

        public int GetWrongCount()
        {
            return wrongCount;
        }

        IEnumerator RestartRoutine()
        {
            AudioManager.instance.FadeTo("Background", 0f, 1f);
            yield return new WaitForSeconds(1f);

            GameManager.instance.Finish();

            /* ResetVariables();

            foreach (Window window in windows)
            {
                window.Reset();
            }

            uiManager.ResetLoadingBar();
            answeredQuestionCount = 0;
            StartGame(); */
        }

        IEnumerator CloseSelectionRoutine()
        {
            selectedWindow.Close();

            uiManager.SetQuestion("choose one of the windows");

            windows[1].MoveTo(windowPositions[0], 0.5f);
            windows[1].transform.SetSiblingIndex(7);
            yield return new WaitForSeconds(0.2f);
            windows[2].MoveTo(windowPositions[1], 0.5f);
            windows[2].transform.SetSiblingIndex(6);
            yield return new WaitForSeconds(0.2f);
            windows[3].MoveTo(windowPositions[2], 0.5f);
            windows[3].transform.SetSiblingIndex(5);
            yield return new WaitForSeconds(0.2f);
            windows[0].gameObject.SetActive(true);
            windows[0].Reset();
            windows[0].MoveTo(windowPositions[3], 0.5f);
            windows[0].transform.SetSiblingIndex(4);

            if (shownWindowCount < itemCountToBeShown)
            {
                shownWindowCount++;
                Item randItem = GetRandomItem();
                windows[0].SetItem(randItem);
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                windows[0].gameObject.SetActive(false);
                CheckEndGame();
            }

            Window window = windows[0];
            windows.Remove(window);
            windows.Add(window);

            if (windows[0].gameObject.activeSelf)
                windows[0].Select();
        }
    }

    [Serializable]
    public class Item
    {
        public Sprite itemSprite;
        public string itemName;
    }

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
}