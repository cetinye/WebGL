using UnityEngine;

namespace Witmina_SpaceBurgers
{
	public class LevelSO : ScriptableObject
	{
		public int levelId;
		public int ingredientsCount;
		public float customerPatienceSec;
		public int byProducts;
		public int maxOrderCount;
		public int numOfCorrectsForLevelUp;
		public int levelDownCriteria;
		public int maxInLevel;
		public int penaltyPoints;
	}
}
