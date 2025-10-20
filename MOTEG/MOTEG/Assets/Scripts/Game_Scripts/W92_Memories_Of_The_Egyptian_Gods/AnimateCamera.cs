using UnityEngine;

namespace W92_Memories_Of_The_Egyptian_Gods
{
    public class AnimateCamera : MonoBehaviour
    {
        public static AnimateCamera instance;
        public GameObject mainCamera;
        public Canvas gameCanvas;

        private void Start()
        {
            instance = this;
        }

        public void SwitchCameraToMain()
        {
            gameCanvas.worldCamera = mainCamera.GetComponent<Camera>();
            mainCamera.SetActive(true);
            gameObject.SetActive(false);
            GameManager.instance.state = GameManager.GameState.Playing;
            GameManager.instance.tappable = true;
            GameManager.instance.faceUpCardCount = 0;
        }

        public void SwitchCameraToSecond()
        {
            gameCanvas.worldCamera = GetComponent<Camera>();
            gameObject.SetActive(true);
            mainCamera.SetActive(false);
        }
    }
}