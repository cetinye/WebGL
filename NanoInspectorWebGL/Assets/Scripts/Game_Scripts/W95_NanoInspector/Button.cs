using UnityEngine;

namespace NanoInspector
{
    public class Button : MonoBehaviour
    {
        [SerializeField] private LevelManager levelManager;

        public void YesButtonClicked()
        {
            if (!levelManager.isButtonPressable)
                return;

            levelManager.isButtonPressable = false;
            AudioManager.instance.PlayOneShot("Button");
            levelManager.CheckAnswer(true);
        }

        public void NoButtonClicked()
        {
            if (!levelManager.isButtonPressable)
                return;

            levelManager.isButtonPressable = false;
            AudioManager.instance.PlayOneShot("Button");
            levelManager.CheckAnswer(false);
        }
    }
}