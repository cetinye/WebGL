using UnityEngine;

namespace Color_Clique
{
    public class LevelSO : ScriptableObject
    {
        public int levelId;
        public int numberOfColors;
        public int shapeCount;
        public int wheelSegments;
        public float spinSpeedMultiplier;
        public float answerTime;
        public bool isWheelBarReversalEnabled;
        public int minChangeFrequency;
        public int maxChangeFrequency;
        public int levelUpCriteria;
        public int levelDownCriteria;
        public int comboScore;
        public int pointsPerQuestion;
        public int penaltyPoints;
    }
}