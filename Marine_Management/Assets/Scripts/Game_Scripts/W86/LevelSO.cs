using UnityEngine;

namespace Witmina_MarineManagement
{
	public class LevelSO : ScriptableObject
	{
		public int levelId;
		public int totalNumOfMarinas;
		public int typesOfServicesMax;
		public int boatDensity;
		public int vipBoats;
		public int powerUps;
		public int specialEventBoats;
		public int levelUpCriteria;
		public int levelDownCriteria;
		public int maxInLevel;
		public int minScore;
		public int penaltyPoints;
	}
}
