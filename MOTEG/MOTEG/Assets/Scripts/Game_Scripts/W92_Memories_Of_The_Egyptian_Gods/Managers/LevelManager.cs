using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace W92_Memories_Of_The_Egyptian_Gods
{
    public class LevelManager : MonoBehaviour
    {
        public int levelId;
        [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
        public static LevelSO LevelSO;

        [SerializeField] TMP_Text levelText;

        public static LevelManager instance;

        public List<Sprite> possibleSymbols = new List<Sprite>();
        public List<GameObject> cardsList = new List<GameObject>();
        public Sprite cardBackImage, cardFrontImage;
        public int cardAmount;
        public int score;

        public int correctCount;
        public int wrongCount;
        public int correct;
        public int wrong;

        [SerializeField] private GameObject objParent;
        [SerializeField] private GameObject cardToSpawn;

        private List<Sprite> newPossibleSymbols = new List<Sprite>();
        private int maxLevelWKeys;

        void Awake()
        {
            instance = this;
        }

        public void AssignLevel()
        {
            maxLevelWKeys = levels.Count;
            Debug.LogWarning("MaxLevelWKeys: " + maxLevelWKeys);

            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            LevelSO = levels[levelId - 1];
            Debug.Log("Level SO: " + LevelSO);

            levelText.text = "Level " + levelId;
        }

        public void SpawnCards()
        {

            levelId = Mathf.Clamp(levelId, 1, maxLevelWKeys);
            UIManager.instance.AssignLevelVariables();

           


            newPossibleSymbols.Clear();

            for (int i = 0; i < cardAmount; i++)
            {
                GameObject spawnedCard = Instantiate(cardToSpawn, objParent.transform);
                spawnedCard.GetComponent<Card>().front = cardFrontImage;
                spawnedCard.GetComponent<Card>().back = cardBackImage;
                cardsList.Add(spawnedCard);
                GameManager.instance.activeCards.Add(spawnedCard);
            }

            AssignSymbols();
        }

        private void AssignSymbols()
        {
            ArrangePossibleSymbols();

            for (int i = 0; i < cardsList.Count; i++)
            {
                cardsList[i].transform.GetChild(0).GetComponent<Image>().sprite = newPossibleSymbols[i];
            }

            if (objParent.TryGetComponent(out ManualGridLayout grid))
                objParent.GetComponent<ManualGridLayout>().ArrangeGrid();
        }

        private void ArrangePossibleSymbols()
        {
            for (int i = 0; i < cardAmount / 2; i++)
            {
                newPossibleSymbols.Add(possibleSymbols[i]);
                newPossibleSymbols.Add(possibleSymbols[i]);
            }

            newPossibleSymbols.Shuffle();
        }

        public void ClearCards()
        {
            cardsList.Clear();

            for (int i = 0; i < objParent.transform.childCount; i++)
            {
                Destroy(objParent.transform.GetChild(i).gameObject);
            }
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