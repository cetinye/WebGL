using UnityEngine;

namespace Name_It_Or_Run_It
{
    [CreateAssetMenu(fileName = "NIORI (1)", menuName = "Data/NIORI/Level")]
    public class LevelSO : ScriptableObject
    {
        public int levelId;
        public float timeToShowItem;
        public bool isEasy;
        public bool isMedium;
        public bool isHard;
        public bool isMixed;
        public float mixRate;
        public bool questionType1;
        public bool questionType2;
        public bool questionType3;
        public bool questionType4;
        public float answerTime;
        public int levelUpCriteria;
        public int levelDownCriteria;
        public int maxInLevel;
        public int minScore;
        public int penaltyPoints;
    }
}