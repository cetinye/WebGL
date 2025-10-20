using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;
using System.Linq;
using Random = UnityEngine.Random;
using Lean.Localization;

namespace NanoInspector
{
    public class LevelManager : MonoBehaviour
    {
        public int levelId;

        public TMPro.TMP_Text levelText;

        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
        [SerializeField] private LevelSO levelSO;

        public GameObject firstOrgObj, secondOrgObj;
        public Organism firstOrganism, secondOrganism, tempOrganism;

        public Question question;
        public bool isQuestionTimerOn = false;
        public bool isButtonPressable;

        public int correctCount = 0;
        public int wrongCount = 0;
        public int score;

        private int upCounter;
        private int downCounter;
        private int correct;
        private int wrong;

        [SerializeField] private UIManager uiManager;
        [SerializeField] private GameObject organismPrefab;
        [SerializeField] private GameObject rightMonitor, leftMonitor;
        [SerializeField] private RectTransform rightSpawnPoint, leftSpawnPoint;
        [SerializeField] private List<Sprite> microorganisms = new List<Sprite>();
        [SerializeField] private List<Color> colors = new List<Color>();
        [SerializeField] private ParticleSystem microscopeParticle;

        private bool isColorRandomized = false;
        private bool isMovementRandomized = false;
        private bool isStartLevelRunning = false;

        private int maxLevelWKeys;

        public void StartLevel()
        {
            if (GameManager.instance.state != GameManager.GameState.Playing)
                return;

            if (!isStartLevelRunning)
            {
                isStartLevelRunning = true;

                maxLevelWKeys = levels.Count;
                Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

                StartCoroutine(SpawnRoutine());
            }
        }

        public void SpawnOrganism()
        {
            if (firstOrgObj)
            {
                firstOrgObj.transform.DOKill();
                Destroy(firstOrgObj);
            }

            if (secondOrgObj)
            {
                secondOrgObj.transform.DOKill();
                Destroy(secondOrgObj);
            }

            firstOrgObj = Instantiate(organismPrefab, rightMonitor.transform);
            secondOrgObj = Instantiate(organismPrefab, leftMonitor.transform);

            firstOrgObj.SetActive(false);
            secondOrgObj.SetActive(false);

            firstOrgObj.GetComponent<RectTransform>().anchoredPosition = rightSpawnPoint.anchoredPosition;
            secondOrgObj.GetComponent<RectTransform>().anchoredPosition = leftSpawnPoint.anchoredPosition;

            firstOrganism = firstOrgObj.GetComponent<Organism>();
            secondOrganism = secondOrgObj.GetComponent<Organism>();

            RandomizeShapes();
            RandomizeColors();
            RandomizeMovement();
        }

        public void RandomizeColors()
        {
            int firstIndex = Random.Range(0, colors.Count);
            int secondIndex = Random.Range(0, colors.Count);

            if (isColorRandomized)
            {
                firstOrganism.SetColor(colors[firstIndex], firstIndex);
                secondOrganism.SetColor(colors[secondIndex], secondIndex);
            }
            else
            {
                firstOrganism.SetColor(colors[firstIndex], firstIndex);
                secondOrganism.SetColor(colors[firstIndex], firstIndex);
            }

        }

        public void RandomizeShapes()
        {
            int firstIndex = Random.Range(0, microorganisms.Count);
            int secondIndex = Random.Range(0, microorganisms.Count);

            ///[x]: https://prnt.sc/tLZIfr-10a0q 
            if (firstOrganism == null || secondOrganism == null) return;

            firstOrganism.SetImage(microorganisms[firstIndex]);
            secondOrganism.SetImage(microorganisms[secondIndex]);
        }

        public void RandomizeMovement()
        {
            if (isMovementRandomized)
            {
                firstOrganism.movementType = (Organism.MovementTypes)Random.Range(0, (float)Enum.GetValues(typeof(Organism.MovementTypes)).Cast<Organism.MovementTypes>().Max());
                secondOrganism.movementType = (Organism.MovementTypes)Random.Range(0, (float)Enum.GetValues(typeof(Organism.MovementTypes)).Cast<Organism.MovementTypes>().Max());
            }
            else
            {
                firstOrganism.movementType = (Organism.MovementTypes)Random.Range(0, (float)Enum.GetValues(typeof(Organism.MovementTypes)).Cast<Organism.MovementTypes>().Max());
                secondOrganism.movementType = firstOrganism.movementType;
            }

            firstOrganism.StartMovement();
            secondOrganism.StartMovement();
        }

