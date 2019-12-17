/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;
using UnityEditor;
using Essentials.SurfaceSystem;

[CustomEditor(typeof(SurfaceType))]
public sealed class SurfaceTypeEditor : Editor
{
    private SurfaceType m_Target;

    private SerializedProperty m_FootstepsSounds;
    private SerializedProperty m_SprintingFootstepsSounds;
    private SerializedProperty m_LandingSounds;
    private SerializedProperty m_BulletImpactMaterial;
    private SerializedProperty m_BulletImpactParticle;
    private SerializedProperty m_BulletImpactSound;

    private void OnEnable ()
    {
        m_Target = (target as SurfaceType);

        m_FootstepsSounds = serializedObject.FindProperty("m_FootstepsSounds");
        m_SprintingFootstepsSounds = serializedObject.FindProperty("m_SprintingFootstepsSounds");
        m_LandingSounds = serializedObject.FindProperty("m_LandingSounds");
        m_BulletImpactMaterial = serializedObject.FindProperty("m_BulletImpactMaterial");
        m_BulletImpactParticle = serializedObject.FindProperty("m_BulletImpactParticle");
        m_BulletImpactSound = serializedObject.FindProperty("m_BulletImpactSound");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Footsteps Settings", m_FootstepsSounds);

        if (m_FootstepsSounds.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("Walking Sounds", EditorStyles.boldLabel);

            if (m_FootstepsSounds.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No audio clip defined", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Add new Audio Clip"))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddFootstepSound();

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_FootstepsSounds.arraySize; i++)
                {
                    DrawFootsteps(i);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sprinting Sounds", EditorStyles.boldLabel);

            if (m_SprintingFootstepsSounds.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No audio clip defined", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Add new Audio Clip"))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddSprintingFootstepSound();

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_SprintingFootstepsSounds.arraySize; i++)
                {
                    DrawSprintingFootsteps(i);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Landing Sounds", EditorStyles.boldLabel);

            if (m_LandingSounds.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No audio clip defined", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Add new Audio Clip"))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddLandingSound();

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_LandingSounds.arraySize; i++)
                {
                    DrawLandingSound(i);
                }
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Bullet Impact Settings", m_BulletImpactMaterial);

        if (m_BulletImpactMaterial.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("Decal Materials", EditorStyles.boldLabel);

            if (m_BulletImpactMaterial.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No material defined", MessageType.Info);
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Add new Material"))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddDecalMaterial();

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_BulletImpactMaterial.arraySize; i++)
                {
                    DrawDecalMaterial(i);
                }

                EditorGUI.indentLevel = 2;

                EditorGUILayout.Space();
                SerializedProperty minDecalSize = serializedObject.FindProperty("m_MinDecalSize");
                SerializedProperty maxDecalSize = serializedObject.FindProperty("m_MaxDecalSize");

                EditorGUIHelper.MinMaxSlider("Decal Size", minDecalSize, maxDecalSize, 0, 3, "F2");
                EditorGUI.indentLevel = 1;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Impact Particles", EditorStyles.boldLabel);

            if (m_BulletImpactParticle.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No particles defined", MessageType.Info);
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Add new Particle"))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddDecalParticle();

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_BulletImpactParticle.arraySize; i++)
                {
                    DrawDecalParticle(i);
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Impact Sounds", EditorStyles.boldLabel);

            if (m_BulletImpactSound.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No audio clip defined", MessageType.Info);
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Add new Audio Clip"))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddDecalSound();

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_BulletImpactSound.arraySize; i++)
                {
                    DrawDecalSound(i);
                }
            }
        }

        EditorGUI.indentLevel = 0;

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawFootsteps (int index)
    {
        SerializedProperty audioClip = m_FootstepsSounds.GetArrayElementAtIndex(index);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(audioClip, new GUIContent("Audio Clip " + (index + 1)));

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.RemoveFootstepSound(index);

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("+", FPSEStyles.rightButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.AddFootstepSound();

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }
        }
    }

    private void DrawSprintingFootsteps (int index)
    {
        SerializedProperty audioClip = m_SprintingFootstepsSounds.GetArrayElementAtIndex(index);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(audioClip, new GUIContent("Audio Clip " + (index + 1)));

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.RemoveSprintingFootstepSound(index);

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("+", FPSEStyles.rightButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.AddSprintingFootstepSound();

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }
        }
    }

    private void DrawLandingSound (int index)
    {
        SerializedProperty audioClip = m_LandingSounds.GetArrayElementAtIndex(index);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(audioClip, new GUIContent("Audio Clip " + (index + 1)));

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.RemoveLandingSound(index);

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("+", FPSEStyles.rightButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.AddLandingSound();

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }
        }
    }

    private void DrawDecalMaterial (int index)
    {
        EditorGUI.indentLevel = 2;
        SerializedProperty material = m_BulletImpactMaterial.GetArrayElementAtIndex(index);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(material, new GUIContent("Material " + (index + 1)));

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.RemoveDecalMaterial(index);

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("+", FPSEStyles.rightButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.AddDecalMaterial();

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }
        }
        EditorGUI.indentLevel = 1;
    }

    private void DrawDecalParticle (int index)
    {
        EditorGUI.indentLevel = 2;
        SerializedProperty particle = m_BulletImpactParticle.GetArrayElementAtIndex(index);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(particle, new GUIContent("Particle " + (index + 1)));

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.RemoveDecalParticle(index);

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("+", FPSEStyles.rightButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.AddDecalParticle();

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }
        }
        EditorGUI.indentLevel = 1;
    }

    private void DrawDecalSound (int index)
    {
        EditorGUI.indentLevel = 2;
        SerializedProperty audioClip = m_BulletImpactSound.GetArrayElementAtIndex(index);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(audioClip, new GUIContent("Particle " + (index + 1)));

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.RemoveDecalSound(index);

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("+", FPSEStyles.rightButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.AddDecalSound();

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }
        }
        EditorGUI.indentLevel = 1;
    }
}