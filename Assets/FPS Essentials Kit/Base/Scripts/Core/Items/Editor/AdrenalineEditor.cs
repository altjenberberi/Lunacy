/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using Essentials.Items;

[CustomEditor(typeof(Adrenaline))]
public sealed class AdrenalineEditor : Editor
{
    private Adrenaline m_Target;
    private SerializedProperty m_HealthController;
    private SerializedProperty m_HealAmount;
    private SerializedProperty m_DelayToInject;
    private SerializedProperty m_StaminaBonusDuration;
    private SerializedProperty m_Amount;
    private SerializedProperty m_InfiniteShots;
    private SerializedProperty m_MaxAmount;
    private SerializedProperty m_Animator;
    private SerializedProperty m_ShotAnimation;
    private SerializedProperty m_ShotSound;
    private SerializedProperty m_ShotVolume;

    private void OnEnable ()
    {
        m_Target = (target as Adrenaline);
        m_Target.DisableShadowCasting();

        m_HealthController = serializedObject.FindProperty("m_HealthController");
        m_HealAmount = serializedObject.FindProperty("m_HealAmount");
        m_DelayToInject = serializedObject.FindProperty("m_DelayToInject");
        m_StaminaBonusDuration = serializedObject.FindProperty("m_StaminaBonusDuration");
        m_Amount = serializedObject.FindProperty("m_Amount");
        m_InfiniteShots = serializedObject.FindProperty("m_InfiniteShots");
        m_MaxAmount = serializedObject.FindProperty("m_MaxAmount");
        m_Animator = serializedObject.FindProperty("m_Animator");
        m_ShotAnimation = serializedObject.FindProperty("m_ShotAnimation");
        m_ShotSound = serializedObject.FindProperty("m_ShotSound");
        m_ShotVolume = serializedObject.FindProperty("m_ShotVolume");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_HealthController);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_HealAmount);
        EditorGUILayout.PropertyField(m_DelayToInject);
        EditorGUILayout.PropertyField(m_StaminaBonusDuration);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_InfiniteShots);

        EditorGUI.indentLevel = 0;
        if (!m_InfiniteShots.boolValue)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_Amount);
            EditorGUILayout.PropertyField(m_MaxAmount);
        }

        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Adrenaline Animations", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_Animator);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_ShotAnimation);
        EditorGUILayout.PropertyField(m_ShotSound);
        EditorGUILayout.PropertyField(m_ShotVolume);

        serializedObject.ApplyModifiedProperties();
    }
}

