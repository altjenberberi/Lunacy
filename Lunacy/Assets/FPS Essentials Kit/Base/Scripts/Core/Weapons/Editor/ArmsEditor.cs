/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;
using Essentials.Weapons;

[CustomEditor(typeof(Arms))]
public sealed class ArmsEditor : Editor
{
    private Arms m_Target;
    private SerializedProperty m_CameraTransformReference;
    private SerializedProperty m_FPController;

    private SerializedProperty m_Range;
    private SerializedProperty m_AttackRate;
    private SerializedProperty m_Force;
    private SerializedProperty m_MinDamage;
    private SerializedProperty m_MaxDamage;

    #region WEAPON SWING

    private SerializedProperty m_WeaponSwing;
    private SerializedProperty m_TiltAngle;
    private SerializedProperty m_MaxTiltAngle;
    private SerializedProperty m_SwingAngle;
    private SerializedProperty m_MaxSwingAngle;
    private SerializedProperty m_Speed;
    private SerializedProperty m_SwingTarget;

    #endregion

    #region MOTION ANIMATIONS

    private SerializedProperty m_MotionAnimation;
    private SerializedProperty m_WalkingMotionData;
    private SerializedProperty m_BrokenLegsMotionData;
    private SerializedProperty m_RunningMotionData;
    private SerializedProperty m_TargetTransform;

    private SerializedProperty m_JumpAnimation;
    private SerializedProperty m_LandingAnimation;

    private SerializedProperty m_BreathingSpeed;
    private SerializedProperty m_BreathingAmplitude;

    #endregion

    private SerializedProperty m_ArmsAnimator;
    private SerializedProperty m_Animator;
    private SerializedProperty m_VelocityParameter;
    private SerializedProperty m_Draw;
    private SerializedProperty m_DrawAnimation;
    private SerializedProperty m_DrawSound;
    private SerializedProperty m_DrawVolume;

    private SerializedProperty m_Hide;
    private SerializedProperty m_HideAnimation;
    private SerializedProperty m_HideSound;
    private SerializedProperty m_HideVolume;

    private SerializedProperty m_Attack;
    private SerializedProperty m_RightAttackAnimationList;
    private SerializedProperty m_LeftAttackAnimationList;
    private SerializedProperty m_AttackAnimationType;
    private SerializedProperty m_AttackSoundList;
    private SerializedProperty m_AttackVolume;
    private SerializedProperty m_HitSoundList;
    private SerializedProperty m_HitVolume;

    private SerializedProperty m_Interact;
    private SerializedProperty m_InteractAnimation;
    private SerializedProperty m_InteractDelay;
    private SerializedProperty m_InteractSound;
    private SerializedProperty m_InteractVolume;

    private SerializedProperty m_Vault;
    private SerializedProperty m_VaultAnimation;
    private SerializedProperty m_VaultSound;
    private SerializedProperty m_VaultVolume;

