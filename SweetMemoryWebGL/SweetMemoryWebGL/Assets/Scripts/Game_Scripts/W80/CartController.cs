using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Sequence = DG.Tweening.Sequence;

namespace Witmina_SweetMemory
{
    public class CartController : MonoBehaviour
    {
        public event Action MoveInCompleted; 
        public event Action CoverCompleted;

        [SerializeField] private Transform _wheelBig;
        [SerializeField] private Transform _wheelSmall;
        [SerializeField] private Transform _cakeParent;
        [SerializeField] private Transform _cover;
        [SerializeField] private Transform _priceTag;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private float _xOffset;
        [SerializeField] private float _coverYOffset = 400f;
        [SerializeField] private float _coverRotation = -50f;
        [SerializeField] private float _moveDuration = 2f;
        [SerializeField] private float _coverDuration = 0.5f;
        [SerializeField] private float _wheelBigRotation = 1440f;
        [SerializeField] private float _wheelSmallRotation = 2880f;
        
        private Cake _cake;
        private Sequence _moveSequence;
        private Sequence _coverSequence;

        public Cake Cake => _cake;

        #if UNITY_EDITOR
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_cake.ScaleWithAnimation)
                    _cake.CakeTransform.localScale = Vector3.one;
                _cake.gameObject.SetActive(true);
                _cover.gameObject.SetActive(false);
                _priceText.gameObject.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                if (_cake.ScaleWithAnimation)
                    _cake.CakeTransform.localScale = new Vector3(1f, 0.25f, 1f);
                _cake.gameObject.SetActive(false);
                _cover.gameObject.SetActive(true);
                _priceText.gameObject.SetActive(false);
            }
        }
        #endif
        
        public void Initialize()
        {
            _moveSequence.Kill();
            _coverSequence.Kill();
            
            transform.localPosition = _xOffset * Vector3.right;
            _cover.localPosition = Vector3.zero;
            _cover.localRotation = Quaternion.identity;
            _cover.gameObject.SetActive(true);
            _priceText.transform.parent.gameObject.SetActive(true);
            _priceText.text = "";
        }
        
        public void MoveIn()
        {
            _moveSequence.Kill();
            
            transform.localPosition = _xOffset * Vector3.right;

            _priceText.gameObject.SetActive(false);

            _moveSequence = DOTween.Sequence().SetEase(Ease.Linear);
            _moveSequence.Append(transform.DOLocalMoveX(0f, _moveDuration)
                .OnStart(() => AudioController.Play(AudioType.Cart))
                .SetEase(Ease.Linear));

            _moveSequence.Insert(0f, 
                _wheelBig.DOLocalRotate(_wheelBigRotation * Vector3.forward,
                    _moveDuration, RotateMode.FastBeyond360).
                    SetEase(Ease.Linear)
                    .SetRelative(true));
            
            _moveSequence.Insert(0f, 
                _wheelSmall.DOLocalRotate(_wheelSmallRotation * Vector3.forward,
                    _moveDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetRelative(true));

            _moveSequence.OnComplete(() => MoveInCompleted?.Invoke());
            _moveSequence.Play();
        }

        public void CoverSequence(float showDuration = 2f)
        {
            _cake.gameObject.SetActive(true);
            
            _coverSequence = DOTween.Sequence().SetEase(Ease.Linear);
            _coverSequence.Append(_cover.DOLocalMoveY(_coverYOffset, _coverDuration)
                    .OnStart(() =>
                    {
                        _cake.gameObject.SetActive(true);
                        _priceText.gameObject.SetActive(true);
                        AudioController.Play(AudioType.Cover);
                    }));
            
            _coverSequence.Insert(0f, _cover.DOLocalRotateQuaternion(
                Quaternion.Euler(0f,0f,_coverRotation), _coverDuration));
            
            if (_cake.ScaleWithAnimation)
            {
                _cake.CakeTransform.localScale = new Vector3(1f, 0.25f, 1f);
                _coverSequence.Insert(0f,
                    _cake.CakeTransform.DOScaleY(1f, 0.25f * _coverDuration));
            }

            _coverSequence.Insert(_coverDuration + showDuration,
                _cover.DOLocalMove(Vector3.zero, _coverDuration)
                    .OnStart(() => AudioController.Play(AudioType.Cover))
                    .OnComplete(() =>
                    {
                        _cake.gameObject.SetActive(false);
                        _priceText.gameObject.SetActive(false);
                    }));

            _coverSequence.Insert(_coverDuration + showDuration,
                _cover.DOLocalRotateQuaternion(Quaternion.identity, _coverDuration));
            
            if(_cake.ScaleWithAnimation)
                _coverSequence.Insert(1.65f * _coverDuration + showDuration,
                    _cake.CakeTransform.DOScaleY(0.25f, 0.35f * _coverDuration));
            
            _coverSequence.OnComplete(() => CoverCompleted?.Invoke());
            _coverSequence.Play();
        }

        public void SetCake(Cake cake)
        {
            if(_cake)
                Destroy(_cake.gameObject);

            _cake = cake;
            _priceTag.gameObject.SetActive(_cake.Price > 0);
            _priceText.text = $"${_cake.Price}";
            _cake.transform.SetParent(_cakeParent);
            _cake.transform.localPosition = Vector3.zero;
            _cake.transform.localScale = Vector3.one;
            _cake.gameObject.SetActive(false);
        }

        public void OnEnd()
        {
            _moveSequence.Kill();
            _coverSequence.Kill();
        }
    }

}
