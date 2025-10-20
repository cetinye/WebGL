using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    [CreateAssetMenu(menuName = MenuName)]
    public class W87_PrefabData : ScriptableObject
    {
        private const string MenuName = "Data/Math of Fuel/PrefabData";

        public List<CarBehaviour> CarPrefabs;
        public List<NumberTierData> NumberTiers;

        public CarBehaviour GetRandomCar()
        {
            return CarPrefabs[Random.Range(0, CarPrefabs.Count)];
        }

        public QuestionTierData GetQuestionTierData()
        {
            QuestionTierData questionTierData = new QuestionTierData();
            questionTierData.ServiceType = (ServiceType)LevelBehaviour.LevelSO.typesOfServicesRange;
            questionTierData.QuestionType = (QuestionType)LevelBehaviour.LevelSO.operationMaxRange;
            questionTierData.Difficulty = (QuestionDifficulty)LevelBehaviour.LevelSO.difficulty;
            questionTierData.WaitTime = LevelBehaviour.LevelSO.customerPatience;
            return questionTierData;
        }

        public List<int> GetNumbersInTier(QuestionDifficulty difficulty)
        {
            var numbers = new List<int>();
            for (int i = 0; i < NumberTiers.Count; i++)
            {
                if (NumberTiers[i].Difficulty <= difficulty)
                    numbers.AddRange(NumberTiers[i].Numbers);
            }

            return numbers;
        }
    }
}

