/*
Copyright (c) 2010, Raphael Lopes Baldi & Aquiris Game Experience LTDA.
See the document "TERMS OF USE" included in the project folder for licencing details.
*/

using System.Collections.Generic;
using UnityEngine;

public enum DecalMode
{
    MeshCollider,
    MeshFilter
}

public sealed class Decal : MonoBehaviour
{
    //The gameObjects that will be affected by the decal.
    private GameObject[] m_AffectedObjects;

    public float maxAngle = 90.0f;

    private float m_AngleCosine;

    #region STATE PROPERTIES

    [HideInInspector]
    private float m_PreviousUVAngle;
    private float m_PreviousMaxAngle;

    private Vector3 m_PreviousPosition;
    private Quaternion m_PreviousRotation;
    private Vector3 m_PreviousScale;

    private float m_PreviousPushDistance = 0.009f;

    private Vector2 m_PreviousTiling;
    private Vector2 m_PreviousOffset;

    #endregion

    #region UV PROPERTIES

    public Vector2 tiling = Vector2.one;
    public Vector2 offset = Vector2.zero;

    private float uvAngle = 0.0f;
    private float uCos;
    private float vSin;

    #endregion

    public Material decalMaterial;

    [HideInInspector]
    public DecalMode decalMode;

    #region DECAL CREATION PROPERTIES

    private List<DecalPolygon> m_StartPolygons;
    private List<DecalPolygon> m_ClippedPolygons;

    private Vector4 m_BottomPlane;
    private Vector4 m_TopPlane;
    private Vector4 m_LeftPlane;
    private Vector4 m_RightPlane;
    private Vector4 m_FrontPlane;
    private Vector4 m_BackPlane;

    private Vector3 m_DecalNormal;
    private Vector3 m_DecalCenter;
    private Vector3 m_DecalTangent;
    private Vector3 m_DecalBinormal;
    private Vector3 m_DecalSize;

    #endregion

    [MinMax(0.001f, Mathf.Infinity)]
    public float pushDistance = 0.009f;

    private List<MeshCombineUtility.MeshInstance> m_InstancesList;

    public LayerMask affectedLayers = 1;

    public bool checkAutomatically;
    public bool affectOtherDecals;
    public bool affectInactiveRenderers;

    #region INSPECTOR PROPERTIES

    public Bounds Bounds { get; private set; }

    #endregion

    public void SetAffectedObject (GameObject affectedObjects)
    {
        m_AffectedObjects = new GameObject[1];
        m_AffectedObjects[0] = affectedObjects;
    }

    public void SetAffectedObjects (GameObject[] affectedObjects)
    {
        m_AffectedObjects = new GameObject[affectedObjects.Length];

        for (int i = 0; i < affectedObjects.Length; i++)
        {
            m_AffectedObjects[i] = affectedObjects[i];
        }
    }

    public void ResetAffectedObjects ()
    {
        m_AffectedObjects = null;
    }

    //Method responsible for calculating decal's bounds, used to detect the
    //affected objects.
    public void CalculateBounds ()
    {
        //Extend decal's bounds a little in order to make sure that everything will
        //still working when the projector is rotated.
        Bounds = new Bounds(transform.position, transform.lossyScale * 1.414214f);
    }

