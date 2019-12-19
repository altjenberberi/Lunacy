/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using Essentials.Controllers;

[CustomEditor(typeof(FirstPersonController))]
public sealed class FirstPersonControllerEditor : Editor
{
    private SerializedProperty m_WalkingSpeed;
    private SerializedProperty m_CrouchSpeed;
    private SerializedProperty m_RunMultiplier;
    private SerializedProperty m_AirControlPercent;
    private SerializedProperty m_JumpForce;
    private SerializedProperty m_SlopeLimit;
    private SerializedProperty m_StepOffset;

    private SerializedProperty m_HeightThreshold;
    private SerializedProperty m_DamageMultiplier;

    private SerializedProperty m_MainCamera;
    private SerializedProperty m_MouseLook;

    #region MOUSELOOK

    private SerializedProperty m_HorizontalSensitivity;
    private SerializedProperty m_VerticalSensitivity;
    private SerializedProperty m_AimingHorizontalSensitivity;
    private SerializedProperty m_AimingVerticalSensitivity;
    private SerializedProperty m_ClampVerticalRotation;
    private SerializedProperty m_MinimumX;
    private SerializedProperty m_MaximumX;
    private SerializedProperty m_Smoothness;

    #endregion

    private SerializedProperty m_Stamina;
    private SerializedProperty m_MaxStaminaAmount;
    private SerializedProperty m_DecrementRatio;
    private SerializedProperty m_BreathSound;

    private SerializedProperty m_Vault;
    private SerializedProperty m_InteractionRange;
    private SerializedProperty m_VaultAnimationCurve;
    private SerializedProperty m_VaultDuration;

    private SerializedProperty m_Footsteps;
    private SerializedProperty m_WalkingVolume;
    private SerializedProperty m_CrouchVolume;
    private SerializedProperty m_RunningVolume;
    private SerializedProperty m_JumpSound;
    private SerializedProperty m_JumpLandingVolume;
    private SerializedProperty m_CrouchDownSound;
    private SerializedProperty m_CrouchUpSound;

    private void OnEnable ()
    {
        //Setup the SerializedProperties
        m_WalkingSpeed = serializedObject.FindProperty("m_WalkingSpeed");
        m_CrouchSpeed = serializedObject.FindProperty("m_CrouchSpeed");
        m_RunMultiplier = serializedObject.FindProperty("m_RunMultiplier");
        m_AirControlPercent = serializedObject.FindProperty("m_AirControlPercent");
        m_JumpForce = serializedObject.FindProperty("m_JumpForce");
        m_SlopeLimit = serializedObject.FindProperty("m_SlopeLimit");
        m_StepOffset = serializedObject.FindProperty("m_StepOffset");

        m_HeightThreshold = serializedObject.FindProperty("m_HeightThreshold");
        m_DamageMultiplier = serializedObject.FindProperty("m_DamageMultiplier");

        m_MainCamera = serializedObject.FindProperty("m_MainCamera");

        m_MouseLook = serializedObject.FindProperty("m_MouseLook");

        m_HorizontalSensitivity = m_MouseLook.FindPropertyRelative("m_HorizontalSensitivity");
        m_VerticalSensitivity = m_MouseLook.FindPropertyRelative("m_VerticalSensitivity");

        m_AimingHorizontalSensitivity = m_MouseLook.FindPropertyRelative("m_AimingHorizontalSensitivity");
        m_AimingVerticalSensitivity = m_MouseLook.FindPropertyRelative("m_AimingVerticalSensitivity");

        m_ClampVerticalRotation = m_MouseLook.FindPropertyRelative("m_ClampVerticalRotation");
        m_MinimumX = m_MouseLook.FindPropertyRelative("m_MinimumX");
        m_MaximumX = m_MouseLook.FindPropertyRelative("m_MaximumX");
        m_Smoothness = m_MouseLook.FindPropertyRelative("m_Smoothness");

        m_Stamina = serializedObject.FindProperty("m_Stamina");
        m_MaxStaminaAmount = serializedObject.FindProperty("m_MaxStaminaAmount");
        m_DecrementRatio = serializedObject.FindProperty("m_DecrementRatio");
        m_BreathSound = serializedObject.FindProperty("m_BreathSound");

        m_Vault = serializedObject.FindProperty("m_Vault");
        m_InteractionRange = serializedObject.FindProperty("m_InteractionRange");
        m_VaultAnimationCurve = serializedObject.FindProperty("m_VaultAnimationCurve");
        m_VaultDuration = serializedObject.FindProperty("m_VaultDuration");

        m_Footsteps = serializedObject.FindProperty("m_Footsteps");
        m_WalkingVolume = serializedObject.FindProperty("m_WalkingVolume");
        m_CrouchVolume = serializedObject.FindProperty("m_CrouchVolume");
        m_RunningVolume = serializedObject.FindProperty("m_RunningVolume");
        m_JumpSound = serializedObject.FindProperty("m_JumpSound");
        m_JumpLandingVolume = serializedObject.FindProperty("m_JumpLandingVolume");

        m_CrouchDownSound = serializedObject.FindProperty("m_CrouchDownSound");
        m_CrouchUpSound = serializedObject.FindProperty("m_CrouchUpSound");
    }

