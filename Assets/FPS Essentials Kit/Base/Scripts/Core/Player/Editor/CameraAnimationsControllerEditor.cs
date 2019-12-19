/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;
using Essentials.Controllers;

[CustomEditor(typeof(CameraAnimationsController))]
public sealed class CameraAnimationsControllerEditor : Editor
{
    private CameraAnimationsController m_Target;

    private SerializedProperty m_FPController;
    private SerializedProperty m_HealthController;

    private SerializedProperty m_MotionAnimation;
    private SerializedProperty m_WalkingMotionData;
    private SerializedProperty m_BrokenLegsMotionData;
    private SerializedProperty m_RunningMotionData;
    private SerializedProperty m_TargetTransform;

    private SerializedProperty m_JumpAnimation;
    private SerializedProperty m_LandingAnimation;

    private SerializedProperty m_BreathingSpeed;
    private SerializedProperty m_BreathingAmplitude;

    private SerializedProperty m_MinHitRotation;
    private SerializedProperty m_MaxHitRotation;
    private SerializedProperty m_HitDuration;

    private SerializedProperty m_VaultAnimation;

    private SerializedProperty m_GrenadeExplosionShake;

    private SerializedProperty m_Lean;
    private SerializedProperty m_LeanAmount;
    private SerializedProperty m_LeanAngle;
    private SerializedProperty m_LeanSpeed;

    private SerializedProperty m_HoldBreath;
    private SerializedProperty m_Exhale;
    private SerializedProperty m_HoldBreathVolume;

    private void OnEnable ()
    {
        m_Target = (target as CameraAnimationsController);

        m_FPController = serializedObject.FindProperty("m_FPController");
        m_HealthController = serializedObject.FindProperty("m_HealthController");

        m_MotionAnimation = serializedObject.FindProperty("m_MotionAnimation");
        m_WalkingMotionData = m_MotionAnimation.FindPropertyRelative("m_WalkingMotionData");
        m_BrokenLegsMotionData = m_MotionAnimation.FindPropertyRelative("m_BrokenLegsMotionData");
        m_RunningMotionData = m_MotionAnimation.FindPropertyRelative("m_RunningMotionData");
        m_TargetTransform = m_MotionAnimation.FindPropertyRelative("m_TargetTransform");
        m_JumpAnimation = m_MotionAnimation.FindPropertyRelative("m_JumpAnimation");
        m_LandingAnimation = m_MotionAnimation.FindPropertyRelative("m_LandingAnimation");
        m_BreathingSpeed = m_MotionAnimation.FindPropertyRelative("m_BreathingSpeed");
        m_BreathingAmplitude = m_MotionAnimation.FindPropertyRelative("m_BreathingAmplitude");
        m_GrenadeExplosionShake = m_MotionAnimation.FindPropertyRelative("m_GrenadeExplosionShake");

        m_MinHitRotation = m_MotionAnimation.FindPropertyRelative("m_MinHitRotation");
        m_MaxHitRotation = m_MotionAnimation.FindPropertyRelative("m_MaxHitRotation");
        m_HitDuration = m_MotionAnimation.FindPropertyRelative("m_HitDuration");

        m_VaultAnimation = m_MotionAnimation.FindPropertyRelative("m_VaultAnimation");

        m_Lean = m_MotionAnimation.FindPropertyRelative("m_Lean");
        m_LeanAmount = m_MotionAnimation.FindPropertyRelative("m_LeanAmount");
        m_LeanAngle = m_MotionAnimation.FindPropertyRelative("m_LeanAngle");
        m_LeanSpeed = m_MotionAnimation.FindPropertyRelative("m_LeanSpeed");

        m_HoldBreath = serializedObject.FindProperty("m_HoldBreath");
        m_Exhale = serializedObject.FindProperty("m_Exhale");
        m_HoldBreathVolume = serializedObject.FindProperty("m_HoldBreathVolume");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_FPController, new GUIContent("FPS Controller"));
        EditorGUILayout.PropertyField(m_HealthController);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_TargetTransform);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_WalkingMotionData);
        EditorGUILayout.PropertyField(m_BrokenLegsMotionData);
        EditorGUILayout.PropertyField(m_RunningMotionData);

        EditorGUILayout.Space();

        using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
        {
            using (new EditorGUILayout.HorizontalScope(FPSEStyles.header))
            {
                GUILayout.Label("Jump Animation");
            }

            EditorGUIHelper.LerpAnimationDrawer(m_JumpAnimation, true, m_Target.transform);
        }

        using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
        {
            using (new EditorGUILayout.HorizontalScope(FPSEStyles.header))
            {
                GUILayout.Label("Landing Animation");
            }

            EditorGUIHelper.LerpAnimationDrawer(m_LandingAnimation, true, m_Target.transform);
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Breath Animation", m_BreathingSpeed);

        if (m_BreathingSpeed.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_BreathingSpeed);
            EditorGUILayout.PropertyField(m_BreathingAmplitude);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_HoldBreath);
            EditorGUILayout.PropertyField(m_Exhale);
            EditorGUILayout.PropertyField(m_HoldBreathVolume);
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Hit Animation", m_MinHitRotation);

        if (m_MinHitRotation.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_MinHitRotation);
            EditorGUILayout.PropertyField(m_MaxHitRotation);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_HitDuration);
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Lean Animation", m_Lean);

        if (m_Lean.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Lean.boolValue))
            {
                EditorGUILayout.PropertyField(m_LeanAmount);
                EditorGUILayout.PropertyField(m_LeanAngle);
                EditorGUILayout.PropertyField(m_LeanSpeed);
            }   
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Vault Animation", m_VaultAnimation);

        if (m_VaultAnimation.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUIHelper.LerpAnimationDrawer(m_VaultAnimation);
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Grenade Explosion Animation", m_GrenadeExplosionShake);

        if (m_GrenadeExplosionShake.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            ShakePropertiesDrawer(m_GrenadeExplosionShake);
        }

        EditorGUI.indentLevel = 0;
        serializedObject.ApplyModifiedProperties();
    }

    private void ShakePropertiesDrawer (SerializedProperty property)
    {
        SerializedProperty m_Angle = property.FindPropertyRelative("m_Angle");
        SerializedProperty m_Strength = property.FindPropertyRelative("m_Strength");
        SerializedProperty m_MinSpeed = property.FindPropertyRelative("m_MinSpeed");
        SerializedProperty m_MaxSpeed = property.FindPropertyRelative("m_MaxSpeed");
        SerializedProperty m_Duration = property.FindPropertyRelative("m_Duration");
        SerializedProperty m_NoisePercent = property.FindPropertyRelative("m_NoisePercent");
        SerializedProperty m_DampingPercent = property.FindPropertyRelative("m_DampingPercent");
        SerializedProperty m_RotationPercent = property.FindPropertyRelative("m_RotationPercent");

        EditorGUILayout.PropertyField(m_Angle);
        EditorGUILayout.PropertyField(m_Strength);
        EditorGUILayout.PropertyField(m_MinSpeed);
        EditorGUILayout.PropertyField(m_MaxSpeed);
        EditorGUILayout.PropertyField(m_Duration);
        EditorGUILayout.PropertyField(m_NoisePercent);
        EditorGUILayout.PropertyField(m_DampingPercent);
        EditorGUILayout.PropertyField(m_RotationPercent);
    }
}