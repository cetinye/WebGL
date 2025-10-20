using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public interface IFood
    {
        public bool IsComplete();
        public bool HasType(IngredientType ingredientType);
        public bool AddIngredient(IngredientType ingredientType);
        
        //public string GetDescription();
    }

}
