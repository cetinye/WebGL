using UnityEngine;
using UnityEngine.UI;

public class W59_AnswerButton : MonoBehaviour
{
   [SerializeField] private W59_LevelController levelController;
   [SerializeField] private Button answerButton;
   [SerializeField] private GameObject ticketContainer;
   [SerializeField] private Image ticketIndicator;
   [SerializeField] private Sprite[] indicatorSprites;
   public W59_Lane assignedLane;
   public KeyCode keyCode;

   void Update()
   {
#if UNITY_WEBGL

      if (Input.GetKeyDown(keyCode))
      {
         OnAnswerSelected();
      }

#endif
   }

   public void OnAnswerSelected()
   {
      levelController.CheckAnswer(this);
   }

   public void ShowTicket(bool isCorrect)
   {
      ticketIndicator.sprite = indicatorSprites[isCorrect ? 0 : 1];
      ticketContainer.SetActive(true);

      Invoke(nameof(CloseTicket), 1.8f);
   }

   public void CloseTicket()
   {
      ticketContainer.SetActive(false);
   }
}