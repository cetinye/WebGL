using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chrono_Capsule_Chronicles
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;

        [SerializeField] private UIManager uiManager;

        public int levelId;
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
        private LevelSO LevelSO;
        private int maxLevelWKeys;

        private int levelUpGoal;
        private int levelDownGoal;
        private int totalScore;
        private int correctCount;
        private int wrongCount;
        [SerializeField] private WordsSO wordsSO;
        [SerializeField] TextAsset textFile;

        [SerializeField] private float feedbackTimeInterval;
        [SerializeField] private string chosenWord;
        [SerializeField] private int letterSize;
        [SerializeField] private int randomizeLetterCount;
        [SerializeField] private int wordAmount;
        private float questionTime;
        public float gameTime;
        [SerializeField] private Word wordPrefab;
        [SerializeField] private Transform verticalGridParent;

        [SerializeField] private List<Word> spawnedWords = new List<Word>();
        [SerializeField] private List<string> availableWords = new List<string>();
        [SerializeField] private List<string> shuffledLetters = new List<string>();

        private char[] alphabetEN = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private char[] alphabetTR = { 'A', 'B', 'C', 'Ç', 'D', 'E', 'F', 'G', 'Ğ', 'H', 'I', 'İ', 'J', 'K', 'L', 'M', 'N', 'O', 'Ö', 'P', 'R', 'S', 'Ş', 'T', 'U', 'Ü', 'V', 'Y', 'Z' };
        private Word clickedWord;
        private float gameTimer, levelTimer;
        private int score;
        public int totalCorrect, totalWrong;
        private bool isGameTimerOn = false;
        private bool isLevelTimerOn = false;
        private bool isFlashable = true;

        [Space(20)]
        [Header("Intro Variables")]
        [SerializeField] private RectTransform deviceRect;
        [SerializeField] private RectTransform shadowRect;
        [SerializeField] private RectTransform startPosDevice;
        [SerializeField] private RectTransform startPosShadow;
        [SerializeField] private RectTransform endPosDevice;
        [SerializeField] private RectTransform endPosShadow;
        [SerializeField] private float moveTime;
        [SerializeField] private ParticleSystem deviceSmoke;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

        }

        public void StartGame()
        {
            maxLevelWKeys = levels.Count / 2;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            AssignLevelVariables();

            gameTimer = gameTime;
            levelTimer = questionTime;

            StartCoroutine(IntroRoutine());
        }

        void Update()
        {
            GameTimer();
            LevelTimer();
        }

        private void GameTimer()
        {
            if (!isGameTimerOn) return;

            gameTimer -= Time.deltaTime;

            if (gameTimer <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
                uiManager.FlashRed();
            }

            if (gameTimer < 0)
            {
                gameTimer = 0;
                isGameTimerOn = false;
                isLevelTimerOn = false;
                EndLevel();
            }

            uiManager.UpdateGameTimer(gameTimer);
        }

        private void LevelTimer()
        {
            if (!isLevelTimerOn) return;

            levelTimer -= Time.deltaTime;

            if (levelTimer < 0)
            {
                levelTimer = 0;
                TimedOut();
            }

            uiManager.UpdateLevelTimer(levelTimer);
        }

        public void NewQuestion()
        {
            ClearLevel();

            AssignLevelVariables();
            GenerateLists(letterSize);
            SpawnWords(wordAmount);
        }

        private void SpawnWords(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Word spawnedWord = Instantiate(wordPrefab, verticalGridParent);
                spawnedWords.Add(spawnedWord);
            }

            chosenWord = LeanLocalization.GetTranslationText(availableWords[UnityEngine.Random.Range(0, availableWords.Count)]);
            uiManager.SetChosenWordText(chosenWord);

            ShuffleLetters(chosenWord);
        }

        /// <summary>
        /// Shuffle the letters of the chosen word. Makes sure randomized letters are not in the same order with the chosen word.
        /// </summary>
        /// <param name="chosenWord"></param>
        private void ShuffleLetters(string chosenWord)
        {
            string shuffledLetter;

            for (int i = 0; i < wordAmount; i++)
            {
                do
                {
                    System.Random r = new System.Random();
                    shuffledLetter = new string(chosenWord.ToCharArray().OrderBy(s => r.Next(2) % 2 == 0).ToArray());

                } while (string.Compare(shuffledLetter, chosenWord) == 0);

                shuffledLetters.Add(shuffledLetter);
            }

            ChangeShuffledLetters(randomizeLetterCount);
            UpdateWordTexts();
        }

        /// <summary>
        /// Change letters of the shuffled letters randomly. Makes sure the random letter is different.
        /// Loop starts from 1 to make sure one word is not changed thus being the correct answer
        /// </summary>
        /// <param name="randomizeLetterCount"></param>
        private void ChangeShuffledLetters(int randomizeLetterCount)
        {
            char[] tmp;

            for (int i = 1; i < shuffledLetters.Count; i++)
            {
                tmp = null;

                for (int j = 0; j < randomizeLetterCount; j++)
                {
                    do
                    {
                        tmp = shuffledLetters[i].ToCharArray();
                        tmp[j] = LeanLocalization.Instances[0].CurrentLanguage.Equals("English") ? alphabetEN[UnityEngine.Random.Range(0, alphabetEN.Length)] : alphabetTR[UnityEngine.Random.Range(0, alphabetTR.Length)];

                    } while (shuffledLetters[i].Contains(tmp[j]));

                    shuffledLetters[i] = string.Join("", tmp);
                }
            }
        }

        private bool CheckAnswer(string answer)
        {
            bool result = false;

            for (int i = 0; i < chosenWord.Length; i++)
            {
                if (chosenWord.Contains(answer[i]))
                {
                    result = true;
                }
                else
                {
                    result = false;
                    return result;
                }
            }
            return result;
        }

        public void GiveFeedback(string answer)
        {
            isLevelTimerOn = false;
            bool state = CheckAnswer(answer);

            StartCoroutine(ShowFeedback(state));
        }

        private void UpdateWordTexts()
        {
            spawnedWords.Shuffle();

            for (int i = 0; i < spawnedWords.Count; i++)
            {
                spawnedWords[i].SetWordText(shuffledLetters[i]);
            }

            AudioManager.instance.PlayOneShot(SoundType.WordSpawn);
            spawnedWords[0].SetClickable(true);
            isLevelTimerOn = true;
        }

        private void GenerateLists(int letterAmount)
        {
            textFile = letterAmount switch
            {
                3 => wordsSO.threeLetters,
                4 => wordsSO.fourLetters,
                5 => wordsSO.fiveLetters,
                6 => wordsSO.sixLetters,
                7 => wordsSO.sevenLetters,
                8 => wordsSO.eightLetters,
                9 => wordsSO.nineLetters,
                _ => wordsSO.nineLetters,
            };

            string[] allLines = new[] { textFile.text };

            foreach (string s in allLines)
            {
                string[] splitData = s.Split(',');

                availableWords = splitData.ToList();
            }
        }

        public void AssignLevelVariables()
        {
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            LevelSO = levels[levelId - 1];

            letterSize = LevelSO.totalNumOfLetters;
            randomizeLetterCount = LevelSO.numOfLettersToChange;
            wordAmount = LevelSO.numOfOptions;
            levelUpGoal = LevelSO.levelUpCriteria;
            levelDownGoal = LevelSO.levelDownCriteria;
            questionTime = LevelSO.timePerQuestion;
            levelTimer = questionTime;
        }

        private void DecideLevel(bool isSuccessful)
        {
            int levelUpCounter = PlayerPrefs.GetInt("ChronoCapsuleChronicles_UpCounter", 0);
            int levelDownCounter = PlayerPrefs.GetInt("ChronoCapsuleChronicles_DownCounter", 0);

            if (isSuccessful)
            {
                if (++levelUpCounter >= levelUpGoal * 2)
                {
                    correctCount = 0;
                    wrongCount = 0;
                    levelId++;
                    levelUpCounter = 0;
                    levelDownCounter = 0;
                }
            }
            else
            {
                if (++levelDownCounter >= levelDownGoal * 2)
                {
                    correctCount = 0;
                    wrongCount = 0;
                    levelId--;
                    levelUpCounter = 0;
                    levelDownCounter = 0;
                }
            }

            PlayerPrefs.SetInt("ChronoCapsuleChronicles_UpCounter", levelUpCounter);
            PlayerPrefs.SetInt("ChronoCapsuleChronicles_DownCounter", levelDownCounter);
        }

        public int CalculateScore()
        {
            float score = (totalCorrect * LevelSO.pointsPerQuestion) - (totalWrong * LevelSO.penaltyPoints);
            score = Mathf.Max(score, 0f);
            float max = (totalCorrect + totalWrong) * LevelSO.pointsPerQuestion;
            int witminaScore = Mathf.CeilToInt(score / max * 1000f);
            witminaScore = Mathf.Clamp(witminaScore, 0, 1000);

            Debug.Log($"Witmina Score: {witminaScore}");
            return witminaScore;
        }

        private void ClearLevel()
        {
            for (int i = 0; i < verticalGridParent.childCount; i++)
            {
                Destroy(verticalGridParent.GetChild(i).gameObject);
            }

            chosenWord = null;

            spawnedWords.Clear();
            availableWords.Clear();
            shuffledLetters.Clear();

            uiManager.SetQuestionTime(questionTime);
            levelTimer = questionTime;
        }

        private void EndLevel()
        {
            StartCoroutine(EndRoutine());
        }

        public void SetClickedWord(Word wordClicked)
        {
            clickedWord = wordClicked;
        }

        IEnumerator IntroRoutine()
        {
            AudioManager.instance.Play(SoundType.Background);

            deviceRect.anchoredPosition = startPosDevice.anchoredPosition;
            shadowRect.anchoredPosition = startPosShadow.anchoredPosition;
            yield return new WaitForSeconds(1f);

            deviceSmoke.Play();
            AudioManager.instance.Play(SoundType.DeviceMove);
            Tween moveCapsule = deviceRect.DOAnchorPosY(endPosDevice.anchoredPosition.y, moveTime).SetEase(Ease.OutCirc);
            shadowRect.DOAnchorPosY(endPosShadow.anchoredPosition.y, moveTime).SetEase(Ease.OutCirc);
            yield return moveCapsule.WaitForCompletion();

            AudioManager.instance.Play(SoundType.DeviceBG);

            NewQuestion();
            isGameTimerOn = true;
        }

        IEnumerator EndRoutine()
        {
            ClearLevel();
            uiManager.CloseAllElements();

            deviceSmoke.Play();
            AudioManager.instance.Play(SoundType.DeviceMove);
            Tween moveCapsule = deviceRect.DOAnchorPosY(startPosDevice.anchoredPosition.y, moveTime).SetEase(Ease.InCirc);
            shadowRect.DOAnchorPosY(startPosShadow.anchoredPosition.y, moveTime).SetEase(Ease.InCirc);
            yield return moveCapsule.WaitForCompletion();

            GameManager.instance.Finish();
        }

        IEnumerator ShowFeedback(bool state)
        {
            yield return new WaitForSeconds(feedbackTimeInterval / 2);

            if (state)
            {
                correctCount++;
                totalCorrect++;
                uiManager.Correct();
                uiManager.UpdateCorrectText(totalCorrect);
                clickedWord.SetFeedback(true);
                AudioManager.instance.PlayOneShot(SoundType.Correct);
            }
            else
            {
                wrongCount++;
                totalWrong++;
                uiManager.Wrong();
                uiManager.UpdateWrongText(totalWrong);
                clickedWord.SetFeedback(false);
                AudioManager.instance.PlayOneShot(SoundType.Wrong);
            }

            yield return new WaitForSeconds(feedbackTimeInterval * 2);

            DecideLevel(state);
            NewQuestion();
        }

        private void TimedOut()
        {
            wrongCount++;
            totalWrong++;
            uiManager.Wrong();
            uiManager.UpdateWrongText(totalWrong);
            AudioManager.instance.PlayOneShot(SoundType.Wrong);

            DecideLevel(false);
            NewQuestion();
        }
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