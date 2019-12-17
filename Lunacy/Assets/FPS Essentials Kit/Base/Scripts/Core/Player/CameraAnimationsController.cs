/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections;
using Essentials.Animation;
using Essentials.Input;
using UnityEngine;

namespace Essentials
{
    namespace Controllers
    {
        [AddComponentMenu("FPS Essentials/Controllers/Camera Animations Controller"), DisallowMultipleComponent]
        public class CameraAnimationsController : MonoBehaviour
        {
            [SerializeField]
            [NotNull]
            protected FirstPersonController m_FPController; // FirstPersonController reference

            [SerializeField]
            [NotNull]
            protected HealthController m_HealthController; // HealthController reference

            // MotionAnimationComponent adds the vital functions for procedural animations
            [SerializeField]
            protected MotionAnimation m_MotionAnimation = new MotionAnimation();

            [SerializeField]
            protected AudioClip m_HoldBreath; // Sound played when holding breath to stabilize the scope

            [SerializeField]
            protected AudioClip m_Exhale; // Sound played when release breath

            [SerializeField]
            [Range(0, 1)]
            protected float m_HoldBreathVolume = 0.3f;

            public bool HoldBreath { protected get; set; }
            protected float m_NextHoldBreathTime;
            protected float m_HoldBreathDuration;
            protected bool m_HoldingBreath;

            protected int m_LeanDirection = 0;
            protected IEnumerator m_CurrentShakeCoroutine; // Explosion shake coroutine
            protected PlayerAudioSource m_PlayerGenericSource;

            public int LeanDirection { get { return m_LeanDirection; } }

            protected virtual void Start ()
            {
                m_FPController.JumpEvent += CameraJump;
                m_FPController.LandingEvent += CameraLanding;
                m_FPController.VaultEvent += Vault;

                m_HealthController.ExplosionEvent += GrenadeExplsion;
                m_HealthController.HitEvent += Hit;

                m_PlayerGenericSource = AudioManager.Instance.RegisterSource("PlayerGenericSource", AudioManager.Instance.transform);
            }

            protected virtual void Update ()
            {
                m_MotionAnimation.MovementAnimation(m_FPController);

                m_MotionAnimation.BreathingAnimation(m_FPController.IsAiming && m_HoldBreathDuration == 0 ? 1 : 0);

                if (HoldBreath)
                {
                    //Hold breath
                    if (InputManager.GetButton("Run") && m_NextHoldBreathTime < Time.time && m_FPController.IsAiming)
                    {
                        if (m_HoldBreathDuration == 0)
                            m_PlayerGenericSource.Play(m_HoldBreath, m_HoldBreathVolume);

                        m_HoldBreathDuration += Time.deltaTime;
                        if (m_HoldBreathDuration > m_HoldBreath.length)
                        {
                            m_NextHoldBreathTime = Time.time + 3 + m_HoldBreathDuration;
                            m_HoldBreathDuration = 0;
                            m_PlayerGenericSource.Play(m_Exhale, m_HoldBreathVolume);
                        }
                    }
                    //Release the breath
                    else
                    {
                        if (m_HoldBreathDuration > 0)
                            m_PlayerGenericSource.Stop();

                        if (m_HoldBreathDuration > m_HoldBreath.length * 0.7f)
                        {
                            m_NextHoldBreathTime = Time.time + 3 + m_HoldBreathDuration;
                            m_PlayerGenericSource.Play(m_Exhale, m_HoldBreathVolume);
                        }

                        m_HoldBreathDuration = 0;
                    }
                }

                if (m_MotionAnimation.Lean)
                {
                    if (m_FPController.State != MotionState.Flying && m_FPController.State != MotionState.Running)
                    {
                        if (GameplayManager.Instance.LeanStyle == GameplayManager.ActionMode.Hold)
                        {
                            if (InputManager.GetButton("Lean Left") && !InputManager.GetButton("Lean Right"))
                            {
                                if (CanLean(Vector3.left))
                                    m_LeanDirection = -1;
                                else
                                    m_LeanDirection = 0;
                            }

                            if (InputManager.GetButton("Lean Right") && !InputManager.GetButton("Lean Left"))
                            {
                                if (CanLean(Vector3.right))
                                    m_LeanDirection = 1;
                                else
                                    m_LeanDirection = 0;
                            }

                            if (!InputManager.GetButton("Lean Left") && !InputManager.GetButton("Lean Right"))
                            {
                                m_LeanDirection = 0;
                            }

                            m_MotionAnimation.LeanAnimation(m_LeanDirection);
                        }
                        else
                        {
                            if (InputManager.GetButtonDown("Lean Left") && m_LeanDirection != -1)
                            {
                                if (m_LeanDirection == 1)
                                    m_LeanDirection = 0;
                                else
                                    m_LeanDirection = -1;
                            }
                            else if (InputManager.GetButtonDown("Lean Left") && m_LeanDirection == -1)
                            {
                                m_LeanDirection = 0;
                            }

                            if (InputManager.GetButtonDown("Lean Right") && m_LeanDirection != 1)
                            {
                                if (m_LeanDirection == -1)
                                    m_LeanDirection = 0;
                                else
                                    m_LeanDirection = 1;
                            }
                            else if (InputManager.GetButtonDown("Lean Right") && m_LeanDirection == 1)
                            {
                                m_LeanDirection = 0;
                            }

                            if ((m_LeanDirection == -1 && !CanLean(Vector3.left)) || (m_LeanDirection == 1 && !CanLean(Vector3.right)))
                                m_LeanDirection = 0;

                            m_MotionAnimation.LeanAnimation(m_LeanDirection);
                        }
                    }
                    else
                    {
                        m_LeanDirection = 0;
                        m_MotionAnimation.LeanAnimation(0);
                    }
                }
            }

