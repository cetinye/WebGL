using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    [CreateAssetMenu(menuName = MenuName)]
    public class PrefabData : ScriptableObject
    {
        private const string MenuName = "Data/Space_Burgers/PrefabData";

        public List<Food> FoodPrefabs;
        public List<Ingredient> IngredientPrefabs;
        public List<Sprite> CustomerSprites;

        public Ingredient GetIngredient(IngredientType ingredientType)
        {
            return IngredientPrefabs.FirstOrDefault(i => i.Type == ingredientType);
        }

        public Sprite GetIngredientSprite(IngredientType ingredientType)
        {
            return GetIngredient(ingredientType).Sprite;
        }

        public List<IngredientType> GetIngredientList(FoodType type)
        {
            var food = FoodPrefabs.FirstOrDefault(f => f.Type == type);
            if (!food)
                return null;

            return new List<IngredientType>(food.IngredientTypes);
        }

        public Sprite GetRandomCustomerSprite(Sprite currentSprite = null)
        {
            if(!currentSprite)
                return CustomerSprites[Random.Range(0, CustomerSprites.Count)];

            var list = CustomerSprites.Where(cs => cs != currentSprite).ToList();
            return list[Random.Range(0, list.Count)];
        }
    }
}

