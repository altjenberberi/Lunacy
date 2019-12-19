/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using Essentials.Animation;

[CustomEditor(typeof(MotionData))]
public sealed class MotionDataEditor : Editor
{
    private SerializedProperty m_Speed;
    private SerializedProperty m_Smoothness;
    private SerializedProperty m_HorizontalAmplitude;
    private SerializedProperty m_VerticalAmplitude;
    private SerializedProperty m_VerticalAnimationCurve;
    private SerializedProperty m_RotationAmplitude;
    private SerializedProperty m_VelocityInfluence;
    private SerializedProperty m_PositionOffset;
    private SerializedProperty m_RotationOffset;

    private void OnEnable ()
    {
        m_Speed = serializedObject.FindProperty("m_Speed");
        m_Smoothness = serializedObject.FindProperty("m_Smoothness");
        m_HorizontalAmplitude = serializedObject.FindProperty("m_HorizontalAmplitude");
        m_VerticalAmplitude = serializedObject.FindProperty("m_VerticalAmplitude");
        m_VerticalAnimationCurve = serializedObject.FindProperty("m_VerticalAnimationCurve");
        m_RotationAmplitude = serializedObject.FindProperty("m_RotationAmplitude");
        m_VelocityInfluence = serializedObject.FindProperty("m_VelocityInfluence");
        m_PositionOffset = serializedObject.FindProperty("m_PositionOffset");
        m_RotationOffset = serializedObject.FindProperty("m_RotationOffset");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_Speed);
        EditorGUILayout.PropertyField(m_Smoothness);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_HorizontalAmplitude);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_VerticalAmplitude);
        EditorGUILayout.PropertyField(m_VerticalAnimationCurve);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_RotationAmplitude);
        EditorGUILayout.Space();
 
        EditorGUILayout.PropertyField(m_VelocityInfluence);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation Offset", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_PositionOffset);
        EditorGUILayout.PropertyField(m_RotationOffset);

        serializedObject.ApplyModifiedProperties();
    }
}