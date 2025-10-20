using UnityEngine;

namespace NanoInspector
{
	public class LevelSO : ScriptableObject
	{
		public int levelId;
		public bool ColorQuestion;
		public bool ShapeQuestion;
		public bool MovementQuestion;
		public bool isTrickQuestionColor;
		public bool isTrickQuestionShape;
		public bool isTrickQuestionMovement;
		public bool isColorRandomized;
		public bool isMovementRandomized;
		public bool isTrickQuestion;
		public float questionTime;
		public int levelUpCriteria;
		public int levelDownCriteria;
		public int maxInLevel;
		public int minScore;
		public int penaltyPoints;
	}
}