    //Method responsible for constructing the Decal Mesh, based on the affected objects.
    public void CalculateDecal ()
    {
        ClearDecals();

        maxAngle = Mathf.Clamp(maxAngle, 0.0f, 180.0f);
        m_AngleCosine = Mathf.Cos(maxAngle * Mathf.Deg2Rad);

        uvAngle = Mathf.Clamp(uvAngle, 0.0f, 360.0f);
        uCos = Mathf.Cos(uvAngle * Mathf.Deg2Rad);
        vSin = Mathf.Sin(uvAngle * Mathf.Deg2Rad);

        if (m_AffectedObjects == null || m_AffectedObjects.Length <= 0)
        {
            //Debug.LogWarning("No object will be affected. Decal will not be calculated.");
            return;
        }

        m_InstancesList = new List<MeshCombineUtility.MeshInstance>();

        for (int i = 0; i < m_AffectedObjects.Length; i++)
        {
            if (m_AffectedObjects[i] == null)
                continue;

            CalculateObjectDecal(m_AffectedObjects[i]);
        }

        if (m_InstancesList.Count > 0)
        {
            MeshCombineUtility.MeshInstance[] instances = new MeshCombineUtility.MeshInstance[m_InstancesList.Count];
            for (int i = 0; i < instances.Length; i++)
            {
                instances[i] = m_InstancesList[i];
            }

            MeshRenderer r = gameObject.GetComponent<MeshRenderer>();
            if (r == null)
            {
                r = gameObject.AddComponent<MeshRenderer>();
            }

            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.material = decalMaterial;

            MeshFilter fi = gameObject.GetComponent<MeshFilter>();

            if (fi == null)
            {
                fi = gameObject.AddComponent<MeshFilter>();
            }
            else
            {
                DestroyImmediate(fi.sharedMesh);
            }

            Mesh finalMesh = MeshCombineUtility.Combine(instances, true);

            if (pushDistance > 0.0f)
            {
                List<List<int>> relations = new List<List<int>>();
                Vector3[] vert = finalMesh.vertices;
                Vector3[] normals = finalMesh.normals;

                bool[] usedIndex = new bool[vert.Length];
                for (int i = 0; i < usedIndex.Length; i++)
                {
                    usedIndex[i] = false;
                }

                for (int i = 0; i < vert.Length; i++)
                {
                    if (usedIndex[i])
                        continue;

                    List<int> c = new List<int>();
                    c.Add(i);

                    usedIndex[i] = true;

                    for (int j = i + 1; j < vert.Length; j++)
                    {
                        if (usedIndex[j])
                            continue;

                        if (Vector3.Distance(vert[i], vert[j]) < 0.001f)
                        {
                            c.Add(j);

                            usedIndex[j] = true;
                        }
                    }

                    relations.Add(c);
                }

                foreach (List<int> l in relations)
                {
                    Vector3 nNormal = Vector3.zero;
                    foreach (int i in l)
                    {
                        nNormal += normals[i];
                    }

                    nNormal = (nNormal / l.Count).normalized;

                    foreach (int i in l)
                    {
                        vert[i] += nNormal * (pushDistance);
                    }
                }

                finalMesh.vertices = vert;
            }

            finalMesh.name = "DecalMesh";

            fi.mesh = finalMesh;

            for (int i = 0; i < m_InstancesList.Count; i++)
            {
                DestroyImmediate(m_InstancesList[i].mesh);
            }
        }

        m_InstancesList.Clear();
        m_InstancesList = null;
    }

    public void ClearDecals ()
    {
        MeshFilter auxFilter = gameObject.GetComponent<MeshFilter>();

        if (auxFilter != null)
        {
            DestroyImmediate(auxFilter.sharedMesh);
            DestroyImmediate(auxFilter);
        }

        MeshRenderer auxRenderer = gameObject.GetComponent<MeshRenderer>();

        if (auxRenderer != null)
        {
            DestroyImmediate(auxRenderer);
        }
    }

