/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    namespace Weapons
    {
        [System.Serializable]
        public sealed class GunEffects
        {
            #region MUZZLEFLASH

            [SerializeField]
            private bool m_MuzzleFlash = false;

            [SerializeField]
            [NotNull]
            private ParticleSystem m_MuzzleParticle;

            #endregion

            #region TRACER

            [SerializeField]
            private bool m_Tracer = false;

            [SerializeField]
            [NotNull]
            private Rigidbody m_TracerPrefab;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_TracerSpeed = 450;

            [SerializeField]
            [NotNull]
            private Transform m_TracerOrigin;

            #endregion

            #region SHELL

            [SerializeField]
            private bool m_Shell = false;

            [SerializeField]
            [NotNull]
            private ParticleSystem m_ShellParticle;

            [SerializeField]
            private float m_MinShellSpeed = 1;

            [SerializeField]
            private float m_MaxShellSpeed = 3;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_StartDelay = 0;

            #endregion

            public float TracerSpeed { get { return m_TracerSpeed; } }

            #region WEAPON CUSTOMIZATION

            internal void UpdateMuzzleFlash (ParticleSystem muzzleParticle)
            {
                m_MuzzleParticle = muzzleParticle;
            }

            #endregion

            internal void Play ()
            {
                if (m_MuzzleFlash)
                {
                    if (m_MuzzleParticle != null)
                        m_MuzzleParticle.Play();
                }

                if (m_Shell)
                {
                    if (m_ShellParticle != null)
                    {
                        ParticleSystem.MainModule mainModule = m_ShellParticle.main;
                        mainModule.startSpeed = Random.Range(m_MinShellSpeed, m_MaxShellSpeed);
                        mainModule.startDelay = m_StartDelay;

                        ParticleSystem[] children = m_ShellParticle.GetComponentsInChildren<ParticleSystem>();

                        foreach (ParticleSystem p in children)
                        {
                            ParticleSystem.MainModule childrenModule = p.main;
                            childrenModule.startDelay = m_StartDelay;
                        }

                        m_ShellParticle.Play();
                    }
                }
            }

            internal void CreateTracer (Transform origin, Vector3 direction, float duration)
            {
                if (m_Tracer)
                {
                    if (m_TracerPrefab != null)
                    {
                        Rigidbody tracer = Object.Instantiate(m_TracerPrefab, m_TracerOrigin.position, origin.rotation);
                        tracer.velocity = direction * m_TracerSpeed;
                        Object.Destroy(tracer.gameObject, duration);
                    }
                }
            }
        }
    }
}

