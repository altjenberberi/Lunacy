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
        [AddComponentMenu("FPS Essentials/Managers/Bullet Marks Manager"), DisallowMultipleComponent]
        public class BulletMarkManager : MonoBehaviour
        {
            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected int m_MaxDecals = 100;

			[SerializeField]
			[Range(0, 1)]
			protected float m_DecalSeparator = 0.1f;

            protected List<GameObject> m_DecalList = new List<GameObject>();
            protected List<float> m_PushDistances = new List<float>();

            public virtual void CreateBulletMark (SurfaceIdentifier surf, RaycastHit hitInfo, LayerMask affectedLayers)
            {
                if (surf != null)
                {
                    if (surf.AllowDecals && m_MaxDecals > 0)
                    {
                        Material material = surf.GetSurfaceData(hitInfo.point).GetRandomDecalMaterial();
                        if (material != null)
                        {
                            GameObject decal = new GameObject("BulletMark");
                            decal.transform.position = hitInfo.point;
                            decal.transform.rotation = Quaternion.FromToRotation(Vector3.back, hitInfo.normal);

                            decal.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));

                            float scale = surf.GetSurfaceData(hitInfo.point).GetRandomDecalSize() / 5;
                            decal.transform.localScale = new Vector3(scale, scale, scale);

                            decal.transform.parent = hitInfo.transform;

                            Decal d = decal.AddComponent<Decal>();
                            d.affectedLayers = affectedLayers;
                            d.SetAffectedObject(hitInfo.collider.gameObject);
                            d.decalMode = DecalMode.MeshCollider;
                            d.maxAngle = 60;
                            d.pushDistance = 0.009f + RegisterDecal(decal, scale);
                            d.decalMaterial = material;
                            d.CalculateDecal();
                        }
                    }

                    GameObject particle = surf.GetSurfaceData(hitInfo.point).GetRandomImpactParticle();
                    if (particle != null)
                    {
                        GameObject impactParticle = Instantiate(particle, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
                        impactParticle.AddComponent<ParticleDestroyer>();
                    }

                    AudioClip clip = surf.GetSurfaceData(hitInfo.point).GetRandomImpactSound();
                    if (clip != null)
                    {
                        AudioManager.Instance.PlayClipAtPoint(clip, hitInfo.point, 1, 25, 0.2f);
                    }
                }
            }

            protected virtual float RegisterDecal (GameObject decal, float scale)
            {
                GameObject auxGO;
                Transform auxT;
                Transform currentT = decal.transform;
                Vector3 currentPos = currentT.position;

                float radius = Mathf.Sqrt((scale * scale * 0.25f) * 3);

                float realRadius = radius * 2;
				radius *= m_DecalSeparator;

                if (m_DecalList.Count == m_MaxDecals)
                {
                    auxGO = m_DecalList[0];
                    Destroy(auxGO);
                    m_DecalList.RemoveAt(0);
                    m_PushDistances.RemoveAt(0);
                }

                float distance;
                float pushDistance = 0;

                for (int i = 0; i < m_DecalList.Count; i++)
                {
                    auxGO = m_DecalList[i];

                    if (auxGO != null)
                    {
                        auxT = auxGO.transform;
                        distance = (auxT.position - currentPos).magnitude;

                        if (distance < radius)
                        {
                            Destroy(auxGO);
                            m_DecalList.RemoveAt(i);
                            m_PushDistances.RemoveAt(i);
                            i--;
                        }
                        else if (distance < realRadius)
                        {
                            float cDist = m_PushDistances[i];
                            pushDistance = Mathf.Max(pushDistance, cDist);
                        }
                    }
                    else
                    {
                        m_DecalList.RemoveAt(i);
                        m_PushDistances.RemoveAt(i);
                        i--;
                    }
                }

                pushDistance += 0.001f;

                m_DecalList.Add(decal);
                m_PushDistances.Add(pushDistance);

                return pushDistance;
            }

            public virtual void RemoveDecals ()
            {
                if (m_DecalList.Count > 0)
                {
                    for (int i = 0; i < m_DecalList.Count; i++)
                    {
                        GameObject go = m_DecalList[i];
                        Destroy(go);
                    }
                    m_DecalList.Clear();
                    m_PushDistances.Clear();
                }
            }
        }
    }
}