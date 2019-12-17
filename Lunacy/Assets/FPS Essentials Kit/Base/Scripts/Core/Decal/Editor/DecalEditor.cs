// DecalEditor.cs
//
// Author:
//      Jackson Medeiros <jackmdros@gmail.com>
//      Raphael Lopes Baldi
//
// Copyright (c) 2017 The Asset Lab & Aquiris Game Experience LTDA. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Decal))]
public sealed class DecalEditor : Editor
{
    private Decal m_Target;
    private SerializedProperty m_MaxAngle;
    private SerializedProperty m_Tiling;
    private SerializedProperty m_Offset;
    private SerializedProperty m_DecalMaterial;
    private SerializedProperty m_PushDistance;
    private SerializedProperty m_AffectedLayers;
    private SerializedProperty m_CheckAutomatically;
    private SerializedProperty m_AffectOtherDecals;
    private SerializedProperty m_AffectInactiveRenderers;

    private void OnEnable ()
    {
        m_Target = (Decal)target;

        m_MaxAngle = serializedObject.FindProperty("maxAngle");
        m_Tiling = serializedObject.FindProperty("tiling");
        m_Offset = serializedObject.FindProperty("offset");
        m_DecalMaterial = serializedObject.FindProperty("decalMaterial");
        m_PushDistance = serializedObject.FindProperty("pushDistance");
        m_AffectedLayers = serializedObject.FindProperty("affectedLayers");
        m_CheckAutomatically = serializedObject.FindProperty("checkAutomatically");
        m_AffectOtherDecals = serializedObject.FindProperty("affectOtherDecals");
        m_AffectInactiveRenderers = serializedObject.FindProperty("affectInactiveRenderers");
    }

    private void OnSceneGUI ()
    {
        Handles.ArrowHandleCap(0, m_Target.transform.position, m_Target.transform.rotation, 1, EventType.Repaint);

        //Will only calculate changes and process the new Decal
        //on Repaint and MouseDrag events.
        //MouseDrag will allow us to handle changes made to scene gizmos, while 
        //Repaint will allow us to handle changes made to the Inspector.

        //If the user want the objects to be updated, let's do it.
        bool hasChanged = m_Target.HasChanged();

        if (!hasChanged && ((Event.current.type != EventType.MouseDrag && Event.current.type != EventType.Repaint) || Event.current.modifiers != 0))
            return;

        if (hasChanged && m_CheckAutomatically.boolValue)
        {
            m_Target.decalMode = DecalMode.MeshFilter;

            GetAffectedObjects(m_Target);
            m_Target.CalculateDecal();

            GUIUtility.ExitGUI();
        }
    }

    //Draw custom Inspector.
    public override void OnInspectorGUI ()
    {
        //Update the serializedProperty - always do this in the beginning of OnInspectorGUI
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_MaxAngle);
        EditorGUILayout.PropertyField(m_PushDistance);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_DecalMaterial);
        EditorGUILayout.PropertyField(m_Tiling);
        EditorGUILayout.PropertyField(m_Offset);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_AffectedLayers);
        EditorGUILayout.PropertyField(m_CheckAutomatically);
        EditorGUILayout.PropertyField(m_AffectOtherDecals);
        EditorGUILayout.PropertyField(m_AffectInactiveRenderers);
        EditorGUILayout.Space();

        if (m_Target.decalMaterial != null)
        {
            if (m_Target.decalMaterial.mainTexture != null)
            {
                if (GUILayout.Button("Edit UV"))
                {
                    DecalUVEditorWindow.Initialize(m_Target);
                }

                if (GUILayout.Button("Calculate Decal"))
                {
                    m_Target.decalMode = DecalMode.MeshFilter;

                    GetAffectedObjects(m_Target);
                    m_Target.CalculateDecal();

                    GUIUtility.ExitGUI();
                    EditorSceneManager.MarkSceneDirty(m_Target.gameObject.scene);
                }
            }
        }

        //Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI
        serializedObject.ApplyModifiedProperties();
    }

    //Get the objects that will be affected by the decal.
    private void GetAffectedObjects (Decal decal)
    {
        int affectedLayers = decal.affectedLayers;

        decal.ResetAffectedObjects();

        if (affectedLayers == 0)
            return;

        Renderer[] renderers = (Renderer[])FindObjectsOfType(typeof(Renderer));

        if (renderers != null)
        {
            Bounds decalBounds = decal.Bounds;

            List<GameObject> affectedObjects = new List<GameObject>();

            int mLength = renderers.Length;

            GameObject decalGO = decal.gameObject;
            GameObject auxGO;
            int cLayer;

            if (mLength > 0)
            {
                for (int i = 0; i < mLength; i++)
                {
                    auxGO = renderers[i].gameObject;

                    //Do not affect the projector
                    if (auxGO == decalGO)
                        continue;

                    //Layer check.
                    //-1 means everything will be affected, so we don't need to check this case.
                    if (affectedLayers != -1)
                    {
                        cLayer = 1 << auxGO.layer;

                        if ((cLayer & affectedLayers) == 0)
                            continue;
                    }

                    if (!decal.affectOtherDecals)
                    {
                        if (auxGO.GetComponent<Decal>() != null)
                            continue;
                    }

                    if (!decal.affectInactiveRenderers)
                    {
                        if (!renderers[i].enabled)
                            continue;
                    }

                    Bounds b = renderers[i].bounds;

                    if (decalBounds.Intersects(b))
                    {
                        affectedObjects.Add(auxGO);
                    }
                }

                decal.SetAffectedObjects(affectedObjects.ToArray());

                affectedObjects.Clear();
            }
        }

        if (decal.gameObject.scene.IsValid())
        {
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(decal.gameObject.scene);
        }
        else
        {
            EditorUtility.SetDirty(decal.gameObject);
        }
    }
}