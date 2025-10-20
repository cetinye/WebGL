using System;
using System.Collections.Generic;
using Lean.Localization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Witmina_SweetMemory
{
    [CreateAssetMenu(menuName = MenuName)]
    public class QuestionData : ScriptableObject
    {
        private const string MenuName = "Data/Sweet_Memory/QuestionData";

        public string PriceText;
        public List<string> CakeNames;
        public List<string> CakeTypeTexts;
        public List<string> FlavorTexts;
        public List<string> ToppingTexts;
        public List<string> CandleColorTexts;
        public string CandleCountText;
        public string CandleNumberText;

        [Range(0, 1)][SerializeField] public float TrueChance = 0.4f;
        [Range(0, 1)][SerializeField] public float NegationChance = 0.4f;
        [Range(0, 1)][SerializeField] public float DoubleChance = 0.4f;

        public CakeQuestion GetRandomQuestion(Cake cake, out string questionString)
        {
            var question = new CakeQuestion();

            var categoryBin = new List<QuestionType>(); // Fill roll bin for random category
            if (cake.CakeType is not CakeType.None)
                categoryBin.Add(QuestionType.CakeType);
            if (cake.Flavor is not CakeFlavor.None)
                categoryBin.Add(QuestionType.CakeFlavor);
            if (cake.ToppingType is not ToppingType.None)
                categoryBin.Add(QuestionType.CakeTopping);
            if (cake.CandleCount is not 0)
                categoryBin.Add(QuestionType.CandleCount);
            if (cake.CandleType is CandleType.Colored)
                categoryBin.Add(QuestionType.CandleColor);
            if (cake.CandleType is CandleType.Numbered)
                categoryBin.Add(QuestionType.CandleNumber);
            if (cake.Price is not 0)
                categoryBin.Add(QuestionType.CakePrice);

            // Roll for category and determine the ranges according to level
            var level = GameManager.Instance.PlayerLevel;
            var maxCakeType = GameManager.LevelSO.typesOfCakesMaxRange switch
            {
                2 => CakeType.Single,
                3 => CakeType.Heart,
                4 => CakeType.Double,
                5 => CakeType.Triple,
                _ => CakeType.Cupcake,
            };
            var trueRoll = Random.Range(0f, 1f) < TrueChance;
            var doubleRoll = level >= 12 && Random.Range(0f, 1f) < DoubleChance;
            var negativeRoll = level >= 5 && !doubleRoll && Random.Range(0f, 1f) < NegationChance;
            var categoryCount = doubleRoll ? 2 : 1;

            for (int i = 0; i < categoryCount; i++)
            {
                var isTrue = trueRoll ||
                             (i == categoryCount - 1) && Random.Range(0f, 1f) < TrueChance;

                // Determine 1 or 2 category depending on the roll.
                var startIndex = i == 0 ? 0 : 1;
                var categoryRoll = categoryBin[Random.Range(startIndex, categoryBin.Count)];
                categoryBin.Remove(categoryRoll);

                // Check roll for category
                switch (categoryRoll)
                {
                    case QuestionType.CakeType: // Cake Type
                        question.CakeType = isTrue ? cake.CakeType
                            : (CakeType)Random.Range(0, (int)maxCakeType + 1);
                        question.CakeTypeNeg = negativeRoll;
                        break;
                    case QuestionType.CakeFlavor: // Flavor
                        question.Flavor = isTrue ? cake.Flavor
                            : (CakeFlavor)Random.Range(0, (int)CakeFlavor.Blueberry + 1);
                        question.FlavorNeg = negativeRoll;
                        break;
                    case QuestionType.CakeTopping: // Topping
                        question.Topping = isTrue ? cake.ToppingType
                            : (ToppingType)Random.Range(0, (int)ToppingType.Lemon + 1);
                        question.ToppingNeg = negativeRoll;
                        break;
                    case QuestionType.CandleColor: // Candle Color
                        question.CandleType = cake.CandleType;
                        question.CandleColor = isTrue ? cake.CandleColor
                            : (CandleColor)Random.Range(0, (int)CandleColor.Blue + 1);
                        break;
                    case QuestionType.CandleCount: // Candle Count
                        question.CandleType = cake.CandleType;
                        question.CandleCount = isTrue ? cake.CandleCount
                            : Random.Range(1, 7);
                        question.CandleCountNeg = negativeRoll;
                        break;
                    case QuestionType.CandleNumber: // Candle Number
                        question.CandleType = cake.CandleType;
                        question.CandleNumber = isTrue ? cake.CandleNumber
                            : Random.Range(10, 78);
                        question.CandleNumberNeg = negativeRoll;
                        break;
                    case QuestionType.CakePrice: // Price
                        question.Price = isTrue ? cake.Price : Random.Range(5, 50);
                        question.PriceNeg = negativeRoll;
                        break;
                }
            }

            questionString = GenerateQuestionText(cake, question);
            return question;
        }

        private string GenerateQuestionText(Cake cake, CakeQuestion question)
        {
            string result = "";

            var cakeName = LeanLocalization.GetTranslationText($"CakeNames{(int)cake.CakeType}");
            if (question.CakeType is not CakeType.None)
            {
                var text = LeanLocalization.GetTranslationText($"CakeTypeTexts{(int)question.CakeType}");
                var sentenceKey = question.CakeTypeNeg ? "CakeTypeNegative" : "CakeTypePositive";
                result += LeanLocalization.GetTranslationText(sentenceKey)
                    .Replace("{cakeName}", cakeName)
                    .Replace("{text}", text);
            }
            if (question.Flavor is not CakeFlavor.None)
            {
                var text = LeanLocalization.GetTranslationText($"FlavorTexts{(int)question.Flavor}");
                var sentenceKey = question.FlavorNeg ? "FlavorNegative" : "FlavorPositive";
                result += (result == "" ? "" : $" {LeanLocalization.GetTranslationText("And")}")
                    + LeanLocalization.GetTranslationText(sentenceKey)
                    .Replace("{cakeName}", cakeName)
                    .Replace("{text}", text);
            }
            if (question.Topping is not ToppingType.None)
            {
                var text = LeanLocalization.GetTranslationText($"ToppingTexts{(int)question.Topping}");
                var sentenceKey = question.ToppingNeg ? "ToppingNegative" : "ToppingPositive";
                result += (result == "" ? "" : $" {LeanLocalization.GetTranslationText("And")}")
                    + LeanLocalization.GetTranslationText(sentenceKey)
                    .Replace("{cakeName}", cakeName)
                    .Replace("{text}", text);
            }
            if (question.Price is not 0)
            {
                PriceText = LeanLocalization.GetTranslationText("PriceText");
                var sentenceKey = question.PriceNeg ? "PriceNegative" : "PricePositive";
                result += (result == "" ? "" : $" {LeanLocalization.GetTranslationText("And")}")
                    + LeanLocalization.GetTranslationText(sentenceKey)
                    .Replace("{cakeName}", cakeName)
                    .Replace("{price}", $"{LeanLocalization.GetTranslationText("$")}" + question.Price);
            }

            if (question.CandleType is CandleType.Colored)
            {
                if (question.CandleCount is not 0)
                {
                    CandleCountText = LeanLocalization.GetTranslationText("CandleCountText");
                    var sentenceKey = question.CandleCountNeg ? "CandleCountNegative" : "CandleCountPositive";
                    result += (result == "" ? "" : $" {LeanLocalization.GetTranslationText("And")}")
                        + LeanLocalization.GetTranslationText(sentenceKey)
                        .Replace("{cakeName}", cakeName)
                        .Replace("{count}", question.CandleCount.ToString());
                }
                if (question.CandleColor is not CandleColor.None)
                {
                    var text = LeanLocalization.GetTranslationText($"CandleColorTexts{(int)question.CandleColor}");
                    var sentenceKey = question.CandleColorNeg ? "CandleColorNegative" : "CandleColorPositive";
                    result += (result == "" ? "" : $" {LeanLocalization.GetTranslationText("And")}")
                        + LeanLocalization.GetTranslationText(sentenceKey)
                        .Replace("{cakeName}", cakeName)
                        .Replace("{color}", text);
                }
            }
            else if (question.CandleType is CandleType.Numbered)
            {
                if (question.CandleNumber is not 0)
                {
                    var sentenceKey = question.CandleNumberNeg ? "CandleNumberNegative" : "CandleNumberPositive";
                    result += (result == "" ? "" : $" {LeanLocalization.GetTranslationText("And")}")
                        + LeanLocalization.GetTranslationText(sentenceKey)
                        .Replace("{cakeName}", cakeName)
                        .Replace("{number}", question.CandleNumber.ToString());
                }
            }

            return result;
        }
    }
}