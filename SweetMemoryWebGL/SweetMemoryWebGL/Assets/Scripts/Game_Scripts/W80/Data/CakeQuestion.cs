using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Witmina_SweetMemory
{
    [Serializable]
    public class CakeQuestion
    {
        public CakeType CakeType = CakeType.None;
        public bool CakeTypeNeg;
        
        public CakeFlavor Flavor = CakeFlavor.None;
        public bool FlavorNeg;
        
        public ToppingType Topping = ToppingType.None;
        public bool ToppingNeg;
        
        public CandleType CandleType = CandleType.None;

        public CandleColor CandleColor = CandleColor.None;
        public bool CandleColorNeg;
        
        public int CandleCount = 0;
        public bool CandleCountNeg;
        
        public int CandleNumber = 0;
        public bool CandleNumberNeg;
        
        public int Price = 0;
        public bool PriceNeg = false;
    }
}