using UnityEngine;

namespace W91_ReflectoGear
{
    public class BottomBars : MonoBehaviour
    {
        public void EnableParticle()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}