        public void GenerateQuestion()
        {
            switch (question.questionIncludes)
            {
                case Question.QuestionIncludes.Color:
                    if (question.isCorrect)
                        GenerateColorQuestion(secondOrganism);
                    else
                        GenerateWrongQuestion(Question.QuestionIncludes.Color);
                    break;
                case Question.QuestionIncludes.Shape:
                    if (question.isCorrect)
                        GenerateShapeQuestion();
                    else
                        GenerateWrongQuestion(Question.QuestionIncludes.Shape);
                    break;
                case Question.QuestionIncludes.Movement:
                    if (question.isCorrect)
                        GenerateMovementQuestion(secondOrganism);
                    else
                        GenerateWrongQuestion(Question.QuestionIncludes.Movement);
                    break;
                default:
                    break;
            }

            question.SetQuestionText(question.questionText);
            uiManager.questionText.enabled = false;
        }

        private void AssignLevelVariables()
        {
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            levelSO = levels[levelId - 1];

            levelText.text = "Level: " + levelId;

            if (levelSO.ColorQuestion)
                question.questionIncludes = Question.QuestionIncludes.Color;
            else if (levelSO.ShapeQuestion)
                question.questionIncludes = Question.QuestionIncludes.Shape;
            else if (levelSO.MovementQuestion)
                question.questionIncludes = Question.QuestionIncludes.Movement;

            question.questionTimer = levelSO.questionTime;
            question.isTrickQuestion = levelSO.isTrickQuestion;
            question.isTrickQuestionColorEnabled = levelSO.isTrickQuestionColor;
            question.isTrickQuestionShapeEnabled = levelSO.isTrickQuestionShape;
            question.isTrickQuestionMovementEnabled = levelSO.isTrickQuestionMovement;
            isColorRandomized = levelSO.isColorRandomized;
            isMovementRandomized = levelSO.isMovementRandomized;

            int randVal = Random.Range(0, 2);
            if (randVal == 0)
                question.isCorrect = true;
            else
                question.isCorrect = false;

            if (question.isTrickQuestion)
            {
                randVal = Random.Range(0, 2);
                if (randVal == 0)
                    question.isTrickQuestion = true;
                else
                    question.isTrickQuestion = false;
            }
        }

        //if sent parameter is secondOrganism => Generates correct question
        //if sent parameter is tempOrganism => Generates wrong question
        private void GenerateMovementQuestion(Organism organism)
        {
            if (!question.isTrickQuestionMovementEnabled)
                question.isTrickQuestion = false;

            if (firstOrganism.movementType.Equals(secondOrganism.movementType))
            {
                string keyBase = "";
                switch (organism.movementType)
                {
                    case Organism.MovementTypes.None:
                        keyBase = "BothNotMoving";
                        break;
                    case Organism.MovementTypes.Shaking:
                        keyBase = "BothShaking";
                        break;
                    case Organism.MovementTypes.Scaling:
                        keyBase = "BothExpanding";
                        break;
                    case Organism.MovementTypes.RotatingRight:
                        keyBase = "BothRotatingRight";
                        break;
                    case Organism.MovementTypes.RotatingLeft:
                        keyBase = "BothRotatingLeft";
                        break;
                    default:
                        return;
                }

                string key = question.isTrickQuestion && question.isTrickQuestionMovementEnabled
                    ? $"{keyBase}_Trick"
                    : keyBase;

                question.questionText = LeanLocalization.GetTranslationText(key);
            }
            else
            {
                UpdateQuestionMovement(firstOrganism);
            }
        }

        private void GenerateShapeQuestion()
        {
            if (!question.isTrickQuestionShapeEnabled)
                question.isTrickQuestion = false;

            string key;

            if (firstOrganism.image.sprite.Equals(secondOrganism.image.sprite))
            {
                key = question.isTrickQuestion && question.isTrickQuestionShapeEnabled
                    ? "BothNotSameShape"
                    : "BothSameShape";
            }
            else
            {
                key = question.isTrickQuestion && question.isTrickQuestionShapeEnabled
                    ? "BothSameShape"
                    : "BothNotSameShape";
            }

            question.questionText = LeanLocalization.GetTranslationText(key);
        }

