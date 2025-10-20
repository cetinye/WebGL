using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_SweetMemory
{
    public enum CakeType
    {
        None = -1,
        Donut,
        Cupcake,
        Single,
        Heart,
        Sliced,
        Double,
        Triple,
    }
    
    public enum CandleType
    {
        None = -1,
        Colored,
        Numbered,
    }

    public enum CakeFlavor
    {
        None = -1,
        Chocolate,
        WhiteChocolate,
        Strawberry,
        Banana,
        Cherry,
        Blueberry,
    }

    public enum ToppingType
    {
        None = -1,
        Blueberry,
        Cherry,
        Strawberry,
        Kiwi,
        Lemon,
    }

    public enum CandleColor
    {
        None = -1,
        Red,
        Yellow,
        Blue,
    }

    public enum QuestionType
    {
        None = -1,
        CakeType,
        CakeFlavor,
        CakeTopping,
        CandleColor,
        CandleCount,
        CandleNumber,
        CakePrice,
    }

    public enum AudioType
    {
        Cart,
        Cover,
        Heels,
        TrueAnswer,
        FalseAnswer,
        Car,
        Door,
    }
}
