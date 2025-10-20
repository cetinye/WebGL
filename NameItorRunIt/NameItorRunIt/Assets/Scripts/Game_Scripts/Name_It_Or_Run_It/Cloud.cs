using UnityEngine;

namespace Name_It_Or_Run_It
{
    public class Cloud : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private Vector3 startingPos;

        void Awake()
        {
            startingPos = transform.position;
        }

        void Update()
        {
            transform.Translate(speed / 100 * Time.deltaTime * Vector3.right);

            if (transform.position.x >= 15f)
                transform.position = startingPos;
        }
    }
}