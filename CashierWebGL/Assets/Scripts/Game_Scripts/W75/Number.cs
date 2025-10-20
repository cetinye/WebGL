using UnityEngine;

namespace Cashier
{
	public class Number : MonoBehaviour
	{
		public static bool isPressable = false;
		public KeyCode keyCode_1, keyCode_2;
		public int value;

		[SerializeField] private LevelManager levelManager;

		void Update()
		{

#if UNITY_WEBGL

			if (Input.GetKeyDown(keyCode_1) || Input.GetKeyDown(keyCode_2))
			{
				ButtonDown(value);
			}

#endif
		}

		public void ButtonDown(int value)
		{
			if (isPressable)
			{
				AudioManager.instance.PlayOneShot(SoundType.OneNumber);
				levelManager.NumberPressed(value);
			}
		}
	}
}
