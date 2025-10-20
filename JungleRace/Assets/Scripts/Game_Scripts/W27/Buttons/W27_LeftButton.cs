using UnityEngine;
public class W27_LeftButton : MonoBehaviour
{
        public W27_GameController gc;

        public void leftButtonClicked()
        {
                gc.mg.left = true;
        }
        
        public void leftButtonReleased()
        {
                gc.mg.left = false;
        }
}