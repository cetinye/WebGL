using UnityEngine;

namespace Chrono_Capsule_Chronicles
{
    public class LevelSO : ScriptableObject
    {
        public int levelId;
        public int totalNumOfLetters;
        public int numOfLettersToChange;
        public int numOfOptions;
        public float timePerQuestion;
        public int levelUpCriteria;
        public int levelDownCriteria;
        public int pointsPerQuestion;
        public int penaltyPoints;
    }
}