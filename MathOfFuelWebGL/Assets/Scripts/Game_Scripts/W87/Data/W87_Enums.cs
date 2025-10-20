using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public enum AudioFxType
    {
        Success,
        Fail,
        Tick,
        ServiceRequest,
        Submit,
    }

    public enum ServiceType
    {
        Gasoline,
        Diesel,
        Electric,
        Wheels,
        Water,
    }

    public enum QuestionType
    {
        Simple,
        Addition,
        Subtraction,
        Multiplication,
    }

    public enum QuestionOperandType
    {
        Fraction,
        Percentage,
        Float
    }

    public enum QuestionDifficulty
    {
        Easy,
        Medium,
        Hard,
    }
}

