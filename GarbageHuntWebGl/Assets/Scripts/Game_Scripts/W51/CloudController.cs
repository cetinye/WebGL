using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    public class CloudController : MonoBehaviour
    {
        [SerializeField] private float _range = 10f;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private List<Transform> _clouds;

        private void Update()
        {
            foreach (var cloud in _clouds)
            {
                cloud.transform.Translate(_speed * Time.deltaTime * Vector3.right);
                
                if (cloud.localPosition.x > _range)
                    cloud.localPosition = -_range * Vector3.right;
            }
        }
    }
}

