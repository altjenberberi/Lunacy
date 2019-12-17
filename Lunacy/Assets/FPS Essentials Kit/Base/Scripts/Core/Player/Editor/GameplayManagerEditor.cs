/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;

[CustomEditor(typeof(GameplayManager))]
public sealed class GameplayManagerEditor : Editor
{
    private SerializedProperty m_InputBindings;
    private SerializedProperty m_AimStyle;
    private SerializedProperty m_CrouchStyle;
    private SerializedProperty m_SprintStyle;
    private SerializedProperty m_LeanStyle;
    private SerializedProperty m_FieldOfView;

    private SerializedProperty m_InvertHorizontalAxis;
    private SerializedProperty m_InvertVerticalAxis;

    private void OnEnable ()
    {
        m_InputBindings = serializedObject.FindProperty("m_InputBindings");
        m_AimStyle = serializedObject.FindProperty("m_AimStyle");
        m_CrouchStyle = serializedObject.FindProperty("m_CrouchStyle");
        m_SprintStyle = serializedObject.FindProperty("m_SprintStyle");
        m_LeanStyle = serializedObject.FindProperty("m_LeanStyle");
        m_FieldOfView = serializedObject.FindProperty("m_FieldOfView");

        m_InvertHorizontalAxis = serializedObject.FindProperty("m_InvertHorizontalAxis");
        m_InvertVerticalAxis = serializedObject.FindProperty("m_InvertVerticalAxis");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_FieldOfView);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_InputBindings);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_AimStyle);
        EditorGUILayout.PropertyField(m_CrouchStyle);
        EditorGUILayout.PropertyField(m_SprintStyle);
        EditorGUILayout.PropertyField(m_LeanStyle);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_InvertHorizontalAxis);
        EditorGUILayout.PropertyField(m_InvertVerticalAxis);

        serializedObject.ApplyModifiedProperties();
    }
}