        //if sent parameter is secondOrganism => Generates correct question
        //if sent parameter is tempOrganism => Generates wrong question
        private void GenerateColorQuestion(Organism organism)
        {
            if (!question.isTrickQuestionColorEnabled)
                question.isTrickQuestion = false;

            if (firstOrganism.color.Equals(secondOrganism.color))
            {
                string keyBase = "";

                switch (organism.colorType)
                {
                    case Organism.Colors.Red:
                        keyBase = "BothRed";
                        break;
                    case Organism.Colors.Pink:
                        keyBase = "BothPink";
                        break;
                    case Organism.Colors.Purple:
                        keyBase = "BothPurple";
                        break;
                    case Organism.Colors.Blue:
                        keyBase = "BothBlue";
                        break;
                    case Organism.Colors.LightBlue:
                        keyBase = "BothLightBlue";
                        break;
                    case Organism.Colors.Green:
                        keyBase = "BothGreen";
                        break;
                    case Organism.Colors.Brown:
                        keyBase = "BothBrown";
                        break;
                    case Organism.Colors.Yellow:
                        keyBase = "BothYellow";
                        break;
                    case Organism.Colors.Orange:
                        keyBase = "BothOrange";
                        break;
                    default:
                        return;
                }

                string key = question.isTrickQuestion && question.isTrickQuestionColorEnabled
                    ? $"{keyBase}_Trick"  // Trick version
                    : keyBase;

                question.questionText = LeanLocalization.GetTranslationText(key);
            }
            else
            {
                UpdateColorQuestion(firstOrganism);
            }
        }

        private void GenerateWrongQuestion(Question.QuestionIncludes questionType)
        {
            switch (questionType)
            {
                case Question.QuestionIncludes.Color:
                    GenerateWrongColorQuestion();
                    break;
                case Question.QuestionIncludes.Shape:
                    GenerateWrongShapeQuestion();
                    break;
                case Question.QuestionIncludes.Movement:
                    GenerateWrongMovementQuestion();
                    break;
                default:
                    break;
            }
        }

        private void GenerateWrongColorQuestion()
        {
            do
            {
                tempOrganism.colorType = tempOrganism.GetRandomColor();
            } while (firstOrganism.colorType == tempOrganism.colorType || secondOrganism.colorType == tempOrganism.colorType);

            if (isColorRandomized)
                UpdateColorQuestion(tempOrganism);
            else
                GenerateColorQuestion(tempOrganism);
        }

        private void GenerateWrongShapeQuestion()
        {
            string key;

            if (firstOrganism.image.sprite.Equals(secondOrganism.image.sprite))
            {
                key = question.isTrickQuestion && question.isTrickQuestionShapeEnabled
                    ? "BothNotSameShape"  // Trick question: wrong statement
                    : "BothSameShape";     // Normal question
            }
            else
            {
                key = question.isTrickQuestion && question.isTrickQuestionShapeEnabled
                    ? "BothSameShape"      // Trick question: wrong statement
                    : "BothNotSameShape";  // Normal question
            }

            question.questionText = LeanLocalization.GetTranslationText(key);
        }

        private void GenerateWrongMovementQuestion()
        {
            do
            {
                tempOrganism.movementType = tempOrganism.GetRandomMovement();
            } while (firstOrganism.movementType == tempOrganism.movementType || secondOrganism.movementType == tempOrganism.movementType);

            if (isMovementRandomized)
                UpdateQuestionMovement(tempOrganism);
            else
                GenerateMovementQuestion(tempOrganism);
        }

        private void UpdateColorQuestion(Organism organism)
        {
            string keyBase = "";

            switch (organism.colorType)
            {
                case Organism.Colors.Red:
                    keyBase = "OneRed";
                    break;
                case Organism.Colors.Pink:
                    keyBase = "OnePink";
                    break;
                case Organism.Colors.Purple:
                    keyBase = "OnePurple";
                    break;
                case Organism.Colors.Blue:
                    keyBase = "OneBlue";
                    break;
                case Organism.Colors.LightBlue:
                    keyBase = "OneLightBlue";
                    break;
                case Organism.Colors.Green:
                    keyBase = "OneGreen";
                    break;
                case Organism.Colors.Brown:
                    keyBase = "OneBrown";
                    break;
                case Organism.Colors.Yellow:
                    keyBase = "OneYellow";
                    break;
                case Organism.Colors.Orange:
                    keyBase = "OneOrange";
                    break;
                default:
                    return;
            }

            string key = question.isTrickQuestion && question.isTrickQuestionColorEnabled
                ? $"{keyBase}_Trick"  // Trick version
                : keyBase;

            question.questionText = LeanLocalization.GetTranslationText(key);
        }

