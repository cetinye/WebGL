using System;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    [Serializable]
    public sealed class AudioFxData
    {
        public AudioFxType Type;
        public AudioClip Clip;
    }

    [Serializable]
    public sealed class QuestionTierData
    {
        public ServiceType ServiceType;
        public QuestionType QuestionType;
        public QuestionDifficulty Difficulty;
        public float WaitTime;
    }

    [Serializable]
    public sealed class NumberTierData
    {
        public QuestionDifficulty Difficulty;
        public int[] Numbers;
    }

    public sealed class QuestionData
    {
        public ServiceType ServiceType;
        public QuestionType QuestionType;
        public QuestionOperandType OperandType;
        public QuestionDifficulty Difficulty;
        public Fraction Result;
        public Fraction Op1;
        public Fraction Op2;
    }
}

