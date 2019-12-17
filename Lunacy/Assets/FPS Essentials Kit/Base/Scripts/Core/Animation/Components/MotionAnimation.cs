/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections;
using Essentials.Controllers;
using UnityEngine;

namespace Essentials
{
    namespace Animation
    {
        [System.Serializable]
        public sealed class MotionAnimation
        {
            [SerializeField]
            private MotionData m_WalkingMotionData; // Walking animation

            [SerializeField]
            private MotionData m_BrokenLegsMotionData; // Walking with lower body damaged

            [SerializeField]
            private MotionData m_RunningMotionData; // Running animation

            [SerializeField]
            [NotNull]
            [Tooltip("Transform used to apply the animations")]
            private Transform m_TargetTransform;

            #region JUMP & FALL ANIMATIONS

            [SerializeField]
            private LerpAnimation m_JumpAnimation = new LerpAnimation(new Vector3(0, 0.05f, 0), new Vector3(10, 0, 0), 0.15f, 0.25f);

            [SerializeField]
            private LerpAnimation m_LandingAnimation = new LerpAnimation(new Vector3(0, -0.075f, 0), Vector3.zero, 0.15f, 0.15f);

            #endregion

            #region DAMAGE

            [SerializeField]
            private Vector3 m_MinHitRotation = new Vector3(5, -5, 0);

            [SerializeField]
            private Vector3 m_MaxHitRotation = new Vector3(5, 5, 0);

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_HitDuration = 0.1f;

            #endregion

            #region BREATH

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_BreathingSpeed = 2;

            [SerializeField]
            private float m_BreathingAmplitude = 1;

            #endregion

            #region RECOIL

            // Weapons
            [SerializeField]
            private bool m_WeaponRecoil = false;

            [SerializeField]
            private Vector3 m_WeaponRecoilPosition = Vector3.zero;

            [SerializeField]
            private Vector3 m_WeaponRecoilRotation = Vector3.zero;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_WeaponRecoilDuration = 0.1f;

            // Camera
            [SerializeField]
            private bool m_CameraRecoil = false;

            [SerializeField]
            private Vector3 m_MinCameraRecoilRotation = Vector3.zero;

            [SerializeField]
            private Vector3 m_MaxCameraRecoilRotation = Vector3.zero;

            [SerializeField]
            [Range(0.01f, 0.25f)]
            private float m_CameraRecoilDuration = 0.05f;

            [SerializeField]
            [Range(0.01f, 0.25f)]
            private float m_CameraReturnDuration = 0.1f;

            #endregion

            #region EXPLOSION

            [System.Serializable]
            public struct ShakeProperties
            {
                [SerializeField]
                [MinMax(0, Mathf.Infinity)]
                private float m_Angle;

                [SerializeField]
                [MinMax(0, Mathf.Infinity)]
                private float m_Strength;

                [SerializeField]
                [MinMax(0, Mathf.Infinity)]
                private float m_MinSpeed;

                [SerializeField]
                [MinMax(0, Mathf.Infinity)]
                private float m_MaxSpeed;

                [SerializeField]
                [MinMax(0, Mathf.Infinity)]
                private float m_Duration;

                [SerializeField]
                [Range(0, 1)]
                private float m_NoisePercent;

                [SerializeField]
                [Range(0, 1)]
                private float m_DampingPercent;

                [SerializeField]
                [Range(0, 1)]
                private float m_RotationPercent;

                public float Angle { get { return m_Angle; } }
                public float Strenght { get { return m_Strength; } }
                public float MinSpeed { get { return m_MinSpeed; } }
                public float MaxSpeed { get { return m_MaxSpeed; } }
                public float Duration { get { return m_Duration; } }
                public float NoisePercent { get { return m_NoisePercent; } }
                public float DampingPercent { get { return m_DampingPercent; } }
                public float RotationPercent { get { return m_RotationPercent; } }

                public ShakeProperties (float angle, float strength, float minSpeed, float maxSpeed, float duration, float noisePercent, float dampingPercent, float rotationPercent)
                {
                    m_Angle = angle;
                    m_Strength = strength;
                    m_MaxSpeed = minSpeed;
                    m_MinSpeed = maxSpeed;
                    m_Duration = duration;
                    m_NoisePercent = Mathf.Clamp01(noisePercent);
                    m_DampingPercent = Mathf.Clamp01(dampingPercent);
                    m_RotationPercent = Mathf.Clamp01(rotationPercent);
                }
            }

            [SerializeField]
            private ShakeProperties m_GrenadeExplosionShake = new ShakeProperties();

            private Vector3 m_ExplosionPos;
            private Vector3 m_ExplosionRot;

            private const float k_MaxAngle = 90.0f;

            #endregion

            #region VAULT

