using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    public class FishHook : MonoBehaviour
    {
        public event Action Launched;
        public event Action<List<CatchableItem>> ItemCaught;
        public event Action<List<CatchableItem>> ItemCollected;

        [SerializeField] private Transform _catchPoint;
        [SerializeField] private float _moveDuration = 3f;
        [SerializeField] private float _minY = -7f;
        [SerializeField] private float _catchRadius = 1f;

        private bool _moving;
        public bool Moving
        {
            get => _moving;
            private set
            {
                _moving = value;
                _hookRenderer.enabled = _moving;
            }
        }
        public bool Catching { get; private set; }

        private Tweener _hookTween;
        [SerializeField] private List<CatchableItem> _caughtItems = new();
        private Renderer _hookRenderer;
        private LayerMask _defaultMask;
        private float _speedMultiplier = 1f;

        private void Awake()
        {
            _hookRenderer = GetComponent<Renderer>();
            _defaultMask = LayerMask.NameToLayer("Default");
        }

        private void OnDestroy()
        {
            _hookTween.Kill();
        }

        public void ResetHook()
        {
            _hookTween.Kill();
            _hookRenderer.enabled = false;
            transform.localPosition = Vector3.zero;

            if (_caughtItems.Count < 1)
                return;

            foreach (var item in _caughtItems)
            {
                if (item is Garbage { StaticGarbage: false } || item is Fish)
                    Destroy(item.gameObject);
            }
            _caughtItems.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Catching)
                return;

            var rb = other.attachedRigidbody;
            if (!rb || !rb.TryGetComponent<CatchableItem>(out var catchItem))
                return;

            catchItem.Catch();
            catchItem.transform.SetParent(_catchPoint);
            catchItem.transform.localPosition = Vector3.zero;
            _caughtItems.Add(catchItem);

            //Search for other garbages in range to catch them
            if (catchItem is Garbage)
            {
                var hits = Physics.SphereCastAll(other.bounds.center - 0.2f * Vector3.down, _catchRadius, Vector3.down,
                    1f, Physics.AllLayers);
                foreach (var h in hits)
                {
                    var r = h.collider.attachedRigidbody;
                    if (r && r.TryGetComponent<Garbage>(out var g) && g != catchItem && !g.transform.IsChildOf(_catchPoint))
                    {
                        g.Catch();
                        g.transform.SetParent(_catchPoint);
                        g.transform.localPosition /= 3f;
                        _caughtItems.Add(g);
                    }

                }
            }

            ItemCaught?.Invoke(new List<CatchableItem>(_caughtItems));
            PullBack();
        }

        public void Throw(float speedMultipler = 1f)
        {
            if (_caughtItems.Count > 0)
                return;

            Moving = true;
            Catching = true;

            _speedMultiplier = speedMultipler;
            var duration = _moveDuration * Mathf.Abs(1f - (transform.localPosition.y / _minY))
                                / _speedMultiplier;

            _hookTween.Kill();
            _hookTween = transform.DOLocalMoveY(_minY, duration)
                .SetEase(Ease.Linear)
                .OnComplete(PullBack);

            Launched?.Invoke();
        }

        public void PullBack()
        {
            if (!Moving)
                return;

            Catching = true;
            var duration = _moveDuration * Mathf.Abs(transform.localPosition.y / _minY)
                / _speedMultiplier;

            //If caught a garbage from ground, extend the pullback duration
            if (_caughtItems.Count > 0 && _caughtItems.Any(i => i is Garbage g && g.StaticGarbage))
                duration *= 1.6f;

            _hookTween.Kill();
            _hookTween = transform.DOLocalMoveY(0f, duration)
                .SetEase(Ease.Linear)
                .OnComplete(OnPullBack);
        }

        private void OnPullBack()
        {
            Moving = false;
            Catching = false;
            ItemCollected?.Invoke(new List<CatchableItem>(_caughtItems));
            _caughtItems.Clear();
        }

        public void OnFinish()
        {
            _hookTween.Kill();
        }
    }
}

