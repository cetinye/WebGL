using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public class FriesFood : Food
    {
        [SerializeField] private Transform _friesTransform;
        [SerializeField] private List<Transform> _sauceSlots;
        //[SerializeField] private List<IngredientType> _ingredientTypes;
        public override bool IsComplete() => _complete;

        //private readonly List<Ingredient> _ingredients = new();

        private void Awake()
        {
            _friesTransform.gameObject.SetActive(false);
        }
        
        public override bool IngredientCanBeAdded(IngredientType ingredientType)
        {
            return HasType(ingredientType) && _ingredients.All(i => i.Type != ingredientType);
        }

        public override bool AddIngredient(IngredientType ingredientType)
        {
            if (!IngredientCanBeAdded(ingredientType))
                return false;

            var ingredientPrefab = GameManager.PrefabData.GetIngredient(ingredientType);
            if (ingredientPrefab is not (FriesIngredient or SauceIngredient))
                return false;

            var parent = _friesTransform;
            if (ingredientType is IngredientType.Fries)
            {
                _complete = true;
                GameManager.PlayAudioFx(AudioFxType.FriesPour);
            }
            else
            {
                var count = _ingredients.Count(i => i is SauceIngredient);
                if (!_complete
                    || count >= _sauceSlots.Count)
                    return false;

                GameManager.PlayAudioFx(AudioFxType.SauceSplash, 0.5f);
                parent = _sauceSlots[count];
            }
            
            _friesTransform.gameObject.SetActive(true);
            
            var ingredient = Instantiate(ingredientPrefab, parent);
            ingredient.transform.localScale = Vector3.one;
            ingredient.transform.localPosition = Vector3.zero;
            ingredient.Animate(0.8f);
            
            _ingredients.Add(ingredient);
            return true;
        }
    }
}

