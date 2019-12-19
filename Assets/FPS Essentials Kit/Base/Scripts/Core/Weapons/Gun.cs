/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System;
using System.Collections;
using Essentials.Animation;
using Essentials.Controllers;
using Essentials.Input;
using UnityEngine;

namespace Essentials
{
    namespace Weapons
    {
        [AddComponentMenu("FPS Essentials/Weapon/Gun"), DisallowMultipleComponent]
        public class Gun : MonoBehaviour, IWeapon
        {
            [SerializeField]
            [NotNull]
            protected GunData m_GunData;

            [SerializeField]
            [NotNull]
            protected BulletMarkManager m_BulletMarkManager;

            [SerializeField]
            [NotNull]
            protected Transform m_CameraTransformReference; // FPS Camera

            [SerializeField]
            [NotNull]
            protected FirstPersonController m_FPController;

            [SerializeField]
            [NotNull]
            protected CameraAnimationsController m_CameraAnimationsController;

            [SerializeField]
            protected WeaponSwing m_WeaponSwing = new WeaponSwing();

            [SerializeField]
            protected MotionAnimation m_MotionAnimation = new MotionAnimation();

            [SerializeField]
            protected GunAnimator m_GunAnimator = new GunAnimator();

            [SerializeField]
            protected GunEffects m_GunEffects = new GunEffects();

            protected bool m_GunActive;
            protected bool m_Aiming;
            protected bool m_IsReloading;
            protected bool m_Attacking;

            protected WaitForSeconds m_ReloadDuration;
            protected WaitForSeconds m_CompleteReloadDuration;

            protected WaitForSeconds m_StartReloadDuration;
            protected WaitForSeconds m_InsertInChamberDuration;
            protected WaitForSeconds m_InsertDuration;
            protected WaitForSeconds m_StopReloadDuration;

            protected float m_FireInterval;
            protected float m_NextFireTime = 0;
            protected float m_NextReloadTime = 0;
            protected float m_NextSwitchModeTime = 0;
            protected float m_NextInteractTime = 0;
            protected float m_Accuracy = 0;

            protected Camera m_Camera;
            protected float m_IsShooting = 0;
            protected Vector3 m_NextShootDirection;

            #region EDITOR

            public GunData.ReloadMode ReloadType { get { return m_GunData != null ? m_GunData.ReloadType : GunData.ReloadMode.Magazines; } }
            public bool HasSecondaryMode { get { return m_GunData != null ? m_GunData.SecondaryFireMode != GunData.FireMode.None : false; } }
            public bool HasChamber { get { return m_GunData != null ? m_GunData.HasChamber : false; } }
            public string InspectorName { get { return m_GunData != null ? m_GunData.GunName : "No Name"; } }

            #endregion

            #region GUN PROPERTIES

            public virtual bool CanSwitch
            {
                get
                {
                    if (m_FPController == null)
                        return false;

                    return m_GunActive && !m_Aiming && m_FPController.State != MotionState.Running && m_NextSwitchModeTime < Time.time && !m_Attacking
                    && m_NextInteractTime < Time.time;
                }
            }

            public virtual bool CanUseItems
            {
                get
                {
                    if (m_FPController == null)
                        return false;

                    return m_GunActive && !m_Aiming && !m_IsReloading && m_NextReloadTime < Time.time && m_FPController.State != MotionState.Running
                    && m_NextFireTime < Time.time && m_NextSwitchModeTime < Time.time && !m_Attacking && m_NextInteractTime < Time.time;
                }
            }

            public virtual bool CanVault
            {
                get
                {
                    if (m_FPController == null)
                        return false;

                    return m_GunActive && !m_Aiming && !m_IsReloading && m_NextReloadTime < Time.time && m_NextFireTime < Time.time 
                    && m_NextSwitchModeTime < Time.time && !m_Attacking && m_NextInteractTime < Time.time;
                }
            }

            public virtual bool IsAiming
            {
                get { return m_Aiming; }
            }

            protected int RoundsPerMagazine { get; set; }
            public int CurrentRounds { get; protected set; }
            public int Magazines { get; protected set; }

