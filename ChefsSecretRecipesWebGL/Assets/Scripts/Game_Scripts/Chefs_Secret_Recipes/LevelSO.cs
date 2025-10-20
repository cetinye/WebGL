using UnityEngine;

namespace Chefs_Secret_Recipes
{
    public class LevelSO : ScriptableObject
    {
        public int levelId;

        // eqn operators
        public bool isAdditionOn;
        public bool isSubtractionOn;
        public bool isMultiplicationOn;

        // eqn types
        public int defaultProbability;
        public int multiplicativeProbability;
        public int productFindingProbability;
        public int multiplicativeProductFindingProbability;

        // limits
        public float timeLimitPerOperation;
        public int levelUpCriteria;
        public int levelDownCriteria;

        // scores
        public int pointsPerCorrectAnswer;
        public int penaltyPoints;
    }
}