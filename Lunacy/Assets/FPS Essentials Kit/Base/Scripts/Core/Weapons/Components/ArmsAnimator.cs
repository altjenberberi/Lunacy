/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using Essentials.Controllers;
using System.Collections.Generic;
using UnityEngine;

namespace Essentials
{
    namespace Weapons
    {
        [System.Serializable]
        public sealed class ArmsAnimator
        {
            public enum AnimationType
            {
                Sequential,
                Random
            }

            [SerializeField]
            [NotNull]
            private Animator m_Animator;

            [SerializeField]
            private string m_VelocityParameter = "Velocity";

            [SerializeField]
            private bool m_Draw = false;

            [SerializeField]
            private string m_DrawAnimation = "Draw";

            [SerializeField]
            private AudioClip m_DrawSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_DrawVolume = 0.25f;

            [SerializeField]
            private bool m_Hide = false;

            [SerializeField]
            private string m_HideAnimation = "Hide";

            [SerializeField]
            private AudioClip m_HideSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_HideVolume = 0.25f;

            [SerializeField]
            private bool m_Attack = false;

            [SerializeField]
            private List<string> m_RightAttackAnimationList = new List<string>();

            [SerializeField]
            private List<string> m_LeftAttackAnimationList = new List<string>();

            [SerializeField]
            private AnimationType m_AttackAnimationType = AnimationType.Sequential;

            [SerializeField]
            private List<AudioClip> m_AttackSoundList = new List<AudioClip>();

            [SerializeField]
            [Range(0, 1)]
            private float m_AttackVolume = 0.5f;

            [SerializeField]
            private List<AudioClip> m_HitSoundList = new List<AudioClip>();

            [SerializeField]
            [Range(0, 1)]
            private float m_HitVolume = 0.3f;

            private int m_LastIndex;

            [SerializeField]
            private bool m_Interact = false;

            [SerializeField]
            private string m_InteractAnimation = "Interact";

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_InteractDelay = 0.1f;

            [SerializeField]
            private AudioClip m_InteractSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_InteractVolume = 0.2f;

            [SerializeField]
            private bool m_Vault = false;

            [SerializeField]
            private string m_VaultAnimation = "Vault";

            [SerializeField]
            private AudioClip m_VaultSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_VaultVolume = 0.2f;

            private float m_Velocity = 0;

            private FirstPersonController m_FPController;
            private PlayerAudioSource m_PlayerBodySource;

            #region PROPERTIES

            public float DrawAnimationLength
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (!m_Draw)
                        return 0;

                    if (m_DrawAnimation.Length == 0)
                        return 0;

                    return m_Animator.GetAnimationClip(m_DrawAnimation).length;
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

                    if (m_HideAnimation.Length == 0)
                        return 0;

                    return m_Animator.GetAnimationClip(m_HideAnimation).length;
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

                    return m_InteractAnimation.Length == 0 ? 0 : m_Animator.GetAnimationClip(m_InteractAnimation).length;

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

            internal void Init (FirstPersonController FPController)
            {
                m_FPController = FPController;
                m_PlayerBodySource = AudioManager.Instance.RegisterSource("PlayerBodySource", AudioManager.Instance.transform, spatialBlend: 0);
            }

            internal void SetSpeed (bool running)
            {
                m_Velocity = Mathf.MoveTowards(m_Velocity, running ? 1 : 0, Time.deltaTime * 5);
                m_Animator.SetFloat(m_VelocityParameter, m_Velocity);

                // Updates animator speed to smoothly change between walking and running animations
                m_Animator.speed = Mathf.Max(m_FPController.CurrentTargetSpeed / (m_FPController.State == MotionState.Running ? 10 : m_FPController.CurrentTargetSpeed), 0.8f);
            }

            internal void LeftAttack ()
            {
                if (m_Animator == null)
                    return;

                m_Animator.speed = 1;

                if (m_Attack)
                {
                    if (m_LeftAttackAnimationList.Count > 0)
                    {
                        if (m_AttackAnimationType == AnimationType.Sequential)
                        {
                            if (m_LastIndex == m_LeftAttackAnimationList.Count || m_LastIndex > m_LeftAttackAnimationList.Count)
                                m_LastIndex = 0;

                            m_Animator.CrossFadeInFixedTime(m_LeftAttackAnimationList[m_LastIndex++], 0.1f);
                        }
                        else
                        {
                            m_Animator.CrossFadeInFixedTime(m_LeftAttackAnimationList[Random.Range(0, m_LeftAttackAnimationList.Count)], 0.1f);
                        }
                    }

                    if (m_AttackSoundList.Count > 0)
                    {
                        if (m_AttackSoundList.Count > 1)
                        {
                            int i = Random.Range(1, m_AttackSoundList.Count);
                            AudioClip a = m_AttackSoundList[i];

                            m_AttackSoundList[i] = m_AttackSoundList[0];
                            m_AttackSoundList[0] = a;

                            m_PlayerBodySource.ForcePlay(a, m_AttackVolume);
                        }
                        else
                        {
                            m_PlayerBodySource.ForcePlay(m_AttackSoundList[0], m_AttackVolume);
                        }
                    }
                }
            }

