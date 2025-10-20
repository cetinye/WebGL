using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Witmina_SpaceBurgers
{
    public class SauceIngredient : Ingredient
    {
        [SerializeField] private Transform _nuzzleTransform;
        [SerializeField] private Transform _bottleTransform;
        [SerializeField] private Transform _sauceTransform;

        private Sequence _sauceSequence;

        private void OnDestroy()
        {
            _sauceSequence.Kill();
        }

        public override void Animate(float duration)
        {
            _sauceSequence.Kill();
            _sauceSequence = DOTween.Sequence().SetEase(Ease.Linear);
            _sauceSequence.InsertCallback(0f, () =>
            {
                _bottleTransform.gameObject.SetActive(true);
                _sauceTransform.gameObject.SetActive(true);
                _sauceTransform.position = _nuzzleTransform.position;
                _sauceTransform.localScale = 0.2f * Vector3.one;
            });
            _sauceSequence.Append(_bottleTransform.DOLocalMoveY(-0.25f, duration / 2f)
                .SetEase(Ease.InQuad));
            _sauceSequence.Insert(duration / 2f,_bottleTransform.DOLocalMoveY(0f, duration / 2f)
                .SetEase(Ease.OutQuad));
            _sauceSequence.Insert(duration / 2f,_sauceTransform.DOLocalMove(Vector3.zero, duration / 2f)
                .SetEase(Ease.OutCubic));
            _sauceSequence.Insert(duration / 2f,_sauceTransform.DOScale(Vector3.one, duration / 2f)
                .SetEase(Ease.InCubic));
            _sauceSequence.InsertCallback(duration, () => _bottleTransform.gameObject.SetActive(false));
            _sauceSequence.Play();
        }
    }
}