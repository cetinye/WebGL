using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Witmina_PaperCycle
{
    public class HouseBehaviour : MonoBehaviour
    {
        [SerializeField] private HouseAlignment _houseAlignment;
        [SerializeField] private Transform _paperTransform;
        [SerializeField] private Renderer _houseRenderer;
        [SerializeField] private float _throwHeight = 1f;
        [SerializeField] private float _throwDuration = 1f;

        public bool GotPaper { get; private set; }
        public HouseAlignment HouseAlignment => _houseAlignment;

        private HouseColorData _colorData;
        public HouseColorData ColorData
        {
            get => _colorData;
            set
            {
                _colorData = value;
                _houseRenderer.materials[0].SetColor("_BaseColor", _colorData.Color);
            }
        }

        private Sequence _paperSequence;

        private void Awake()
        {
            GotPaper = false;
        }

        private void OnDestroy()
        {
            _paperSequence.Kill();
        }

        public void ThrowPaper(Transform paper)
        {
            GotPaper = true;
            paper.SetParent(_paperTransform);
            _paperSequence.Kill();
            _paperSequence = DOTween.Sequence()
                .SetEase(Ease.Linear);
            _paperSequence.Append(
                paper.transform.DOLocalJump(Vector3.zero, _throwHeight, 1, _throwDuration)
                    .SetEase(Ease.Linear));
            _paperSequence.Insert(0f, 
                paper.transform.DORotate(new Vector3(0f, 360f, 0f), _throwDuration)
                    .SetEase(Ease.Linear)
                    .SetRelative(true));
            
            
            _paperSequence.OnComplete(() => Destroy(paper.gameObject));
            _paperSequence.Play();
        }
    }
}

