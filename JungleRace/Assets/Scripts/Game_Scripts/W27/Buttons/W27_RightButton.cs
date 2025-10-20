using UnityEngine;
public class W27_RightButton : MonoBehaviour
{
        public W27_GameController gc;

        public void rightButtonClicked()
        {
                gc.mg.right = true;
        }
        
        public void rightButtonReleased()
        {
                gc.mg.right = false;
        }
}