    private void OnEnable ()
    {
        m_Target = (target as Arms);
        m_Target.DisableShadowCasting();

        m_CameraTransformReference = serializedObject.FindProperty("m_CameraTransformReference");
        m_FPController = serializedObject.FindProperty("m_FPController");
        m_Range = serializedObject.FindProperty("m_Range");
        m_AttackRate = serializedObject.FindProperty("m_AttackRate");
        m_Force = serializedObject.FindProperty("m_Force");
        m_MinDamage = serializedObject.FindProperty("m_MinDamage");
        m_MaxDamage = serializedObject.FindProperty("m_MaxDamage");

        m_WeaponSwing = serializedObject.FindProperty("m_WeaponSwing");
        m_TiltAngle = m_WeaponSwing.FindPropertyRelative("m_TiltAngle");
        m_MaxTiltAngle = m_WeaponSwing.FindPropertyRelative("m_MaxTiltAngle");
        m_SwingAngle = m_WeaponSwing.FindPropertyRelative("m_SwingAngle");
        m_MaxSwingAngle = m_WeaponSwing.FindPropertyRelative("m_MaxSwingAngle");
        m_Speed = m_WeaponSwing.FindPropertyRelative("m_Speed");
        m_SwingTarget = m_WeaponSwing.FindPropertyRelative("m_SwingTarget");

        m_MotionAnimation = serializedObject.FindProperty("m_MotionAnimation");
        m_WalkingMotionData = m_MotionAnimation.FindPropertyRelative("m_WalkingMotionData");
        m_BrokenLegsMotionData = m_MotionAnimation.FindPropertyRelative("m_BrokenLegsMotionData");
        m_RunningMotionData = m_MotionAnimation.FindPropertyRelative("m_RunningMotionData");
        m_TargetTransform = m_MotionAnimation.FindPropertyRelative("m_TargetTransform");
        m_JumpAnimation = m_MotionAnimation.FindPropertyRelative("m_JumpAnimation");
        m_LandingAnimation = m_MotionAnimation.FindPropertyRelative("m_LandingAnimation");
        m_BreathingSpeed = m_MotionAnimation.FindPropertyRelative("m_BreathingSpeed");
        m_BreathingAmplitude = m_MotionAnimation.FindPropertyRelative("m_BreathingAmplitude");

        m_ArmsAnimator = serializedObject.FindProperty("m_ArmsAnimator");
        m_Animator = m_ArmsAnimator.FindPropertyRelative("m_Animator");
        m_VelocityParameter = m_ArmsAnimator.FindPropertyRelative("m_VelocityParameter");

        m_Draw = m_ArmsAnimator.FindPropertyRelative("m_Draw");
        m_DrawAnimation = m_ArmsAnimator.FindPropertyRelative("m_DrawAnimation");
        m_DrawSound = m_ArmsAnimator.FindPropertyRelative("m_DrawSound");
        m_DrawVolume = m_ArmsAnimator.FindPropertyRelative("m_DrawVolume");

        m_Hide = m_ArmsAnimator.FindPropertyRelative("m_Hide");
        m_HideAnimation = m_ArmsAnimator.FindPropertyRelative("m_HideAnimation");
        m_HideSound = m_ArmsAnimator.FindPropertyRelative("m_HideSound");
        m_HideVolume = m_ArmsAnimator.FindPropertyRelative("m_HideVolume");

        m_Attack = m_ArmsAnimator.FindPropertyRelative("m_Attack");
        m_RightAttackAnimationList = m_ArmsAnimator.FindPropertyRelative("m_RightAttackAnimationList");
        m_LeftAttackAnimationList = m_ArmsAnimator.FindPropertyRelative("m_LeftAttackAnimationList");
        m_AttackAnimationType = m_ArmsAnimator.FindPropertyRelative("m_AttackAnimationType");
        m_AttackSoundList = m_ArmsAnimator.FindPropertyRelative("m_AttackSoundList");
        m_AttackVolume = m_ArmsAnimator.FindPropertyRelative("m_AttackVolume");
        m_HitSoundList = m_ArmsAnimator.FindPropertyRelative("m_HitSoundList");
        m_HitVolume = m_ArmsAnimator.FindPropertyRelative("m_HitVolume");

        m_Interact = m_ArmsAnimator.FindPropertyRelative("m_Interact");
        m_InteractAnimation = m_ArmsAnimator.FindPropertyRelative("m_InteractAnimation");
        m_InteractDelay = m_ArmsAnimator.FindPropertyRelative("m_InteractDelay");
        m_InteractSound = m_ArmsAnimator.FindPropertyRelative("m_InteractSound");
        m_InteractVolume = m_ArmsAnimator.FindPropertyRelative("m_InteractVolume");

        m_Vault = m_ArmsAnimator.FindPropertyRelative("m_Vault");
        m_VaultAnimation = m_ArmsAnimator.FindPropertyRelative("m_VaultAnimation");
        m_VaultSound = m_ArmsAnimator.FindPropertyRelative("m_VaultSound");
        m_VaultVolume = m_ArmsAnimator.FindPropertyRelative("m_VaultVolume");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Controllers & Managers", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_FPController, new GUIContent("FPS Controller"));
        EditorGUILayout.PropertyField(m_CameraTransformReference);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_Range);
        EditorGUILayout.PropertyField(m_AttackRate);
        EditorGUILayout.PropertyField(m_Force);
        EditorGUIHelper.MinMaxSlider("Damage ", m_MinDamage, m_MaxDamage, 0, 100);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Procedural Animations", EditorStyles.boldLabel);
        Animations();

