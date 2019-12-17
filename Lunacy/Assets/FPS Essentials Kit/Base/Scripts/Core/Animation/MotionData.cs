/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    namespace Animation
    {
        [CreateAssetMenu(menuName = "Motion Data", fileName = "Motion Data", order = 101)]
        public sealed class MotionData : ScriptableObject
        {
            [SerializeField]
            private float m_Speed = 1; // Overall animation speed

            [SerializeField]
            [Range(0.1f, 3)]
            private float m_Smoothness = 1; // Interpolation smoothness 

            [SerializeField]
            private float m_HorizontalAmplitude = 1;

            [SerializeField]
            private float m_VerticalAmplitude = 1;

            [SerializeField]
            private AnimationCurve m_VerticalAnimationCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

            [SerializeField]
            private float m_RotationAmplitude = 0; // Z axis rotation

            [SerializeField]
            [Range(-2, 2)]
            private float m_VelocityInfluence = 1; // Player velocity influence on this animation

            [SerializeField]
            private Vector3 m_PositionOffset;

            [SerializeField]
            private Vector3 m_RotationOffset;

            #region PROPERTIES

            public float Speed { get { return m_Speed; } }
            public float Smoothness { get { return m_Smoothness; } }

            public float VelocityInfluence { get { return m_VelocityInfluence; } }

            public float HorizontalAmplitude { get { return m_HorizontalAmplitude; } }
            public float VerticalAmplitude { get { return m_VerticalAmplitude; } }
            public AnimationCurve VerticalAnimationCurve { get { return m_VerticalAnimationCurve; } }
            public float RotationAmplitude { get { return m_RotationAmplitude; } }

            public Vector3 PositionOffset { get { return m_PositionOffset; } }
            public Vector3 RotationOffset { get { return m_RotationOffset; } }

            #endregion
        }
    }
}
