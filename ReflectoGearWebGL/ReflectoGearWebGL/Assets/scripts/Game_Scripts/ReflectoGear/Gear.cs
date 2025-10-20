using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace W91_ReflectoGear
{
    public class Gear : MonoBehaviour, IGear
    {
        public static bool isTappable = true;
        public LevelManager levelManager;
        public bool changable = true;
        public int X;
        public int Y;
        public bool highlighted = false;
        public bool endgameFlag = false;
        public bool isCalculated = false;


        public Sprite selectedImage;
        public Sprite unselectedImage;
        [SerializeField] private Image cardImage;
        [SerializeField] private float timeToColor = 1;

        private int tapCounter = 0;

        public void Tapped()
        {
            //Debug.Log("X: " + X  + ", Y: " + Y);

            if (changable)
            {
                //tap counter for deciding whether highlight the gear or not
                tapCounter++;

                if (tapCounter % 2 == 0)
                {
                    cardImage.sprite = unselectedImage;
                    highlighted = false;

                    isTappable = true;
                }
                else
                {
                    isTappable = false;

                    cardImage.sprite = selectedImage;
                    highlighted = true;

                    levelManager.Check(this);
                }
            }
        }

        public void TurnGreen()
        {
            StartCoroutine(TurnGreenRoutine());
        }

        IEnumerator TurnGreenRoutine()
        {
            GetComponent<Image>().color = Color.green;
            yield return new WaitForSeconds(timeToColor);
            GetComponent<Image>().color = Color.white;
        }

        public void TurnRed()
        {
            StartCoroutine(TurnRedRoutine());
        }

        IEnumerator TurnRedRoutine()
        {
            GetComponent<Image>().color = Color.red;
            yield return new WaitForSeconds(timeToColor);
            GetComponent<Image>().color = Color.white;
            Tapped();
        }
    }
}