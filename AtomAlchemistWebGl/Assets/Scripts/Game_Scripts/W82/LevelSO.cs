using UnityEngine;

namespace Witmina_AtomAlchemist
{
	public class LevelSO : ScriptableObject
	{
		public int levelId;
		public float innerRingSpeed;
		public float ringSpeedMultiplier;
		public int hitsToTransmute;
		public int minRandom;
		public int maxRandom;
		public int levelUpCriteria;
		public int levelDownCriteria;
		public int pointsPerCorrect;
		public int penaltyPoints;
	}
}