            //Properties
            public int Identifier
            {
                get { return m_GunData != null ? m_GunData.GetInstanceID() : -1; }
            }

            public string GunName
            {
                get { return m_GunData != null ? m_GunData.GunName : "No Name"; }
            }

            public float Accuracy
            {
                get { return m_Accuracy; }
            }

            public GunData.FireMode FireMode
            {
                get;
                protected set;
            }

            public Sprite Icon
            {
                get { return m_GunData != null ? m_GunData.Icon : null; }
            }

            public GameObject Viewmodel
            {
                get { return gameObject; }
            }

            public GameObject DroppablePrefab
            {
                get { return m_GunData != null ? m_GunData.DroppablePrefab : null; }
            }
            
            public float Weight
            {
                get { return m_GunData != null ? m_GunData.Weight : 0; }
            }

            public float Size
            {
                get { return m_GunData != null ? m_GunData.Size : 0; }
            }

            public float HideAnimationLength { get { return m_GunAnimator.HideAnimationLength; } }
            public float InteractAnimationLength { get { return m_GunAnimator.InteractAnimationLength; } }
            public float InteractDelay { get { return m_GunAnimator.InteractDelay; } }

            #endregion

            protected virtual void Awake ()
            {
                if (m_GunData == null)
                {
                    throw new Exception("Gun Controller was not assigned");
                }

                if (m_CameraTransformReference == null)
                {
                    throw new Exception("Camera Transform Reference was not assigned");
                }

                if (m_CameraAnimationsController == null)
                {
                    throw new Exception("Camera Animations Controller was not assigned");
                }

                if (m_BulletMarkManager == null)
                {
                    throw new Exception("Bullet Mark Manager was not assigned");
                }

                if (m_FPController == null)
                {
                    throw new Exception("FPController was not assigned");
                }
            }

            protected virtual void Start ()
            {
                m_Camera = m_CameraTransformReference.GetComponent<Camera>();

                FireMode = m_GunData.PrimaryFireMode;
                m_FireInterval = m_GunData.PrimaryRateOfFire;
                RoundsPerMagazine = m_GunData.RoundsPerMagazine;

                if (CurrentRounds == 0 && Magazines == 0)
                    SetAmmo(m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine, (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine) * m_GunData.InitialMagazines);

                if (m_MotionAnimation.Lean)
                    m_MotionAnimation.LeanAmount = 0;

                m_GunAnimator.Init(transform, m_Camera);

                if (m_GunData.ReloadType == GunData.ReloadMode.Magazines)
                {
                    m_ReloadDuration = new WaitForSeconds(m_GunAnimator.ReloadAnimationLength);
                    m_CompleteReloadDuration = new WaitForSeconds(m_GunAnimator.CompleteReloadAnimationLength);
                }
                else if (m_GunData.ReloadType == GunData.ReloadMode.BulletByBullet)
                {
                    m_StartReloadDuration = new WaitForSeconds(m_GunAnimator.StartReloadAnimationLength);
                    m_InsertInChamberDuration = new WaitForSeconds(m_GunAnimator.InsertInChamberAnimationLength / 2);
                    m_InsertDuration = new WaitForSeconds(m_GunAnimator.InsertAnimationLength / 2);
                    m_StopReloadDuration = new WaitForSeconds(m_GunAnimator.StopReloadAnimationLength);
                }

                InitSwing(transform);
                SetWeaponViewModel();
                DisableShadowCasting();

                m_FPController.JumpEvent += WeaponJump;
                m_FPController.LandingEvent += WeaponLanding;
                m_FPController.VaultEvent += Vault;
            }

