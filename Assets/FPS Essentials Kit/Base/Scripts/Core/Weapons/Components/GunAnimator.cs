/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections.Generic;
using UnityEngine;

namespace Essentials
{
    namespace Weapons
    {
        [System.Serializable]
        public sealed class GunAnimator
        {
            public enum AnimationType
            {
                Sequential,
                Random
            }

            #region RUNNING

            [SerializeField]
            private bool m_RunAnimation = false;

            [SerializeField]
            private Vector3 m_RunningPosition;

            [SerializeField]
            private Vector3 m_RunningRotation;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_RunningSpeed = 10;

            #endregion

            #region AIMING

            [SerializeField]
            private bool m_AimAnimation = false;

            [SerializeField]
            private Vector3 m_AimingPosition;

            [SerializeField]
            private Vector3 m_AimingRotation;

            [SerializeField]
            private AudioClip m_AimInSound;

            [SerializeField]
            private AudioClip m_AimOutSound;

            [SerializeField]
            private bool m_ZoomAnimation = false;

            [SerializeField]
            [Range(1, 179)]
            private float m_AimFOV = 50;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_AimingSpeed = 5;

            [SerializeField]
            private bool m_HoldBreath = false;

            #endregion

            [SerializeField]
            [NotNull]
            private Animator m_Animator;

            [SerializeField]
            private string m_SpeedParameter = "Speed";

            #region DRAW

            [SerializeField]
            private bool m_Draw = false;

            [SerializeField]
            private string m_DrawAnimation = "Draw";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_DrawSpeed = 1;

            [SerializeField]
            private AudioClip m_DrawSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_DrawVolume = 0.25f;

            #endregion

            #region HIDE

            [SerializeField]
            private bool m_Hide = false;

            [SerializeField]
            private string m_HideAnimation = "Hide";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_HideSpeed = 1;

            [SerializeField]
            private AudioClip m_HideSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_HideVolume = 0.25f;

            #endregion

            #region FIRE
            [SerializeField]
            private bool m_Fire = false;

            [SerializeField]
            private List<string> m_FireAnimationList = new List<string>();

            [SerializeField]
            private List<string> m_AimedFireAnimationList = new List<string>();

            [SerializeField]
            private AnimationType m_FireAnimationType = AnimationType.Sequential;

            [SerializeField]
            private List<AudioClip> m_FireSoundList = new List<AudioClip>();

            [SerializeField]
            private bool m_OverrideLastFire;

            [SerializeField]
            private string m_LastFireAnimation = "Last Fire";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_FireSpeed = 1;

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_AimedFireSpeed = 1;

            [SerializeField]
            private AudioClip m_OutOfAmmoSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_FireVolume = 0.5f;

            private int m_LastIndex;

            #endregion

            #region RELOAD

            [SerializeField]
            private bool m_Reload = false;

            [SerializeField]
            private string m_ReloadAnimation = "Reload";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_ReloadSpeed = 1;

            [SerializeField]
            private AudioClip m_ReloadSound;

            [SerializeField]
            private string m_ReloadEmptyAnimation = "ReloadEmpty";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_ReloadEmptySpeed = 1;

            [SerializeField]
            private AudioClip m_ReloadEmptySound;

            [SerializeField]
            [Range(0, 1)]
            private float m_ReloadVolume = 0.25f;

            [SerializeField]
            private string m_StartReloadAnimation = "Start Reload";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_StartReloadSpeed = 1;

            [SerializeField]
            private AudioClip m_StartReloadSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_StartReloadVolume = 0.25f;

            [SerializeField]
            private string m_InsertInChamberAnimation = "Insert Chamber";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_InsertInChamberSpeed = 1;

            [SerializeField]
            private AudioClip m_InsertInChamberSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_InsertInChamberVolume = 0.25f;

            [SerializeField]
            private string m_InsertAnimation = "Insert Reload";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_InsertSpeed = 1;

            [SerializeField]
            private AudioClip m_InsertSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_InsertVolume = 0.25f;

            [SerializeField]
            private string m_StopReloadAnimation = "Stop Reload";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_StopReloadSpeed = 1;

            [SerializeField]
            private AudioClip m_StopReloadSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_StopReloadVolume = 0.25f;

            #endregion

            #region MELEE

            [SerializeField]
            private bool m_Melee = false;

            [SerializeField]
            private string m_MeleeAnimation = "Melee";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_MeleeSpeed = 1;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_MeleeDelay = 0.1f;

            [SerializeField]
            private AudioClip m_MeleeSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_MeleeVolume = 0.2f;

            [SerializeField]
            private List<AudioClip> m_HitSoundList = new List<AudioClip>();

