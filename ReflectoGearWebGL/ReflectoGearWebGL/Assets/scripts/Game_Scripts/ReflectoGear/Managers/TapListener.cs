using UnityEngine;

namespace W91_ReflectoGear
{
    public class TapListener : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            CheckTap();
        }

        private void CheckTap()
        {
            //Checking if user is tapping anywhere on the scene
            if (GameManager.instance.state == GameManager.GameState.Playing && Input.GetMouseButtonDown(0))
            {
                //if yes, get the position
                var worldPoint = GameManager.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var touchPos = new Vector2(worldPoint.x, worldPoint.y);

                //checking if user tapped on a gear
                if (Gear.isTappable && Physics2D.OverlapPoint(touchPos) != null &&
                    Physics2D.OverlapPoint(touchPos).TryGetComponent(out IGear iGear))
                {
                    iGear.Tapped();
                }
            }
        }
    }
}