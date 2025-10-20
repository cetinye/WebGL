using UnityEngine;

namespace ElectroBirds
{
	public class W57_LevelSO : ScriptableObject
	{
		public int levelId;
		public int numOfBirds;
		public float flowDuration;
		public float flowInterval;
		public int levelUpCriteria;
		public int levelDownCriteria;
		public int pointsPerCorrect;
		public int maxInGame;
		public int minScore;
		public int penaltyPoints;
	}
}