            protected virtual void Update ()
            {
                if (m_GunActive)
                {
                    m_CameraAnimationsController.HoldBreath = m_GunAnimator.CanHoldBreath;
                    m_FPController.IsAiming = m_GunAnimator.IsAiming;
                    m_FPController.CanVault = CanVault;
                    m_IsShooting = Mathf.MoveTowards(m_IsShooting, 0, Time.deltaTime);

                    if (m_FPController.Controllable)
                        HandleInput();
                }

                if (m_Aiming)
                {
                    m_GunAnimator.Aim(true);
                }
                else
                {
                    bool canSprint = m_FPController.State == MotionState.Running && !m_IsReloading && m_NextReloadTime < Time.time
                                     && m_NextSwitchModeTime < Time.time && m_NextFireTime < Time.time && m_GunActive && !m_Attacking
                                     && m_NextInteractTime < Time.time;
                    m_GunAnimator.Sprint(canSprint);
                }

                m_Accuracy = Mathf.Clamp(Mathf.MoveTowards(m_Accuracy, GetCurrentAccuracy(), Time.deltaTime *
                                        (m_IsShooting > 0 ? m_GunData.DecreaseRateByShooting : m_GunData.DecreaseRateByWalking)),
                                        m_GunData.MinimumAccuracy, m_GunData.AIMAccuracy);

                bool canLean = !m_Attacking &&  m_FPController.State != MotionState.Running && m_NextInteractTime < Time.time
                                            && m_CameraAnimationsController != null && m_MotionAnimation.Lean;

                if (canLean)
                {
                    m_MotionAnimation.LeanAnimation(m_CameraAnimationsController.LeanDirection);
                }
                else
                {
                    m_MotionAnimation.LeanAnimation(0);
                }

                m_WeaponSwing.Swing(transform.parent, m_FPController);
                m_MotionAnimation.MovementAnimation(m_FPController);
                m_MotionAnimation.BreathingAnimation(m_FPController.IsAiming ? 0 : 1);
            }

            protected virtual float GetCurrentAccuracy ()
            {
                if (m_GunAnimator.IsAiming)
                {
                    if (m_IsShooting > 0)
                        return m_GunData.HIPAccuracy;

                    return m_FPController.State != MotionState.Idle ? m_GunData.HIPAccuracy : m_GunData.AIMAccuracy;
                }
                if (m_IsShooting > 0)
                    return m_GunData.MinimumAccuracy;

                return m_FPController.State != MotionState.Idle ? m_GunData.MinimumAccuracy : m_GunData.HIPAccuracy;
            }

            public virtual void Select ()
            {
                m_GunAnimator.Draw();
                StartCoroutine(Draw());
            }

            protected virtual IEnumerator Draw ()
            {
                yield return new WaitForSeconds(m_GunAnimator.DrawAnimationLength);
                m_GunActive = true;
            }

            public virtual void Deselect ()
            {
                m_GunActive = false;
                m_Aiming = false;
                m_IsReloading = false;
                m_NextReloadTime = 0;
                m_FPController.IsAiming = false;
                m_FPController.CanVault = false;
                m_IsShooting = 0;
                m_GunAnimator.Hide();
            }

