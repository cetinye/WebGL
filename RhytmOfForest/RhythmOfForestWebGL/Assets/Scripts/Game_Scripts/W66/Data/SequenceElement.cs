using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_rotf
{
    [Serializable]
    public class SequenceElement
    {
        public int Index;
        public float Time;

        public SequenceElement(int index, float time)
        {
            Index = index;
            Time = time;
        }
    }
}
