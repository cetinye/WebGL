using UnityEngine;
using DG.Tweening;

namespace Customs_Scanner
{
    public class Wheel : MonoBehaviour
    {
        public float turnTime;

        public void TurnWheel()
        {
            transform.DORotate(new Vector3(0, 0, transform.rotation.z + 720f), turnTime, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        }
    }
}