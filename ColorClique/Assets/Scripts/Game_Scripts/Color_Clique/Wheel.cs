using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

namespace Color_Clique
{
    public class Wheel : MonoBehaviour
    {
        public int numberOfSlots;
        public Transform center;
        [SerializeField] private Needle needle;
        [SerializeField] private SpriteRenderer feedbackRenderer;
        private WheelPart spawnedWheel;
        private int numberOfColors;
        private int shapeCount;

        [Header("Wheel Prefabs")]
        [SerializeField] private WheelPart wheel4;
        [SerializeField] private WheelPart wheel5;
        [SerializeField] private WheelPart wheel6;
        [SerializeField] private WheelPart wheel8;
        [SerializeField] private WheelPart wheel9;
        [SerializeField] private WheelPart wheel10;
        [SerializeField] private WheelPart wheel12;

        [Header("Lists")]
        [SerializeField] private List<Sprite> items = new List<Sprite>();
        [SerializeField] private List<Color> colors = new List<Color>();
        [SerializeField] private List<Sprite> usedItems = new List<Sprite>();
        [SerializeField] private List<Color> usedColors = new List<Color>();

        [Header("Slider")]
        [SerializeField] private UnityEngine.UI.Slider slider;
        private float passedTime;
        private float timePerQuestion;
        private bool isSliderActive;

        void Awake()
        {
            isSliderActive = false;

            items.Shuffle();
            colors.Shuffle();
        }

        void Update()
        {
            if (!isSliderActive) return;

            passedTime += Time.deltaTime;
            slider.value = 1f - (passedTime / timePerQuestion);

            if (slider.value <= 0)
            {
                isSliderActive = false;
                passedTime = 0f;
                slider.value = 1f - (passedTime / timePerQuestion);
                LevelManager.instance.Check(null, Color.clear, true);
            }
        }

        public void AssignWheelVariables(int numberOfSlots, float needleRotateSpeed)
        {
            this.numberOfSlots = numberOfSlots;
            needle.SetNeedleSpeed(needleRotateSpeed);
        }

        public void Initialize()
        {
            numberOfColors = LevelManager.instance.GetNumberOfColors();
            shapeCount = LevelManager.instance.GetShapeCount();

            SpawnWheel(numberOfSlots);
        }

        public void SpawnWheel(int numberOfSlots)
        {
            if (spawnedWheel != null)
            {
                Reset();
                Destroy(spawnedWheel.gameObject);
            }

            spawnedWheel = numberOfSlots switch
            {
                4 => Instantiate(wheel4, transform),
                5 => Instantiate(wheel5, transform),
                6 => Instantiate(wheel6, transform),
                8 => Instantiate(wheel8, transform),
                9 => Instantiate(wheel9, transform),
                10 => Instantiate(wheel10, transform),
                12 => Instantiate(wheel12, transform),
                _ => Instantiate(wheel12, transform),
            };

            spawnedWheel.Initialize(numberOfColors, shapeCount);
            spawnedWheel.transform.SetSiblingIndex(0);
        }

        public Slot SelectSlot()
        {
            return spawnedWheel.GetRandomSlot();
        }

        public void GiveFeedback(Color feedbackColor)
        {
            feedbackRenderer.color = feedbackColor;
            StopCoroutine(FlashFeedback());
            StartCoroutine(FlashFeedback());
        }

        public void SetNeedleColor(Color color, float duration)
        {
            needle.SetNeedleColor(color, duration);
        }

        public void ReverseNeedle()
        {
            needle.ReverseNeedle();
        }

        public Slot GetClickedSlot()
        {
            return needle.GetOverlappingSlot();
        }

        public void SetSliderState(bool state)
        {
            isSliderActive = state;
        }

        public Sprite GetRandomItem()
        {
            Sprite item;

            do
            {
                item = items[Random.Range(0, items.Count)];

            } while (usedItems.Contains(item));

            usedItems.Add(item);
            return item;
        }

        public Color GetRandomColor()
        {
            Color randColor;

            do
            {
                randColor = colors[Random.Range(0, items.Count)];

            } while (usedColors.Contains(randColor));

            usedColors.Add(randColor);
            return randColor;
        }

        public void StartTimer(float value)
        {
            passedTime = 0f;
            slider.value = 1f;
            timePerQuestion = value;
            isSliderActive = true;
        }

        private void Reset()
        {
            usedItems.Clear();
            usedColors.Clear();
        }

        public void ResetColors()
        {
            usedColors.Clear();
        }

        IEnumerator FlashFeedback()
        {
            feedbackRenderer.enabled = true;
            yield return new WaitForSeconds(0.2f);
            feedbackRenderer.enabled = false;
            yield return new WaitForSeconds(0.2f);
            feedbackRenderer.enabled = true;
            yield return new WaitForSeconds(0.2f);
            feedbackRenderer.enabled = false;
            yield return new WaitForSeconds(0.2f);
            feedbackRenderer.enabled = true;
            yield return new WaitForSeconds(0.2f);
            feedbackRenderer.enabled = false;
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