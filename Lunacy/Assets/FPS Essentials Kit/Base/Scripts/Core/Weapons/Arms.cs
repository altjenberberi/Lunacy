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
        public class Arms : MonoBehaviour, IWeapon
        {
            [SerializeField]
            [NotNull]
            protected Transform m_CameraTransformReference; // FPS Camera

            [SerializeField]
            [NotNull]
            protected FirstPersonController m_FPController;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_Range = 2;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_AttackRate = 0.25f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_Force = 5;

            [SerializeField]
            protected float m_MinDamage = 15;

            [SerializeField]
            protected float m_MaxDamage = 30;

            [SerializeField]
            protected WeaponSwing m_WeaponSwing = new WeaponSwing();

            [SerializeField]
            protected MotionAnimation m_MotionAnimation = new MotionAnimation();

            [SerializeField]
            protected ArmsAnimator m_ArmsAnimator = new ArmsAnimator();

            protected Camera m_Camera;
            protected bool m_ArmsActive;
            protected float m_NexAttackTime = 0;
            protected float m_NextInteractTime = 0;

            #region PROPERTIES

            public int Identifier { get { return GetInstanceID(); } }
            public GameObject Viewmodel { get { return gameObject; } }

            public virtual bool CanSwitch
            {
                get
                {
                    return m_ArmsActive && m_FPController.State != MotionState.Running && m_NexAttackTime < Time.time;
                }
            }

            public virtual bool CanUseItems
            {
                get
                {
                    return m_ArmsActive && m_FPController.State != MotionState.Running && m_NexAttackTime < Time.time;
                }
            }

            public virtual bool CanVault
            {
                get
                {
                    return m_ArmsActive && m_NexAttackTime < Time.time;
                }
            }

            public float Size { get { return m_Range; } }
            public float HideAnimationLength { get { return m_ArmsAnimator.HideAnimationLength; } }
            public float InteractAnimationLength { get { return m_ArmsAnimator.InteractAnimationLength; } }
            public float InteractDelay { get { return m_ArmsAnimator.InteractDelay; } }

            #endregion

            #region EDITOR

            public void AddRightAttack ()
            {
                m_ArmsAnimator.AddRightAttack();
            }

            public void RemoveRightAttack (int index)
            {
                m_ArmsAnimator.RemoveRightAttack(index);
            }

            public void AddLeftAttack ()
            {
                m_ArmsAnimator.AddLeftAttack();
            }

            public void RemoveLeftAttack (int index)
            {
                m_ArmsAnimator.RemoveLeftAttack(index);
            }

            public void AddAttackSound ()
            {
                m_ArmsAnimator.AddAttackSound();
            }

            public void RemoveAttackSound (int index)
            {
                m_ArmsAnimator.RemoveAttackSound(index);
            }

            public void AddHitSound ()
            {
                m_ArmsAnimator.AddHitSound();
            }

            public void RemoveHitSound (int index)
            {
                m_ArmsAnimator.RemoveHitSound(index);
            }

            #endregion

            protected virtual void Awake ()
            {
                if (m_CameraTransformReference == null)
                {
                    throw new Exception("Camera Transform Reference was not assigned");
                }

                if (m_FPController == null)
                {
                    throw new Exception("FPController was not assigned");
                }
            }

            protected virtual void Start ()
            {
                m_Camera = m_CameraTransformReference.GetComponent<Camera>();

                InitSwing(transform);
                SetWeaponViewModel();
                DisableShadowCasting();

                m_ArmsAnimator.Init(m_FPController);

                m_FPController.JumpEvent += WeaponJump;
                m_FPController.LandingEvent += WeaponLanding;
                m_FPController.VaultEvent += Vault;
            }

            protected virtual void Update ()
            {
                if (m_ArmsActive)
                {
                    if (m_FPController.Controllable)
                        HandleInput();

                    m_FPController.CanVault = CanVault;
                    m_ArmsAnimator.SetSpeed(m_FPController.State == MotionState.Running); 
                }

                m_WeaponSwing.Swing(transform.parent, m_FPController);
                m_MotionAnimation.MovementAnimation(m_FPController);
                m_MotionAnimation.BreathingAnimation(m_FPController.IsAiming ? 0 : 1);
                m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, GameplayManager.Instance.FieldOfView, Time.deltaTime * 10);
            }

            public virtual void Select ()
            {
                m_ArmsAnimator.Draw();
                StartCoroutine(Draw());
            }

            public virtual void Deselect ()
            {
                m_ArmsActive = false;
                m_FPController.CanVault = false;
                m_ArmsAnimator.Hide();
            }

            protected virtual void HandleInput ()
            {
                bool canAttack = m_FPController.State != MotionState.Running && m_NexAttackTime < Time.time && m_NextInteractTime < Time.time;

                if (canAttack)
                {
                    if (InputManager.GetButtonDown("Fire"))
                    {
                        m_ArmsAnimator.LeftAttack();
                        StartCoroutine(Attack());
                    }
                    else if (InputManager.GetButtonDown("Aim"))
                    {
                        m_ArmsAnimator.RightAttack();
                        StartCoroutine(Attack());
                    }
                }
            }

            protected virtual IEnumerator Attack ()
            {
                m_NexAttackTime = Time.time + m_AttackRate;

                yield return new WaitForSeconds(0.1f); // Wait 0.1 seconds before applying damage/force.

                Vector3 direction = m_CameraTransformReference.TransformDirection(Vector3.forward);
                Vector3 origin = m_CameraTransformReference.transform.position;

                Ray ray = new Ray(origin, direction);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, m_Range, Physics.AllLayers, QueryTriggerInteraction.Collide))
                {
                    m_ArmsAnimator.Hit(hitInfo.point);

                    // If hit a rigidbody applies force to push.
                    Rigidbody rigidbody = hitInfo.collider.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.AddForce(direction * m_Force, ForceMode.Impulse);
                    }

                    if (hitInfo.transform.root != transform.root)
                    {
                        IBulletDamageable damageableTarget = hitInfo.collider.GetComponent<IBulletDamageable>();
                        if (damageableTarget != null)
                        {
                            damageableTarget.BulletDamage(UnityEngine.Random.Range(m_MinDamage, m_MaxDamage), transform.root.position);
                        }
                    }
                }
            }

            protected virtual IEnumerator Draw ()
            {
                yield return new WaitForSeconds(m_ArmsAnimator.DrawAnimationLength);
                m_ArmsActive = true;
            }

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
                if (m_ArmsActive)
                    StartCoroutine(m_MotionAnimation.JumpAnimation.Play());
            }

            protected void WeaponLanding (float fallDamage)
            {
                if (m_ArmsActive)
                    StartCoroutine(m_MotionAnimation.LandingAnimation.Play());
            }

            #endregion

            public void DisableShadowCasting ()
            {
                // For each object that has a renderer inside the weapon gameObject
                foreach (Renderer r in GetComponentsInChildren<Renderer>())
                {
                    r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; //Prevents the weapon produces shadows to avoid ghosts weapons bugs
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
                m_ArmsAnimator.Interact();
            }

            protected virtual void Vault ()
            {
                if (m_ArmsActive)
                    m_ArmsAnimator.Vault();
            }
        }
    }
}