            [SerializeField]
            private LerpAnimation m_VaultAnimation = new LerpAnimation(Vector3.zero, new Vector3(-10, 0, 10), 0.3f, 0.3f);

            #endregion

            #region LEAN

            [SerializeField]
            private bool m_Lean = true;

            [SerializeField]
            [Range(0, 0.2f)]
            private float m_LeanAmount = 0.2f;

            [SerializeField]
            private float m_LeanAngle = 20;

            [SerializeField]
            [MinMax(1, 20)]
            private float m_LeanSpeed = 5;

            #endregion

            private Vector3 m_TargetPos;
            private Quaternion m_TargetRot;

            private float m_Angle;
            private float m_VerticalInfluence;

            private Vector3 m_CurrentPos;
            private Vector3 m_CurrentRot;

            private Vector3 m_DamageRot;

            private Vector3 m_RecoilPos;
            private Vector3 m_RecoilRot;

            private Vector3 m_LeanPos;
            private Vector3 m_LeanRot;

            private Vector3 m_BreathingRot;
            private float m_BreathingProgress;

            #region PROPERTIES

            public bool CameraRecoil
            {
                get { return m_CameraRecoil; }
                set { m_CameraRecoil = value; }
            }

            public Vector3 MinCameraRecoilRotation
            {
                get { return m_MinCameraRecoilRotation; }
                set { m_MinCameraRecoilRotation = value; }
            }

            public Vector3 MaxCameraRecoilRotation
            {
                get { return m_MaxCameraRecoilRotation; }
                set { m_MaxCameraRecoilRotation = value; }
            }

            public float CameraRecoilDuration
            {
                get { return m_CameraRecoilDuration; }
                set { m_CameraRecoilDuration = value; }
            }

            public float CameraReturnDuration
            {
                get { return m_CameraReturnDuration; }
                set { m_CameraReturnDuration = value; }
            }

            public ShakeProperties GrenadeExplosionShake { get { return m_GrenadeExplosionShake; } }

            public LerpAnimation JumpAnimation { get { return m_JumpAnimation; } }
            public LerpAnimation LandingAnimation { get { return m_LandingAnimation; } }
            public LerpAnimation VaultAnimation { get { return m_VaultAnimation; } }

            public bool Lean { get { return m_Lean; } }
            public float LeanAmount
            {
                get { return m_LeanAmount; }
                set { m_LeanAmount = Mathf.Clamp(value, 0, 0.2f); }
            }

            #endregion

            public void BreathingAnimation (float speed = 1)
            {
                m_BreathingProgress += Time.deltaTime * m_BreathingSpeed * speed;

                if (m_BreathingProgress >= Mathf.PI * 2)
                    m_BreathingProgress -= Mathf.PI * 2;

                if (speed > 0)
                {
                    float sin = Mathf.Sin(m_BreathingProgress);
                    float cos = Mathf.Cos(m_BreathingProgress);

                    // Calculates the target rotation
                    Vector3 breathingRot = new Vector3(sin * cos * m_BreathingAmplitude, sin * m_BreathingAmplitude);
                    m_BreathingRot = Vector3.Lerp(m_BreathingRot, breathingRot, Time.deltaTime * 5 * m_BreathingSpeed * speed);
                }
                else
                {
                    m_BreathingRot = Vector3.Lerp(m_BreathingRot, Vector3.zero, Time.deltaTime * 5);
                }
            }

            public void MovementAnimation (FirstPersonController FPController)
            {
                //Stops the movement animation
                if (FPController.State == MotionState.Flying || FPController.State == MotionState.Idle)
                {
                    PerformMovementAnimation(null, Vector3.zero, 0);
                }
                else
                {
                    // Calculates the animation speed
                    float speed = Mathf.Max(FPController.State == MotionState.Running ? 6.5f : 2.75f, FPController.CurrentTargetSpeed);

                    // Lowerbody damaged (broken legs)
                    if (FPController.LowerBodyDamaged && !FPController.IsAiming)
                    {
                        PerformMovementAnimation(m_BrokenLegsMotionData, FPController.Velocity, speed);
                    }
                    else
                    {
                        PerformMovementAnimation(FPController.State == MotionState.Running ? m_RunningMotionData : m_WalkingMotionData, FPController.Velocity, speed);
                    }
                }

                if (m_TargetTransform != null)
                {
                    m_TargetTransform.localPosition = m_TargetPos;
                    m_TargetTransform.localRotation = m_TargetRot;
                }
            }

