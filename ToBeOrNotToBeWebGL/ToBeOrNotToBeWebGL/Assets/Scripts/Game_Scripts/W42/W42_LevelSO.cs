using UnityEngine;

namespace To_Be_Or_Not_To_Be
{
	public class W42_LevelSO : ScriptableObject
	{
		public int levelId;
		public bool colorIncluded;
		public bool modelNameIncluded;
		public bool positiveStatementIncluded;
		public bool negativeStatementIncluded;
		public float questionLoadTime;
		public float remainingTimeToAnswer;
		public float totalTime;
		public float requiredMinQuestionsToSolve;
		public int levelUpCriteria;
		public int levelDownCriteria;
		public int maxInLevel;
		public int gameScore;
		public int minRequiredScore;
		public int penaltyPoints;
	}
}
