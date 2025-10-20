using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Witmina_SweetMemory
{
    public class CutoutMask : Image
    {
        public override Material materialForRendering
        {
            get
            {
                Material material = new Material(base.materialForRendering);
                material.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
                return material;
            }
        }
    }
}

