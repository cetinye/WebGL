using UnityEngine;

namespace Public_Transport
{
	public class W44_LevelSO : ScriptableObject
	{
		public int levelId;
		public int numOfQuestions;
		public float answerTime;
		public int wrongAnswerLimit;
		public int pointsPerCorrectAnswer;
		public int maxInGame;
		public int minScoreRequired;
		public int penaltyPoints;
	}
}