        private void UpdateQuestionMovement(Organism organism)
        {
            string keyBase = "";

            switch (organism.movementType)
            {
                case Organism.MovementTypes.None:
                    keyBase = "OneMoving";
                    break;
                case Organism.MovementTypes.Shaking:
                    keyBase = "OneShaking";
                    break;
                case Organism.MovementTypes.Scaling:
                    keyBase = "OneExpanding";
                    break;
                case Organism.MovementTypes.RotatingRight:
                    keyBase = "OneRotatingRight";
                    break;
                case Organism.MovementTypes.RotatingLeft:
                    keyBase = "OneRotatingLeft";
                    break;
                default:
                    return;
            }

            string key = question.isTrickQuestion && question.isTrickQuestionMovementEnabled
                ? $"{keyBase}_Trick"  // Trick version
                : keyBase;

            question.questionText = LeanLocalization.GetTranslationText(key);
        }

        public void CheckAnswer(bool buttonFlag)
        {
            if (buttonFlag)
            {
                if (question.isCorrect && !question.isTrickQuestion)
                {
                    Debug.LogWarning("CORRECT");
                    IncreaseCorrectCount();
                }
                else if (question.isCorrect && question.isTrickQuestion)
                {
                    Debug.LogWarning("WRONG");
                    IncreaseWrongCount();
                }
                else if (!question.isCorrect && !question.isTrickQuestion)
                {
                    Debug.LogWarning("WRONG");
                    IncreaseWrongCount();
                }
                else if (!question.isCorrect && question.isTrickQuestion)
                {
                    Debug.LogWarning("CORRECT");
                    IncreaseCorrectCount();
                }
            }
            else
            {
                if (question.isCorrect && !question.isTrickQuestion)
                {
                    Debug.LogWarning("WRONG");
                    IncreaseWrongCount();
                }
                else if (question.isCorrect && question.isTrickQuestion)
                {
                    Debug.LogWarning("CORRECT");
                    IncreaseCorrectCount();
                }
                else if (!question.isCorrect && !question.isTrickQuestion)
                {
                    Debug.LogWarning("CORRECT");
                    IncreaseCorrectCount();
                }
                else if (!question.isCorrect && question.isTrickQuestion)
                {
                    Debug.LogWarning("WRONG");
                    IncreaseWrongCount();
                }
            }

            PlayerPrefs.SetInt("NanoInspector_UpCounter", upCounter);
            PlayerPrefs.SetInt("NanoInspector_DownCounter", downCounter);

            DecideLevel();
            StartLevel();
        }

        public void EndLevel()
        {
            isQuestionTimerOn = false;
            uiManager.SetQuestionPanel(false);
            rightMonitor.SetActive(false);
            leftMonitor.SetActive(false);

            firstOrganism.transform.DOKill();
            secondOrganism.transform.DOKill();

            GameManager.instance.Finish();
        }

        public void IncreaseWrongCount()
        {
            AudioManager.instance.PlayOneShot("Wrong");
            wrongCount++;
            wrong++;
            downCounter++;
            uiManager.LightRed();
            uiManager.SetStats(correctCount, wrongCount);
        }

        public void IncreaseCorrectCount()
        {
            AudioManager.instance.PlayOneShot("Correct");
            correctCount++;
            correct++;
            upCounter++;
            uiManager.LightGreen();
            uiManager.SetStats(correctCount, wrongCount);
        }

        public int GetPenaltyPoints()
        {
            return levelSO.penaltyPoints;
        }

        public int GetMaxInLevel()
        {
            return levelSO.maxInLevel;
        }

