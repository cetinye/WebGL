using UnityEngine;

namespace W92_Memories_Of_The_Egyptian_Gods
{
	public class LevelSO : ScriptableObject
	{
		public int levelId;
		public int numOfCards;
		public int moveLimit;
		public int numOfCorrectReviewsForLevelUp;
		public int levelDownCriteria;
		public int totalRounds;
		public int maxInLevel;
		public int penaltyPoints;
	}
}
