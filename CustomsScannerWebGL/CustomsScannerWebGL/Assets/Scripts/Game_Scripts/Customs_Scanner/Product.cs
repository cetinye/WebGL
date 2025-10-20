using UnityEngine;

namespace Customs_Scanner
{
    public class Product : MonoBehaviour
    {
        public bool isTouchable = false;
        public bool isCorrect;
        public bool isForbiddenProduct = false;
        public bool isAlreadyClicked = false;
        public SpriteRenderer spriteRenderer;

        public void Tapped()
        {
            //make only clickable once
            if (isTouchable && !isAlreadyClicked)
            {
                Taptic.Light();

                if (isForbiddenProduct)
                {
                    AudioManager.instance.PlayOneShot("Correct");
                    spriteRenderer.color = Color.green;
                    isCorrect = true;
                }
                else
                {
                    AudioManager.instance.PlayOneShot("Wrong");
                    spriteRenderer.color = Color.red;
                    isCorrect = false;
                }
            }
        }
    }
}