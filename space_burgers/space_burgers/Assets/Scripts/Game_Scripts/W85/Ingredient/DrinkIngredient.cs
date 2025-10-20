using DG.Tweening;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    [RequireComponent(typeof(BoxCollider))]
    public class DrinkIngredient : Ingredient
    {
        private Transform _pivot;
        private GameObject _drinkSprite;
        private void Awake()
        {
            _pivot = transform.GetChild(0);
        }

        private void OnDestroy()
        {
            transform.DOKill();
            _pivot.DOKill();
        }

        public override void Animate(float duration)
        {
            //transform.DOLocalMoveY(0f, duration / 2f);
            _pivot.DOPunchScale(-0.1f * Vector3.up, duration);
        }
    }
}