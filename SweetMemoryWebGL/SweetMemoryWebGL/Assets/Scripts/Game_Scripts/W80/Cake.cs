using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Witmina_SweetMemory
{
    public class Cake : MonoBehaviour
    {
        #region SerializedFields
        [SerializeField] private int _level;
        [SerializeField] private CakeType _cakeType;
        [SerializeField] private Image _cream;
        [SerializeField] private Image _topping;
        [SerializeField] private bool _scaleWithAnimation;
        [SerializeField] private GameObject[] _candlesParent;
        [SerializeField] private GameObject[] _numberedCandlesParent;
        [SerializeField] private List<Image> _candles;
        [SerializeField] private List<Image> _numberCandles;
        #endregion
        #region Fields
        public int Price;
        public int Level => _level;
        public CakeType CakeType => _cakeType;
        public Transform CakeTransform => transform.GetChild(1);
        public bool ScaleWithAnimation => _scaleWithAnimation;

        private CandleType _candleType = CandleType.Colored;
        public CandleType CandleType
        {
            get => _candleType;
            set
            {
                _candleType = value;
                foreach (var cp in _candlesParent)
                {
                    cp.SetActive(_candleType is CandleType.Colored);
                }
                foreach (var ncp in _numberedCandlesParent)
                {
                    ncp.SetActive(_candleType is CandleType.Numbered);
                }
            }
        }

        private CakeFlavor _flavor;
        public CakeFlavor Flavor
        {
            get => _flavor;
            set
            {
                _flavor = value;
                if (_flavor is CakeFlavor.None)
                    return;

                _cream.color = GameManager.CakeData.Colors[(int)_flavor];
            }
        }

        private ToppingType _toppingType;
        public ToppingType ToppingType
        {
            get => _toppingType;
            set
            {
                if (!_topping || value is ToppingType.None)
                {
                    _toppingType = ToppingType.None;
                    return;
                }

                _toppingType = value;
                _topping.sprite = GameManager.CakeData.Toppings[(int)_toppingType];
            }
        }

        private CandleColor _candleColor;
        public CandleColor CandleColor
        {
            get => _candleColor;
            set
            {
                _candleColor = value;
                if (_candleColor is CandleColor.None)
                    return;

                var candleSprite = GameManager.CakeData.Candles[(int)_candleColor];
                foreach (var candle in _candles)
                {
                    candle.sprite = candleSprite;
                }
            }
        }

        private int _candleCount;
        public int CandleCount
        {
            get => _candleCount;
            set
            {
                _candleCount = value;
                for (int i = 0; i < _candles.Count; i++)
                {
                    _candles[i].gameObject.SetActive(i < _candleCount);
                }
            }
        }

        public int MaxCandles => _candles.Count;

        private int _candleNumber;
        public int CandleNumber
        {
            get => _candleNumber;
            set
            {
                _candleNumber = value;
                var sprites = GameManager.CakeData.Numbered_Candles;
                _numberCandles[0].sprite = sprites[_candleNumber / 10];
                _numberCandles[1].sprite = sprites[_candleNumber % 10];
            }
        }
        #endregion

        #region Methods

        public bool EvaluateQuery(CakeQuestion question)
        {
            var result = true;

            result &= question.Price is 0
                      || (question.Price == Price) ^ question.PriceNeg;
            result &= question.CakeType is CakeType.None
                      || (question.CakeType == CakeType) ^ question.CakeTypeNeg;
            result &= question.Flavor is CakeFlavor.None
                      || (question.Flavor == Flavor) ^ question.FlavorNeg;
            result &= question.Topping is ToppingType.None
                      || (question.Topping == ToppingType) ^ question.ToppingNeg;
            result &= question.CandleType is CandleType.None
                      || question.CandleType == CandleType;

            if (question.CandleType is CandleType.Colored)
            {
                result &= question.CandleCount is 0
                          || (question.CandleCount == CandleCount) ^ question.CandleCountNeg;
                result &= question.CandleColor is CandleColor.None
                          || (question.CandleColor == CandleColor) ^ question.CandleColorNeg;
            }
            else if (question.CandleType is CandleType.Numbered)
            {
                result &= question.CandleNumber is 0
                          || (question.CandleNumber == CandleNumber) ^ question.CandleNumberNeg;
            }

            return result;
        }

        #endregion
    }
}