            [SerializeField]
            [Range(0, 1)]
            private float m_HitVolume = 0.3f;

            #endregion

            #region SWITCH MODE

            [SerializeField]
            private bool m_SwitchMode = false;

            [SerializeField]
            private string m_SwitchModeAnimation = "SwitchMode";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_SwitchModeSpeed = 1;

            [SerializeField]
            private AudioClip m_SwitchModeSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_SwitchModeVolume = 0.2f;

            #endregion

            #region INTERACT

            [SerializeField]
            private bool m_Interact = false;

            [SerializeField]
            private string m_InteractAnimation = "Interact";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_InteractSpeed = 1;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_InteractDelay = 0.25f;

            [SerializeField]
            private AudioClip m_InteractSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_InteractVolume = 0.2f;

            #endregion

            #region VAULT

            [SerializeField]
            private bool m_Vault = false;

            [SerializeField]
            private string m_VaultAnimation = "Vault";

            [SerializeField]
            [MinMax(0.001f, Mathf.Infinity)]
            private float m_VaultSpeed = 1;

            [SerializeField]
            private AudioClip m_VaultSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_VaultVolume = 0.2f;

            #endregion

            private Camera m_Camera;
            private Transform m_TargetTransform;

            private Vector3 m_TargetPos;
            private Quaternion m_TargetRot;

            private Vector3 m_HIPPosition;
            private Vector3 m_HIPRotation;

            private PlayerAudioSource m_PlayerBodySource;
            private PlayerAudioSource m_PlayerWeaponSource;
            private PlayerAudioSource m_PlayerWeaponGenericSource;

            #region PROPERTIES

            public bool IsAiming
            {
                get
                {
                    if (m_AimAnimation && (m_AimingPosition == m_HIPPosition && m_AimingRotation == m_HIPRotation))
                    {
                        Debug.LogError("Aiming Position/Rotation must be different from the initial position of the weapon. " +
                            "Disable the animation if you do not need to use.");
                        return false;
                    }

                    if (m_ZoomAnimation && (Mathf.Abs(m_AimFOV - GameplayManager.Instance.FieldOfView) < Mathf.Epsilon))
                    {
                        Debug.LogError("Aim FOV must be different from the initial camera FOV. " +
                            "Disable the animation if you do not need to use.");
                        return false;
                    }

                    bool position = (m_AimingPosition - m_TargetTransform.localPosition).sqrMagnitude < 0.001f;
                    bool rotation = (m_AimingRotation - m_TargetTransform.localRotation.eulerAngles).sqrMagnitude < 0.001f;
                    bool zoom = Mathf.Abs((m_AimFOV - m_Camera.fieldOfView)) < 0.1f;

                    return (position && rotation) || zoom;
                }
            }

            public bool CanHoldBreath { get { return m_HoldBreath; } }
            public bool CanMeleeAttack { get { return m_Melee; } }

            public float DrawAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Draw)
                        return 0;

