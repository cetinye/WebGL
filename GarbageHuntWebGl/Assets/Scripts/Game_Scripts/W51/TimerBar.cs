using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    public class TimerBar : MonoBehaviour
    {
        [SerializeField] private Transform _anchorTransform;
        [SerializeField] private float _maxOffset = 1f;
        public void SetFill(float value)
        {
            _anchorTransform.localPosition = value * _maxOffset * Vector3.right;
        }

        public void ResetFill()
        {
            _anchorTransform.localPosition = _maxOffset * Vector3.right;
        }
    }
}

