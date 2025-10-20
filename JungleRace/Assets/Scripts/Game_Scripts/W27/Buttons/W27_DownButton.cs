using UnityEngine;
public class W27_DownButton : MonoBehaviour
{
        public W27_GameController gc;

        public void downButtonClicked()
        {
                gc.mg.down = true;
        }
        
        public void downButtonReleased()
        {
                gc.mg.down = false;
        }
}