                    return m_DrawAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_DrawAnimation).length / m_DrawSpeed;

                }
            }

            public float HideAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Hide)
                        return 0;

                    return m_HideAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_HideAnimation).length / m_HideSpeed;

                }
            }

            public float ReloadAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Reload)
                        return 0;

                    return m_ReloadAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_ReloadAnimation).length / m_ReloadSpeed;
                }
            }

            public float CompleteReloadAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Reload)
                        return 0;

                    return m_ReloadEmptyAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_ReloadEmptyAnimation).length / m_ReloadEmptySpeed;

                }
            }

            public float StartReloadAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Reload)
                        return 0;

                    return m_StartReloadAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_StartReloadAnimation).length / m_StartReloadSpeed;
                }
            }

            public float InsertInChamberAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Reload)
                        return 0;

                    return m_InsertInChamberAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_InsertInChamberAnimation).length / m_InsertInChamberSpeed;
                }
            }

            public float InsertAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Reload)
                        return 0;

                    return m_InsertAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_InsertAnimation).length / m_InsertSpeed;
                }
            }

            public float StopReloadAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Reload)
                        return 0;

                    return m_StopReloadAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_StopReloadAnimation).length / m_StopReloadSpeed;
                }
            }

            public float MeleeAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Melee)
                        return 0;

                    return m_MeleeAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_MeleeAnimation).length / m_MeleeSpeed;
                }
            }

            public float MeleeDelay
            {
                get
                {
                    if (!m_Melee)
                        return 0;

                    return m_MeleeDelay > MeleeAnimationLength ? MeleeAnimationLength : m_MeleeDelay;
                }
            }

            public float SwitchModeAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_SwitchMode)
                        return 0;

                    return m_SwitchModeAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_SwitchModeAnimation).length / m_SwitchModeSpeed;
                }
            }

            public float InteractAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Interact)
                        return 0;

                    return m_InteractAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_InteractAnimation).length / m_InteractSpeed;

                }
            }

            public float InteractDelay
            {
                get
                {
                    if (!m_Interact)
                        return 0;

                    return m_InteractDelay;

                }
            }

            #endregion

            #region WEAPON CUSTOMIZATION

            internal void UpdateAiming (Vector3 aimingPosition, Vector3 aimingRotation, bool zoomAnimation = false, float aimFOV = 50)
            {
                m_AimingPosition = aimingPosition;
                m_AimingRotation = aimingRotation;
                m_ZoomAnimation = zoomAnimation;
                m_AimFOV = aimFOV;
            }

            internal void UpdateFireSound (AudioClip[] fireSoundList)
            {
                m_FireSoundList.Clear();
                m_FireSoundList.AddRange(fireSoundList);
            }

            #endregion

            internal void Init (Transform targetTransform, Camera camera)
            {
                m_Camera = camera;
                m_TargetTransform = targetTransform;
                m_HIPPosition = targetTransform.localPosition;
                m_HIPRotation = targetTransform.localRotation.eulerAngles;

                m_PlayerBodySource = AudioManager.Instance.RegisterSource("PlayerBodySource", AudioManager.Instance.transform, spatialBlend: 0);
                m_PlayerWeaponGenericSource = AudioManager.Instance.RegisterSource("PlayerWeaponGenericSource", AudioManager.Instance.transform, spatialBlend: 0);
                m_PlayerWeaponSource = AudioManager.Instance.RegisterSource("PlayerWeaponSource", AudioManager.Instance.transform, AudioCategory.SFx, 5, 15, 0);
            }

            internal void Aim (bool canAim)
            {
                if (canAim)
                {
                    if (!IsAiming)
                        m_PlayerBodySource.Play(m_AimInSound, 0.1f);

                    if (m_AimAnimation)
                    {
                        m_TargetPos = Vector3.Lerp(m_TargetPos, m_AimingPosition, Time.deltaTime * m_AimingSpeed);
                        m_TargetRot = Quaternion.Lerp(m_TargetRot, Quaternion.Euler(m_AimingRotation), Time.deltaTime * m_AimingSpeed);
                    }

                    if (m_ZoomAnimation)
                        m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_AimFOV, Time.deltaTime * m_AimingSpeed);
                }
                else //Stop Sprint Animation
                {
                    m_TargetPos = Vector3.Lerp(m_TargetPos, m_HIPPosition, Time.deltaTime * m_AimingSpeed);
                    m_TargetRot = Quaternion.Lerp(m_TargetRot, Quaternion.Euler(m_HIPRotation), Time.deltaTime * m_AimingSpeed);
                    m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, GameplayManager.Instance.FieldOfView, Time.deltaTime * m_AimingSpeed);
                }

                PerformAnimation();
            }

            internal void Sprint (bool canRun)
            {
                if (canRun && m_RunAnimation)
                {
                    m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, GameplayManager.Instance.FieldOfView, Time.deltaTime * m_AimingSpeed);
                    m_TargetPos = Vector3.Lerp(m_TargetPos, m_RunningPosition, Time.deltaTime * m_RunningSpeed);
                    m_TargetRot = Quaternion.Lerp(m_TargetRot, Quaternion.Euler(m_RunningRotation), Time.deltaTime * m_RunningSpeed);
                }
                else //Stop Aiming Animation
                {
                    m_TargetPos = Vector3.Lerp(m_TargetPos, m_HIPPosition, Time.deltaTime * m_RunningSpeed);
                    m_TargetRot = Quaternion.Lerp(m_TargetRot, Quaternion.Euler(m_HIPRotation), Time.deltaTime * m_RunningSpeed);
                    m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, GameplayManager.Instance.FieldOfView, Time.deltaTime * m_AimingSpeed);

                    if (IsAiming)
                        m_PlayerBodySource.Play(m_AimOutSound, 0.1f);
                }

                PerformAnimation();
            }

            private void PerformAnimation ()
            {
                m_TargetTransform.localPosition = m_TargetPos;
                m_TargetTransform.localRotation = m_TargetRot;
            }

            internal void Draw ()
            {
                if (m_Draw)
                {
                    if (m_Animator == null)
                        return;

                    m_Animator.SetFloat(m_SpeedParameter, m_DrawSpeed);

                    if (m_DrawAnimation.Length == 0)
                        return;

                    m_Animator.Play(m_DrawAnimation);

                    if (m_PlayerBodySource == null)
                        m_PlayerBodySource = AudioManager.Instance.RegisterSource("PlayerBodySource", AudioManager.Instance.transform, spatialBlend: 0);

                    m_PlayerBodySource.ForcePlay(m_DrawSound, m_DrawVolume);
                }
            }

            internal void Hide ()
            {
                if (m_Hide)
                {
                    if (m_Animator == null)
                        return;

                    m_Animator.SetFloat(m_SpeedParameter, m_HideSpeed);

                    if (m_HideAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_HideAnimation, 0.1f);
                    m_PlayerWeaponSource.Stop();
                    m_PlayerWeaponGenericSource.Stop();
                    m_PlayerBodySource.ForcePlay(m_HideSound, m_HideVolume);
                }
            }

            internal void Shot (bool lastBullet)
            {
                if (m_Fire)
                {
                    if (m_Animator == null)
                        return;

                    if (m_OverrideLastFire && lastBullet)
                    {
                        m_Animator.SetFloat(m_SpeedParameter, m_FireSpeed);

                        if (m_LastFireAnimation.Length > 0)
                            m_Animator.CrossFadeInFixedTime(m_LastFireAnimation, 0.1f);
                    }
                    else
                    {
                        if (IsAiming)
                        {
                            if (m_AimedFireAnimationList.Count > 0)
                            {
                                m_Animator.SetFloat(m_SpeedParameter, m_AimedFireSpeed);

                                if (m_FireAnimationType == AnimationType.Sequential)
                                {
                                    if (m_LastIndex == m_AimedFireAnimationList.Count || m_LastIndex > m_AimedFireAnimationList.Count)
                                        m_LastIndex = 0;

                                    m_Animator.CrossFadeInFixedTime(m_AimedFireAnimationList[m_LastIndex++], 0.1f);
                                }
                                else
                                {
                                    m_Animator.CrossFadeInFixedTime(m_AimedFireAnimationList[Random.Range(0, m_AimedFireAnimationList.Count)], 0.1f);
                                }
                            }
                        }
                        else
                        {
                            if (m_FireAnimationList.Count > 0)
                            {
                                m_Animator.SetFloat(m_SpeedParameter, m_FireSpeed);

                                if (m_FireAnimationType == AnimationType.Sequential)
                                {
                                    if (m_LastIndex == m_FireAnimationList.Count || m_LastIndex > m_FireAnimationList.Count)
                                        m_LastIndex = 0;

                                    m_Animator.CrossFadeInFixedTime(m_FireAnimationList[m_LastIndex++], 0.1f);
                                }
                                else
                                {
                                    m_Animator.CrossFadeInFixedTime(m_FireAnimationList[Random.Range(0, m_FireAnimationList.Count)], 0.1f);
                                }
                            }
                        }
                    }

                    if (m_FireSoundList.Count > 0)
                    {
                        if (m_FireSoundList.Count > 1)
                        {
                            int i = Random.Range(1, m_FireSoundList.Count);
                            AudioClip a = m_FireSoundList[i];

                            m_FireSoundList[i] = m_FireSoundList[0];
                            m_FireSoundList[0] = a;

                            m_PlayerWeaponSource.ForcePlay(a, m_FireVolume);
                        }
                        else
                        {
                            m_PlayerWeaponSource.ForcePlay(m_FireSoundList[0], m_FireVolume);
                        }
                    }
                }
            }

            internal void OutOfAmmo ()
            {
                m_PlayerBodySource.Play(m_OutOfAmmoSound, m_FireVolume);
            }

            internal void Reload (bool outOfAmmo)
            {
                if (m_Reload)
                {
                    if (m_Animator == null)
                        return;

                    if (outOfAmmo)
                    {
                        m_Animator.SetFloat(m_SpeedParameter, m_ReloadEmptySpeed);

                        if (m_ReloadEmptyAnimation.Length == 0)
                            return;

                        m_Animator.CrossFadeInFixedTime(m_ReloadEmptyAnimation, 0.1f);
                        m_PlayerWeaponGenericSource.ForcePlay(m_ReloadEmptySound, m_ReloadVolume);
                    }
                    else
                    {
                        m_Animator.SetFloat(m_SpeedParameter, m_ReloadSpeed);

                        if (m_ReloadAnimation.Length == 0)
                            return;

                        m_Animator.CrossFadeInFixedTime(m_ReloadAnimation, 0.1f);
                        m_PlayerWeaponGenericSource.ForcePlay(m_ReloadSound, m_ReloadVolume);
                    }
                }
            }

            internal void StartReload (bool outOfAmmo)
            {
                if (m_Reload)
                {
                    if (m_Animator == null)
                        return;

                    if (outOfAmmo)
                    {
                        m_Animator.SetFloat(m_SpeedParameter, m_InsertInChamberSpeed);

                        if (m_InsertInChamberAnimation.Length == 0)
                            return;

                        m_Animator.CrossFadeInFixedTime(m_InsertInChamberAnimation, 0.1f);
                        m_PlayerWeaponSource.Stop();
                        m_PlayerWeaponGenericSource.ForcePlay(m_InsertInChamberSound, m_InsertInChamberVolume);
                    }
                    else
                    {
                        m_Animator.SetFloat(m_SpeedParameter, m_StartReloadSpeed);

                        if (m_StartReloadAnimation.Length == 0)
                            return;

                        m_Animator.CrossFadeInFixedTime(m_StartReloadAnimation, 0.1f);
                        m_PlayerWeaponSource.Stop();
                        m_PlayerWeaponGenericSource.ForcePlay(m_StartReloadSound, m_StartReloadVolume);
                    }
                }
            }

            internal void Insert ()
            {
                if (m_Reload)
                {
                    if (m_Animator == null)
                        return;

                    m_Animator.SetFloat(m_SpeedParameter, m_InsertSpeed);

                    if (m_InsertAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_InsertAnimation, 0.1f);
                    m_PlayerWeaponGenericSource.ForcePlay(m_InsertSound, m_InsertVolume);
                }
            }

            internal void StopReload ()
            {
                if (m_Reload)
                {
                    if (m_Animator == null)
                        return;

                    m_Animator.SetFloat(m_SpeedParameter, m_StopReloadSpeed);

                    if (m_StopReloadAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_StopReloadAnimation, 0.1f);
                    m_PlayerWeaponGenericSource.ForcePlay(m_StopReloadSound, m_StopReloadVolume);
                }
            }

            internal void Melee ()
            {
                if (m_Melee)
                {
                    if (m_Animator == null)
                        return;

                    m_Animator.SetFloat(m_SpeedParameter, m_MeleeSpeed);

                    if (m_MeleeAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_MeleeAnimation, 0.1f);
                    m_PlayerBodySource.ForcePlay(m_MeleeSound, m_MeleeVolume);
                }
            }

            internal void Hit (Vector3 position)
            {
                if (m_HitSoundList.Count > 0)
                    AudioManager.Instance.PlayClipAtPoint(m_HitSoundList[Random.Range(0, m_HitSoundList.Count)], position, 3, 10, m_HitVolume);
            }

            internal void SwitchMode ()
            {
                if (m_SwitchMode)
                {
                    if (m_Animator == null)
                        return;

                    m_Animator.SetFloat(m_SpeedParameter, m_SwitchModeSpeed);

                    if (m_SwitchModeAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_SwitchModeAnimation, 0.1f);
                    m_PlayerBodySource.ForcePlay(m_SwitchModeSound, m_SwitchModeVolume);
                }
            }

            internal void Interact ()
            {
                if (m_Interact)
                {
                    if (m_Animator == null)
                        return;

                    m_Animator.SetFloat(m_SpeedParameter, m_InteractSpeed);

                    if (m_InteractAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_InteractAnimation, 0.1f);
                    m_PlayerBodySource.ForcePlay(m_InteractSound, m_InteractVolume);
                }
            }

            internal void Vault ()
            {
                if (m_Vault)
                {
                    if (m_Animator == null)
                        return;

                    m_Animator.SetFloat(m_SpeedParameter, m_VaultSpeed);

                    if (m_VaultAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_VaultAnimation, 0.1f);
                    m_PlayerBodySource.ForcePlay(m_VaultSound, m_VaultVolume);
                }
            }

            #region EDITOR HELPER METHODS

            internal void AddAimedShotAnimation ()
            {
                m_AimedFireAnimationList.Add("Animation Name");
            }

            internal void RemoveAimedShotAnimation (int index)
            {
                m_AimedFireAnimationList.RemoveAt(index);
            }

            internal void AddShotAnimation ()
            {
                m_FireAnimationList.Add("Animation Name");
            }

            internal void RemoveShotAnimation (int index)
            {
                m_FireAnimationList.RemoveAt(index);
            }

            internal void AddShotSound ()
            {
                m_FireSoundList.Add(null);
            }

            internal void RemoveShotSound (int index)
            {
                m_FireSoundList.RemoveAt(index);
            }

            public void AddHitSound ()
            {
                m_HitSoundList.Add(null);
            }

            public void RemoveHitSound (int index)
            {
                m_HitSoundList.RemoveAt(index);
            }

            #endregion
        }
    }
}

