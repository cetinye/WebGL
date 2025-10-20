using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Witmina_SpaceBurgers
{
    public class Container : MonoBehaviour
    {        
        public static event Action<Container> IngredientSelected;
        public static event Action<Container> IngredientAdded;

        [SerializeField] private IngredientType _ingredient;

        [SerializeField] private SpriteRenderer _foodSprite;
        [SerializeField] private List<Sprite> _sprites;
        [SerializeField] private GameObject _refillPanel;
        [SerializeField] private GameObject _refillSprite;
        [SerializeField] private Slider _refillSlider;
        [SerializeField] private float _refillDuration = 1f;

        public IngredientType IngredientType => _ingredient;

        private int MaxStock => _sprites.Count;
        private int _stock;
        private float _refillTimer;
        private bool _refillTimerRunning;

        public bool Active;
        public int Stock
        {
            get => _stock;
            private set
            {
                _stock = value;
                if (_stock > 0)
                {
                    _refillPanel.SetActive(false);
                    _foodSprite.gameObject.SetActive(true);
                    _foodSprite.sprite = _sprites[MaxStock - _stock];
                }
                else
                {
                    _foodSprite.gameObject.SetActive(false);
                    _refillPanel.SetActive(true);
                    _refillSprite.SetActive(true);
                    _refillTimer = 0;
                }
            }
        }

        private void Awake()
        {
            Active = false;
        }

        public void Initialize()
        {
            Stock = MaxStock;
            _refillSlider.gameObject.SetActive(false);
            Active = true;
        }

        private void Start()
        {
            Active = true;
        }

        public void OnSelect(PointerEventData eventData)
        {
            if (!Active)
                return;
            
            if (Stock == 0 && !_refillTimerRunning)
            {
                _refillTimerRunning = true;
                _refillSprite.SetActive(false);
                GameManager.PlayAudioFx(AudioFxType.RefillStart);
                return;
            }
            
            IngredientSelected?.Invoke(this);
        }

        public virtual void AddIngredient()
        {
            IngredientAdded?.Invoke(this);
        }

        public virtual void DecreaseStock()
        {
            Stock--;
        }

        protected virtual void Update()
        {
            if (!_refillTimerRunning)
                return;

            if (_refillTimer < _refillDuration)
            {
                _refillSlider.gameObject.SetActive(true);
                _refillSlider.value = _refillTimer / _refillDuration;
                _refillTimer += Time.deltaTime;
            }
            else
            {
                _refillTimerRunning = false;
                Stock = MaxStock;
                _refillTimer = 0;
                
                _refillSlider.gameObject.SetActive(false);
                _refillSprite.SetActive(true);
                
                GameManager.PlayAudioFx(AudioFxType.RefillComplete);
            }
        }
    }
}