            protected virtual void HandleInput ()
            {
                // Restrictions:
                // Is firing = m_NextFireTime > Time.time
                // Is reloading = m_IsReloading || m_NextReloadTime > Time.time
                // Is empty = CurrentRounds == 0
                // Is running = m_FPController.State == MotionState.Running
                // Is attacking = m_Attacking
                // Is switching mode = m_NextSwitchModeTime > Time.time
                // Is interacting = m_NextInteractTime > Time.time
                // Can reload = Magazines > 0

                bool canShoot = !m_IsReloading && m_NextReloadTime < Time.time && m_NextFireTime < Time.time && CurrentRounds >= 0
                                && m_FPController.State != MotionState.Running && !m_Attacking && m_NextSwitchModeTime < Time.time && m_NextInteractTime < Time.time;

                if (canShoot)
                {
                    if (FireMode == GunData.FireMode.FullAuto || FireMode == GunData.FireMode.ShotgunAuto)
                    {
                        if (InputManager.GetButton("Fire"))
                        {
                            if (CurrentRounds == 0 && Magazines > 0)
                            {
                                Reload();
                            }
                            else
                            {
                                PullTheTrigger();
                            }
                        }
                    }
                    else if (FireMode == GunData.FireMode.Single || FireMode == GunData.FireMode.ShotgunSingle || FireMode == GunData.FireMode.Burst)
                    {
                        if (InputManager.GetButtonDown("Fire"))
                        {
                            if (CurrentRounds == 0 && Magazines > 0)
                            {
                                Reload();
                            }
                            else
                            {
                                PullTheTrigger();
                            }
                        }
                    }
                }

                if (m_GunData.ReloadType == GunData.ReloadMode.BulletByBullet && m_IsReloading && m_NextReloadTime < Time.time && CurrentRounds > (m_GunData.HasChamber ? 1 : 0))
                {
                    if (InputManager.GetButtonDown("Fire"))
                    {
                        m_IsReloading = false;
                        StartCoroutine(StopReload());
                    }
                }

                bool canAim = !m_IsReloading && m_NextReloadTime < Time.time && m_FPController.State != MotionState.Running && !m_Attacking && m_NextInteractTime < Time.time;

                if (canAim)
                {
                    if (GameplayManager.Instance.AimStyle == GameplayManager.ActionMode.Press)
                    {
                        if (InputManager.GetButtonDown("Aim") && !m_Aiming)
                        {
                            m_Aiming = !m_Aiming;
                        }
                        else if (InputManager.GetButtonDown("Aim") && m_Aiming && m_GunAnimator.IsAiming)
                        {
                            m_Aiming = !m_Aiming;
                        }
                    }
                    else
                    {
                        m_Aiming = InputManager.GetButton("Aim");
                    }
                }
                else
                {
                    m_Aiming = false;
                }

                bool canReload = !m_IsReloading && m_NextReloadTime < Time.time && CurrentRounds < (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine) && Magazines > 0
                                 && m_FPController.State != MotionState.Running && !m_Attacking && m_NextSwitchModeTime < Time.time && m_NextInteractTime < Time.time
                                 && m_NextFireTime < Time.time;

                if (canReload)
                {
                    if (InputManager.GetButtonDown("Reload"))
                    {
                        Reload();
                    }
                }

                bool canAttack = !m_Attacking && !m_IsReloading && m_NextReloadTime < Time.time && m_FPController.State != MotionState.Running && !m_Aiming
                                 && m_NextFireTime < Time.time && m_GunAnimator.CanMeleeAttack && m_NextSwitchModeTime < Time.time && m_NextInteractTime < Time.time;

                if (canAttack)
                {
                    if (InputManager.GetButtonDown("Melee"))
                    {
                        StartCoroutine(MeleeAttack());
                    }
                }

                bool canChangeFireMode = HasSecondaryMode && !m_Attacking && !m_IsReloading
                                         && m_NextReloadTime < Time.time && m_FPController.State != MotionState.Running && m_NextSwitchModeTime < Time.time
                                         && m_NextInteractTime < Time.time;

                if (canChangeFireMode)
                {
                    if (InputManager.GetButtonDown("Fire Mode"))
                    {
                        if (FireMode == m_GunData.PrimaryFireMode)
                        {
                            m_NextSwitchModeTime = Time.time + m_GunAnimator.SwitchModeAnimationLength;
                            m_GunAnimator.SwitchMode();

                            FireMode = m_GunData.SecondaryFireMode;
                            m_FireInterval = m_GunData.SecondaryRateOfFire;
                        }
                        else
                        {
                            m_NextSwitchModeTime = Time.time + m_GunAnimator.SwitchModeAnimationLength;
                            m_GunAnimator.SwitchMode();

                            FireMode = m_GunData.PrimaryFireMode;
                            m_FireInterval = m_GunData.PrimaryRateOfFire;
                        }
                    }
                }
            }

