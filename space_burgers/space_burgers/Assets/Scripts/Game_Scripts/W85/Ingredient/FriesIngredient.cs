using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace Witmina_SpaceBurgers
{
    public class FriesIngredient : Ingredient
    {
        private Transform _pivot;
        private List<Transform> _pieces = new();

        private Sequence _pourSequence;
        private void Awake()
        {
            _pivot = transform.GetChild(0);
            for (int i = 0; i < _pivot.childCount; i++)
            {
                _pieces.Add(_pivot.GetChild(i));
            }
        }

        private void OnDestroy()
        {
            transform.DOKill();
            _pourSequence.Kill();
        }

        public override void Animate(float duration)
        {
            base.Animate(duration);
            
            _pourSequence.Kill();
            _pourSequence = DOTween.Sequence().SetEase(Ease.Linear);

            var timeSpan = 0.02f;
            for (int i = 0; i < _pieces.Count; i++)
            {
                var piece = _pieces[i];
                var yPos = piece.transform.localPosition.y;
                piece.transform.localPosition += 1f * Vector3.up;
                piece.gameObject.SetActive(false);

                _pourSequence.Insert(i * timeSpan,
                    piece.DOLocalMoveY(yPos, duration - _pieces.Count * timeSpan)
                        .OnStart(() => piece.gameObject.SetActive(true)));
                _pourSequence.Insert((i + 1) * timeSpan,
                    piece.DOPunchScale(-0.5f * Vector3.up, 0.05f));
            }
            //_pourSequence.Append(_pivot.DOPunchScale(-0.3f * Vector3.up, 0.1f));
            _pourSequence.Play();
        }
    }
}