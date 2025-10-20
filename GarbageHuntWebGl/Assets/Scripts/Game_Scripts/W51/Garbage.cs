using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    public class Garbage : CatchableItem
    {
        public event Action<Garbage, bool> GarbageFinished;
        
        [SerializeField] private bool _staticGarbage;
        public bool StaticGarbage => _staticGarbage;
        public bool Sinking { get; private set; }

        private Tweener _sinkTween;
        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = transform.position;
        }

        public void ResetGarbage()
        {
            transform.position = _startPos;
            Sinking = false;
        }

        private void OnDestroy()
        {
            _sinkTween.Kill();
        }

        public void Release(float minY, float duration)
        {
            if (Sinking)
                return;
            
            Sinking = true;
            _sinkTween.Kill();
            _sinkTween = transform.DOMoveY(minY, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => GarbageFinished?.Invoke(this, false));
        }

        public override void Catch()
        {
            if (!Sinking)
                return;
            
            Sinking = false;
            _sinkTween.Kill();
            GarbageFinished?.Invoke(this, true);
        }
    }
}

