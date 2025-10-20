using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity_CS
{
    public static class _UnityCSTransform
    {
        #region TRANSFORM EXTENSIONS

        #region SET POSITION
        #region LOCAL POSITION
        /// <summary>
        /// Sets given <see cref="Transform"/>'s local position to a new <see cref="Vector3"/> position.
        /// </summary>
        /// <param name="t"><see cref="Transform"/></param>
        /// <param name="v3"><see cref="Vector3"/></param>
        /// <returns></returns>
        public static Transform _SetLocalPosition(this Transform t, Vector3 v3)
        {
            t.localPosition = v3;
            return t;
        }

        /// <summary>
        /// Sets given <see cref="Transform"/>'s local position to a new <see cref="Vector3"/>(<see cref="float"/> x, <see cref="float"/> y, <see cref="float"/> z) position.
        /// </summary>
        /// <param name="t"><see cref="Transform"/></param>
        /// <param name="x"><see cref="float"/></param>
        /// <param name="y"><see cref="float"/></param>
        /// <param name="z"><see cref="float"/></param>
        /// <returns></returns>
        public static Transform _SetLocalPosition(this Transform t, float x, float y, float z)
        {
            return t._SetLocalPosition(new Vector3(x, y, z));
        }

        /// <summary>
        /// Sets give <see cref="Transform"/>'s local position to a new same <see cref="Vector3"/>(<see cref="float"/> xyz) value
        /// </summary>
        /// <param name="t"><see cref="Transform"/></param>
        /// <param name="xyz"><see cref="float"/></param>
        /// <returns></returns>
        public static Transform _SetLocalPositionSame(this Transform t, float xyz)
        {
            return t._SetLocalPosition(new Vector3(xyz, xyz, xyz));
        }
        
        public static Transform _SetLocalPositionXY(this Transform t, float x, float y)
        {
            return t._SetLocalPosition(x, y, t.localPosition.z);
        }

        public static Transform _SetLocalPositionX(this Transform t, float x)
        {
            return t._SetLocalPosition(new Vector3(x, t.localPosition.y, t.localPosition.z));
        }

        public static Transform _SetLocalPositionY(this Transform t, float y)
        {
            return t._SetLocalPosition(new Vector3(t.localPosition.x, y, t.localPosition.z));
        }

        public static Transform _SetLocalPositionZ(this Transform t, float z)
        {
            return t._SetLocalPosition(new Vector3(t.localPosition.x, t.localPosition.y, z));
        }
        #endregion

        #region WORLD POSITION

        public static Transform _SetPosition(this Transform t, Vector3 v3)
        {
            t.position = v3;
            return t;
        }

        public static Transform _SetPosition(this Transform t, float x, float y, float z)
        {
            return t._SetPosition(new Vector3(x,y,z));
        }

        public static Transform _SetPositionSame(this Transform t, float xyz)
        {
            return t._SetPosition(new Vector3(xyz, xyz, xyz));
        }
        public static Transform _SetPositionXY(this Transform t, float x, float y)
        {
            return t._SetPosition(new Vector3(x, y, t.position.z));
        }

        public static Transform _SetPositionX(this Transform t, float x)
        {
            var position = t.position;
            return t._SetPosition(new Vector3(x, position.y, position.z));
        }
        public static Transform _SetPositionY(this Transform t, float y)
        {
            var position = t.position;
            return t._SetPosition(new Vector3(position.x,y,position.z));
        }
        public static Transform _SetPositionZ(this Transform t, float z)
        {
            var position = t.position;
            return t._SetPosition(new Vector3(position.x, position.y, z));
        }
        #endregion
        #endregion

        #region ADD TO POSITION
        public static Transform _AddLocalPosition(this Transform t, Vector3 v3)
        {
            t.localPosition += v3;
            return t;
        }

        public static Transform _AddLocalPosition(this Transform t, float x, float y, float z)
        {
            return t._AddLocalPosition(new Vector3(x, y, z));
        }

        public static Transform _AddLocalPositionXY(this Transform t, float x, float y)
        {
            return t._AddLocalPosition(new Vector3(x, y, 0f));
        }

        public static Transform _AddLocalPositionX(this Transform t, float x)
        {
            return t._AddLocalPosition(new Vector3(x, 0f, 0f));
        }

        public static Transform _AddLocalPositionY(this Transform t, float y)
        {
            return t._AddLocalPosition(new Vector3(0f, y, 0f));
        }

        public static Transform _AddLocalPositionZ(this Transform t, float z)
        {
            return t._AddLocalPosition(new Vector3(0f, 0f, z));
        }

        public static Transform _AddPosition(this Transform t, Vector3 v3)
        {
            t.position += v3;
            return t;
        }

        public static Transform _AddPosition(this Transform t, float x, float y, float z)
        {
            return t._AddPosition(new Vector3(x, y, z));
        }

        public static Transform _AddPositionXY(this Transform t, float x, float y)
        {
            return t._AddPosition(new Vector3(x, y, 0f));
        }

        public static Transform _AddPositionX(this Transform t, float x)
        {
            return t._AddPosition(new Vector3(x, 0f, 0f));
        }

        public static Transform _AddPositionY(this Transform t, float y)
        {
            return t._AddPosition(new Vector3(0f, y, 0f));
        }

        public static Transform _AddPositionZ(this Transform t, float z)
        {
            return t._AddPosition(new Vector3(0f, 0f, z));
        }
        

        #endregion

        #region RESET POSITION
        public static Transform _ResetLocalPosition(this Transform t)
        {
            t.localPosition = Vector3.zero;
            return t;
        }

        public static Transform _ResetLocalPositionX(this Transform t)
        {
            return t._SetLocalPositionX(0f);
        }

        public static Transform _ResetLocalPositionY(this Transform t)
        {
            return t._SetLocalPositionY(0f);
        }

        public static Transform _ResetLocalPositionXY(this Transform t)
        {
            return t._SetLocalPositionXY(0f, 0f);
        }

        public static Transform _ResetLocalPositionZ(this Transform t)
        {
            return t._SetLocalPositionZ(0f);
        }

        public static Transform _ResetPosition(this Transform t)
        {
            return t._SetPositionSame(0f);
        }

        public static Transform _ResetPositionX(this Transform t)
        {
            return t._SetPositionX(0f);
        }

        public static Transform _ResetPositionY(this Transform t)
        {
            return t._SetPositionY(0f);
        }

        public static Transform _ResetPositionXY(this Transform t)
        {
            return t._SetPositionXY(0f, 0f);
        }

        public static Transform _ResetPositionZ(this Transform t)
        {
            return t._SetPositionZ(0f);
        }
        

        #endregion

        #region SET EULER ANGLES
        public static Transform _SetLocalEulerAngles(this Transform t, Vector3 v3)
        {
            t.localEulerAngles = v3;
            return t;
        }

        public static Transform _SetLocalEulerAngles(this Transform t, float x, float y, float z)
        {
            return t._SetLocalEulerAngles(new Vector3(x, y, z));
        }

        public static Transform _SetLocalEulerAngles(this Transform t, float xyz)
        {
            t.localEulerAngles = new Vector3(xyz, xyz, xyz);
            return t;
        }

        public static Transform _SetLocalEulerAnglesXY(this Transform t, float x, float y)
        {
            return t._SetLocalEulerAngles(new Vector3(x, y, t.localEulerAngles.z));
        }

        public static Transform _SetLocalEulerAnglesX(this Transform t, float x)
        {
            return t._SetLocalEulerAngles(new Vector3(x, t.localEulerAngles.y, t.localEulerAngles.z));
        }

        public static Transform _SetLocalEulerAnglesY(this Transform t, float y)
        {
            return t._SetLocalEulerAngles(new Vector3(t.localEulerAngles.x, y, t.localEulerAngles.z));
        }

        public static Transform _SetLocalEulerAnglesZ(this Transform t, float z)
        {
            return t._SetLocalEulerAngles(new Vector3(t.localEulerAngles.x, t.localEulerAngles.y, z));
        }

        public static Transform _SetEulerAngles(this Transform t, Vector3 v3)
        {
            t.eulerAngles = v3;
            return t;
        }

        public static Transform _SetEulerAngles(this Transform t, float x, float y, float z)
        {
            return t._SetEulerAngles(new Vector3(x, y, z));
        }

        public static Transform _SetEulerAngles(this Transform t, float xyz)
        {
            return t._SetEulerAngles(new Vector3(xyz, xyz, xyz));
        }

        public static Transform _SetEulerAnglesXY(this Transform t, float x, float y)
        {
            return t._SetEulerAngles(new Vector3(x, y, t.eulerAngles.z));
        }

        public static Transform _SetEulerAnglesX(this Transform t, float x)
        {
            return t._SetEulerAngles(new Vector3(x, t.eulerAngles.y, t.eulerAngles.z));
        }

        public static Transform _SetEulerAnglesY(this Transform t, float y)
        {
            return t._SetEulerAngles(new Vector3(t.eulerAngles.x, y, t.eulerAngles.z));
        }

        public static Transform _SetEulerAnglesZ(this Transform t, float z)
        {
            return t._SetEulerAngles(new Vector3(t.eulerAngles.x, t.eulerAngles.y, z));
        }
        

        #endregion

        #region ADD TO EULER ANGLES
        public static Transform _AddLocalEulerAngles(this Transform t, Vector3 v3)
        {
            t.localEulerAngles += v3;
            return t;
        }

        public static Transform _AddLocalEulerAngles(this Transform t, float x, float y, float z)
        {
            return t._AddLocalEulerAngles(new Vector3(x, y, z));
        }

        public static Transform _AddLocalEulerAnglesXY(this Transform t, float x, float y)
        {
            return t._AddLocalEulerAngles(new Vector3(x, y, 0f));
        }

        public static Transform _AddLocalEulerAnglesX(this Transform t, float x)
        {
            return t._AddLocalEulerAngles(new Vector3(x, 0f, 0f));
        }

        public static Transform _AddLocalEulerAnglesY(this Transform t, float y)
        {
            return t._AddLocalEulerAngles(new Vector3(0f, y, 0f));
        }

        public static Transform _AddLocalEulerAnglesZ(this Transform t, float z)
        {
            return t._AddLocalEulerAngles(new Vector3(0f, 0f, z));
        }

        public static Transform _AddEulerAngles(this Transform t, Vector3 v3)
        {
            t.eulerAngles += v3;
            return t;
        }

        public static Transform _AddEulerAngles(this Transform t, float x, float y, float z)
        {
            return t._AddEulerAngles(new Vector3(x, y, z));
        }

        public static Transform _AddEulerAnglesXY(this Transform t, float x, float y)
        {
            return t._AddEulerAngles(new Vector3(x, y, 0f));
        }

        public static Transform _AddEulerAnglesX(this Transform t, float x)
        {
            return t._AddEulerAngles(new Vector3(x, 0f, 0f));
        }

        public static Transform _AddEulerAnglesY(this Transform t, float y)
        {
            return t._AddEulerAngles(new Vector3(0f, y, 0f));
        }

        public static Transform _AddEulerAnglesZ(this Transform t, float z)
        {
            return t._AddEulerAngles(new Vector3(0f, 0f, z));
        }
        
        #endregion

        #region RESET EULER ANGLES
        public static Transform _ResetLocalEulerAngles(this Transform t)
        {
            t.localEulerAngles = Vector3.zero;
            return t;
        }

        public static Transform _ResetLocalEulerAnglesX(this Transform t)
        {
            return t._SetLocalEulerAnglesX(0f);
        }

        public static Transform _ResetLocalEulerAnglesY(this Transform t)
        {
            return t._SetLocalEulerAnglesY(0f);
        }

        public static Transform _ResetLocalEulerAnglesXY(this Transform t)
        {
            return t._SetLocalEulerAnglesXY(0f, 0f);
        }

        public static Transform _ResetLocalEulerAnglesZ(this Transform t)
        {
            return t._SetLocalEulerAnglesZ(0f);
        }

        public static Transform _ResetEulerAngles(this Transform t)
        {
            return t._SetEulerAngles(0f);
        }

        public static Transform _ResetEulerAnglesX(this Transform t)
        {
            return t._SetEulerAnglesX(0f);
        }

        public static Transform _ResetEulerAnglesY(this Transform t)
        {
            return t._SetEulerAnglesY(0f);
        }

        public static Transform _ResetEulerAnglesXY(this Transform t)
        {
            return t._SetEulerAnglesXY(0f, 0f);
        }

        public static Transform _ResetEulerAnglesZ(this Transform t)
        {
            return t._SetEulerAnglesZ(0f);
        }
        

        #endregion

        #region SET SCALE
        public static Transform _SetScale(this Transform t, Vector3 v3)
        {
            t.localScale = v3;
            return t;
        }

        public static Transform _SetScale(this Transform t, float x, float y, float z)
        {
            return t._SetScale(new Vector3(x, y, z));
        }

        public static Transform _SetScale(this Transform t, float xyz)
        {
            t.localScale = _CSV3.Same(xyz);
            return t;
        }

        public static Transform _SetScaleXY(this Transform t, float x, float y)
        {
            return t._SetScale(new Vector3(x, y, t.localScale.z));
        }

        public static Transform _SetScaleX(this Transform t, float x)
        {
            return t._SetScale(new Vector3(x, t.localScale.y, t.localScale.z));
        }

        public static Transform _SetScaleY(this Transform t, float y)
        {
            return t._SetScale(new Vector3(t.localScale.x, y, t.localScale.z));
        }

        public static Transform _SetScaleZ(this Transform t, float z)
        {
            return t._SetScale(new Vector3(t.localScale.x, t.localScale.y, z));
        }
        

        #endregion

        #region ADD TO SCALE
        public static Transform _AddScale(this Transform t, Vector3 v3)
        {
            t.localScale += v3;
            return t;
        }

        public static Transform _AddScale(this Transform t, float x, float y, float z)
        {
            return t._AddScale(new Vector3(x, y, z));
        }

        public static Transform _AddScaleXY(this Transform t, float x, float y)
        {
            return t._AddScale(new Vector3(x, y, 0f));
        }

        public static Transform _AddScaleX(this Transform t, float x)
        {
            return t._AddScale(new Vector3(x, 0f, 0f));
        }

        public static Transform _AddScaleY(this Transform t, float y)
        {
            return t._AddScale(new Vector3(0f, y, 0f));
        }

        public static Transform _AddScaleZ(this Transform t, float z)
        {
            return t._AddScale(new Vector3(0f, 0f, z));
        }
        

        #endregion

        #region RESET SCALE
        public static Transform _ResetScale(this Transform t)
        {
            t.localScale = Vector3.one;
            return t;
        }

        public static Transform _ResetScaleX(this Transform t)
        {
            return t._SetScaleX(0f);
        }

        public static Transform _ResetScaleY(this Transform t)
        {
            return t._SetScaleY(0f);
        }

        public static Transform _ResetScaleXY(this Transform t)
        {
            return t._SetScaleXY(0f, 0f);
        }

        public static Transform _ResetScaleZ(this Transform t)
        {
            return t._SetScaleZ(0f);
        }
        

        #endregion
        #endregion
    }
}

