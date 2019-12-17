/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    namespace Weapons
    {
        [DisallowMultipleComponent]
        public class ViewmodelProjector : MonoBehaviour
        {
            // 0 - Auto
            // 1 - 4:3
            // 2 - 5:4
            // 3 - 16:10
            // 4 - 16:9
            [SerializeField]
            private int m_Aspect = 0;

            [SerializeField]
            [Range(1, 179)]
            private float m_FieldOfView = 60;

            [SerializeField]
            [MinMax(0.001f, 1000)]
            private float m_ZNear = 0.075f;

            [SerializeField]
            [MinMax(0.001f, 1000)]
            private float m_ZFar = 100;

            private float Aspect
            {
                get
                {
                    switch (m_Aspect)
                    {
                        case 0:
                            return (float)Screen.width / Screen.height;
                        case 1:
                            return 1.333f;
                        case 2:
                            return 1.25f;
                        case 3:
                            return 1.6f;
                        case 4:
                            return 1.777f;
                        default:
                            return (float)Screen.width / Screen.height;
                    }
                }
            }

            private void LateUpdate ()
            {
                Shader.SetGlobalMatrix("CUSTOM_MATRIX_P", GL.GetGPUProjectionMatrix(Matrix4x4.Perspective(m_FieldOfView, Aspect, m_ZNear, m_ZFar), false));
            }
        }
    }
}
