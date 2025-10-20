using UnityEngine;

namespace Chroma_City_Neon_Racing
{

    public class LevelSO : ScriptableObject
    {
        public int levelId;
        public float minSpeedRange;
        public float maxSpeedRange;
        public float ballSpeedChangeAmount;
        public float speedPenatlyAmount;
        public int pathLength;
        public int shieldPowerup;
        public int speedPowerup;
        public int timePowerup;
        public int durationOfPowerups;
        public int timeLimit;
        public float maxScore;
        public float penaltyPoints;
    }
}