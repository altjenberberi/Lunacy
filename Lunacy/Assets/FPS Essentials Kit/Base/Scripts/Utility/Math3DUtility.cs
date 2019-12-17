/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    public sealed class Math3DUtility
    {
        public static Vector3 RandomInsideBounds (Vector3 min, Vector3 max)
        {
            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        }

        public static Vector3 Midpoint (Vector3 a, Vector3 b)
        {
            return new Vector3((a.x + b.x) / 2, (a.y + b.y) / 2, (a.z + b.z) / 2);
        }

        public static Quaternion ClampRotationAroundXAxis (Quaternion q, float minimum, float maximum)
        {
            q.x /= q.w;
            q.y = 0;
            q.z = 0;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, minimum, maximum);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}