            protected bool CanLean (Vector3 direction)
            {
                Ray ray = new Ray(m_FPController.transform.position, m_FPController.transform.TransformDirection(direction));
                RaycastHit hitInfo;
                return !Physics.SphereCast(ray, m_MotionAnimation.LeanAmount, out hitInfo, m_MotionAnimation.LeanAmount * 2, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            }

            protected virtual void CameraJump ()
            {
                StartCoroutine(m_MotionAnimation.JumpAnimation.Play());
            }

            protected virtual void CameraLanding (float fallDamage)
            {
                StartCoroutine(m_MotionAnimation.LandingAnimation.Play());
            }

            public virtual void ApplyRecoil (bool cameraRecoil, Vector3 minCameraRecoilRotation, Vector3 maxCameraRecoilRotation, float cameraRecoilDuration, float cameraReturnDuration)
            {
                // Update camera recoil properties
                m_MotionAnimation.CameraRecoil = cameraRecoil;
                m_MotionAnimation.MinCameraRecoilRotation = minCameraRecoilRotation;
                m_MotionAnimation.MaxCameraRecoilRotation = maxCameraRecoilRotation;
                m_MotionAnimation.CameraRecoilDuration = cameraRecoilDuration;
                m_MotionAnimation.CameraReturnDuration = cameraReturnDuration;

                StopCoroutine(m_MotionAnimation.RecoilAnimation(true));
                StartCoroutine(m_MotionAnimation.RecoilAnimation(true));
            }

            protected virtual void GrenadeExplsion ()
            {
                if (m_CurrentShakeCoroutine != null)
                {
                    StopCoroutine(m_CurrentShakeCoroutine);
                }

                m_CurrentShakeCoroutine = m_MotionAnimation.Shake(m_MotionAnimation.GrenadeExplosionShake);
                StartCoroutine(m_CurrentShakeCoroutine);
            }

            protected virtual void Hit ()
            {
                StartCoroutine(m_MotionAnimation.HitAnimation());
            }

            protected virtual void Vault ()
            {
                StartCoroutine(m_MotionAnimation.VaultAnimation.Play());
            }
        }
    }
}