    public void CalculateObjectDecal (GameObject obj)
    {
        Mesh m = null;

        if (decalMode == DecalMode.MeshCollider)
        {
            if (obj.GetComponent<MeshCollider>() != null)
            {
                m = obj.GetComponent<MeshCollider>().sharedMesh;
            }
            else
            {
                m = null;
            }
        }

        if (m == null || decalMode == DecalMode.MeshFilter)
        {
            if (obj.GetComponent<MeshFilter>() == null)
                return;

            m = obj.GetComponent<MeshFilter>().sharedMesh;
        }

        if (m == null || m.name.ToLower().Contains("combined") || m.tangents.Length == 0)
        {
            return;
        }

        m_DecalNormal = obj.transform.InverseTransformDirection(transform.forward);
        m_DecalCenter = obj.transform.InverseTransformPoint(transform.position);
        m_DecalTangent = obj.transform.InverseTransformDirection(transform.right);
        m_DecalBinormal = obj.transform.InverseTransformDirection(transform.up);
        m_DecalSize = new Vector3(transform.lossyScale.x / obj.transform.lossyScale.x, transform.lossyScale.y / obj.transform.lossyScale.y, transform.lossyScale.z / obj.transform.lossyScale.z);//transf.MultiplyPoint(transform.lossyScale);

        m_BottomPlane = new Vector4(-m_DecalBinormal.x, -m_DecalBinormal.y, -m_DecalBinormal.z, (m_DecalSize.y * 0.5f) + Vector3.Dot(m_DecalCenter, m_DecalBinormal));
        m_TopPlane = new Vector4(m_DecalBinormal.x, m_DecalBinormal.y, m_DecalBinormal.z, (m_DecalSize.y * 0.5f) - Vector3.Dot(m_DecalCenter, m_DecalBinormal));
        m_RightPlane = new Vector4(-m_DecalTangent.x, -m_DecalTangent.y, -m_DecalTangent.z, (m_DecalSize.x * 0.5f) + Vector3.Dot(m_DecalCenter, m_DecalTangent));
        m_LeftPlane = new Vector4(m_DecalTangent.x, m_DecalTangent.y, m_DecalTangent.z, (m_DecalSize.x * 0.5f) - Vector3.Dot(m_DecalCenter, m_DecalTangent));
        m_FrontPlane = new Vector4(m_DecalNormal.x, m_DecalNormal.y, m_DecalNormal.z, (m_DecalSize.z * 0.5f) - Vector3.Dot(m_DecalCenter, m_DecalNormal));
        m_BackPlane = new Vector4(-m_DecalNormal.x, -m_DecalNormal.y, -m_DecalNormal.z, (m_DecalSize.z * 0.5f) + Vector3.Dot(m_DecalCenter, m_DecalNormal));

        int[] triangles = m.triangles;
        Vector3[] vertices = m.vertices;
        Vector3[] normals = m.normals;
        Vector4[] tangents = m.tangents;

        float dot;
        int i1, i2, i3;
        Vector3 v1, v2, v3;
        Vector3 n1;
        Vector3 t1, t2, t3;

        DecalPolygon t;
        m_StartPolygons = new List<DecalPolygon>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            i1 = triangles[i];
            i2 = triangles[i + 1];
            i3 = triangles[i + 2];

            v1 = vertices[i1];
            v2 = vertices[i2];
            v3 = vertices[i3];

            n1 = normals[i1];

            dot = Vector3.Dot(n1, -m_DecalNormal);

            if (dot <= m_AngleCosine)
                continue;

            t1 = tangents[i1];
            t2 = tangents[i2];
            t3 = tangents[i3];

            t = new DecalPolygon
            {
                verticeCount = 3
            };

            t.vertice[0] = v1;
            t.vertice[1] = v2;
            t.vertice[2] = v3;

            t.normal[0] = n1;
            t.normal[1] = n1;
            t.normal[2] = n1;

            t.tangent[0] = t1;
            t.tangent[1] = t2;
            t.tangent[2] = t3;

            m_StartPolygons.Add(t);
        }

        Mesh aux = CreateMesh(ClipMesh());

        if (aux != null)
        {
            MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance
            {
                mesh = aux,
                subMeshIndex = 0,
                transform = transform.worldToLocalMatrix * obj.transform.localToWorldMatrix
            };
            m_InstancesList.Add(instance);
        }
       
