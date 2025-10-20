using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    [Serializable]
    public class Order
    {
        public Sprite Sprite;
        public FoodType FoodType;
        public List<IngredientType> Ingredients;
        public int Tier = 1;
        
        public Order(FoodType foodType, List<IngredientType> ingredients)
        {
            FoodType = foodType;
            Ingredients = ingredients;
        }
    }
}