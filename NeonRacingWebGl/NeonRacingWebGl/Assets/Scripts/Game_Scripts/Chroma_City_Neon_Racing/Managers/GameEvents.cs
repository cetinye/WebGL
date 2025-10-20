using System;
using UnityEngine;

namespace Chroma_City_Neon_Racing
{

    public class GameEvents : MonoBehaviour
    {
        public static GameEvents instance;

        void Awake()
        {
            instance = this;
        }

        public event Action timePickedUp;
        public void TimePickedUp()
        {
            if (timePickedUp != null)
            {
                timePickedUp?.Invoke();
            }
        }

        public event Action speedPickedUp;
        public void SpeedPickedUp()
        {
            if (speedPickedUp != null)
            {
                speedPickedUp?.Invoke();
            }
        }

        public event Action shieldPickedUp;
        public void ShieldPickedUp()
        {
            if (shieldPickedUp != null)
            {
                shieldPickedUp?.Invoke();
            }
        }
    }
}