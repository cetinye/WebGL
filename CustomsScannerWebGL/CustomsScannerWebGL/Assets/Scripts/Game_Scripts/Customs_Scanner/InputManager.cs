using UnityEngine;

namespace Customs_Scanner
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private UIManager uiManager;

        void Update()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RaycastHit2D rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Input.GetTouch(0).position));

                if (rayHit.collider != null && rayHit.collider.TryGetComponent<Product>(out Product _product))
                {
                    if (_product.isAlreadyClicked)
                        return;

                    _product.Tapped();

                    if (_product.isTouchable)
                        uiManager.SetXrayLight(_product.isCorrect);

                    _product.isAlreadyClicked = true;
                }
            }

#if UNITY_WEBGL

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D rayHit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Input.mousePosition));

                if (rayHit.collider != null && rayHit.collider.TryGetComponent<Product>(out Product _product))
                {
                    if (_product.isAlreadyClicked)
                        return;

                    _product.Tapped();

                    if (_product.isTouchable)
                        uiManager.SetXrayLight(_product.isCorrect);

                    _product.isAlreadyClicked = true;
                }
            }

#endif
        }
    }
}