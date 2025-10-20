using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public class DrinkFood : Food
    {
        [SerializeField] private Transform _foodTransform;
        //[SerializeField] private List<IngredientType> _ingredientTypes;
        public override bool IsComplete() => _complete;

        //private readonly List<DrinkIngredient> _ingredients = new();
        public override bool IngredientCanBeAdded(IngredientType ingredientType)
        {
            var can = HasType(ingredientType) && !_complete;
            return can;
        }

        public override void PlayFx()
        {
            GameManager.PlayAudioFx(AudioFxType.DrinkPour);
        }
        public override bool AddIngredient(IngredientType ingredientType)
        {
            if (!IngredientCanBeAdded(ingredientType))
                return false;

            if (GameManager.PrefabData.GetIngredient(ingredientType) is not DrinkIngredient ingredientPrefab)
                return false;
            
            _complete = true;
            
            var ingredient = Instantiate(ingredientPrefab, _foodTransform);
            ingredient.transform.localScale = Vector3.one;

            ingredient.transform.localPosition = 2f * Vector3.up;
            ingredient.transform.DOLocalMoveY(0f, 0.25f)
                .OnComplete(() => ingredient.Animate(0.1f));
            _ingredients.Add(ingredient);
            
            return true;
        }
    }
}

