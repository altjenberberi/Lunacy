/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using Essentials.Controllers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthController))]
public sealed class HealthControllerEditor : Editor
{
    private HealthController m_Target;
    private SerializedProperty m_Life;
    private SerializedProperty m_Regenerate;
    private SerializedProperty m_StartDelay;
    private SerializedProperty m_RecoverSpeed;

    private SerializedProperty m_HitSounds;
    private SerializedProperty m_HitVolume;
    private SerializedProperty m_BreakLegsSound;
    private SerializedProperty m_BreakLegsVolume;
    private SerializedProperty m_ExplosionNoiseVolume;
    private SerializedProperty m_HealSound;
    private SerializedProperty m_HealVolume;

    private SerializedProperty m_ExplosionNoise;
    private SerializedProperty m_NormalSnapshot;
    private SerializedProperty m_StunnedSnapshot;

    private SerializedProperty m_DeadPlayer;

    private void OnEnable ()
    {
        m_Target = (target as HealthController);
        m_Life = serializedObject.FindProperty("m_Life");
        m_Regenerate = serializedObject.FindProperty("m_Regenerate");
        m_StartDelay = serializedObject.FindProperty("m_StartDelay");
        m_RecoverSpeed = serializedObject.FindProperty("m_RegenerateSpeed");
        m_HitSounds = serializedObject.FindProperty("m_HitSounds");
        m_HitVolume = serializedObject.FindProperty("m_HitVolume");
        m_BreakLegsSound = serializedObject.FindProperty("m_BreakLegsSound");
        m_BreakLegsVolume = serializedObject.FindProperty("m_BreakLegsVolume");
        m_HealSound = serializedObject.FindProperty("m_HealSound");
        m_HealVolume = serializedObject.FindProperty("m_HealVolume");

        m_ExplosionNoise = serializedObject.FindProperty("m_ExplosionNoise");
        m_ExplosionNoiseVolume = serializedObject.FindProperty("m_ExplosionNoiseVolume");
        m_NormalSnapshot = serializedObject.FindProperty("m_NormalSnapshot");
        m_StunnedSnapshot = serializedObject.FindProperty("m_StunnedSnapshot");

        m_DeadPlayer = serializedObject.FindProperty("m_DeadPlayer");
    }

    public override void OnInspectorGUI ()
    {
        //Update the serializedProperty - always do this in the beginning of OnInspectorGUI
        serializedObject.Update();

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Health Settings", m_Life);

        if (m_Life.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_Life);
            EditorGUILayout.PropertyField(m_Regenerate);

            if (m_Regenerate.boolValue)
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.PropertyField(m_StartDelay);
                EditorGUILayout.PropertyField(m_RecoverSpeed);
            }

            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_DeadPlayer);
        }

        EditorGUIHelper.FoldoutHeader("Sounds", m_HitSounds);

        if (m_HitSounds.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
            {
                EditorGUILayout.LabelField("Hit Sounds", FPSEStyles.midLabel);

                if (m_HitSounds.arraySize == 0)
                {
                    m_Target.AddHitSound();
                    return;
                }

                for (int i = 0; i < m_HitSounds.arraySize; i++)
                {
                    SerializedProperty sound = m_HitSounds.GetArrayElementAtIndex(i);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PropertyField(sound, new GUIContent(i == 0 ? "Hit Sound" : "Hit Sound " + i));

                        using (new EditorGUI.DisabledGroupScope(m_HitSounds.arraySize == 1))
                        {
                            EditorGUI.BeginChangeCheck();
                            if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
                            {
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(m_Target, "Undo Inspector");
                                    m_Target.RemoveHitSound(i);
                                    EditorUtility.SetDirty(m_Target);
                                    return;
                                }
                            }
                        }

                        EditorGUI.BeginChangeCheck();
                        if (GUILayout.Button("+", FPSEStyles.rightButton, GUILayout.Width(24)))
                        {
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(m_Target, "Undo Inspector");
                                m_Target.AddHitSound();

                                EditorUtility.SetDirty(m_Target);
                                return;
                            }
                        }
                    }
                }
            }
            EditorGUILayout.PropertyField(m_HitVolume);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_HealSound);
            EditorGUILayout.PropertyField(m_HealVolume);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_BreakLegsSound);
            EditorGUILayout.PropertyField(m_BreakLegsVolume);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_ExplosionNoise);
            EditorGUILayout.PropertyField(m_ExplosionNoiseVolume);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_NormalSnapshot);
            EditorGUILayout.PropertyField(m_StunnedSnapshot);
        }

        //Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI
        serializedObject.ApplyModifiedProperties();
    }
}

