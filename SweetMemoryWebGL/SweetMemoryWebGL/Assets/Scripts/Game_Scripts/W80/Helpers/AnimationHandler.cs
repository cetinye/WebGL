using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_SweetMemory
{
    public class AnimationHandler : MonoBehaviour
    {
        public void Play(int audioIndex)
        {
            AudioController.Play((AudioType)audioIndex, 0.3f);
        }
    }
}