            protected virtual void PullTheTrigger ()
            {
                if (CurrentRounds > 0 && Magazines >= 0)
                {
                    if (FireMode == GunData.FireMode.FullAuto || FireMode == GunData.FireMode.Single)
                    {
                        m_NextFireTime = Time.time + m_FireInterval;
                        CurrentRounds--;

                        m_NextShootDirection = GetShotDirection();
                        Shot();

                        m_IsShooting = 0.1f;

                        m_GunAnimator.Shot(CurrentRounds == 0);
                        m_GunEffects.Play();

                        StartCoroutine(m_MotionAnimation.RecoilAnimation(false));

                        if (m_CameraAnimationsController != null)
                        {
                            m_CameraAnimationsController.ApplyRecoil(m_MotionAnimation.CameraRecoil, m_MotionAnimation.MinCameraRecoilRotation, m_MotionAnimation.MaxCameraRecoilRotation,
                                                                     m_MotionAnimation.CameraRecoilDuration, m_MotionAnimation.CameraReturnDuration);
                        }
                    }
                    else if (FireMode == GunData.FireMode.ShotgunAuto || FireMode == GunData.FireMode.ShotgunSingle)
                    {
                        m_NextFireTime = Time.time + m_FireInterval;
                        CurrentRounds--;

                        for (int i = 0; i < m_GunData.BulletsPerShoot; i++)
                        {
                            m_NextShootDirection = GetShotDirection();
                            Shot();
                        }

                        m_IsShooting = 0.1f;

                        m_GunAnimator.Shot(CurrentRounds == 0);
                        m_GunEffects.Play();

                        StartCoroutine(m_MotionAnimation.RecoilAnimation(false));

                        if (m_CameraAnimationsController != null)
                        {
                            m_CameraAnimationsController.ApplyRecoil(m_MotionAnimation.CameraRecoil, m_MotionAnimation.MinCameraRecoilRotation, m_MotionAnimation.MaxCameraRecoilRotation,
                                                                     m_MotionAnimation.CameraRecoilDuration, m_MotionAnimation.CameraReturnDuration);
                        }

                    }
                    else if (FireMode == GunData.FireMode.Burst)
                    {
                        m_NextFireTime = Time.time + m_FireInterval * (m_GunData.BulletsPerBurst + 1);
                        StartCoroutine(Burst());
                    }
                }
                else
                {
                    m_NextFireTime = Time.time + 0.25f;
                    m_GunAnimator.OutOfAmmo();
                }
            }

            protected virtual IEnumerator Burst ()
            {
                for (int i = 0; i < m_GunData.BulletsPerBurst; i++)
                {
                    if (CurrentRounds == 0)
                        break;

                    m_NextShootDirection = GetShotDirection();
                    CurrentRounds--;
                    Shot();

                    m_IsShooting = 0.1f;

                    m_GunAnimator.Shot(CurrentRounds == 0);
                    m_GunEffects.Play();

                    StartCoroutine(m_MotionAnimation.RecoilAnimation(false));

                    if (m_CameraAnimationsController != null)
                    {
                        m_CameraAnimationsController.ApplyRecoil(m_MotionAnimation.CameraRecoil, m_MotionAnimation.MinCameraRecoilRotation, m_MotionAnimation.MaxCameraRecoilRotation,
                                                                 m_MotionAnimation.CameraRecoilDuration, m_MotionAnimation.CameraReturnDuration);
                    }
                    yield return new WaitForSeconds(m_FireInterval);
                }
            }

