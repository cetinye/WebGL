using UnityEngine;

namespace Chroma_City_Neon_Racing
{

    public class CameraFollow : MonoBehaviour
    {
        public static CameraFollow instance;

        [SerializeField] private Transform player;
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;

        [SerializeField] private float smoothTime = 0.3f;
        private Vector3 velocity = Vector3.zero;
        private bool isLookAtTarget = false;
        private Vector3 resetPos;
        private Vector3 resetRot;

        void Awake()
        {
            instance = this;

            resetPos = transform.localPosition;
            resetRot = transform.localEulerAngles;
        }

        void LateUpdate()
        {
            if (isLookAtTarget)
            {
                Vector3 direction = (target.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction, target.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime * Time.deltaTime);
            }
        }

        public void DetachFromPlayer()
        {
            transform.SetParent(null);
            isLookAtTarget = true;
        }

        public void Reset()
        {
            isLookAtTarget = false;
            transform.SetParent(player);
            transform.SetLocalPositionAndRotation(resetPos, Quaternion.Euler(resetRot));
        }
    }
}