            private void PerformMovementAnimation (MotionData motionData, Vector3 movementDirection, float speed)
            {
                if (speed > 0 && motionData != null)
                {
                    m_Angle += Time.deltaTime * motionData.Speed * speed;

                    if (m_Angle >= Mathf.PI * 2)
                        m_Angle -= Mathf.PI * 2;

                    float sin = Mathf.Sin(m_Angle);
                    float cos = Mathf.Cos(m_Angle);

                    // Calculates the velocity influence
                    m_VerticalInfluence = Mathf.Lerp(m_VerticalInfluence, movementDirection.y * motionData.VerticalAmplitude * motionData.VelocityInfluence, Time.deltaTime * speed);

                    // Sets the target position
                    m_CurrentPos = motionData.PositionOffset + m_JumpAnimation.Position + m_LandingAnimation.Position + m_VaultAnimation.Position + m_RecoilPos + m_ExplosionPos + m_LeanPos;

                    m_CurrentRot = new Vector3((-Mathf.Abs(sin) * motionData.VerticalAmplitude + motionData.VerticalAmplitude)
                        * motionData.VerticalAnimationCurve.Evaluate(Mathf.Abs(cos)), cos * motionData.HorizontalAmplitude, cos * -motionData.RotationAmplitude)
                    + new Vector3(m_VerticalInfluence, 0, 0) + motionData.RotationOffset + m_JumpAnimation.Rotation + m_LandingAnimation.Rotation + m_RecoilRot
                                  + m_ExplosionRot + m_DamageRot + m_VaultAnimation.Rotation + m_LeanRot;

                    m_TargetPos = Vector3.Lerp(m_TargetPos, m_CurrentPos, Time.deltaTime * motionData.Speed * speed / motionData.Smoothness);
                    m_TargetRot = Quaternion.Slerp(m_TargetRot, Quaternion.Euler(m_CurrentRot), Time.deltaTime * motionData.Speed * speed / motionData.Smoothness);
                }
                else
                {
                    m_TargetPos = Vector3.Lerp(m_TargetPos, Vector3.zero + m_JumpAnimation.Position + m_LandingAnimation.Position + m_VaultAnimation.Position + m_RecoilPos + m_LeanPos, Time.deltaTime * 5);

                    m_TargetRot = Quaternion.Slerp(m_TargetRot, Quaternion.identity * Quaternion.Euler(m_JumpAnimation.Rotation) * Quaternion.Euler(m_LandingAnimation.Rotation)
                        * Quaternion.Euler(m_BreathingRot) * Quaternion.Euler(m_RecoilRot) * Quaternion.Euler(m_ExplosionRot) * Quaternion.Euler(m_DamageRot)
                        * Quaternion.Euler(m_VaultAnimation.Rotation) * Quaternion.Euler(m_LeanRot), Time.deltaTime * 5);

                    m_VerticalInfluence = 0;

                    m_Angle = Random.Range(0, 10) % 2 == 0 ? 0 : Mathf.PI;
                }
            }

            public void LeanAnimation (int direction)
            {
                if (m_Lean)
                {
                    if (direction == 1)
                    {
                        m_LeanPos = Vector3.Lerp(m_LeanPos, new Vector3(m_LeanAmount, 0, 0), Time.deltaTime * m_LeanSpeed);
                        m_LeanRot = Vector3.Lerp(m_LeanRot, new Vector3(0, 0, -m_LeanAngle), Time.deltaTime * m_LeanSpeed);
                    }
                    else if (direction == -1)
                    {
                        m_LeanPos = Vector3.Lerp(m_LeanPos, new Vector3(-m_LeanAmount, 0, 0), Time.deltaTime * m_LeanSpeed);
                        m_LeanRot = Vector3.Lerp(m_LeanRot, new Vector3(0, 0, m_LeanAngle), Time.deltaTime * m_LeanSpeed);
                    }
                    else
                    {
                        m_LeanPos = Vector3.Lerp(m_LeanPos, Vector3.zero, Time.deltaTime * m_LeanSpeed);
                        m_LeanRot = Vector3.Lerp(m_LeanRot, Vector3.zero, Time.deltaTime * m_LeanSpeed);
                    }
                }
                else
                {
                    m_LeanPos = Vector3.zero;
                    m_LeanRot = Vector3.zero;
                }
            }

            public IEnumerator HitAnimation ()
            {
                Vector3 initialRot = Math3DUtility.RandomInsideBounds(m_MinHitRotation, m_MaxHitRotation);

                // Make the GameObject move to target slightly
                for (float t = 0f; t <= m_HitDuration; t += Time.deltaTime)
                {
                    m_DamageRot = Vector3.Lerp(initialRot, initialRot, t / m_HitDuration);
                    yield return new WaitForFixedUpdate();
                }

                // Make it move back to neutral
                for (float t = 0f; t <= m_HitDuration; t += Time.deltaTime)
                {
                    m_DamageRot = Vector3.Lerp(initialRot, Vector3.zero, t / m_HitDuration);
                    yield return new WaitForFixedUpdate();
                }

                m_DamageRot = Vector3.zero;
            }

