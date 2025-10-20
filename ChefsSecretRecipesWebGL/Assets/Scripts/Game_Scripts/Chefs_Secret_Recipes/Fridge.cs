using UnityEngine;
using UnityEngine.UI;

namespace Chefs_Secret_Recipes
{
    public class Fridge : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private Image fridgeImage;

        void Awake()
        {
            fridgeImage = GetComponent<Image>();
        }

        public void OpenFridge()
        {
            Reset();
            animator.Play("FridgeOpen");
        }

        public void CloseFridge()
        {
            animator.Play("FridgeClose");
        }

        public void Reset()
        {
            CloseFridge();
            animator.Play("Idle");
            animator.enabled = false;
            animator.enabled = true;
        }
    }
}