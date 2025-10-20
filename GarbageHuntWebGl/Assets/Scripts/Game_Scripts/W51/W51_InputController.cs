using System;
using UnityEngine;

namespace Witmina_GarbageHunt
{
    public class W51_InputController : MonoBehaviour
    {
        public event Action ShootButtonDown;
        public event Action ShootButtonUp;
        public event Action LeftButtonDown;
        public event Action LeftButtonUp;
        public event Action RightButtonDown;
        public event Action RightButtonUp;

        void Update()
        {

#if UNITY_WEBGL

            if (Input.GetKeyUp(KeyCode.Space))
            {
                OnShootButtonUp();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                OnShootButtonDown();
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                OnLeftButtonUp();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                OnLeftButtonDown();
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                OnRightButtonUp();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                OnRightButtonDown();
            }

#endif
        }

        public void OnShootButtonDown() { ShootButtonDown?.Invoke(); }

        public void OnShootButtonUp() { ShootButtonUp?.Invoke(); }

        public void OnLeftButtonDown() { LeftButtonDown?.Invoke(); }

        public void OnLeftButtonUp() { LeftButtonUp?.Invoke(); }

        public void OnRightButtonDown() { RightButtonDown?.Invoke(); }

        public void OnRightButtonUp() { RightButtonUp?.Invoke(); }
    }
}