    public override void OnInspectorGUI ()
    {
        //Update the serializedProperty - always do this in the beginning of OnInspectorGUI
        serializedObject.Update();

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Movement Settings", m_WalkingSpeed);

        if (m_WalkingSpeed.isExpanded)
        {
            EditorGUI.indentLevel = 1;

            EditorGUILayout.PropertyField(m_WalkingSpeed);
            EditorGUILayout.PropertyField(m_CrouchSpeed);
            EditorGUILayout.PropertyField(m_RunMultiplier);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_JumpForce);
            EditorGUILayout.PropertyField(m_AirControlPercent);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_SlopeLimit);
            EditorGUILayout.PropertyField(m_StepOffset);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Fall", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(m_HeightThreshold);
            EditorGUILayout.PropertyField(m_DamageMultiplier);
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Mouse Look Settings", m_MouseLook);

        if (m_MouseLook.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_MainCamera);

            EditorGUILayout.PropertyField(m_HorizontalSensitivity);
            EditorGUILayout.PropertyField(m_VerticalSensitivity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_AimingHorizontalSensitivity);
            EditorGUILayout.PropertyField(m_AimingVerticalSensitivity);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_ClampVerticalRotation);

            if (m_ClampVerticalRotation.boolValue)
            {
                EditorGUIHelper.MinMaxSlider("Limit Vertical Angle", m_MinimumX, m_MaximumX, -90f, 90f, "F0");
            }

            EditorGUILayout.PropertyField(m_Smoothness);
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Stamina", m_Stamina);

        if (m_Stamina.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Stamina.boolValue))
            {
                EditorGUILayout.PropertyField(m_MaxStaminaAmount);
                EditorGUILayout.PropertyField(m_DecrementRatio);
                EditorGUILayout.PropertyField(m_BreathSound);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Parkour", m_Vault);

        if (m_Vault.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Vault.boolValue))
            {
                EditorGUILayout.PropertyField(m_InteractionRange);
                EditorGUILayout.PropertyField(m_VaultAnimationCurve);
                EditorGUILayout.PropertyField(m_VaultDuration);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Footsteps", m_Footsteps);

        if (m_Footsteps.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Footsteps.boolValue))
            {
                EditorGUILayout.PropertyField(m_WalkingVolume);
                EditorGUILayout.PropertyField(m_RunningVolume);
                EditorGUILayout.PropertyField(m_CrouchVolume);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_JumpSound);
                EditorGUILayout.PropertyField(m_JumpLandingVolume);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_CrouchUpSound);
                EditorGUILayout.PropertyField(m_CrouchDownSound);
            }
        }

        //Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI
        serializedObject.ApplyModifiedProperties();
    }
}