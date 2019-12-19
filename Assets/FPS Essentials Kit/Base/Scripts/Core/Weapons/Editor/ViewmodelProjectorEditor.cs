/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;
using Essentials.Weapons;

[CustomEditor(typeof(ViewmodelProjector))]
public class ViewmodelProjectorEditor : Editor
{
    private SerializedProperty m_Aspect;

    string[] names = new string[] { "Automatic", "4:3", "5:4", "16:10", "16:9" };
    int[] values = { 0, 1, 2, 3, 4 };

    private SerializedProperty m_FieldOfView;
    private SerializedProperty m_ZNear;
    private SerializedProperty m_ZFar;

    private void OnEnable ()
    {
        m_Aspect = serializedObject.FindProperty("m_Aspect");
        m_FieldOfView = serializedObject.FindProperty("m_FieldOfView");
        m_ZNear = serializedObject.FindProperty("m_ZNear");
        m_ZFar = serializedObject.FindProperty("m_ZFar");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        m_Aspect.intValue = EditorGUILayout.IntPopup(m_Aspect.displayName, m_Aspect.intValue, names, values);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_FieldOfView);

        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Clipping Planes", EditorStyles.boldLabel);

        EditorGUI.indentLevel = 1;
        EditorGUILayout.PropertyField(m_ZNear, new GUIContent("Near"));
        EditorGUILayout.PropertyField(m_ZFar, new GUIContent("Far"));
        EditorGUI.indentLevel = 0;

        serializedObject.ApplyModifiedProperties();
    }
}

