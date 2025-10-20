using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Chefs_Secret_Recipes
{
    public class QuestionPanel : MonoBehaviour
    {
        [SerializeField] private List<Hint> ingredients = new List<Hint>();
        [SerializeField] private Equation defaultEqn;
        [SerializeField] private Equation multiplierEqn;
        [SerializeField] private Equation missingDefaultEqn;
        [SerializeField] private Equation missingMultiplierEqn;
        [SerializeField] private Image mealImage;
        private Image panelImage;
        private Vector3 mealDefaultPos;
        private Tween mealTween;
        private List<Hint> usedHints = new List<Hint>();
        private Equation selectedEqn;
        private List<Equation> eqnPrefabs = new List<Equation>();

        [Header("Answer Variables")]
        [SerializeField] private Transform answerPanel;
        [SerializeField] private int answerCount;
        [SerializeField] private Answer answerPref;
        private List<Answer> answers = new List<Answer>();

        [Header("Particle Variables")]
        [SerializeField] private ParticleSystem tastyParticle;
        [SerializeField] private ParticleSystem pukeParticle;

        void Awake()
        {
            panelImage = GetComponent<Image>();
            mealDefaultPos = mealImage.transform.localPosition;
        }

        public void ShowMeal(Sprite mealSprite)
        {
            mealImage.sprite = mealSprite;
            mealImage.DOFade(0f, 0f);
            mealImage.enabled = true;
            mealImage.DOFade(1f, 1f);
        }

        public Equation GenerateEquation()
        {
            LevelSO levelSO = LevelManager.instance.levelSO;

            AddIntoEqnPrefabs(defaultEqn, levelSO.defaultProbability);
            AddIntoEqnPrefabs(multiplierEqn, levelSO.multiplicativeProbability);
            AddIntoEqnPrefabs(missingDefaultEqn, levelSO.productFindingProbability);
            AddIntoEqnPrefabs(missingMultiplierEqn, levelSO.multiplicativeProductFindingProbability);

            selectedEqn = Instantiate(GetRandomEqn(), transform);
            selectedEqn.transform.SetSiblingIndex(transform.childCount - 2); //to make sure that meal image stays always in front

            mealTween = mealImage.transform.DOLocalRotate(new Vector3(0f, 0f, mealImage.transform.rotation.eulerAngles.z + 30f), 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutCubic);

            return selectedEqn;
        }

        private void AddIntoEqnPrefabs(Equation eqn, float probability)
        {
            for (int i = 0; i < probability; i++)
            {
                eqnPrefabs.Add(eqn);
            }
        }

        private Equation GetRandomEqn()
        {
            return eqnPrefabs[Random.Range(0, eqnPrefabs.Count)];
        }

        public void AddToIngredients(Hint ingredient)
        {
            ingredients.Add(ingredient);
        }

        public Hint GetIngredient()
        {
            Hint selectedHint = null;

            do
            {
                selectedHint = ingredients[Random.Range(0, ingredients.Count)];

            } while (usedHints.Contains(selectedHint));

            usedHints.Add(selectedHint);
            return selectedHint;
        }

        public void SpawnAnswers(bool isMissingQuestion = false)
        {
            Answer answer = null;

            for (int i = 0; i < answerCount; i++)
            {
                answer = Instantiate(answerPref, answerPanel);
                answers.Add(answer);
            }

            SetAnswers(isMissingQuestion);
        }

        public void SetAnswers(bool isMissingQuestion = false)
        {
            List<int> chosenAnswers = new List<int>();
            List<Sprite> usedSprites = new List<Sprite>();
            int correctAnswer = selectedEqn.GetCorrectAnswer();
            int offset = 0;

            for (int i = 0; i < answerCount; i++)
            {
                do
                {
                    offset = Random.Range(-4, 0) + Random.Range(1, 5);

                } while (correctAnswer + offset == correctAnswer || chosenAnswers.Contains(correctAnswer + offset));

                chosenAnswers.Add(correctAnswer + offset);
                answers[i].SetText(correctAnswer + offset);
            }

            answers[Random.Range(0, answerCount)].SetText(correctAnswer);

            if (isMissingQuestion)
            {
                Sprite chosenSp = null;

                for (int i = 0; i < answers.Count; i++)
                {
                    do
                    {
                        chosenSp = ingredients[Random.Range(0, ingredients.Count)].GetSprite();

                    } while (GetCorrectSprite(correctAnswer) == chosenSp || usedSprites.Contains(chosenSp));
                    usedSprites.Add(chosenSp);
                    answers[i].SetImage(chosenSp);
                }
                int randIndex = Random.Range(0, answerCount);
                answers[randIndex].SetImage(GetCorrectSprite(correctAnswer));
                answers[randIndex].SetText(correctAnswer);
            }
        }

        public int GetCorrectAnswer()
        {
            return selectedEqn.GetCorrectAnswer();
        }

        public Sprite GetCorrectSprite(int val)
        {
            for (int i = 0; i < ingredients.Count; i++)
            {
                if (ingredients[i].GetValue() == val)
                    return ingredients[i].GetSprite();
            }

            return null;
        }

        public void SetPanelColor(Color newColor)
        {
            panelImage.DOColor(newColor, 0.5f);
        }

        public Image GetMealImage()
        {
            return mealImage;
        }

        public Tween FadeMeal(float alpha, float timeToShow)
        {
            return mealImage.DOFade(alpha, timeToShow).SetEase(Ease.Linear);
        }

        public void PlayParticle(bool isCorrect)
        {
            if (isCorrect)
            {
                tastyParticle.gameObject.SetActive(true);
                tastyParticle.Play();
            }
            else
            {
                pukeParticle.gameObject.SetActive(true);
                pukeParticle.Play();
            }
        }

        public void Disappear()
        {
            AudioManager.instance.PlayOneShot(SoundType.ScaleDown);

            foreach (Hint ingredient in ingredients)
            {
                ingredient.transform.DOScale(Vector3.zero, 0.5f);
            }

            foreach (Answer answerItem in answers)
            {
                answerItem.transform.DOScale(Vector3.zero, 0.5f);
            }

            mealImage.transform.DOScale(Vector3.zero, 0.5f);

            if (selectedEqn != null)
                selectedEqn.transform.DOScale(Vector3.zero, 0.5f);
        }

        public void Reset(bool isFullReset = true)
        {
            if (isFullReset)
            {
                foreach (Hint ingredient in ingredients)
                {
                    Destroy(ingredient.gameObject);
                }
            }

            foreach (Answer answerItem in answers)
            {
                Destroy(answerItem.gameObject);
            }

            SetPanelColor(Color.white);
            eqnPrefabs.Clear();
            answers.Clear();
            usedHints.Clear();
            ingredients.Clear();
            Destroy(selectedEqn != null ? selectedEqn.gameObject : null);
            mealImage.enabled = false;
            mealImage.transform.localPosition = mealDefaultPos;
            mealImage.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            pukeParticle.Stop();
            pukeParticle.gameObject.SetActive(false);
            tastyParticle.Stop();
            tastyParticle.gameObject.SetActive(false);
        }
    }
}