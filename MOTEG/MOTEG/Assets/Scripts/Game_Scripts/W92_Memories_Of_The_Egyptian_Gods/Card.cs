using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace W92_Memories_Of_The_Egyptian_Gods
{
    public class Card : MonoBehaviour, ICard
    {
        public bool facedUp;
        public Sprite front, back;
        public float timeToFlip = 0.125f;

        private Image cardImage, childImage;
        private bool flipRoutineAllowed;


        private void Start()
        {
            cardImage = GetComponent<Image>();
            cardImage.sprite = back;
            flipRoutineAllowed = true;
            facedUp = false;
            childImage = transform.GetChild(0).GetComponent<Image>();
            childImage.gameObject.SetActive(false);
        }

        private void Update()
        {
            //resize collider
            GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<RectTransform>().rect.width - 30f, GetComponent<RectTransform>().rect.height - 10f);
        }

        public void Tapped()
        {
            if (GameManager.instance.tappable)
            {
                if (!facedUp && !GameManager.instance.tappedCards.Contains(this.gameObject))
                {
                    UIManager.instance.moveCounter++;
                    GameManager.instance.tappedCards.Add(this.gameObject);
                    AudioManager.instance.PlayOneShot("CardTapped");

                    if (flipRoutineAllowed)
                    {
                        StartCoroutine(FlipCard());
                    }
                }

                else if (facedUp && this.gameObject != GameManager.instance.card1 && this.gameObject != GameManager.instance.card2)
                {
                    GameManager.instance.tappedCards.Remove(this.gameObject);
                    AudioManager.instance.PlayOneShot("CardTapped");

                    if (flipRoutineAllowed)
                    {
                        StartCoroutine(FlipCard());
                    }
                }
            }
        }

        public void FlipCardRoutine()
        {
            StartCoroutine(FlipCard());
        }

        IEnumerator FlipCard()
        {
            flipRoutineAllowed = false;

            if (!facedUp)
            {
                Tween flipOpen = transform.DORotate(new Vector3(0f, 90f, 0f), timeToFlip);

                yield return flipOpen.WaitForCompletion();
                cardImage.sprite = front;
                childImage.gameObject.SetActive(true);

                Tween flipOpenContinue = transform.DORotate(new Vector3(0f, 180f, 0f), timeToFlip);
                yield return flipOpenContinue.WaitForCompletion();
            }

            else if (facedUp)
            {
                Tween flipClose = transform.DORotate(new Vector3(0f, 90f, 0f), timeToFlip);

                yield return flipClose.WaitForCompletion();
                cardImage.sprite = back;
                childImage.gameObject.SetActive(false);

                Tween flipCloseContinue = transform.DORotate(new Vector3(0f, 0f, 0f), timeToFlip);
                yield return flipCloseContinue.WaitForCompletion();
            }

            facedUp = !facedUp;
            flipRoutineAllowed = true;
        }
    }
}