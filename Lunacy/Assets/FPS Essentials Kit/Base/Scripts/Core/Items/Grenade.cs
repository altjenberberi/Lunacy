/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections;
using UnityEngine;

namespace Essentials
{
    namespace Items
    {
        [AddComponentMenu("FPS Essentials/Items/Grenade"), DisallowMultipleComponent]
        public class Grenade : MonoBehaviour, IUsable
        {
            [SerializeField]
            [NotNull]
            protected Rigidbody m_Grenade; // Grenade explosive prefab

            [SerializeField]
            [NotNull]
            protected Transform m_ThrowTransformReference; // Where the grenade will be instantiated

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_ThrowForce = 10;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_DelayToInstantiate = 0.3f; // Delay in secs until instantiate the grenade (to match with the animation)

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected int m_Amount = 3; // Amount of grenades

            [SerializeField]
            protected bool m_InfiniteGrenades;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected int m_MaxAmount = 3; // Max amount of grenades

            [SerializeField]
            [NotNull]
            protected Animator m_Animator;

            [SerializeField]
            protected string m_PullAnimation;

            [SerializeField]
            protected AudioClip m_PullSound;

            [SerializeField]
            [Range(0, 1)]
            protected float m_PullVolume;

            [SerializeField]
            protected string m_ThrowAnimation;

            [SerializeField]
            protected AudioClip m_ThrowSound;

            [SerializeField]
            [Range(0, 1)]
            protected float m_ThrowVolume;

            protected WaitForSeconds m_PullDuration = null;
            protected WaitForSeconds m_InstantiateDelay = null;
            protected PlayerAudioSource m_PlayerBodySource;

            public int Amount
            {
                get
                {
                    return m_InfiniteGrenades ? 99 : m_Amount;
                }
            }

            public virtual float PullAndThrowLenght
            {
                get
                {
                    if (m_Animator == null)
                        return 0;

                    if (m_PullAnimation.Length == 0 && m_ThrowAnimation.Length == 0)
                        return 0;

                    if (m_Animator.GetAnimationClip(m_ThrowAnimation).length < m_DelayToInstantiate)
                        return m_Animator.GetAnimationClip(m_PullAnimation).length + m_DelayToInstantiate;

                    return m_Animator.GetAnimationClip(m_PullAnimation).length + m_Animator.GetAnimationClip(m_ThrowAnimation).length;
                }
            }

            protected virtual void Init ()
            {
                SetWeaponViewModel();
                DisableShadowCasting();

                m_PullDuration = new WaitForSeconds(m_Animator.GetAnimationClip(m_PullAnimation).length);
                m_InstantiateDelay = new WaitForSeconds(m_DelayToInstantiate);
            }


            public virtual void Use ()
            {
                if (m_PullDuration == null || m_InstantiateDelay == null)
                {
                    Init();
                }

                if (m_Grenade != null && m_ThrowTransformReference != null)
                    StartCoroutine(ThrowGrenade());
            }

            protected virtual IEnumerator ThrowGrenade ()
            {
                if (m_Animator != null)
                    m_Animator.CrossFadeInFixedTime(m_PullAnimation, 0.1f);

                if (m_PlayerBodySource == null)
                    m_PlayerBodySource = AudioManager.Instance.RegisterSource("PlayerBodySource", AudioManager.Instance.transform, spatialBlend: 0);

                m_PlayerBodySource.ForcePlay(m_PullSound, m_PullVolume);

                yield return m_PullDuration;

                if (m_Animator != null)
                    m_Animator.CrossFadeInFixedTime(m_ThrowAnimation, 0.1f);

                m_PlayerBodySource.ForcePlay(m_ThrowSound, m_ThrowVolume);

                yield return m_InstantiateDelay;

                InstantiateGrenade();
            }

            protected virtual void InstantiateGrenade ()
            {
                if (m_Grenade != null)
                {
                    Rigidbody clone = Instantiate(m_Grenade, m_ThrowTransformReference.position, m_ThrowTransformReference.rotation);
                    clone.velocity = clone.transform.TransformDirection(Vector3.forward) * m_ThrowForce;

                    if (!m_InfiniteGrenades)
                        m_Amount--;
                }
            }

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

            public virtual void Refill ()
            {
                m_Amount = m_MaxAmount;
            }
        }
    }
}

