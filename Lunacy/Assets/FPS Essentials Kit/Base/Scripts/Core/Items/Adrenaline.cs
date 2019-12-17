/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections;
using Essentials.Controllers;
using UnityEngine;

namespace Essentials
{
    namespace Items
    {
        [AddComponentMenu("FPS Essentials/Items/Adrenaline"), DisallowMultipleComponent]
        public class Adrenaline : MonoBehaviour, IUsable
        {
            [SerializeField]
            [NotNull]
            protected HealthController m_HealthController; // HealthController reference

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_HealAmount = 50;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_DelayToInject = 0.5f; // Delay in secs to apply the effect

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_StaminaBonusDuration = 10; // Effect duration

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected int m_Amount = 3; // Syringes amount

            [SerializeField]
            protected bool m_InfiniteShots;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected int m_MaxAmount = 3; // Max syringes amount

            [SerializeField]
            protected Animator m_Animator; // Animator reference

            [SerializeField]
            protected string m_ShotAnimation;

            [SerializeField]
            protected AudioClip m_ShotSound;

            [SerializeField]
            [Range(0, 1)]
            protected float m_ShotVolume = 0.3f;

            protected WaitForSeconds m_ShotDuration = null;
            protected PlayerAudioSource m_PlayerBodySource;

            public int Amount
            {
                get
                {
                    return m_InfiniteShots ? 99 : m_Amount;
                }
            }

            public float ShotLenght
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (m_ShotAnimation.Length == 0)
                        return 0;
                    return m_Animator.GetAnimationClip(m_ShotAnimation).length > m_DelayToInject ? m_Animator.GetAnimationClip(m_ShotAnimation).length : m_DelayToInject;
                }
            }

            protected virtual void Init ()
            {
                SetWeaponViewModel();
                DisableShadowCasting();

                m_ShotDuration = new WaitForSeconds(m_Animator.GetAnimationClip(m_ShotAnimation).length > m_DelayToInject
                    ? m_Animator.GetAnimationClip(m_ShotAnimation).length - m_DelayToInject : m_DelayToInject);
            }

            public virtual void Use ()
            {
                if (m_ShotDuration == null)
                {
                    Init();
                }
                StartCoroutine(AdrenalineShot());
            }

            protected virtual IEnumerator AdrenalineShot ()
            {
                if (m_Animator != null)
                    m_Animator.CrossFadeInFixedTime(m_ShotAnimation, 0.1f);

                if (m_PlayerBodySource == null)
                    m_PlayerBodySource = AudioManager.Instance.RegisterSource("PlayerBodySource", AudioManager.Instance.transform, spatialBlend: 0);

                m_PlayerBodySource.ForcePlay(m_ShotSound, m_ShotVolume);

                yield return new WaitForSeconds(m_DelayToInject);
                m_HealthController.Heal(m_HealAmount, m_StaminaBonusDuration > 0, m_StaminaBonusDuration);

                if (!m_InfiniteShots)
                    m_Amount--;

                yield return m_ShotDuration;
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

            public virtual void Refill ()
            {
                m_Amount = m_MaxAmount;
            }
        }
    }
}

