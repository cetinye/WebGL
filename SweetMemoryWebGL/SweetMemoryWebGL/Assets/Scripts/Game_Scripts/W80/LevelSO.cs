using UnityEngine;

namespace Witmina_SweetMemory
{
	public class LevelSO : ScriptableObject
	{
		public int levelId;
		public int typesOfCakesMaxRange;
		public int cakeColorsMaxRange;
		public int fruitTypesOnCakeMaxRange;
		public int numOfCandlesOnTheCake;
		public int numberedCandlesOnCake;
		public int cakePrice;
		public float timePerQuestion;
		public float totalTimeForProductDisplay;
		public int totalGameTime;
		public int levelUpCriteria;
		public int levelDownCriteria;
		public int maxInLevel;
		public int minRequiredToPass;
		public int penaltyPoint;
	}
}
