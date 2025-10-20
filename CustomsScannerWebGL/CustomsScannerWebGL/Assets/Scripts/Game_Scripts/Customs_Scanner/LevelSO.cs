using UnityEngine;

namespace Customs_Scanner
{
    [CreateAssetMenu(fileName = "CS_Level", menuName = "Data/Customs_Scanner/Level")]
    public class LevelSO : ScriptableObject
    {
        public int levelId;
        public float vehicleMoveTime;
        [Range(0, 100)] public int productFillPercent;
        public bool isSecretItemEnabled;
        [Range(0, 8)] public int forbiddenItemAmount;
        public int forbiddenBoxAmount;
        public int vehicleAmount;
        public int totalNumOfForbiddenProducts;
        public float timeToShowItemList;
        public int levelDownCriteria;
        public int maxInGame;
        public int minScore;
        public int penaltyPoints;
    }
}