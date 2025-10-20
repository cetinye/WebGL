using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Witmina_SpaceBurgers
{
    public class Customer : MonoBehaviour
    {
        public event Action TimeUp;

        [SerializeField] private GameObject _speechBalloon;
        [SerializeField] private SpriteRenderer _customerSprite;
        [SerializeField] private Animator _feedbackAnimator;
        [SerializeField] private float _patienceTimer = 30f;
        [SerializeField] private float _initialOffset = 8f;
        [SerializeField] private SpriteRenderer _emojiSprite;
        [SerializeField] private List<Sprite> _emojis;
        [SerializeField] private TMP_Text _orderText;
        [SerializeField] private Transform _burgerSpritesParent;
        [SerializeField] private Transform _friesSpritesParent;
        [SerializeField] private Transform _drinkSpritesParent;
        [SerializeField] private float _burgerSpritesOffset = 0.1f;
        [SerializeField] private float _friesSpritesOffset;
        [SerializeField] private int _spritesMinSortOrder = -11;

        [Header("Mask")]
        [SerializeField] private Transform mask;
        [SerializeField] private Transform maskStartPos;
        [SerializeField] private Transform maskEndPos;

        private List<Order> _orders;
        private readonly List<SpriteRenderer> _orderSprites = new();

        private bool _timerRunning;

        public bool TimerRunning
        {
            get => _timerRunning;
            set
            {
                _timerRunning = value;
                _emojiSprite.gameObject.SetActive(_timerRunning);
            }
        }
        public int OrderCount;

        private bool _orderGenerated = false;
        private bool _finished;

        private float _timer;

        public float Timer
        {
            get => _timer;
            set
            {
                _timer = value;
                var index = Mathf.FloorToInt(_emojis.Count * _timer / Mathf.Max(_patienceTimer, 1));
                _emojiSprite.sprite = _emojis[Math.Clamp(index, 0, _emojis.Count - 1)];
            }
        }

        public List<Order> Orders => _orders;

        private void Awake()
        {
            _customerSprite.sprite = null;
            _feedbackAnimator.gameObject.SetActive(false);
            TimerRunning = false;
        }

        private void Update()
        {
            if (_finished || !TimerRunning)
                return;

            if (Timer > 0)
                Timer -= Time.deltaTime;
            else
            {
                Timer = 0;
                _finished = true;
                TimeUp?.Invoke();
            }
        }

        public void Initialize()
        {
            _speechBalloon.SetActive(false);
            _feedbackAnimator.gameObject.SetActive(false);
            TimerRunning = false;
            _orderGenerated = false;
            Timer = _patienceTimer;
            _finished = false;
        }

        public void ResetSprite()
        {
            _customerSprite.sprite = GameManager.PrefabData.GetRandomCustomerSprite(_customerSprite.sprite);
        }

        public void RotateSprite(float val)
        {
            _customerSprite.transform.rotation = new Quaternion(0f, val, 0f, 1f);
        }

        public void GenerateOrders()
        {
            _orders = new List<Order>();
            var orderCount = LevelBehaviour.LevelSO.maxOrderCount;

            List<FoodType> foodTypes = new();
            if (LevelBehaviour.LevelSO.byProducts >= 2)
                foodTypes = new List<FoodType> { FoodType.Burger, FoodType.Fries, FoodType.Drink };
            else if (LevelBehaviour.LevelSO.byProducts >= 1)
                foodTypes = new List<FoodType> { FoodType.Burger, FoodType.Fries };
            else if (LevelBehaviour.LevelSO.byProducts >= 0)
                foodTypes = new List<FoodType> { FoodType.Burger };

            for (int i = 0; i < orderCount && foodTypes.Count > 0; i++)
            {
                for (int k = 0; k < foodTypes.Count; k++)
                {

                    var type = i == 0 ? foodTypes[0] : foodTypes[Random.Range(0, foodTypes.Count)];
                    foodTypes.Remove(type);

                    var ingredientList = GameManager.PrefabData.GetIngredientList(type);
                    var ingredientCount = LevelBehaviour.LevelSO.ingredientsCount - 3;
                    var ingredients = new List<IngredientType>();
                    var byProductCount = LevelBehaviour.LevelSO.byProducts;

                    switch (type)
                    {
                        case FoodType.Burger:
                            ingredients.Add(IngredientType.BreadBottom);
                            ingredientList.Remove(IngredientType.BreadBottom);

                            Debug.Log(ingredientCount);
                            var meatType = ingredientCount >= 1 ? Random.Range(0f, 1f) < 0.5f ? IngredientType.Beef : IngredientType.Chicken : IngredientType.Chicken;

                            ingredients.Add(meatType);
                            ingredientList.Remove(IngredientType.Beef);
                            ingredientList.Remove(IngredientType.Chicken);

                            ingredients.Add(IngredientType.BreadTop);
                            ingredientList.Remove(IngredientType.BreadTop);
                            for (int j = 0; j < ingredientCount && j < ingredientList.Count; j++)
                            {
                                IngredientType ingredient;

                                if (LevelBehaviour.LevelSO.ingredientsCount - 3 >= 4)
                                    ingredient = ingredientList[Random.Range(0, ingredientList.Count)];

                                else if (LevelBehaviour.LevelSO.ingredientsCount - 3 >= 3)
                                    ingredient = ingredientList[Random.Range(1, ingredientList.Count)];

                                else if (LevelBehaviour.LevelSO.ingredientsCount - 3 >= 2)
                                    ingredient = ingredientList[Random.Range(2, ingredientList.Count)];

                                else
                                    ingredient = ingredientList[Random.Range(3, ingredientList.Count)];

                                ingredients.Insert(ingredients.Count - 1, ingredient);
                                ingredientList.Remove(ingredient);
                            }
                            break;
                        case FoodType.Fries:
                            ingredients.Add(IngredientType.Fries);
                            ingredientList.Remove(IngredientType.Fries);
                            for (int j = 0; j < byProductCount && j < ingredientList.Count; j++)
                            {
                                var ingredient = ingredientList[Random.Range(0, ingredientList.Count)];
                                ingredients.Add(ingredient);
                                ingredientList.Remove(ingredient);
                            }
                            break;
                        case FoodType.Drink:
                            ingredients.Add(ingredientList[Random.Range(0, ingredientList.Count)]);
                            ingredientList.Clear();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var order = new Order(type, ingredients);
                    _orders.Add(order);
                }

                SetOrderSprites();
                //SetOrderText();
                _speechBalloon.SetActive(true);
                GameManager.PlayAudioFx(AudioFxType.OrderGive);
                _orderGenerated = true;
            }
        }

        public void StartMaskMovement()
        {
            mask.localPosition = maskStartPos.localPosition;
            mask.DOLocalMove(maskEndPos.localPosition, _patienceTimer).SetEase(Ease.Linear);
        }

        private void ResetMask()
        {
            mask.DOKill();
            mask.localPosition = maskStartPos.localPosition;
        }

        public void GiveFeedback(bool success)
        {
            ClearOrderSprites();
            _feedbackAnimator.gameObject.SetActive(true);
            ResetMask();
            _feedbackAnimator.Play(success ? "Success" : "Fail");
        }

        private void ClearOrderSprites()
        {
            for (int i = 0; i < _orderSprites.Count; i++)
            {
                Destroy(_orderSprites[i].gameObject);
            }
            _orderSprites.Clear();
        }

        private void SetOrderSprites()
        {
            ClearOrderSprites();

            for (var o = 0; o < _orders.Count; o++)
            {
                var order = _orders[o];

                var ingredientCount = order.Ingredients.Count;
                for (int i = 0; i < ingredientCount; i++)
                {
                    var ingredient = order.Ingredients[i];
                    var sprite = Instantiate(new GameObject(ingredient.ToString()))
                        .AddComponent<SpriteRenderer>();

                    sprite.sprite = GameManager.PrefabData.GetIngredientSprite(ingredient);
                    sprite.sortingLayerName = "Sprites";

                    switch (order.FoodType)
                    {
                        case FoodType.Burger:
                            sprite.sortingLayerID = SortingLayer.NameToID("Front");
                            sprite.sortingOrder = _spritesMinSortOrder + i;
                            sprite.transform.SetParent(_burgerSpritesParent);
                            sprite.transform.localScale = Vector3.one;
                            sprite.transform.localPosition =
                                (-0.5f * _burgerSpritesOffset * ingredientCount + i * _burgerSpritesOffset)
                                * Vector3.up;
                            break;
                        case FoodType.Fries:
                            sprite.sortingLayerID = SortingLayer.NameToID("Front");
                            sprite.sortingOrder = _spritesMinSortOrder + i;
                            sprite.transform.SetParent(_friesSpritesParent);
                            sprite.transform.localScale = Vector3.one;
                            sprite.transform.localPosition = i == 0 ? Vector3.zero
                                : (0.2f + i * _friesSpritesOffset) * Vector3.right;
                            break;
                        case FoodType.Drink:
                            sprite.sortingLayerID = SortingLayer.NameToID("Front");
                            sprite.sortingOrder = _spritesMinSortOrder;
                            sprite.transform.SetParent(_drinkSpritesParent);
                            sprite.transform.localScale = Vector3.one;
                            sprite.transform.localPosition = Vector3.zero;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _orderSprites.Add(sprite);
                }
            }
        }

        private void SetOrderText()
        {
            _orderText.text = "Hi, I would like ";
            for (var o = 0; o < _orders.Count; o++)
            {
                if (o > 0)
                {
                    _orderText.text += o == _orders.Count - 1 ? ", and " : ", ";
                }
                var order = _orders[o];

                switch (order.FoodType)
                {
                    case FoodType.Burger:
                        var prefix =
                            order.Ingredients.First(i => i is IngredientType.Beef or IngredientType.Chicken);
                        _orderText.text += "a " + prefix.ToString().Replace("_", " ").ToLower();
                        break;
                    case FoodType.Fries:
                        break;
                    case FoodType.Drink:
                        _orderText.text += "a " + order.Ingredients[0].ToString().Replace("_", " ").ToLower();
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _orderText.text += " " + order.FoodType.ToString().Replace("_", " ").ToLower();

                var first = true;
                for (int i = 0; i < order.Ingredients.Count; i++)
                {
                    var ingredient = order.Ingredients[i];
                    if (ingredient is IngredientType.BreadBottom or IngredientType.BreadTop or IngredientType.Beef
                        or IngredientType.Chicken or IngredientType.Fries)
                        continue;
                    if (first)
                    {
                        first = false;
                        _orderText.text += " with ";
                    }
                    else
                    {
                        _orderText.text += i == order.Ingredients.Count - 1 ? " and " : ", ";
                    }

                    _orderText.text += order.Ingredients[i].ToString().ToLower();
                }
            }

            _orderText.text += " please.";
        }
    }
}
