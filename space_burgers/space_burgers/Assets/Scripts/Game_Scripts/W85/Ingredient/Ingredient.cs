using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public abstract class Ingredient : MonoBehaviour
    {
        [SerializeField] protected IngredientType _type;
        [SerializeField] protected Sprite _sprite;
        public IngredientType Type => _type;
        public Sprite Sprite => _sprite;
        public virtual void Animate(float duration) {}
    }
}

