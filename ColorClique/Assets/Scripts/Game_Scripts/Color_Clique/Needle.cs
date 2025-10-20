using UnityEngine;
using DG.Tweening;

namespace Color_Clique
{
    public class Needle : MonoBehaviour
    {
        [SerializeField] private Wheel wheel;
        [SerializeField] private SpriteRenderer spRenderer;
        [SerializeField] private float baseSpeed;
        [SerializeField] private float needleRotateSpeed;
        [SerializeField] private Slot overlappingSlot;

        private void Update()
        {
            if (LevelManager.instance.IsTimerOn)
                transform.RotateAround(wheel.center.position, new Vector3(0, 0, 1), needleRotateSpeed * Time.deltaTime);
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Slot slot))
                overlappingSlot = slot;
        }

        public void SetNeedleColor(Color color, float duration)
        {
            spRenderer.DOColor(color, duration / 2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuart).OnComplete(() => LevelManager.instance.SetIsClickable(true));
        }

        public void SetNeedleSpeed(float speed)
        {
            needleRotateSpeed = baseSpeed * speed;
        }

        public void ReverseNeedle()
        {
            needleRotateSpeed = -needleRotateSpeed;
        }

        public Slot GetOverlappingSlot()
        {
            return overlappingSlot;
        }
    }
}