            internal void RightAttack ()
            {
                if (m_Animator == null)
                    return;

                m_Animator.speed = 1;

                if (m_Attack)
                {
                    if (m_RightAttackAnimationList.Count > 0)
                    {
                        if (m_AttackAnimationType == AnimationType.Sequential)
                        {
                            if (m_LastIndex == m_RightAttackAnimationList.Count || m_LastIndex > m_RightAttackAnimationList.Count)
                                m_LastIndex = 0;

                            m_Animator.CrossFadeInFixedTime(m_RightAttackAnimationList[m_LastIndex++], 0.1f);
                        }
                        else
                        {
                            m_Animator.CrossFadeInFixedTime(m_RightAttackAnimationList[Random.Range(0, m_RightAttackAnimationList.Count)], 0.1f);
                        }
                    }

                    if (m_AttackSoundList.Count > 0)
                    {
                        if (m_AttackSoundList.Count > 1)
                        {
                            int i = Random.Range(1, m_AttackSoundList.Count);
                            AudioClip a = m_AttackSoundList[i];

                            m_AttackSoundList[i] = m_AttackSoundList[0];
                            m_AttackSoundList[0] = a;

                            m_PlayerBodySource.ForcePlay(a, m_AttackVolume);
                        }
                        else
                        {
                            m_PlayerBodySource.ForcePlay(m_AttackSoundList[0], m_AttackVolume);
                        }
                    }
                }
            }

            internal void Hit (Vector3 position)
            {
                if (m_HitSoundList.Count > 0)
                    AudioManager.Instance.PlayClipAtPoint(m_HitSoundList[Random.Range(0, m_HitSoundList.Count)], position, 3, 10, m_HitVolume);
            }

            internal void Draw ()
            {
                if (m_Animator == null)
                    return;

                m_Animator.speed = 1;

                if (m_Draw)
                {
                    if (m_DrawAnimation.Length == 0)
                        return;

                    m_Animator.Play(m_DrawAnimation);

                    m_PlayerBodySource = AudioManager.Instance.RegisterSource("PlayerBodySource", AudioManager.Instance.transform, spatialBlend: 0);

                    m_PlayerBodySource.ForcePlay(m_DrawSound, m_DrawVolume);
                }
            }

            internal void Hide ()
            {
                if (m_Animator == null)
                    return;

                m_Animator.speed = 1;

                if (m_Hide)
                {
                    if (m_HideAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_HideAnimation, 0.1f);
                    m_PlayerBodySource.Stop();
                    m_PlayerBodySource.ForcePlay(m_HideSound, m_HideVolume);
                }
            }

            internal void Interact ()
            {
                if (m_Animator == null)
                    return;

                m_Animator.speed = 1;

                if (m_Interact)
                {
                    if (m_InteractAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_InteractAnimation, 0.1f);
                    m_PlayerBodySource.ForcePlay(m_InteractSound, m_InteractVolume);
                }
            }

            internal void Vault ()
            {
                if (m_Animator == null)
                    return;

                m_Animator.speed = 1;

                if (m_Vault)
                {
                    if (m_VaultAnimation.Length == 0)
                        return;

                    m_Animator.CrossFadeInFixedTime(m_VaultAnimation, 0.1f);
                    m_PlayerBodySource.ForcePlay(m_VaultSound, m_VaultVolume);
                }
            }

            #region EDITOR

            public void AddRightAttack ()
            {
                m_RightAttackAnimationList.Add("");
            }

            public void RemoveRightAttack (int index)
            {
                m_RightAttackAnimationList.RemoveAt(index);
            }

            public void AddLeftAttack ()
            {
                m_LeftAttackAnimationList.Add("");
            }

            public void RemoveLeftAttack (int index)
            {
                m_LeftAttackAnimationList.RemoveAt(index);
            }

            public void AddAttackSound ()
            {
                m_AttackSoundList.Add(null);
            }

            public void RemoveAttackSound (int index)
            {
                m_AttackSoundList.RemoveAt(index);
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