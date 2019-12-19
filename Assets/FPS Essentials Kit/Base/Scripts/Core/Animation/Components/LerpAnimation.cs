/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;
using System.Collections;

namespace Essentials
{
    namespace Animation
    {
        [System.Serializable]
        public class LerpAnimation
        {
            [SerializeField]
            private Vector3 m_TargetPosition;

            [SerializeField]
            private Vector3 m_TargetRotation;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_Duration = 0.25f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_ReturnDuration = 0.25f;

            // Animation Position / Rotation getters
            public Vector3 Position { get; set; } 
            public Vector3 Rotation { get; set; }

            public LerpAnimation (Vector3 targetPosition, Vector3 targetRotation, float duration, float returnDuration)
            {
                m_TargetPosition = targetPosition;
                m_TargetRotation = targetRotation;
                m_Duration = duration;
                m_ReturnDuration = returnDuration;
            }

            public void SetTargets (Vector3 targetPosition, Vector3 targetRotation)
            {
                m_TargetPosition = targetPosition;
                m_TargetRotation = targetRotation;
            }

            public IEnumerator Play ()
            {
                Vector3 initialPos = Position;
                Vector3 initialRot = Rotation;

                // Make the GameObject move to target slightly
                for (float t = 0f; t <= m_Duration; t += Time.deltaTime)
                {
                    Position = Vector3.Lerp(initialPos, m_TargetPosition, t / m_Duration);
                    Rotation = Vector3.Lerp(initialRot, m_TargetRotation, t / m_Duration);
                    yield return new WaitForFixedUpdate();
                }

                // Make it move back to neutral
                for (float t = 0f; t <= m_ReturnDuration; t += Time.deltaTime)
                {
                    Position = Vector3.Lerp(m_TargetPosition, Vector3.zero, t / m_ReturnDuration);
                    Rotation = Vector3.Lerp(m_TargetRotation, Vector3.zero, t / m_ReturnDuration);
                    yield return new WaitForFixedUpdate();
                }

                Position = Vector3.zero;
                Rotation = Vector3.zero;
            }
        }
    }
}