            protected virtual void Shot ()
            {
                Vector3 direction = m_CameraTransformReference.TransformDirection(m_NextShootDirection);
                Vector3 origin = m_CameraTransformReference.transform.position;

                Ray ray = new Ray(origin, direction);
                RaycastHit hitInfo;

                float tracerDuration = 2;
                float damage;

                if (Physics.Raycast(ray, out hitInfo, m_GunData.Range, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
                {
					SurfaceIdentifier surf = hitInfo.collider.GetSurface();
                    damage = m_GunData.DamageType == GunData.DamageMode.Constant ? m_GunData.Damage 
                        : m_GunData.Damage * m_GunData.DamageFalloffCurve.Evaluate(hitInfo.distance / m_GunData.Range);

                    if (surf != null)
                    {
                        m_BulletMarkManager.CreateBulletMark(surf, hitInfo, m_GunData.AffectedLayers);

                        if (m_GunData.PenetrateObjects && surf.CanPenetrate)
                        {
                            Penetrate(hitInfo, direction, surf, m_GunData.Range - hitInfo.distance, damage);
                        }
                    }

                    // If hit a rigidbody applies force to push.
                    Rigidbody rigidbody = hitInfo.collider.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddForce(direction * m_GunData.Force, ForceMode.Impulse);
                    }

                    if (hitInfo.transform.root != transform.root)
                    {
                        IBulletDamageable damageableTarget = hitInfo.collider.GetComponent<IBulletDamageable>();
                        if (damageableTarget != null)
                        {
                            damageableTarget.BulletDamage(damage, transform.root.position);
                        }
                    }

                    tracerDuration = hitInfo.distance / m_GunEffects.TracerSpeed;
                }

                if (tracerDuration > 0.05f)
                    m_GunEffects.CreateTracer(transform, direction, tracerDuration);
            }

            protected virtual void Penetrate (RaycastHit lastHitInfo, Vector3 direction, SurfaceIdentifier surf, float range, float damage)
            {
                Ray ray = new Ray(lastHitInfo.point + direction * 0.1f, direction);
                RaycastHit hitInfo;

                int affectedObjectID = lastHitInfo.collider.GetInstanceID();

                if (Physics.Raycast(ray, out hitInfo, range, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
                {
                    // Get the surface type of the object.
                    SurfaceIdentifier newSurf = hitInfo.collider.GetSurface();

                    // Exit hole
                    Ray exitRay = new Ray(hitInfo.point, direction * -1);
                    RaycastHit exitInfo;

                    if (Physics.Raycast(exitRay, out exitInfo, range, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
                    {
                        float distanceTraveled = Vector3.Distance(lastHitInfo.point, exitInfo.point) * surf.Density;

                        // Does the bullet get through?
                        if (m_GunData.PenetrationPower > distanceTraveled)
                        {
                            if (newSurf != null)
                                m_BulletMarkManager.CreateBulletMark(newSurf, hitInfo, m_GunData.AffectedLayers);

                            if (affectedObjectID == exitInfo.collider.GetInstanceID())
                                m_BulletMarkManager.CreateBulletMark(surf, exitInfo, m_GunData.AffectedLayers);

                            // If hit a rigidbody applies force to push.
                            Rigidbody rigidbody = hitInfo.collider.GetComponent<Rigidbody>();
                            if (rigidbody != null)
                            {
                                rigidbody.AddForce(direction * m_GunData.Force, ForceMode.Impulse);
                            }

                            if (hitInfo.transform.root != transform.root)
                            {
                                IBulletDamageable damageableTarget = hitInfo.collider.GetComponent<IBulletDamageable>();
                                if (damageableTarget != null)
                                {
                                    damageableTarget.BulletDamage(damage * (distanceTraveled / m_GunData.PenetrationPower), transform.root.position);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Exit hole
                    Ray exitRay = new Ray(lastHitInfo.point + direction * m_GunData.PenetrationPower, direction * -1);
                    RaycastHit exitInfo;

                    if (Physics.Raycast(exitRay, out exitInfo, m_GunData.PenetrationPower, m_GunData.AffectedLayers, QueryTriggerInteraction.Collide))
                    {
                        if (affectedObjectID == exitInfo.collider.GetInstanceID())
                            m_BulletMarkManager.CreateBulletMark(surf, exitInfo, m_GunData.AffectedLayers);
                    }
                }
            }

            protected virtual Vector3 GetShotDirection ()
            {
                if (Mathf.Abs(m_Accuracy - 1) < Mathf.Epsilon)
                {
                    return new Vector3(0, 0, 1);
                }
                else
                {
                    Vector2 randonPointInScreen = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f)) * ((1 - m_Accuracy) / 10);
                    return new Vector3(randonPointInScreen.x, randonPointInScreen.y, 1);
                }
            }

            protected virtual IEnumerator MeleeAttack ()
            {
                m_Attacking = true;
                m_GunAnimator.Melee();
                yield return new WaitForSeconds(m_GunAnimator.MeleeDelay);

                Vector3 direction = m_CameraTransformReference.TransformDirection(Vector3.forward);
                Vector3 origin = m_CameraTransformReference.transform.position;
                float range = Mathf.Clamp(m_GunData.Size, 1, 2);

                Ray ray = new Ray(origin, direction);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, range, Physics.AllLayers, QueryTriggerInteraction.Collide))
                {
                    m_GunAnimator.Hit(hitInfo.point);

                    // If hit a rigidbody applies force to push.
                    Rigidbody rigidbody = hitInfo.collider.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddForce(direction * m_GunData.MeleeForce, ForceMode.Impulse);
                    }

                    if (hitInfo.transform.root != transform.root)
                    {
                        IBulletDamageable damageableTarget = hitInfo.collider.GetComponent<IBulletDamageable>();
                        if (damageableTarget != null)
                        {
                            damageableTarget.BulletDamage(m_GunData.MeleeDamage, transform.root.position);
                        }
                    }
                }

                yield return new WaitForSeconds(m_GunAnimator.MeleeAnimationLength - m_GunAnimator.MeleeDelay);
                m_Attacking = false;
            }

            #region RELOAD METHODS

            protected virtual void Reload ()
            {
                if (m_GunData.ReloadType == GunData.ReloadMode.Magazines)
                {
                    StartCoroutine(ReloadMagazines());
                }
                else if (m_GunData.ReloadType == GunData.ReloadMode.BulletByBullet)
                {
                    StartCoroutine(ReloadBulletByBullet());
                }
            }

            protected virtual IEnumerator ReloadMagazines ()
            {
                m_IsReloading = true;

                m_GunAnimator.Reload(CurrentRounds == 0);

                yield return CurrentRounds == 0 ? m_CompleteReloadDuration : m_ReloadDuration;

                if (m_GunActive && m_IsReloading)
                {
                    if (CurrentRounds > 0)
                    {
                        if (CurrentRounds + Magazines > (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine))
                        {
                            Magazines -= (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine) - CurrentRounds;
                            CurrentRounds = (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine);
                        }
                        else
                        {
                            CurrentRounds += Magazines;
                            Magazines = 0;
                        }
                    }
                    else
                    {
                        if (CurrentRounds + Magazines > RoundsPerMagazine)
                        {
                            Magazines -= RoundsPerMagazine - CurrentRounds;
                            CurrentRounds = RoundsPerMagazine;
                        }
                        else
                        {
                            CurrentRounds += Magazines;
                            Magazines = 0;
                        }
                    }
                }

                m_IsReloading = false;
            }

            protected virtual IEnumerator ReloadBulletByBullet ()
            {
                m_IsReloading = true;

                m_GunAnimator.StartReload(CurrentRounds == 0);

                if (CurrentRounds == 0)
                {
                    yield return m_InsertInChamberDuration;
                    CurrentRounds++;
                    Magazines--;
                    yield return m_InsertInChamberDuration;
                }
                else
                {
                    yield return m_StartReloadDuration;
                }

                while (m_GunActive && (CurrentRounds < (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine) && Magazines > 0) && m_IsReloading)
                {
                    m_GunAnimator.Insert();
                    yield return m_InsertDuration;

                    if (m_GunActive && m_IsReloading)
                    {
                        CurrentRounds++;
                        Magazines--;
                    }
                    yield return m_InsertDuration;
                }

                if (m_GunActive && m_IsReloading)
                {
                    StartCoroutine(StopReload());
                }
            }

            protected virtual IEnumerator StopReload ()
            {
                m_GunAnimator.StopReload();
                m_IsReloading = false;
                m_NextReloadTime = m_GunAnimator.StopReloadAnimationLength + Time.time;
                yield return m_StopReloadDuration;
            }

            #endregion

            #region ANIMATIONS

            protected void InitSwing (Transform weaponSwing)
            {
				if (weaponSwing.parent.name.Equals("WeaponSwing"))
					m_WeaponSwing.Init(weaponSwing.parent);
				else
				{
					Transform parent = weaponSwing.parent.Find("WeaponSwing");

					if (parent != null)
					{
						weaponSwing.parent = parent;
					}
					else
					{
						GameObject weaponController = new GameObject("WeaponSwing");
						weaponController.transform.SetParent(weaponSwing.parent, false);
						weaponSwing.parent = weaponController.transform;
					}
				}
            }

            protected void WeaponJump ()
            {
                if (m_GunActive)
                    StartCoroutine(m_MotionAnimation.JumpAnimation.Play());
            }

            protected void WeaponLanding (float fallDamage)
            {
                if (m_GunActive)
                    StartCoroutine(m_MotionAnimation.LandingAnimation.Play());
            }

            #endregion

            internal virtual void Refill ()
            {
                if (RoundsPerMagazine == 0)
                    RoundsPerMagazine = m_GunData.RoundsPerMagazine;

                SetAmmo(m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine, m_GunData.MaxMagazines * (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine));
            }

            internal virtual void SetAmmo (int currentRounds, int magazines)
            {
                CurrentRounds = Mathf.Max(0, Mathf.Min(currentRounds, m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine));
                Magazines = Mathf.Max(0, Mathf.Min(magazines, m_GunData.MaxMagazines * (m_GunData.HasChamber ? RoundsPerMagazine + 1 : RoundsPerMagazine)));
            }

            public void DisableShadowCasting ()
            {
                // For each object that has a renderer inside the weapon gameObject
                foreach (Renderer r in GetComponentsInChildren<Renderer>())
                {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Prevents the weapon produces shadows to avoid ghosts weapons bugs
                }
            }

            protected void SetWeaponViewModel ()
            {
                // For each object that has a renderer inside the weapon gameObject
                foreach (Renderer r in GetComponentsInChildren<Renderer>())
                {
					r.material.EnableKeyword("VIEWMODEL");
                }
            }

            public virtual void Interact ()
            {
                m_NextInteractTime = Time.time + Mathf.Max(InteractAnimationLength, InteractDelay);
                m_GunAnimator.Interact();
            }

            protected virtual void Vault ()
            {
                if (m_GunActive)
                    m_GunAnimator.Vault();
            }

            #region WEAPON CUSTOMIZATION

            internal virtual void UpdateAiming (Vector3 aimingPosition, Vector3 aimingRotation, bool zoomAnimation = false, float aimFOV = 50)
            {
                m_GunAnimator.UpdateAiming(aimingPosition, aimingRotation, zoomAnimation, aimFOV);
            }

            internal virtual void UpdateFireSound (AudioClip[] fireSoundList)
            {
                m_GunAnimator.UpdateFireSound(fireSoundList);
            }

            internal virtual void UpdateMuzzleFlash (ParticleSystem muzzleParticle)
            {
                m_GunEffects.UpdateMuzzleFlash(muzzleParticle);
            }

            internal virtual void UpdateRecoil (Vector3 minCameraRecoilRotation, Vector3 maxCameraRecoilRotation)
            {
                m_MotionAnimation.MinCameraRecoilRotation = minCameraRecoilRotation;
                m_MotionAnimation.MaxCameraRecoilRotation = maxCameraRecoilRotation;
            }

            internal virtual void UpdateRoundsPerMagazine (int roundsPerMagazine)
            {
                RoundsPerMagazine = roundsPerMagazine;
            }

            #endregion

            #region EDITOR METHODS

            public void AddAimedShotAnimation ()
            {
                m_GunAnimator.AddAimedShotAnimation();
            }

            public void RemoveAimedShotAnimation (int index)
            {
                m_GunAnimator.RemoveAimedShotAnimation(index);
            }

            public void AddShotAnimation ()
            {
                m_GunAnimator.AddShotAnimation();
            }

            public void RemoveShotAnimation (int index)
            {
                m_GunAnimator.RemoveShotAnimation(index);
            }

            public void AddShotSound ()
            {
                m_GunAnimator.AddShotSound();
            }

            public void RemoveShotSound (int index)
            {
                m_GunAnimator.RemoveShotSound(index);
            }

            public void AddHitSound ()
            {
                m_GunAnimator.AddHitSound();
            }

            public void RemoveHitSound (int index)
            {
                m_GunAnimator.RemoveHitSound(index);
            }

            #endregion
        }
    }
}
