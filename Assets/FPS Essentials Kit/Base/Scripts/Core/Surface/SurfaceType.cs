/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections.Generic;
using UnityEngine;

namespace Essentials
{
    namespace SurfaceSystem
    {
        [CreateAssetMenu(menuName = "Surface Type", fileName = "New Surface", order = 101)]
        public sealed class SurfaceType : ScriptableObject
        {
            // Footsteps Sounds
            [SerializeField]
            private List<AudioClip> m_FootstepsSounds = new List<AudioClip>();

            [SerializeField]
            private List<AudioClip> m_SprintingFootstepsSounds = new List<AudioClip>();

            [SerializeField]
            private List<AudioClip> m_LandingSounds = new List<AudioClip>();

            // Bullet Marks
            [SerializeField]
            private float m_MinDecalSize = 0.75f;

            [SerializeField]
            private float m_MaxDecalSize = 1.5f;

            [SerializeField]
            private List<Material> m_BulletImpactMaterial = new List<Material>();

            [SerializeField]
            private List<GameObject> m_BulletImpactParticle = new List<GameObject>();

            [SerializeField]
            private List<AudioClip> m_BulletImpactSound = new List<AudioClip>();

            #region EDITOR

            public void AddFootstepSound ()
            {
                m_FootstepsSounds.Add(null);
            }

            public void RemoveFootstepSound (int index)
            {
                m_FootstepsSounds.RemoveAt(index);
            }

            public void AddSprintingFootstepSound ()
            {
                m_SprintingFootstepsSounds.Add(null);
            }

            public void RemoveSprintingFootstepSound (int index)
            {
                m_SprintingFootstepsSounds.RemoveAt(index);
            }

            public void AddLandingSound ()
            {
                m_LandingSounds.Add(null);
            }

            public void RemoveLandingSound (int index)
            {
                m_LandingSounds.RemoveAt(index);
            }

            public void AddDecalMaterial ()
            {
                m_BulletImpactMaterial.Add(null);
            }

            public void RemoveDecalMaterial (int index)
            {
                m_BulletImpactMaterial.RemoveAt(index);
            }

            public void AddDecalParticle ()
            {
                m_BulletImpactParticle.Add(null);
            }

            public void RemoveDecalParticle (int index)
            {
                m_BulletImpactParticle.RemoveAt(index);
            }

            public void AddDecalSound ()
            {
                m_BulletImpactSound.Add(null);
            }

            public void RemoveDecalSound (int index)
            {
                m_BulletImpactSound.RemoveAt(index);
            }

            #endregion

            public AudioClip GetRandomFootsteps ()
            {
                if (m_FootstepsSounds.Count > 0)
                {
                    if (m_FootstepsSounds.Count > 1)
                    {
                        int i = Random.Range(1, m_FootstepsSounds.Count);
                        AudioClip a = m_FootstepsSounds[i];

                        m_FootstepsSounds[i] = m_FootstepsSounds[0];
                        m_FootstepsSounds[0] = a;

                        return a;
                    }
                    return m_FootstepsSounds[0];
                }
                return null;
            }

            public AudioClip GetRandomSprintingFootsteps ()
            {
                if (m_SprintingFootstepsSounds.Count > 0)
                {
                    if (m_SprintingFootstepsSounds.Count > 1)
                    {
                        int i = Random.Range(1, m_SprintingFootstepsSounds.Count);
                        AudioClip a = m_SprintingFootstepsSounds[i];

                        m_SprintingFootstepsSounds[i] = m_SprintingFootstepsSounds[0];
                        m_SprintingFootstepsSounds[0] = a;

                        return a;
                    }
                    return m_SprintingFootstepsSounds[0];
                }
                return null;
            }

            public AudioClip GetRandomLandingSound ()
            {
                if (m_LandingSounds.Count > 0)
                {
                    if (m_LandingSounds.Count > 1)
                    {
                        int i = Random.Range(1, m_LandingSounds.Count);
                        AudioClip a = m_LandingSounds[i];

                        m_LandingSounds[i] = m_LandingSounds[0];
                        m_LandingSounds[0] = a;

                        return a;
                    }
                    return m_LandingSounds[0];
                }
                return null;
            }

            public Material GetRandomDecalMaterial ()
            {
                if (m_BulletImpactMaterial.Count > 0)
                {
                    if (m_BulletImpactMaterial.Count > 1)
                    {
                        int i = Random.Range(1, m_BulletImpactMaterial.Count);
                        Material m = m_BulletImpactMaterial[i];

                        m_BulletImpactMaterial[i] = m_BulletImpactMaterial[0];
                        m_BulletImpactMaterial[0] = m;

                        return m;
                    }
                    return m_BulletImpactMaterial[0];
                }
                return null;
            }

            public GameObject GetRandomImpactParticle ()
            {
                if (m_BulletImpactParticle.Count > 0)
                {
                    if (m_BulletImpactParticle.Count > 1)
                    {
                        int i = Random.Range(1, m_BulletImpactParticle.Count);
                        GameObject g = m_BulletImpactParticle[i];

                        m_BulletImpactParticle[i] = m_BulletImpactParticle[0];
                        m_BulletImpactParticle[0] = g;

                        return g;
                    }
                    return m_BulletImpactParticle[0];
                }
                return null;
            }

            public AudioClip GetRandomImpactSound ()
            {
                if (m_BulletImpactSound.Count > 0)
                {
                    if (m_BulletImpactSound.Count > 1)
                    {
                        int i = Random.Range(1, m_BulletImpactSound.Count);
                        AudioClip a = m_BulletImpactSound[i];

                        m_BulletImpactSound[i] = m_BulletImpactSound[0];
                        m_BulletImpactSound[0] = a;

                        return a;
                    }
                    return m_BulletImpactSound[0];
                }
                return null;
            }

            public float GetRandomDecalSize ()
            {
                return Random.Range(m_MinDecalSize, m_MaxDecalSize);
            }
        }
    }
}