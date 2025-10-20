using UnityEngine;

namespace Guess_The_Move
{
    public class LevelSO : ScriptableObject
    {
        public int levelId;
        public int personTypeRange;
        public int movementTypeRange;
        public int outfitTypeRange;
        public int hairColorRange;
        public int topOutfitColorRange;
        public int bottomOutfitColorRange;
        public int shoesColorRange;
        public float questionDisplayTime;
        public int levelUpCriteria;
        public int levelDownCriteria;
        public int pointsPerCorrect;
        public int penaltyPoints;
    }   
}