using UnityEngine;

namespace W92_Memories_Of_The_Egyptian_Gods
{
    public class TapListener : MonoBehaviour
    {
        private GameObject tappedCard;

        // Update is called once per frame
        void Update()
        {
            CheckTap();
        }

        private void CheckTap()
        {
            //Checking if user is tapping anywhere on the scene
            if (GameManager.instance.state == GameManager.GameState.Playing && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //if yes, get the position
                var worldPoint = GameManager.instance.mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
                var touchPos = new Vector2(worldPoint.x, worldPoint.y);

                //checking if user tapped on a card
                if (Physics2D.OverlapPoint(touchPos) != null &&
                    GameManager.instance.tappable &&
                    Physics2D.OverlapPoint(touchPos).TryGetComponent(out ICard iCard))
                {
                    tappedCard = Physics2D.OverlapPoint(touchPos).gameObject;
                    Card card = tappedCard.GetComponent<Card>();

                    if (GameManager.instance.tappedCards.Count <= 2 && !card.facedUp && !GameManager.instance.tappedCards.Contains(tappedCard))
                    {
                        UIManager.instance.moveCounter++;
                        GameManager.instance.tappedCards.Add(tappedCard);
                    }

                    if (card.facedUp && tappedCard != GameManager.instance.card1 && tappedCard != GameManager.instance.card2)
                        GameManager.instance.tappedCards.Remove(tappedCard);

                    iCard.Tapped();
                    AudioManager.instance.PlayOneShot("CardTapped");
                }
            }

#if UNITY_WEBGL

            // Checking if user is clicking anywhere on the scene
            if (GameManager.instance.state == GameManager.GameState.Playing && Input.GetMouseButtonDown(0))
            {
                // If yes, get the position
                var worldPoint = GameManager.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var touchPos = new Vector2(worldPoint.x, worldPoint.y);

                // Checking if user clicked on a card
                if (Physics2D.OverlapPoint(touchPos) != null &&
                    GameManager.instance.tappable &&
                    Physics2D.OverlapPoint(touchPos).TryGetComponent(out ICard iCard))
                {
                    tappedCard = Physics2D.OverlapPoint(touchPos).gameObject;
                    Card card = tappedCard.GetComponent<Card>();

                    if (GameManager.instance.tappedCards.Count <= 2 && !card.facedUp && !GameManager.instance.tappedCards.Contains(tappedCard))
                    {
                        UIManager.instance.moveCounter++;
                        GameManager.instance.tappedCards.Add(tappedCard);
                    }

                    if (card.facedUp && tappedCard != GameManager.instance.card1 && tappedCard != GameManager.instance.card2)
                        GameManager.instance.tappedCards.Remove(tappedCard);

                    iCard.Tapped();
                    AudioManager.instance.PlayOneShot("CardTapped");
                }
            }

#endif
        }
    }
}