            public IEnumerator RecoilAnimation (bool camera)
            {
                if (camera)
                {
                    if (m_CameraRecoil)
                    {
                        Vector3 initialPos = m_RecoilPos;
                        Vector3 initialRot = Math3DUtility.RandomInsideBounds(m_MinCameraRecoilRotation, m_MaxCameraRecoilRotation);

                        // Make the GameObject move to target slightly
                        for (float t = 0f; t <= m_CameraRecoilDuration; t += Time.deltaTime)
                        {
                            m_RecoilRot = Vector3.Lerp(initialRot, initialRot, t / m_CameraRecoilDuration);
                            yield return new WaitForFixedUpdate();
                        }

                        // Make it move back to neutral
                        for (float t = 0f; t <= m_CameraReturnDuration; t += Time.deltaTime)
                        {
                            m_RecoilPos = Vector3.Lerp(initialPos, Vector3.zero, t / m_CameraReturnDuration);
                            m_RecoilRot = Vector3.Lerp(initialRot, Vector3.zero, t / m_CameraReturnDuration);
                            yield return new WaitForFixedUpdate();
                        }
                    }
                }
                else
                {
                    if (m_WeaponRecoil)
                    {
                        Vector3 initialPos = m_WeaponRecoilPosition;
                        Vector3 initialRot = m_WeaponRecoilRotation;

                        // Make the GameObject move to target slightly
                        for (float t = 0f; t <= m_WeaponRecoilDuration; t += Time.deltaTime)
                        {
                            m_RecoilPos = Vector3.Lerp(initialPos, m_WeaponRecoilPosition, t / m_WeaponRecoilDuration);
                            m_RecoilRot = Vector3.Lerp(initialRot, m_WeaponRecoilRotation, t / m_WeaponRecoilDuration);
                            yield return new WaitForFixedUpdate();
                        }

                        // Make it move back to neutral
                        for (float t = 0f; t <= m_WeaponRecoilDuration; t += Time.deltaTime)
                        {
                            m_RecoilPos = Vector3.Lerp(m_WeaponRecoilPosition, Vector3.zero, t / m_WeaponRecoilDuration);
                            m_RecoilRot = Vector3.Lerp(m_WeaponRecoilRotation, Vector3.zero, t / m_WeaponRecoilDuration);
                            yield return new WaitForFixedUpdate();
                        }
                    }
                }

                m_RecoilPos = Vector3.zero;
                m_RecoilPos = Vector3.zero;
            }

            public IEnumerator Shake (ShakeProperties prop)
            {
                //Original code written by Sebastian Lague
                //https://github.com/SebLague/Camera-Shake

                float completionPercent = 0;
                float movePercent = 0;

                float radians = prop.Angle * Mathf.Deg2Rad - Mathf.PI;
                Vector3 previousWaypoint = Vector3.zero;
                Vector3 currentWaypoint = Vector3.zero;
                float moveDistance = 0;
                float speed = 0;

                Vector3 targetRotation = Vector3.zero;
                Vector3 previousRotation = Vector3.zero;

                do
                {
                    if (movePercent >= 1 || Mathf.Abs(completionPercent) < Mathf.Epsilon)
                    {
                        float dampingFactor = DampingCurve(completionPercent, prop.DampingPercent);
                        float noiseAngle = (Random.value - 0.5f) * Mathf.PI;
                        radians += Mathf.PI + noiseAngle * prop.NoisePercent;
                        currentWaypoint = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians)) * prop.Strenght * dampingFactor;
                        previousWaypoint = m_ExplosionPos;
                        moveDistance = Vector3.Distance(currentWaypoint, previousWaypoint);

                        targetRotation = new Vector3(currentWaypoint.y, currentWaypoint.x).normalized * prop.RotationPercent * dampingFactor * k_MaxAngle;
                        previousRotation = m_ExplosionRot;

                        speed = Mathf.Lerp(prop.MinSpeed, prop.MaxSpeed, dampingFactor);

                        movePercent = 0;
                    }

                    completionPercent += Time.deltaTime / prop.Duration;
                    movePercent += Time.deltaTime / moveDistance * speed;
                    m_ExplosionPos = Vector3.Lerp(previousWaypoint, currentWaypoint, movePercent);
                    m_ExplosionRot = Vector3.Lerp(previousRotation, targetRotation, movePercent);

                    yield return null;

                } while (moveDistance > 0);
            }

            private static float DampingCurve (float x, float dampingPercent)
            {
                x = Mathf.Clamp01(x);
                float a = Mathf.Lerp(2, .25f, dampingPercent);
                float b = 1 - Mathf.Pow(x, a);
                return Mathf.Pow(b, 3);
            }
        }
    }
}

