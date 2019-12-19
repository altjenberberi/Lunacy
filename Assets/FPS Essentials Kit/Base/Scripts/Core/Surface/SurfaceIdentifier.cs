/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using Essentials.SurfaceSystem;
using UnityEngine;

[AddComponentMenu("FPS Essentials/Surface/Surface Identifier"), DisallowMultipleComponent]
public sealed class SurfaceIdentifier : MonoBehaviour
{
    #region Surface

    [System.Serializable]
    public struct Surface
    {
        [SerializeField]
        [NotNull]
        private SurfaceType m_SurfaceType;

        [SerializeField]
        private bool m_AllowDecals;

        [SerializeField]
        private bool m_Penetration;

        [SerializeField]
        [MinMax(0, Mathf.Infinity)]
        private float m_Density;

        [SerializeField]
        private Texture m_Texture;

        public SurfaceType SurfaceInfo { get { return m_SurfaceType; } }
        public bool AllowDecals { get { return m_AllowDecals; } }
        public bool Penetration { get { return m_Penetration; } }
        public float Density { get { return m_Density; } }
        public Texture Texture { get { return m_Texture; } }

        public Surface (Texture tex)
        {
            m_SurfaceType = null;
            m_AllowDecals = true;
            m_Penetration = false;
            m_Density = 1;
            m_Texture = tex;
        }
    }

    #endregion

    // List of textures in object
    [SerializeField]
    private Surface[] m_SurfaceList = new Surface[1];

    private static Terrain ActiveTerrain { get { return Terrain.activeTerrain; } }

    public bool IsTerrain { get { return GetComponent<Terrain>() != null; } }

    #region Surface Properties

    public bool AllowDecals
    {
        get
        {
            if (m_SurfaceList != null)
            {
                if (!IsTerrain)
                {
                    return m_SurfaceList[0].AllowDecals;
                }
            }
            return false;
        }
    }

    public bool CanPenetrate
    {
        get
        {
            if (m_SurfaceList != null)
            {
                if (!IsTerrain)
                {
                    return m_SurfaceList[0].Penetration;
                }
            }
            return false;
        }
    }

    public float Density
    {
        get
        {
            if (m_SurfaceList != null)
            {
                if (!IsTerrain)
                {
                    return m_SurfaceList[0].Density;
                }
            }
            return 1;
        }
    }

    #endregion

    // Get surface of the object or of the terrain at given position
    public SurfaceType GetSurfaceData (Vector3 position)
    {
        if (m_SurfaceList != null && m_SurfaceList.Length > 0)
        {
            return IsTerrain ? m_SurfaceList[SurfaceUtility.GetMainTexture(position, ActiveTerrain.transform.position, ActiveTerrain.terrainData)].SurfaceInfo : m_SurfaceList[0].SurfaceInfo;
        }
        return null;
    }

    // Get surface of the object or of the terrain at given position
    public SurfaceType GetSurfaceData ()
    {
        if (m_SurfaceList != null && m_SurfaceList.Length > 0)
        {
            return m_SurfaceList[0].SurfaceInfo;
        }
        return null;
    }

    public void Reset ()
    {
        m_SurfaceList = SetSurfaceList();
    }

    // Returns all textures applied to the object, be it a mesh or a terrain
    private Surface[] SetSurfaceList ()
    {
        Surface[] surfaces;

        // Is this component attached to a terrain?
        if (IsTerrain)
        {
            SplatPrototype[] splatPrototypes = ActiveTerrain.terrainData.splatPrototypes;
            surfaces = new Surface[splatPrototypes.Length];

            for (int i = 0; i < splatPrototypes.Length; i++)
                surfaces[i] = new Surface(splatPrototypes[i].texture);
        }
        else
        {
            surfaces = new Surface[1];
            Renderer renderer = gameObject.GetComponent<Renderer>();

            if (renderer != null && renderer.sharedMaterial.mainTexture != null)
            {
                surfaces[0] = new Surface(renderer.sharedMaterial.mainTexture);
            }
            else
            {
                surfaces[0] = new Surface(null);
            }
        }
        return surfaces;
    }
}
