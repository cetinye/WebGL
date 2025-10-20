using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_MathOfFuel
{
    public abstract class GaugeScale : MonoBehaviour
    {
        protected readonly List<LineRenderer> Lines = new();
        protected static readonly int[] BigDenoms = {10, 5, 4, 3, 2};

        public virtual void SetScale(int denominator)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                Destroy(Lines[i].gameObject);
            }
            Lines.Clear();
        }
    }
}

