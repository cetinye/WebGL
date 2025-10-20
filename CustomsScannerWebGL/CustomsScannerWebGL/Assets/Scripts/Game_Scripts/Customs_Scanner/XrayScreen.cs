using UnityEngine;

namespace Customs_Scanner
{
    public class XrayScreen : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Product>(out Product _prod))
                _prod.isTouchable = true;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.TryGetComponent<Product>(out Product _prod))
                _prod.isTouchable = false;

            if (_prod.isForbiddenProduct && !_prod.isAlreadyClicked)
            {
                GameManager.instance.Wrong();
            }
        }
    }
}