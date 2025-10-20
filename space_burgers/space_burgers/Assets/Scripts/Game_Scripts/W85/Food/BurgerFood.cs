using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public class BurgerFood : Food
    {
        [SerializeField] private Transform _foodTransform;
        //[SerializeField] private List<IngredientType> _ingredientTypes;

        //private readonly List<BurgerIngredient> _ingredients = new();
        private static readonly int MaxIngredients = 10;

        private void Awake()
        {
            _complete = false;
        }

        public override bool IsComplete() => _complete;
        
        public override bool IngredientCanBeAdded(IngredientType ingredientType)
        {
            return HasType(ingredientType) && !_complete;
        }
        public override bool AddIngredient(IngredientType ingredientType)
        {
            if (!IngredientCanBeAdded(ingredientType))
                return false;

            Ingredient ingredientPrefab;
            if (ingredientType is IngredientType.BreadBottom && _ingredients.Count > 0)
            {
                ingredientPrefab = GameManager.PrefabData.GetIngredient(IngredientType.BreadTop);
                _complete = true;
            }
            else
                ingredientPrefab = GameManager.PrefabData.GetIngredient(ingredientType);

            if (ingredientPrefab is not BurgerIngredient)
                return false;
            

            var ingredient = Instantiate(ingredientPrefab, _foodTransform);
            ingredient.transform.localScale = Vector3.one;
            
            var sum = _ingredients.Sum(i => ((BurgerIngredient)i).Height);
            var yPos = sum + ((BurgerIngredient)ingredient).Height / 2f;
            
            
            ingredient.transform.localPosition = (yPos + 2f) * Vector3.up;
            ingredient.transform.DOLocalMoveY(yPos, 0.25f)
                .OnComplete(() => ingredient.Animate(0.1f));
            
            if(_ingredients.Count == 0)
                GameManager.PlayAudioFx(AudioFxType.BurgerPlate);
            
            _ingredients.Add(ingredient);
            if (_ingredients.Count >= MaxIngredients)
                _complete = true;
            
            return true;
        }
    }
}

