using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Localization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Guess_The_Move
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager instance;

        [SerializeField] private UIManager uiManager;

        [Header("Level Variables")]
        [SerializeField]
        public int levelId;

        [SerializeField] private LevelSO levelSO;
        [SerializeField] private List<LevelSO> levels = new();
        public int totalCorrectCount;
        public int totalWrongCount;
        [SerializeField] private int correctCount;
        [SerializeField] private int wrongCount;
        [SerializeField] private int score;
        [SerializeField] private int correctAnswerStreak;
        [SerializeField] private int wrongAnswerStreak;

        [Header("Characters")] public Character characterCurrent;

        public Character characterOld;
        [SerializeField] private float timeToMove;
        [SerializeField] private Transform startPos;
        [SerializeField] private Transform middlePos;
        [SerializeField] private Transform endPos;

        [Header("Question")][SerializeField] private Question selectedQuestion;

        [SerializeField] private List<Question> questions = new();
        [SerializeField] private List<Question> availableQuestions = new();

        [Header("Lists")][SerializeField] private List<Character> characters = new();

        public List<Color> colors = new();
        [SerializeField] private List<float> scores = new();

        [Header("Flash Interval")]
        [SerializeField]
        private bool isFlashable = true;

        private bool isLevelTimerOn;
        private bool isQuestionTimerOn;
        private float passedLevelTime;
        private float passedQuestionTime;
        private float questionDisplayTime;
        private int maxLevelWKeys;

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;
        }

        private void Update()
        {
            QuestionTimer();
            LevelTimer();
        }

        private void OnDisable()
        {
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

        public void StartGame()
        {
            maxLevelWKeys = levels.Count / 2;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            passedLevelTime = 60f;
            SetLevelVariables();

            AudioManager.instance.Play(SoundType.BackgroundAmbient);
            AudioManager.instance.Play(SoundType.BackgroundMusic);
            AudioManager.instance.Play(SoundType.CrowdCheer);

            StartCoroutine(FirstShowcase());
        }

        private void LevelTimer()
        {
            if (!isLevelTimerOn) return;

            passedLevelTime -= Time.deltaTime;
            uiManager.SetTimerText(passedLevelTime);

            if (passedLevelTime < 0f)
            {
                isLevelTimerOn = false;
                isQuestionTimerOn = false;
                passedLevelTime = 0;
                uiManager.SetTimerText(passedLevelTime);
                DOTween.KillAll();

                GameManager.instance.Finish();
            }

            if (passedLevelTime <= 5.2f && isFlashable)
            {
                isFlashable = false;
                // GameManager.instance.PlayFx("Countdown", 0.7f, 1f);
                uiManager.FlashRed();
            }
        }

        private void QuestionTimer()
        {
            if (!isQuestionTimerOn) return;

            passedQuestionTime -= Time.deltaTime;
            uiManager.UpdateQuestionTimer(passedQuestionTime, questionDisplayTime);

            if (passedQuestionTime <= 0f)
            {
                isQuestionTimerOn = false;
                DecideOnLevel(false);
                passedQuestionTime = 0;
                uiManager.SetButtonsPressable(false);
                uiManager.UpdateQuestionTimer(passedQuestionTime, questionDisplayTime);
                uiManager.UpdateCorrectWrongUI(totalCorrectCount, totalWrongCount);
                uiManager.Feedback(false);
            }
        }

        private void SetLevelVariables()
        {
            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            levelSO = levels[levelId - 1];

            questionDisplayTime = levelSO.questionDisplayTime;
            passedQuestionTime = questionDisplayTime;

            availableQuestions.Clear();
            SelectAvailableQuestions();
        }

        public void NewQuestion()
        {
            StartCoroutine(NewQuestionRoutine());
        }

        private IEnumerator NewQuestionRoutine()
        {
            AudioManager.instance.PlayOneShot(SoundType.Slide);
            Tween moveTween = characterCurrent.transform.DOMove(endPos.position, timeToMove);
            yield return moveTween.WaitForCompletion();

            passedQuestionTime = questionDisplayTime;

            selectedQuestion = GetRandomQuestion();

            characterOld = characterCurrent;
            characterOld.SetVisibility(false);

            if (Random.Range(1, 101) >= 50) //old character respawn
            {
                Debug.Log("Same character");
                characterCurrent = Instantiate(characterOld);
                characterCurrent.transform.position = startPos.position;
                SetAttributes(false);
                characterCurrent.Randomize();
                characterCurrent.SetVisibility(true);
            }
            else //new random character
            {
                Debug.LogWarning("DIFFERENT character");
                characterCurrent = Instantiate(GetRandomCharacter());
                characterCurrent.transform.position = startPos.position;
                SetAttributes(true);
                characterCurrent.Randomize();
            }

            AudioManager.instance.PlayOneShot(SoundType.Slide);
            moveTween = characterCurrent.transform.DOMove(middlePos.position, timeToMove);
            yield return moveTween.WaitForCompletion();
            AudioManager.instance.Play(SoundType.FootSound);

            uiManager.SetText(LeanLocalization.GetTranslationText(selectedQuestion.questionText));
            uiManager.SetButtonsPressable(true);
            isQuestionTimerOn = true;
        }

        private void SetAttributes(bool isChangable)
        {
            characterCurrent.isHairRandomized = !isChangable;
            characterCurrent.isShirtRandomized = !isChangable;
            characterCurrent.isPantRandomized = !isChangable;
            characterCurrent.isShoesRandomized = !isChangable;
            characterCurrent.isDanceRandomized = !isChangable;
            characterCurrent.isOutfitRandomized = !isChangable;

            foreach (var attributeToCheck in selectedQuestion.attributesToCheck)
            {
                if (attributeToCheck == Attributes.HAIR)
                    characterCurrent.isHairRandomized = isChangable;

                if (attributeToCheck == Attributes.SHIRT)
                    characterCurrent.isShirtRandomized = isChangable;

                if (attributeToCheck == Attributes.PANT)
                    characterCurrent.isPantRandomized = isChangable;

                if (attributeToCheck == Attributes.SHOES)
                    characterCurrent.isShoesRandomized = isChangable;

                if (attributeToCheck == Attributes.DANCE)
                    characterCurrent.isDanceRandomized = isChangable;

                if (attributeToCheck == Attributes.OUTFIT)
                    characterCurrent.isOutfitRandomized = isChangable;
            }
        }

        private void SelectAvailableQuestions()
        {
            if (levelSO.personTypeRange == 1)
                availableQuestions.Add(questions[0]);
            if (levelSO.personTypeRange == 2)
                availableQuestions.Add(questions[1]);
            if (levelSO.movementTypeRange == 1)
                availableQuestions.Add(questions[2]);
            if (levelSO.movementTypeRange == 2)
                availableQuestions.Add(questions[3]);
            if (levelSO.outfitTypeRange == 1)
                availableQuestions.Add(questions[4]);
            if (levelSO.outfitTypeRange == 2)
                availableQuestions.Add(questions[5]);
            if (levelSO.topOutfitColorRange == 1)
                availableQuestions.Add(questions[6]);
            if (levelSO.topOutfitColorRange == 2)
                availableQuestions.Add(questions[7]);
            if (levelSO.bottomOutfitColorRange == 1)
                availableQuestions.Add(questions[8]);
            if (levelSO.bottomOutfitColorRange == 2)
                availableQuestions.Add(questions[9]);
            if (levelSO.shoesColorRange == 1)
                availableQuestions.Add(questions[10]);
            if (levelSO.shoesColorRange == 2)
                availableQuestions.Add(questions[11]);
            if (levelSO.hairColorRange == 1)
                availableQuestions.Add(questions[12]);
            if (levelSO.hairColorRange == 2)
                availableQuestions.Add(questions[13]);
        }

        private Question GetRandomQuestion()
        {
            return availableQuestions[Random.Range(0, availableQuestions.Count)];
        }

        private IEnumerator FirstShowcase()
        {
            uiManager.SetButtonsPressable(false);

            characterOld = Instantiate(GetRandomCharacter());
            characterOld.transform.position = startPos.position;
            characterOld.InitialRandomize();

            AudioManager.instance.PlayOneShot(SoundType.Slide);
            Tween moveTween = characterOld.transform.DOMove(middlePos.position, timeToMove);
            yield return moveTween.WaitForCompletion();
            AudioManager.instance.Play(SoundType.FootSound);

            uiManager.SetText($"{LeanLocalization.GetTranslationText("MEMORIZE")}");
            yield return new WaitForSeconds(3);
            selectedQuestion = GetRandomQuestion();

            AudioManager.instance.PlayOneShot(SoundType.Slide);
            AudioManager.instance.Stop(SoundType.FootSound);
            moveTween = characterOld.transform.DOMove(endPos.position, timeToMove);
            yield return moveTween.WaitForCompletion();
            characterOld.SetVisibility(false);

            characterCurrent = Instantiate(GetRandomCharacter());
            characterCurrent.transform.position = startPos.position;
            characterCurrent.Randomize();

            AudioManager.instance.PlayOneShot(SoundType.Slide);
            moveTween = characterCurrent.transform.DOMove(middlePos.position, timeToMove);
            yield return moveTween.WaitForCompletion();
            AudioManager.instance.Play(SoundType.FootSound);

            uiManager.SetText(LeanLocalization.GetTranslationText(selectedQuestion.questionText));

            isQuestionTimerOn = true;
            isLevelTimerOn = true;
            uiManager.SetButtonsPressable(true);
        }

        private Character GetRandomCharacter()
        {
            return characters[Random.Range(0, characters.Count)];
        }

        public void CheckAnswer(bool isSwipedRight)
        {
            AudioManager.instance.PlayOneShot(SoundType.ButtonClick);
            AudioManager.instance.Stop(SoundType.FootSound);

            uiManager.SetButtonsPressable(false);

            if (isSwipedRight)
                uiManager.YesPressed();
            else
                uiManager.NoPressed();

            isQuestionTimerOn = false;

            var isSame = true;

            foreach (var attributeToCheck in selectedQuestion.attributesToCheck)
                if (!Compare(characterOld, characterCurrent, attributeToCheck))
                {
                    isSame = false;
                    break;
                }

            if (!selectedQuestion.isNegative && isSwipedRight && isSame)
            {
                DecideOnLevel(true);
                uiManager.Feedback(true);
            }
            else if (!selectedQuestion.isNegative && !isSwipedRight && !isSame)
            {
                DecideOnLevel(true);
                uiManager.Feedback(true);
            }
            else if (selectedQuestion.isNegative && isSwipedRight && !isSame)
            {
                DecideOnLevel(true);
                uiManager.Feedback(true);
            }
            else if (selectedQuestion.isNegative && !isSwipedRight && isSame)
            {
                DecideOnLevel(true);
                uiManager.Feedback(true);
            }
            else
            {
                DecideOnLevel(false);
                uiManager.Feedback(false);
            }

            uiManager.UpdateCorrectWrongUI(totalCorrectCount, totalWrongCount);
        }

        private void DecideOnLevel(bool isCorrect)
        {
            if (isCorrect)
            {
                totalCorrectCount++;
                correctCount++;
                correctAnswerStreak++;

                if (correctCount >= levelSO.levelUpCriteria * 2)
                {
                    correctCount = 0;
                    levelId++;
                }
            }
            else
            {
                totalWrongCount++;
                wrongCount++;
                wrongAnswerStreak++;
                correctAnswerStreak = 0;

                if (wrongCount >= levelSO.levelDownCriteria* 2)
                {
                    wrongCount = 0;
                    levelId--;
                }
            }

            SetLevelVariables();
        }

        public int GetTotalScore()
        {
            float totalScore = totalCorrectCount * levelSO.pointsPerCorrect +
                               totalWrongCount * levelSO.penaltyPoints;
            float maxInGame = (totalCorrectCount + totalWrongCount) * levelSO.pointsPerCorrect;
            var witminaScore = totalScore / maxInGame * 1000f;

            return Mathf.Clamp(Mathf.CeilToInt(witminaScore), 0, 1000);
        }

        private bool Compare(Character oldChar, Character currChar, Attributes attributeToCompare)
        {
            switch (attributeToCompare)
            {
                case Attributes.HAIR:
                    return oldChar.meshRenderer.materials[oldChar.hairMaterialIndex].color
                        .Equals(currChar.meshRenderer.materials[currChar.hairMaterialIndex].color);

                case Attributes.SHIRT:
                    return oldChar.meshRenderer.materials[oldChar.shirtMaterialIndex].color
                        .Equals(currChar.meshRenderer.materials[currChar.shirtMaterialIndex].color);

                case Attributes.PANT:
                    return oldChar.meshRenderer.materials[oldChar.pantMaterialIndex].color
                        .Equals(currChar.meshRenderer.materials[currChar.pantMaterialIndex].color);

                case Attributes.SHOES:
                    return oldChar.meshRenderer.materials[oldChar.shoesMaterialIndex].color
                        .Equals(currChar.meshRenderer.materials[currChar.shoesMaterialIndex].color);

                case Attributes.DANCE:
                    return oldChar.dance.Equals(currChar.dance);

                case Attributes.OUTFIT:
                    return oldChar.charType.Equals(currChar.charType);
            }

            return false;
        }
    }

    [Serializable]
    public class Question
    {
        public string questionText;
        public bool isNegative;
        public List<Attributes> attributesToCheck;
    }

    public enum Attributes
    {
        HAIR,
        SHIRT,
        PANT,
        SHOES,
        DANCE,
        OUTFIT
    }
}