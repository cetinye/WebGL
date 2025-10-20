using UnityEngine;

namespace Customs_Scanner
{
    [CreateAssetMenu(fileName = "CS_Product", menuName = "Data/Customs_Scanner/Product")]
    public class ProductSO : ScriptableObject
    {
        public Products productName;
        public Sprite productSprite;
    }
}