        m_StartPolygons.Clear();
        m_StartPolygons = null;
        m_ClippedPolygons.Clear();
        m_ClippedPolygons = null;
    }

    private Mesh CreateMesh (int totalVertices)
    {
        if (m_ClippedPolygons == null)
            return null;

        if (m_ClippedPolygons.Count <= 0)
            return null;

        if (totalVertices < 3)
            return null;

        int[] newTris = new int[(totalVertices - 2) * 3];

        Vector3[] newVertices = new Vector3[totalVertices];
        Vector3[] newNormals = new Vector3[totalVertices];
        Vector2[] newUv = new Vector2[totalVertices];
        Vector4[] newTangents = new Vector4[totalVertices];

        int count = 0;
        int trisCount = 0;
        int oCount = 0;

        float u, v, tempU, tempV;
        Vector3 dir;

        float one_over_w = 1.0f / m_DecalSize.x;
        float one_over_h = 1.0f / m_DecalSize.y;

        foreach (DecalPolygon p in m_ClippedPolygons)
        {
            for (int i = 0; i < p.verticeCount; i++)
            {
                newVertices[count] = p.vertice[i];
                newNormals[count] = p.normal[i];

                newTangents[count] = p.tangent[i];

                if (i < p.verticeCount - 2)
                {
                    newTris[trisCount] = oCount;
                    newTris[trisCount + 1] = count + 1;
                    newTris[trisCount + 2] = count + 2;
                    trisCount += 3;
                }

                count++;
            }
            oCount = count;
        }

        for (int i = 0; i < newVertices.Length; i++)
        {
            dir = newVertices[i] - m_DecalCenter;

            tempU = (Vector3.Dot(dir, m_DecalTangent) * one_over_w);// + 0.5f);// * tiling.x;// + offset.x;
            tempV = (Vector3.Dot(dir, m_DecalBinormal) * one_over_h);// + 0.5f);// * tiling.y;// + offset.y;		

            u = tempU * uCos - tempV * vSin + 0.5f;
            v = tempU * vSin + tempV * uCos + 0.5f;

            u *= tiling.x;
            v *= tiling.y;

            u += offset.x;
            v += offset.y;

            newUv[i] = new Vector2(u, v);// * vSin);
        }

        Mesh m = new Mesh
        {
            vertices = newVertices,
            normals = newNormals,
            triangles = newTris,
            uv = newUv,
            uv2 = newUv
        };
        m.uv2 = newUv;

        m.tangents = newTangents;

        return m;
    }

    private int ClipMesh ()
    {
        DecalPolygon tempFace, face;

        int totalVertices = 0;

        if (m_ClippedPolygons == null)
            m_ClippedPolygons = new List<DecalPolygon>();
        else
            m_ClippedPolygons.Clear();

        for (int i = 0; i < m_StartPolygons.Count; i++)
        {
            face = m_StartPolygons[i];

            tempFace = DecalPolygon.ClipPolygonAgainstPlane(face, m_FrontPlane);
            if (tempFace != null)
            {
                tempFace = DecalPolygon.ClipPolygonAgainstPlane(tempFace, m_BackPlane);
                if (tempFace != null)
                {
                    tempFace = DecalPolygon.ClipPolygonAgainstPlane(tempFace, m_RightPlane);
                    if (tempFace != null)
                    {
                        tempFace = DecalPolygon.ClipPolygonAgainstPlane(tempFace, m_LeftPlane);
                        if (tempFace != null)
                        {
                            tempFace = DecalPolygon.ClipPolygonAgainstPlane(tempFace, m_BottomPlane);
                            if (tempFace != null)
                            {
                                tempFace = DecalPolygon.ClipPolygonAgainstPlane(tempFace, m_TopPlane);
                                if (tempFace != null)
                                {
                                    totalVertices += tempFace.verticeCount;
                                    m_ClippedPolygons.Add(tempFace);
                                }
                            }
                        }
                    }
                }
            }
        }
        return totalVertices;
    }

    //Has the Decal changed since the last call to this method?
    public bool HasChanged ()
    {
        Transform t = transform;
        bool changed = false;
        maxAngle = Mathf.Clamp(maxAngle, 0.0f, 180.0f);
        uvAngle = Mathf.Clamp(uvAngle, 0.0f, 360.0f);

        if (m_PreviousPosition != t.position)
        {
            changed = true;
        }
        else if (m_PreviousScale != t.lossyScale)
        {
            changed = true;
        }
        else if (m_PreviousRotation != t.rotation)
        {
            changed = true;
        }
        else if (Mathf.Abs(m_PreviousPushDistance - pushDistance) > Mathf.Epsilon)
        {
            changed = true;
        }
        else if (m_PreviousTiling != tiling)
        {
            changed = true;
        }
        else if (m_PreviousOffset != offset)
        {
            changed = true;
        }
        else if (Mathf.Abs(m_PreviousMaxAngle - maxAngle) > Mathf.Epsilon)
        {
            changed = true;
        }
        else changed |= Mathf.Abs(m_PreviousUVAngle - uvAngle) > Mathf.Epsilon;

        m_PreviousUVAngle = uvAngle;
        m_PreviousMaxAngle = maxAngle;
        m_PreviousTiling = tiling;
        m_PreviousOffset = offset;
        m_PreviousPushDistance = pushDistance;
        m_PreviousPosition = t.position;
        m_PreviousRotation = t.rotation;
        m_PreviousScale = t.lossyScale;

        return changed;
    }

    private void OnDrawGizmosSelected ()
    {
        //Calculate current decal bounds.
        CalculateBounds();

        //Draw the helper gizmos with the correct object matrix.
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}