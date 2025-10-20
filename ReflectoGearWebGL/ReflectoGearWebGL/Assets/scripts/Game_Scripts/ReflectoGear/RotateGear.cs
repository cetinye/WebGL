using UnityEngine;

namespace W91_ReflectoGear
{
    public class RotateGear : MonoBehaviour
    {
        [Header("-1 for counterclockwise 1 for clockwise")]
        [SerializeField] private float rotateClockwise;
        [SerializeField] private float zRotationVal;
        [SerializeField] private GameManager.GameState rotateOnState;
        private bool isLevelGearsPlaying = false;
        private bool isBottomLeftGearsPlaying = false;

        private GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.instance;    
        }

        // Update is called once per frame
        void Update()
        {
            if (gameManager.state == rotateOnState)
                transform.Rotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, rotateClockwise * zRotationVal * Time.deltaTime));

            if (gameManager.state == GameManager.GameState.Idle && !isLevelGearsPlaying)
            {
                isLevelGearsPlaying = true;
                isBottomLeftGearsPlaying = false;
                AudioManager.instance.Stop("BottomLeftGears");
                AudioManager.instance.Play("LevelGears");
            }

            if (gameManager.state == GameManager.GameState.Playing && !isBottomLeftGearsPlaying)
            {
                isBottomLeftGearsPlaying = true;
                isLevelGearsPlaying = false;
                AudioManager.instance.Stop("LevelGears");
                AudioManager.instance.Play("BottomLeftGears");
            }
        }
    }
}