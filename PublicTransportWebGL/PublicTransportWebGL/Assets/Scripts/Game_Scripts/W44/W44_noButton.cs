using UnityEngine;

public class W44_noButton : MonoBehaviour
{
    public W44_MainController mc;

    public void OnNoClicked()
    {
        mc.selectedAnswerFromButtons = 2;
        mc.selectedButton = gameObject;
        mc.checkGivenAnswerCorrect();
    }
}