using UnityEngine;

namespace JungleRace
{
	public class W27_LevelSO : ScriptableObject
	{
		public int levelId;
		public int mapSize;
		public float turtleSpeed;
		public float rabbitSpeed;
		public bool rotation180;
		public bool rotation90;
		public int numOfRotations;
		public float cooldown;
		public int totalRounds;
		public int levelUpCriteria;
		public int levelDownCriteria;
		public int maxInLevel;
		public int penaltyPoints;
	}
}
