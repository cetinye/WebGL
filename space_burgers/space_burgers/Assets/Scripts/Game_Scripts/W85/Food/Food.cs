using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public abstract class Food : MonoBehaviour ,IFood
    {
        [SerializeField] private FoodType _type;
        [SerializeField] protected List<IngredientType> _ingredientTypes;

        protected readonly List<Ingredient> _ingredients = new();
        public FoodType Type => _type;
        public List<IngredientType> IngredientTypes => _ingredientTypes;

        public List<Ingredient> Ingredients => _ingredients;

        protected bool _complete;
        public abstract bool IsComplete();

        public bool HasType(IngredientType ingredientType) => _ingredientTypes.Contains(ingredientType);
        public abstract bool IngredientCanBeAdded(IngredientType ingredientType);
        public abstract bool AddIngredient(IngredientType ingredientType);

        private Vector3 _initPos;
        private void Awake()
        {
            _initPos = transform.position;
        }

        public virtual void ResetFood()
        {
            for (int i = 0; i < _ingredients.Count; i++)
            {
                Destroy(_ingredients[i].gameObject);
            }
            _ingredients.Clear();
            _complete = false;
        }

        public virtual void PlayFx() {}
    }
}