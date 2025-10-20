using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    [RequireComponent(typeof(BoxCollider))]
    public class BurgerIngredient : Ingredient
    {
        public float Height => _boxCollider.size.y;

        private BoxCollider _boxCollider;
        private Transform _pivot;
        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _pivot = transform.GetChild(0);
        }

        private void OnDestroy()
        {
            transform.DOKill();
            _pivot.DOKill();
        }

        public override void Animate(float duration)
        {
            base.Animate(duration);
            _pivot.DOPunchScale(-0.3f * Vector3.up, duration);
        }
    }
}