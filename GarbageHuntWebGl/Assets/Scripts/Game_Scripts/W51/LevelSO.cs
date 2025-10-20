using UnityEngine;

namespace Witmina_GarbageHunt
{
	public class LevelSO : ScriptableObject
	{
		public int levelId;
		public int oceanLayers;
		public int numOfFish;
		public float topSpeedFish;
		public float garbageSpawnInterval;
		public int garbageOnSeabed;
		public int garbageOnSeabedRenewalInterval;
		public int extraTime;
		public int maxGarbageCount;
		public int levelUpCriteria;
		public int levelDownCriteria;
		public int pointsPerCorrect;
		public int penaltyPoints;
	}
}
