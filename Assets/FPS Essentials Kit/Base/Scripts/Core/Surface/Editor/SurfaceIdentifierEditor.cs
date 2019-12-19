/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SurfaceIdentifier)), CanEditMultipleObjects]
public sealed class SurfaceIdentifierEditor : Editor
{
    private SurfaceIdentifier m_Target;
    private SerializedProperty m_SurfaceList;

    private void OnEnable ()
    {
        m_Target = (target as SurfaceIdentifier);
        m_SurfaceList = serializedObject.FindProperty("m_SurfaceList");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope(FPSEStyles.header))
        {
            GUILayout.Label("Texture List");
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Update Surface", FPSEStyles.miniButton))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.Reset();

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }
        }

        for (int i = 0; i < m_SurfaceList.arraySize; i++)
        {
            DrawSurfaceData(i);
        }

        if (m_Target.IsTerrain)
        {
            EditorGUILayout.HelpBox("Can not generate decals on terrain!", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSurfaceData (int index)
    {
        SerializedProperty surface = m_SurfaceList.GetArrayElementAtIndex(index);

        SerializedProperty surfaceType = surface.FindPropertyRelative("m_SurfaceType");
        SerializedProperty allowDecals = surface.FindPropertyRelative("m_AllowDecals");
        SerializedProperty penetration = surface.FindPropertyRelative("m_Penetration");
        SerializedProperty density = surface.FindPropertyRelative("m_Density");

        using (new EditorGUILayout.HorizontalScope(FPSEStyles.background))
        {
            Texture2D texture = (Texture2D)surface.FindPropertyRelative("m_Texture").objectReferenceValue
                                ?? EditorGUIHelper.Load<Texture2D>("/Surface/Editor Resources/", "NoTexture.png");
                
            GUILayout.Label(texture, GUILayout.Height(64), GUILayout.Width(64));
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.PropertyField(surfaceType);

                if (surfaceType.objectReferenceValue != null)
                {
                    if (!m_Target.IsTerrain)
                    {
                        EditorGUILayout.PropertyField(allowDecals);
                        EditorGUILayout.PropertyField(penetration);
                    }

                    if (penetration.boolValue)
                    {
                        EditorGUILayout.PropertyField(density);
                    }
                }
            }
        }
    }
}
