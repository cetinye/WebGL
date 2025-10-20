using UnityEngine;
using Lean.Touch;

namespace Grill_Thrills
{
    public class TapManager : MonoBehaviour
    {
        private void OnEnable()
        {
            LeanTouch.OnFingerTap += HandleFingerTap;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= HandleFingerTap;
        }

        private void HandleFingerTap(LeanFinger finger)
        {
            Ray rayCam = LevelManager.instance.mainCamera.ScreenPointToRay(finger.ScreenPosition);
            if (Physics.Raycast(rayCam, out RaycastHit hitCam) && hitCam.collider.transform.parent.TryGetComponent<IFood>(out IFood _food))
            {
                _food.Tapped();
            }
        }
    }
}
