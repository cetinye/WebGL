using UnityEngine;
public class W27_UpButton : MonoBehaviour
{
        public W27_GameController gc;

        public void upButtonClicked()
        {
                gc.mg.up = true;
        }
        
        public void upButtonReleased()
        {
                gc.mg.up = false;
        }
}