        private void DecideLevel()
        {
            upCounter = PlayerPrefs.GetInt("NanoInspector_UpCounter", 0);
            downCounter = PlayerPrefs.GetInt("NanoInspector_DownCounter", 0);

            if (upCounter >= levelSO.levelUpCriteria * 2)
            {
                if (CalculateLevelScore(correct, wrong, levelSO.penaltyPoints, levelSO.maxInLevel) >= levelSO.minScore)
                {
                    LevelUp();
                }
                else
                {
                    LevelDown();
                }
            }
            else if (downCounter >= levelSO.levelDownCriteria)
            {
                LevelDown();
            }

            PlayerPrefs.SetInt("NanoInspector_UpCounter", upCounter);
            PlayerPrefs.SetInt("NanoInspector_DownCounter", downCounter);
        }

        public int CalculateLevelScore(int correct, int wrong, int penaltyPoints, int maxScore)
        {
            int levelScore = Mathf.CeilToInt((correct * 100) - (wrong * penaltyPoints));
            levelScore = Mathf.Clamp(levelScore, 0, maxScore);

            // scoreText.text = $"Score: {levelScore}";

            return levelScore;
        }

        public int CalculateScore()
        {
            int numerator = (correctCount * 100) - (wrongCount * levelSO.penaltyPoints);
            Debug.Log($"CalculateScore: correctCount = {correctCount}, wrongCount = {wrongCount}, penaltyPoints = {levelSO.penaltyPoints}, numerator = {numerator}");
            int denominator = (correctCount + wrongCount) * 100;
            Debug.Log($"CalculateScore: correctCount = {correctCount}, wrongCount = {wrongCount}, denominator = {denominator}");
            float intermediateResult = (float)numerator / denominator;
            Debug.Log($"CalculateScore: numerator = {numerator}, denominator = {denominator}, intermediateResult = {intermediateResult}");
            int score = Mathf.CeilToInt(intermediateResult * 1000);
            Debug.Log($"CalculateScore: score = {score}");
            score = Mathf.Clamp(score, 0, 1000);
            Debug.Log($"CalculateScore: Clamped score to: {score} (between 0 and 1000)");

            return score;
        }

        private void LevelUp()
        {
            levelId++;
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            levelText.text = "Level: " + levelId;

            downCounter = 0;
            upCounter = 0;
            wrong = 0;
            correct = 0;
        }

        private void LevelDown()
        {
            levelId--;
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            levelText.text = "Level: " + levelId;

            downCounter = 0;
            upCounter = 0;
            wrong = 0;
            correct = 0;
        }

        IEnumerator SpawnRoutine()
        {
            isQuestionTimerOn = false;
            yield return new WaitForSeconds(0.33f);

            AssignLevelVariables();
            SpawnOrganism();
            GenerateQuestion();
            uiManager.SetVariables();

            AudioManager.instance.PlayOneShot("MicroscopeStartup");
            microscopeParticle.gameObject.SetActive(true);
            microscopeParticle.Play();
            yield return new WaitForSeconds(0.18f);

            if (firstOrgObj != null)
            {
                firstOrgObj.SetActive(false);
                secondOrgObj.SetActive(false);
                uiManager.questionText.enabled = false;
            }

            AudioManager.instance.PlayOneShot("MicroscopeCalibration");
            yield return new WaitForSeconds(0.25f);

            firstOrgObj.transform.localScale = Vector3.zero;
            secondOrgObj.transform.localScale = Vector3.zero;

            firstOrgObj.SetActive(true);
            secondOrgObj.SetActive(true);

            firstOrgObj.transform.DOPause();
            secondOrgObj.transform.DOPause();

            Tween firstZoomTween = firstOrgObj.transform.DOScale(0.20f, 0.25f).SetEase(Ease.InOutExpo);
            secondOrgObj.transform.DOScale(0.20f, 0.25f).SetEase(Ease.InOutExpo);
            yield return firstZoomTween.WaitForCompletion();

            Tween secondZoomTween = firstOrgObj.transform.DOScale(0.38f, 0.3f).SetEase(Ease.InOutExpo);
            secondOrgObj.transform.DOScale(0.38f, 0.3f).SetEase(Ease.InOutExpo);
            yield return secondZoomTween.WaitForCompletion();

            firstOrgObj.transform.DOPlay();
            secondOrgObj.transform.DOPlay();
            uiManager.questionText.enabled = true;
            isButtonPressable = true;
            isQuestionTimerOn = true;
            isStartLevelRunning = false;
        }
    }
}