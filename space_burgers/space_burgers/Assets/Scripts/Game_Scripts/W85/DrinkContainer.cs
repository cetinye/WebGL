using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public class DrinkContainer : Container
    {
        [SerializeField] private SpriteRenderer _glassSprite;
        [SerializeField] private float _pourDuration = 1f;

        private bool _pouring;
        private void Awake()
        {
            _glassSprite.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public override void AddIngredient()
        {
            if (_pouring)
                return;
            
            StartCoroutine(PourRoutine());
        }
        
        private IEnumerator PourRoutine()
        {
            _pouring = true;
            var glass = _glassSprite.gameObject;
            if(glass.activeSelf)
                yield break;
                        
            glass.SetActive(true);
            yield return new WaitForSeconds(_pourDuration);
            glass.SetActive(false);
            base.AddIngredient();
            _pouring = false;
        }
    }
}

