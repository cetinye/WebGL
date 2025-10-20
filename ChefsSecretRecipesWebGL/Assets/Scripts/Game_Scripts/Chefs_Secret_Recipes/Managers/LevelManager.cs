using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Chefs_Secret_Recipes
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;

        [Header("Level Variables")] public int levelId;

        public LevelSO levelSO;
        [SerializeField] private List<LevelSO> levels = new();

        [Header("Scene Variables")]
        [SerializeField]
        private Equation selectedEqn;

        [SerializeField] private int amountOfHints;
        [SerializeField] private Meal selectedMeal;
        [SerializeField] private Meals meals = new();
        [SerializeField] private List<Sprite> mealSprites = new();
        [SerializeField] private List<Sprite> ingredientSprites = new();
        public int totalCorrectCount, totalWrongCount;

        [Header("Scene Components")]
        [SerializeField]
        private QuestionPanel questionPanel;

        [SerializeField] private GridLayoutGroup grid;
        [SerializeField] private Fridge fridge;

        [Header("Prefabs")][SerializeField] private TextAsset mealsJSON;

        [SerializeField] private Hint hintPref;

        [Header("Scene Move Variables")]
        [SerializeField]
        private Transform sceneObjectsToMoveParent;

        [SerializeField] private Vector3 ovenPos;
        [SerializeField] private Vector3 fridgePos;
        [SerializeField] private float timeToMove;
        [SerializeField] private Image backgroundBlur_1;
        [SerializeField] private Image backgroundBlur_2;
        [SerializeField] private Image backgroundBlur_3;

        [Header("Score Variables")]
        [Header("Flash Interval")]
        [SerializeField]
        private bool isFlashable = true;

        private readonly List<int> usedIngredientIndexes = new();
        private readonly List<int> usedValues = new();

        private int correctCount, wrongCount;
        private bool isCorrect;
        private bool isLevelTimerOn;
        private bool isQuestionTimerOn;
        private float levelTimer, levelTime;
        private float questionTimer;
        private Tween sceneMoveTween;
        private int shownEqnCount;

        private IEnumerator startRoutine;

        private UIManager uiManager;
        private int maxLevelWKeys;

        private void Awake()
        {

            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }

        private void Start()
        {
            GameStateManager.OnGameStateChanged += OnGameStateChanged;
            
        }

        private void Update()
        {
            LevelTimer();
            QuestionTimer();
        }

        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= OnGameStateChanged;
            CancelInvoke();
            StopAllCoroutines();
            DOTween.KillAll();
        }

        private void OnDestroy()
        {
            CancelInvoke();
            StopAllCoroutines();
            DOTween.KillAll();

            instance = null;
        }

        private void LevelTimer()
        {
            if (!isLevelTimerOn) return;

            levelTimer -= Time.deltaTime;
            uiManager.SetLevelTimeText(levelTimer);

            if (levelTimer < 0)
            {
                isLevelTimerOn = false;
                levelTimer = 0;
                uiManager.SetLevelTimeText(0);
                GameManager.instance.Finish();
            }

            if (levelTimer <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
                uiManager.FlashRed();
            }
        }

        private void QuestionTimer()
        {
            if (!isQuestionTimerOn) return;

            questionTimer -= Time.deltaTime;
            uiManager.SetQuestionTime(questionTimer);

            if (questionTimer < 0)
            {
                isQuestionTimerOn = false;
                wrongCount++;
                totalWrongCount++;
                uiManager.UpdateWrongText(totalWrongCount);
                questionTimer = 0;
                uiManager.SetQuestionTime(0);
                NextQuestion(false);
            }
        }

        public void StartGame()
        {
            uiManager = UIManager.instance;
            Debug.Log("UIManager instance assigned");

            
            Debug.Log("Game Started");
            ReadMealData();

            maxLevelWKeys = levels.Count;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            levelTime = 60f;
            levelTimer = levelTime;

            AudioManager.instance.Play(SoundType.Background);

            GameStateManager.SetGameState(GameState.Idle);

            AssignLevelVariables();
            Debug.Log("assigned uimanager:   " + uiManager);
            uiManager.ShowLevelId(levelId);

            isLevelTimerOn = true;

            if (startRoutine != null)
            {
                StopCoroutine(startRoutine);
                startRoutine = null;
            }

            startRoutine = StartRoutine();
            StartCoroutine(startRoutine);
        }

        public void NextQuestion(bool isCorrect)
        {
            GameStateManager.SetGameState(GameState.Idle);

            if (++shownEqnCount <= levelSO.levelUpCriteria)
            {
                usedIngredientIndexes.Clear();
                usedValues.Clear();
                questionPanel.Reset(false);
                AssignLevelVariables();
                GenerateIngredients();
                selectedEqn = questionPanel.GenerateEquation();
                selectedEqn.GenerateEquation();
                ShowMeal();
            }
            else
            {
                DecideLevel();

                shownEqnCount = 0;
                StopAllCoroutines();
                StartCoroutine(ResetRoutine());
            }
        }

        private void DecideLevel()
        {
            if (wrongCount >= levelSO.levelDownCriteria)
                levelId--;
            else
            {
                int upCounter = PlayerPrefs.GetInt("ChefsSecretRecipes_UpCounter", 0);
                if (++upCounter >= 2)
                {
                    upCounter = 0;
                    levelId++;
                }

                Debug.LogWarning("UpCounter: " + upCounter);
                PlayerPrefs.SetInt("ChefsSecretRecipes_UpCounter", upCounter);
            }

            correctCount = 0;
            wrongCount = 0;
        }

        private void AssignLevelVariables()
        {
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            levelSO = levels[levelId - 1];

            questionTimer = levelSO.timeLimitPerOperation;
        }

        private void OnGameStateChanged()
        {
            switch (GameStateManager.GetGameState())
            {
                case GameState.Idle:
                    isQuestionTimerOn = false;
                    uiManager.SetSliderVisibility(false);
                    break;

                case GameState.Question:
                    isQuestionTimerOn = true;
                    uiManager.SetSliderVisibility(true);
                    break;

                case GameState.Failed:
                    break;

                case GameState.Success:
                    break;
            }
        }

        private Meal PickRandomMeal()
        {
            return meals.mealData[Random.Range(0, meals.mealData.Length)];
        }

        private int GetRandomIngredientIndex()
        {
            var index = 0;

            do
            {
                index = Random.Range(0, selectedMeal.ingredients.Count);

                if (usedIngredientIndexes.Count == selectedMeal.ingredients.Count)
                    return -1;
            } while (usedIngredientIndexes.Contains(selectedMeal.ingredients[index]));

            usedIngredientIndexes.Add(selectedMeal.ingredients[index]);
            return selectedMeal.ingredients[index];
        }

        private Sprite GetIngredientSprite(int index)
        {
            return ingredientSprites[index - 1];
        }

        public void SpawnHint()
        {
            var value = 0;
            var ingredientIndex = GetRandomIngredientIndex();

            // break if no more ingredients
            if (ingredientIndex == -1) return;

            var ingredientSprite = GetIngredientSprite(ingredientIndex);

            do
            {
                value = Random.Range(selectedEqn.minVal, selectedEqn.maxVal);
            } while (usedValues.Contains(value));

            usedValues.Add(value);
            var hint = Instantiate(hintPref, grid.transform);
            hint.SetHint(ingredientSprite, value);
            AudioManager.instance.PlayOneShot(SoundType.IngredientSpawn);
        }

        public void ShowMeal()
        {
            questionPanel.ShowMeal(mealSprites[selectedMeal.mealId - 1]);
        }

        public void MoveToOven()
        {
            if (this == null) return;
            sceneMoveTween = sceneObjectsToMoveParent.DOLocalMove(ovenPos, timeToMove).OnComplete(() =>
            {
                if (this == null) return;
                selectedEqn.GenerateEquation();
                ShowMeal();
            });
        }

        public void MoveToFridge(bool isInstant = false)
        {
            if (!isInstant)
                sceneMoveTween = sceneObjectsToMoveParent.DOLocalMove(fridgePos, timeToMove);
            else
                sceneMoveTween = sceneObjectsToMoveParent.DOLocalMove(fridgePos, 0f);
        }

        public void GenerateIngredients()
        {
            for (var i = 0; i < grid.transform.childCount; i++)
                questionPanel.AddToIngredients(grid.transform.GetChild(i).TryGetComponent(out Hint hint) ? hint : null);
        }

        public bool CheckAnswer(int givenAnswer)
        {
            GameStateManager.SetGameState(GameState.Idle);

            if (givenAnswer == questionPanel.GetCorrectAnswer())
            {
                isCorrect = true;
                correctCount++;
                totalCorrectCount++;
                AudioManager.instance.PlayAfterXSeconds(SoundType.Correct, 0.3f);
                questionPanel.SetPanelColor(Color.green);
            }
            else
            {
                isCorrect = false;
                wrongCount++;
                totalWrongCount++;
                AudioManager.instance.PlayAfterXSeconds(SoundType.Wrong, 0.3f);
                questionPanel.SetPanelColor(Color.red);
                questionPanel.ShowMeal(mealSprites[^1]);
            }

            uiManager.UpdateCorrectText(totalCorrectCount);
            uiManager.UpdateWrongText(totalWrongCount);

            StartCoroutine(EndRoutine());
            return isCorrect;
        }

        public int CalculateTotalScore()
        {
            float maxInGame = (totalCorrectCount + totalWrongCount) * levelSO.pointsPerCorrectAnswer;
            float score = totalCorrectCount * levelSO.pointsPerCorrectAnswer - totalWrongCount * levelSO.penaltyPoints;
            var witminaScore = score / maxInGame * 1000f;
            Debug.Log("Score: " + Mathf.Clamp(Mathf.CeilToInt(witminaScore), 0, 1000));
            return Mathf.Clamp(Mathf.CeilToInt(witminaScore), 0, 1000);
        }

        private void ResetGrid()
        {
            for (var i = 0; i < grid.transform.childCount; i++) Destroy(grid.transform.GetChild(i).gameObject);
        }

        public QuestionPanel GetQuestionPanel()
        {
            return questionPanel;
        }

        private void ReadMealData()
        {
            meals = JsonUtility.FromJson<Meals>(mealsJSON.text);
        }

        private void BlurBackgroundState(float alpha, float time)
        {
            backgroundBlur_1.DOFade(alpha, time);
            backgroundBlur_2.DOFade(alpha, time);
            backgroundBlur_3.DOFade(alpha, time);
        }

        private IEnumerator StartRoutine()
        {
            selectedMeal = PickRandomMeal();
            yield return new WaitForSeconds(1f);

            fridge.OpenFridge();
            AudioManager.instance.PlayOneShot(SoundType.FridgeOpen);
            yield return new WaitForSeconds(1f);

            selectedEqn = questionPanel.GenerateEquation();

            for (var i = 0; i < amountOfHints; i++)
            {
                SpawnHint();
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(0.5f);

            fridge.CloseFridge();
            AudioManager.instance.PlayOneShot(SoundType.FridgeClose);
            yield return new WaitForSeconds(0.5f);

            BlurBackgroundState(1f, 1f);
            GenerateIngredients();
            MoveToOven();
            shownEqnCount++;
        }

        private IEnumerator ResetRoutine()
        {
            DOTween.CompleteAll();

            uiManager.ShowLevelId(levelId);

            fridge.Reset();
            usedIngredientIndexes.Clear();
            usedValues.Clear();
            questionPanel.Disappear();
            yield return new WaitForSeconds(1f);
            questionPanel.Reset();
            ResetGrid();
            BlurBackgroundState(0f, 1f);
            yield return new WaitForSeconds(0.5f);

            MoveToFridge();
            yield return new WaitForSeconds(2f);

            AssignLevelVariables();

            yield return StartRoutine();
        }

        private IEnumerator EndRoutine()
        {
            var mealImage = questionPanel.GetMealImage();
            yield return new WaitForSeconds(0.5f);

            mealImage.transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.5f);
            mealImage.transform.DOScale(new Vector3(2.5f, 2.5f, 2.5f), 0.5f);
            yield return new WaitForSeconds(0.5f);

            AudioManager.instance.PlayCustomer(isCorrect);
            questionPanel.PlayParticle(isCorrect);

            yield return new WaitForSeconds(1.5f);
            NextQuestion(isCorrect);
        }
    }

    [Serializable]
    public class Meals
    {
        public Meal[] mealData;
    }

    [Serializable]
    public class Meal
    {
        public int mealId;
        public List<int> ingredients;
    }
}