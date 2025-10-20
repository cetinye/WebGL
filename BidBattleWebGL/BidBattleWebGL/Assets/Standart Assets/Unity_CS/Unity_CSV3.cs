using UnityEngine;

namespace Unity_CS
{
    public static class _CSV3
    {
        public static Vector3 Same(float xyz)
        {
            return new Vector3(xyz, xyz, xyz);
        }

        public static Vector3 Same(int xyz)
        {
            return new Vector3(xyz, xyz, xyz);
        }

        public static Vector3 _Print(this Vector3 v3)
        {
            Debug.Log($"x: {v3.x} y: {v3.y} z: {v3.z}");
            return v3;
        }

        public static Vector3 _GetWorldPointV3(this Vector3 v, Camera cam)
        {
            return cam.ScreenToWorldPoint(v);
        }
        public static Vector2 _GetWorldPointV2(this Vector3 v, Camera cam)
        {
            return cam.ScreenToWorldPoint(v);
        }
    }
}