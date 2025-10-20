using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Witmina_InputController;

namespace Witmina_SpaceBurgers
{
    public class FoodArea : MonoBehaviour
    {
        [SerializeField] private List<Food> _foods;
        [SerializeField] private GameObject _serveButton;
        [SerializeField] private TrashCan _trashCan;

        public List<Food> Foods => _foods;
        
        private Vector3 _initPos;
        private Vector3 _dragOffset;
        private bool _clicked;
        private bool _tcOpen;
        private LayerMask _bgMask;
        private LayerMask _foodMask;
        private LayerMask _uiMask;

        private Transform _foodTransform;

        public bool Active;

        public void Awake()
        {
            _bgMask = LayerMask.GetMask("Water");
            _foodMask = LayerMask.GetMask("TransparentFX");
            _uiMask = LayerMask.GetMask("UI");
            foreach (var food in _foods)
            {
                food.gameObject.SetActive(false);
            }
        }

        public void Initialize()
        {
            foreach (var food in _foods)
            {
                food.ResetFood();
                food.gameObject.SetActive(false);
            }
            Container.IngredientSelected += OnContainerSelected;
            Container.IngredientAdded += OnContainerIngredientAdded;

            InputController.Instance.Pressed += OnPointerDown;
            InputController.Instance.Moved += OnPointerMove;
            InputController.Instance.Released += OnPointerUp;

            _tcOpen = false;
            _clicked = false;
            Active = false;
        }

        private void Update()
        {
            _serveButton.SetActive(_foods.Any(f => f.gameObject.activeSelf));

            _trashCan.Open = _clicked && _tcOpen;
        }

        public void OnDestroy()
        {
            Container.IngredientSelected -= OnContainerSelected;
            Container.IngredientAdded -= OnContainerIngredientAdded;

            if (!InputController.Instance)
                return;
            
            InputController.Instance.Pressed -= OnPointerDown;
            InputController.Instance.Moved -= OnPointerMove;
            InputController.Instance.Released -= OnPointerUp;
        }

        private void OnContainerSelected(Container fromContainer)
        {
            if (!Active)
                return;
            
            var ingredientType = fromContainer.IngredientType;
            var food = _foods.FirstOrDefault(f => f.HasType(ingredientType));
            if (food == null)
            {
                Debug.LogError($"No food that has ingredient {ingredientType} in it was found");
                return;
            }

            if (food.IngredientCanBeAdded(ingredientType))
            {
                food.PlayFx();
                fromContainer.DecreaseStock();
                fromContainer.AddIngredient();
            }
        }
        
        private void OnContainerIngredientAdded(Container fromContainer)
        {
            var ingredientType = fromContainer.IngredientType;
            var food = _foods.FirstOrDefault(f => f.HasType(ingredientType));
            if (food == null)
            {
                Debug.LogError($"No food that has ingredient {ingredientType} in it was found");
                return;
            }
            
            food.gameObject.SetActive(true);
            if (food.AddIngredient(ingredientType))
            {
                GameManager.PlayAudioFx(AudioFxType.SelectIngredient);
            }
        }

        public void OnFinish()
        {
            Container.IngredientSelected -= OnContainerSelected;
            Container.IngredientAdded -= OnContainerIngredientAdded;
            
            InputController.Instance.Pressed -= OnPointerDown;
            InputController.Instance.Moved -= OnPointerMove;
            InputController.Instance.Released -= OnPointerUp;
            
            
            Active = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Active)
                return;
            
            var ray = GameManager.Instance.MainCamera.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out var hit, 20f, _foodMask))
            {
                _clicked = true;

                _foodTransform = hit.transform;
                _initPos = _foodTransform.position;
                    
                _dragOffset = hit.point - _initPos;
                _dragOffset.z = 0f;
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!_clicked)
                return;
            
            var ray = GameManager.Instance.MainCamera.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out var hit, 20f, _bgMask))
            {
                _foodTransform.position = hit.point - _dragOffset;
            }

            _tcOpen = Physics.Raycast(ray, out var trashHit, 20f, _uiMask)
                      && trashHit.collider.TryGetComponent<TrashCan>(out var tc);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_clicked)
                return;
            
            _clicked = false;
            _tcOpen = false;
                
            var ray = GameManager.Instance.MainCamera.ScreenPointToRay(eventData.position);
            if (Physics.Raycast(ray, out var hit, 20f, _uiMask)
                && hit.collider.TryGetComponent<TrashCan>(out var tc))
            {
                var food = _foodTransform.GetComponent<Food>();
                food.ResetFood();
                food.gameObject.SetActive(false);
                    
                GameManager.PlayAudioFx(AudioFxType.TrashThrow);
            }

            _foodTransform.position = _initPos;
        }
    }
}