using UnityEngine;

public class W44_yesButton : MonoBehaviour
{
    public W44_MainController mc;

    public void OnYesClicked()
    {
        mc.selectedAnswerFromButtons = 1;
        mc.selectedButton = gameObject;
        mc.checkGivenAnswerCorrect();
    }
}