        serializedObject.ApplyModifiedProperties();
    }

    private void Animations ()
    {
        #region WEAPON SWING
        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Swing", m_WeaponSwing);

        if (m_WeaponSwing.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_TiltAngle);
            EditorGUILayout.PropertyField(m_MaxTiltAngle);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_SwingAngle);
            EditorGUILayout.PropertyField(m_MaxSwingAngle);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Speed);
            EditorGUILayout.PropertyField(m_SwingTarget);
        }
        #endregion

        #region MOTION ANIMATION
        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Motion", m_MotionAnimation);

        if (m_MotionAnimation.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_TargetTransform);
            EditorGUILayout.PropertyField(m_WalkingMotionData);
            EditorGUILayout.PropertyField(m_BrokenLegsMotionData);
            EditorGUILayout.PropertyField(m_RunningMotionData);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Jump Animation", FPSEStyles.midLabel);
            using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
            {
                EditorGUIHelper.LerpAnimationDrawer(m_JumpAnimation, true, m_Target.transform);
            }

            EditorGUILayout.LabelField("Landing Animation", FPSEStyles.midLabel);
            using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
            {
                EditorGUIHelper.LerpAnimationDrawer(m_LandingAnimation, true, m_Target.transform);
            }

            EditorGUILayout.LabelField("Breath Animation", FPSEStyles.midLabel);
            using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
            {
                EditorGUILayout.PropertyField(m_BreathingSpeed);
                EditorGUILayout.PropertyField(m_BreathingAmplitude);
            }
        }
        #endregion

        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Arms Animations", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_Animator);
        EditorGUILayout.PropertyField(m_VelocityParameter); 

        EditorGUIHelper.ToggleHeader("Draw", m_Draw);

        if (m_Draw.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Draw.boolValue))
            {
                EditorGUILayout.PropertyField(m_DrawAnimation);
                EditorGUILayout.PropertyField(m_DrawSound);
                EditorGUILayout.PropertyField(m_DrawVolume);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Hide", m_Hide);

        if (m_Hide.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Hide.boolValue))
            {
                EditorGUILayout.PropertyField(m_HideAnimation);
                EditorGUILayout.PropertyField(m_HideSound);
                EditorGUILayout.PropertyField(m_HideVolume);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Attack", m_Attack);

        if (m_Attack.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Attack.boolValue))
            {
                using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
                {
                    EditorGUILayout.LabelField("Right Attack Animations", FPSEStyles.midLabel);

                    if (m_RightAttackAnimationList.arraySize == 0)
                    {
                        m_Target.AddRightAttack();
                        return;
                    }

                    for (int i = 0; i < m_RightAttackAnimationList.arraySize; i++)
                    {
                        SerializedProperty animation = m_RightAttackAnimationList.GetArrayElementAtIndex(i);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(animation, new GUIContent(i == 0 ? "Right Attack Animation" : "Right Attack Animation " + i));

                            using (new EditorGUI.DisabledGroupScope(m_RightAttackAnimationList.arraySize == 1))
                            {
                                EditorGUI.BeginChangeCheck();
                                if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
                                {
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(m_Target, "Undo Inspector");
                                        m_Target.RemoveRightAttack(i);
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
                                    m_Target.AddRightAttack();

                                    EditorUtility.SetDirty(m_Target);
                                    return;
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
                {
                    EditorGUILayout.LabelField("Left Attack Animations", FPSEStyles.midLabel);

                    if (m_LeftAttackAnimationList.arraySize == 0)
                    {
                        m_Target.AddLeftAttack();
                        return;
                    }

                    for (int i = 0; i < m_LeftAttackAnimationList.arraySize; i++)
                    {
                        SerializedProperty animation = m_LeftAttackAnimationList.GetArrayElementAtIndex(i);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(animation, new GUIContent(i == 0 ? "Left Attack Animation" : "Left Attack Animation " + i));

                            using (new EditorGUI.DisabledGroupScope(m_LeftAttackAnimationList.arraySize == 1))
                            {
                                EditorGUI.BeginChangeCheck();
                                if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
                                {
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(m_Target, "Undo Inspector");
                                        m_Target.RemoveLeftAttack(i);
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
                                    m_Target.AddLeftAttack();

                                    EditorUtility.SetDirty(m_Target);
                                    return;
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.PropertyField(m_AttackAnimationType);

                EditorGUILayout.Space();

                using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
                {
                    EditorGUILayout.LabelField("Attack Sounds", FPSEStyles.midLabel);

                    if (m_AttackSoundList.arraySize == 0)
                    {
                        m_Target.AddAttackSound();
                        return;
                    }

                    for (int i = 0; i < m_AttackSoundList.arraySize; i++)
                    {
                        SerializedProperty sound = m_AttackSoundList.GetArrayElementAtIndex(i);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(sound, new GUIContent(i == 0 ? "Attack Sound" : "Attack Sound " + i));

                            using (new EditorGUI.DisabledGroupScope(m_AttackSoundList.arraySize == 1))
                            {
                                EditorGUI.BeginChangeCheck();
                                if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
                                {
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(m_Target, "Undo Inspector");
                                        m_Target.RemoveAttackSound(i);
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
                                    m_Target.AddAttackSound();

                                    EditorUtility.SetDirty(m_Target);
                                    return;
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_AttackVolume);

                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
                {
                    EditorGUILayout.LabelField("Hit Sounds", FPSEStyles.midLabel);

                    if (m_HitSoundList.arraySize == 0)
                    {
                        m_Target.AddHitSound();
                        return;
                    }

                    for (int i = 0; i < m_HitSoundList.arraySize; i++)
                    {
                        SerializedProperty sound = m_HitSoundList.GetArrayElementAtIndex(i);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(sound, new GUIContent(i == 0 ? "Hit Sound" : "Hit Sound " + i));

                            using (new EditorGUI.DisabledGroupScope(m_HitSoundList.arraySize == 1))
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

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_HitVolume);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Interact", m_Interact);

        if (m_Interact.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Interact.boolValue))
            {
                EditorGUILayout.PropertyField(m_InteractAnimation);
                EditorGUILayout.PropertyField(m_InteractSound);
                EditorGUILayout.PropertyField(m_InteractVolume);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_InteractDelay);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Vault", m_Vault);

        if (m_Vault.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Vault.boolValue))
            {
                EditorGUILayout.PropertyField(m_VaultAnimation);
                EditorGUILayout.PropertyField(m_VaultSound);
                EditorGUILayout.PropertyField(m_VaultVolume);
            }
        }
    }
}