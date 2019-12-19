/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;
using Essentials.Weapons;

[CustomEditor(typeof(Gun))]
public sealed class GunEditor : Editor
{
    private Gun m_Target;
    private SerializedProperty m_GunData;
    private SerializedProperty m_BulletMarkManager;
    private SerializedProperty m_CameraTransformReference;
    private SerializedProperty m_FPController;

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

    private SerializedProperty m_WeaponRecoil;
    private SerializedProperty m_WeaponRecoilPosition;
    private SerializedProperty m_WeaponRecoilRotation;
    private SerializedProperty m_WeaponRecoilDuration;

    private SerializedProperty m_CameraRecoil;
    private SerializedProperty m_CameraAnimationsController;
    private SerializedProperty m_MinCameraRecoilRotation;
    private SerializedProperty m_MaxCameraRecoilRotation;
    private SerializedProperty m_CameraRecoilDuration;
    private SerializedProperty m_CameraReturnDuration;

    private SerializedProperty m_BreathingSpeed;
    private SerializedProperty m_BreathingAmplitude;

    private SerializedProperty m_Lean;
    private SerializedProperty m_LeanAngle;
    private SerializedProperty m_LeanSpeed;
    #endregion

    #region GUN ANIMATOR
    private SerializedProperty m_GunAnimator;
    private SerializedProperty m_Animator;
    private SerializedProperty m_SpeedParameter;

    private SerializedProperty m_Draw;
    private SerializedProperty m_DrawAnimation;
    private SerializedProperty m_DrawSpeed;
    private SerializedProperty m_DrawSound;
    private SerializedProperty m_DrawVolume;

    private SerializedProperty m_Hide;
    private SerializedProperty m_HideAnimation;
    private SerializedProperty m_HideSpeed;
    private SerializedProperty m_HideSound;
    private SerializedProperty m_HideVolume;

    private SerializedProperty m_Fire;
    private SerializedProperty m_FireAnimationList;
    private SerializedProperty m_AimedFireAnimationList;
    private SerializedProperty m_OverrideLastFire;
    private SerializedProperty m_FireSpeed;
    private SerializedProperty m_AimedFireSpeed;
    private SerializedProperty m_LastFireAnimation;
    private SerializedProperty m_FireAnimationType;
    private SerializedProperty m_FireSoundList;
    private SerializedProperty m_OutOfAmmoSound;
    private SerializedProperty m_FireVolume;

    private SerializedProperty m_Reload;
    private SerializedProperty m_ReloadAnimation;
    private SerializedProperty m_ReloadSpeed;
    private SerializedProperty m_ReloadSound;
    private SerializedProperty m_ReloadEmptyAnimation;
    private SerializedProperty m_ReloadEmptySpeed;
    private SerializedProperty m_ReloadEmptySound;
    private SerializedProperty m_ReloadVolume;

    private SerializedProperty m_StartReloadAnimation;
    private SerializedProperty m_StartReloadSpeed;
    private SerializedProperty m_StartReloadSound;
    private SerializedProperty m_StartReloadVolume;

    private SerializedProperty m_InsertInChamberAnimation;
    private SerializedProperty m_InsertInChamberSpeed;
    private SerializedProperty m_InsertInChamberSound;
    private SerializedProperty m_InsertInChamberVolume;

    private SerializedProperty m_InsertAnimation;
    private SerializedProperty m_InsertSpeed;
    private SerializedProperty m_InsertSound;
    private SerializedProperty m_InsertVolume;

    private SerializedProperty m_StopReloadAnimation;
    private SerializedProperty m_StopReloadSpeed;
    private SerializedProperty m_StopReloadSound;
    private SerializedProperty m_StopReloadVolume;

    private SerializedProperty m_Melee;
    private SerializedProperty m_MeleeAnimation;
    private SerializedProperty m_MeleeSpeed;
    private SerializedProperty m_MeleeDelay;
    private SerializedProperty m_MeleeSound;
    private SerializedProperty m_MeleeVolume;
    private SerializedProperty m_HitSoundList;
    private SerializedProperty m_HitVolume;

    private SerializedProperty m_SwitchMode;
    private SerializedProperty m_SwitchModeAnimation;
    private SerializedProperty m_SwitchModeSpeed;
    private SerializedProperty m_SwitchModeSound;
    private SerializedProperty m_SwitchModeVolume;

    private SerializedProperty m_Interact;
    private SerializedProperty m_InteractAnimation;
    private SerializedProperty m_InteractDelay;
    private SerializedProperty m_InteractSpeed;
    private SerializedProperty m_InteractSound;
    private SerializedProperty m_InteractVolume;

    private SerializedProperty m_Vault;
    private SerializedProperty m_VaultAnimation;
    private SerializedProperty m_VaultSpeed;
    private SerializedProperty m_VaultSound;
    private SerializedProperty m_VaultVolume;

    private SerializedProperty m_RunAnimation;
    private SerializedProperty m_RunningPosition;
    private SerializedProperty m_RunningRotation;
    private SerializedProperty m_RunningSpeed;

    private SerializedProperty m_AimAnimation;
    private SerializedProperty m_AimingPosition;
    private SerializedProperty m_AimingRotation;
    private SerializedProperty m_AimInSound;
    private SerializedProperty m_AimOutSound;
    private SerializedProperty m_ZoomAnimation;
    private SerializedProperty m_AimFOV;
    private SerializedProperty m_AimingSpeed;
    private SerializedProperty m_HoldBreath;
    #endregion

    #region GUN EFFECTS
    private SerializedProperty m_GunEffects;

    private SerializedProperty m_MuzzleFlash;
    private SerializedProperty m_MuzzleParticle;

    private SerializedProperty m_Tracer;
    private SerializedProperty m_TracerPrefab;
    private SerializedProperty m_TracerSpeed;
    private SerializedProperty m_TracerOrigin;

    private SerializedProperty m_Shell;
    private SerializedProperty m_ShellParticle;
    private SerializedProperty m_MinShellSpeed;
    private SerializedProperty m_MaxShellSpeed;
    private SerializedProperty m_StartDelay;
    #endregion

    private void OnEnable ()
    {
        m_Target = (target as Gun);
        m_Target.DisableShadowCasting();

        m_GunData = serializedObject.FindProperty("m_GunData");
        m_BulletMarkManager = serializedObject.FindProperty("m_BulletMarkManager");
        m_CameraTransformReference = serializedObject.FindProperty("m_CameraTransformReference");
        m_FPController = serializedObject.FindProperty("m_FPController");

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

        m_Lean = m_MotionAnimation.FindPropertyRelative("m_Lean");
        m_LeanAngle = m_MotionAnimation.FindPropertyRelative("m_LeanAngle");
        m_LeanSpeed = m_MotionAnimation.FindPropertyRelative("m_LeanSpeed");

        m_WeaponRecoil = m_MotionAnimation.FindPropertyRelative("m_WeaponRecoil");
        m_WeaponRecoilPosition = m_MotionAnimation.FindPropertyRelative("m_WeaponRecoilPosition");
        m_WeaponRecoilRotation = m_MotionAnimation.FindPropertyRelative("m_WeaponRecoilRotation");
        m_WeaponRecoilDuration = m_MotionAnimation.FindPropertyRelative("m_WeaponRecoilDuration");

        m_CameraRecoil = m_MotionAnimation.FindPropertyRelative("m_CameraRecoil");
        m_CameraAnimationsController = serializedObject.FindProperty("m_CameraAnimationsController");
        m_MinCameraRecoilRotation = m_MotionAnimation.FindPropertyRelative("m_MinCameraRecoilRotation");
        m_MaxCameraRecoilRotation = m_MotionAnimation.FindPropertyRelative("m_MaxCameraRecoilRotation");
        m_CameraRecoilDuration = m_MotionAnimation.FindPropertyRelative("m_CameraRecoilDuration");
        m_CameraReturnDuration = m_MotionAnimation.FindPropertyRelative("m_CameraReturnDuration");

        m_GunAnimator = serializedObject.FindProperty("m_GunAnimator");
        m_Animator = m_GunAnimator.FindPropertyRelative("m_Animator");
        m_SpeedParameter = m_GunAnimator.FindPropertyRelative("m_SpeedParameter");

        m_Draw = m_GunAnimator.FindPropertyRelative("m_Draw");
        m_DrawAnimation = m_GunAnimator.FindPropertyRelative("m_DrawAnimation");
        m_DrawSpeed = m_GunAnimator.FindPropertyRelative("m_DrawSpeed");
        m_DrawSound = m_GunAnimator.FindPropertyRelative("m_DrawSound");
        m_DrawVolume = m_GunAnimator.FindPropertyRelative("m_DrawVolume");

        m_Hide = m_GunAnimator.FindPropertyRelative("m_Hide");
        m_HideAnimation = m_GunAnimator.FindPropertyRelative("m_HideAnimation");
        m_HideSpeed = m_GunAnimator.FindPropertyRelative("m_HideSpeed");
        m_HideSound = m_GunAnimator.FindPropertyRelative("m_HideSound");
        m_HideVolume = m_GunAnimator.FindPropertyRelative("m_HideVolume");

        m_Fire = m_GunAnimator.FindPropertyRelative("m_Fire");
        m_FireAnimationList = m_GunAnimator.FindPropertyRelative("m_FireAnimationList");
        m_AimedFireAnimationList = m_GunAnimator.FindPropertyRelative("m_AimedFireAnimationList");
        m_LastFireAnimation = m_GunAnimator.FindPropertyRelative("m_LastFireAnimation");
        m_OverrideLastFire = m_GunAnimator.FindPropertyRelative("m_OverrideLastFire");
        m_FireAnimationType = m_GunAnimator.FindPropertyRelative("m_FireAnimationType");
        m_FireSoundList = m_GunAnimator.FindPropertyRelative("m_FireSoundList");
        m_FireSpeed = m_GunAnimator.FindPropertyRelative("m_FireSpeed");
        m_AimedFireSpeed = m_GunAnimator.FindPropertyRelative("m_AimedFireSpeed");
        m_OutOfAmmoSound = m_GunAnimator.FindPropertyRelative("m_OutOfAmmoSound");
        m_FireVolume = m_GunAnimator.FindPropertyRelative("m_FireVolume");

        m_Reload = m_GunAnimator.FindPropertyRelative("m_Reload");
        m_ReloadAnimation = m_GunAnimator.FindPropertyRelative("m_ReloadAnimation");
        m_ReloadSpeed = m_GunAnimator.FindPropertyRelative("m_ReloadSpeed");
        m_ReloadSound = m_GunAnimator.FindPropertyRelative("m_ReloadSound");
        m_ReloadEmptyAnimation = m_GunAnimator.FindPropertyRelative("m_ReloadEmptyAnimation");
        m_ReloadEmptySpeed = m_GunAnimator.FindPropertyRelative("m_ReloadEmptySpeed");
        m_ReloadEmptySound = m_GunAnimator.FindPropertyRelative("m_ReloadEmptySound");
        m_ReloadVolume = m_GunAnimator.FindPropertyRelative("m_ReloadVolume");
        m_StartReloadAnimation = m_GunAnimator.FindPropertyRelative("m_StartReloadAnimation");
        m_StartReloadSpeed = m_GunAnimator.FindPropertyRelative("m_StartReloadSpeed");
        m_StartReloadSound = m_GunAnimator.FindPropertyRelative("m_StartReloadSound");
        m_StartReloadVolume = m_GunAnimator.FindPropertyRelative("m_StartReloadVolume");
        m_InsertInChamberAnimation = m_GunAnimator.FindPropertyRelative("m_InsertInChamberAnimation");
        m_InsertInChamberSpeed = m_GunAnimator.FindPropertyRelative("m_InsertInChamberSpeed");
        m_InsertInChamberSound = m_GunAnimator.FindPropertyRelative("m_InsertInChamberSound");
        m_InsertInChamberVolume = m_GunAnimator.FindPropertyRelative("m_InsertInChamberVolume");
        m_InsertAnimation = m_GunAnimator.FindPropertyRelative("m_InsertAnimation");
        m_InsertSpeed = m_GunAnimator.FindPropertyRelative("m_InsertSpeed");
        m_InsertSound = m_GunAnimator.FindPropertyRelative("m_InsertSound");
        m_InsertVolume = m_GunAnimator.FindPropertyRelative("m_InsertVolume");
        m_StopReloadAnimation = m_GunAnimator.FindPropertyRelative("m_StopReloadAnimation");
        m_StopReloadSpeed = m_GunAnimator.FindPropertyRelative("m_StopReloadSpeed");
        m_StopReloadSound = m_GunAnimator.FindPropertyRelative("m_StopReloadSound");
        m_StopReloadVolume = m_GunAnimator.FindPropertyRelative("m_StopReloadVolume");

        m_Melee = m_GunAnimator.FindPropertyRelative("m_Melee");
        m_MeleeAnimation = m_GunAnimator.FindPropertyRelative("m_MeleeAnimation");
        m_MeleeSpeed = m_GunAnimator.FindPropertyRelative("m_MeleeSpeed");
        m_MeleeDelay = m_GunAnimator.FindPropertyRelative("m_MeleeDelay");
        m_MeleeSound = m_GunAnimator.FindPropertyRelative("m_MeleeSound");
        m_MeleeVolume = m_GunAnimator.FindPropertyRelative("m_MeleeVolume");
        m_HitSoundList = m_GunAnimator.FindPropertyRelative("m_HitSoundList");
        m_HitVolume = m_GunAnimator.FindPropertyRelative("m_HitVolume");

        m_SwitchMode = m_GunAnimator.FindPropertyRelative("m_SwitchMode");
        m_SwitchModeAnimation = m_GunAnimator.FindPropertyRelative("m_SwitchModeAnimation");
        m_SwitchModeSpeed = m_GunAnimator.FindPropertyRelative("m_SwitchModeSpeed");
        m_SwitchModeSound = m_GunAnimator.FindPropertyRelative("m_SwitchModeSound");
        m_SwitchModeVolume = m_GunAnimator.FindPropertyRelative("m_SwitchModeVolume");

        m_Interact = m_GunAnimator.FindPropertyRelative("m_Interact");
        m_InteractAnimation = m_GunAnimator.FindPropertyRelative("m_InteractAnimation");
        m_InteractDelay = m_GunAnimator.FindPropertyRelative("m_InteractDelay");
        m_InteractSpeed = m_GunAnimator.FindPropertyRelative("m_InteractSpeed");
        m_InteractSound = m_GunAnimator.FindPropertyRelative("m_InteractSound");
        m_InteractVolume = m_GunAnimator.FindPropertyRelative("m_InteractVolume");

        m_Vault = m_GunAnimator.FindPropertyRelative("m_Vault");
        m_VaultAnimation = m_GunAnimator.FindPropertyRelative("m_VaultAnimation");
        m_VaultSpeed = m_GunAnimator.FindPropertyRelative("m_VaultSpeed");
        m_VaultSound = m_GunAnimator.FindPropertyRelative("m_VaultSound");
        m_VaultVolume = m_GunAnimator.FindPropertyRelative("m_VaultVolume");

        m_RunAnimation = m_GunAnimator.FindPropertyRelative("m_RunAnimation");
        m_RunningPosition = m_GunAnimator.FindPropertyRelative("m_RunningPosition");
        m_RunningRotation = m_GunAnimator.FindPropertyRelative("m_RunningRotation");
        m_RunningSpeed = m_GunAnimator.FindPropertyRelative("m_RunningSpeed");

        m_AimAnimation = m_GunAnimator.FindPropertyRelative("m_AimAnimation");
        m_AimingPosition = m_GunAnimator.FindPropertyRelative("m_AimingPosition");
        m_AimingRotation = m_GunAnimator.FindPropertyRelative("m_AimingRotation");
        m_AimInSound = m_GunAnimator.FindPropertyRelative("m_AimInSound");
        m_AimOutSound = m_GunAnimator.FindPropertyRelative("m_AimOutSound");
        m_ZoomAnimation = m_GunAnimator.FindPropertyRelative("m_ZoomAnimation");
        m_AimFOV = m_GunAnimator.FindPropertyRelative("m_AimFOV");
        m_AimingSpeed = m_GunAnimator.FindPropertyRelative("m_AimingSpeed");
        m_HoldBreath = m_GunAnimator.FindPropertyRelative("m_HoldBreath");

        m_GunEffects = serializedObject.FindProperty("m_GunEffects");
        m_MuzzleFlash = m_GunEffects.FindPropertyRelative("m_MuzzleFlash");
        m_MuzzleParticle = m_GunEffects.FindPropertyRelative("m_MuzzleParticle");

        m_Tracer = m_GunEffects.FindPropertyRelative("m_Tracer");
        m_TracerPrefab = m_GunEffects.FindPropertyRelative("m_TracerPrefab");
        m_TracerSpeed = m_GunEffects.FindPropertyRelative("m_TracerSpeed");
        m_TracerOrigin = m_GunEffects.FindPropertyRelative("m_TracerOrigin");

        m_Shell = m_GunEffects.FindPropertyRelative("m_Shell");
        m_ShellParticle = m_GunEffects.FindPropertyRelative("m_ShellParticle");
        m_MinShellSpeed = m_GunEffects.FindPropertyRelative("m_MinShellSpeed");
        m_MaxShellSpeed = m_GunEffects.FindPropertyRelative("m_MaxShellSpeed");
        m_StartDelay = m_GunEffects.FindPropertyRelative("m_StartDelay");

        m_Target.name = m_Target.InspectorName;
    }

    public override void OnInspectorGUI ()
    {
        //Update the serializedProperty - always do this in the beginning of OnInspectorGUI
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Controllers & Managers", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_GunData);
        EditorGUILayout.PropertyField(m_BulletMarkManager);
        EditorGUILayout.PropertyField(m_CameraAnimationsController);
        EditorGUILayout.PropertyField(m_FPController, new GUIContent("FPS Controller"));
        EditorGUILayout.PropertyField(m_CameraTransformReference);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Procedural Animations", EditorStyles.boldLabel);
        Animations();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Muzzle Flash", m_MuzzleFlash);

        if (m_MuzzleFlash.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_MuzzleFlash.boolValue))
            {
                EditorGUILayout.PropertyField(m_MuzzleParticle);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Tracer", m_Tracer);

        if (m_Tracer.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Tracer.boolValue))
            {
                EditorGUILayout.PropertyField(m_TracerPrefab);
                EditorGUILayout.PropertyField(m_TracerSpeed);

                if (m_TracerSpeed.floatValue < 0)
                    EditorGUILayout.HelpBox("Tracer Speed must be greater than 0.", MessageType.Warning);

                EditorGUILayout.PropertyField(m_TracerOrigin);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Shell", m_Shell);

        if (m_Shell.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Shell.boolValue))
            {
                EditorGUILayout.PropertyField(m_ShellParticle);
                EditorGUIHelper.MinMaxSlider("Speed", m_MinShellSpeed, m_MaxShellSpeed, 0, 5);
                EditorGUILayout.PropertyField(m_StartDelay);
            }
        }

        //Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI
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

            EditorGUILayout.LabelField("Lean Animation", FPSEStyles.midLabel);
            using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
            {
                EditorGUILayout.PropertyField(m_Lean);
                if (m_Lean.boolValue)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(m_LeanAngle);
                    EditorGUILayout.PropertyField(m_LeanSpeed);
                } 
            }
            EditorGUI.indentLevel = 1;
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Weapon Recoil", m_WeaponRecoil);

        if (m_WeaponRecoil.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_WeaponRecoil.boolValue))
            {
                EditorGUILayout.PropertyField(m_WeaponRecoilPosition);
                EditorGUILayout.PropertyField(m_WeaponRecoilRotation);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_WeaponRecoilDuration);

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Set as current Position & Rotation", FPSEStyles.leftButton))
                    {
                        m_WeaponRecoilPosition.vector3Value = m_Target.transform.localPosition;
                        m_WeaponRecoilRotation.vector3Value = m_Target.transform.localEulerAngles;
                    }

                    if (GUILayout.Button("Reset", FPSEStyles.rightButton))
                    {
                        m_WeaponRecoilPosition.vector3Value = new Vector3(0, 0, 0);
                        m_WeaponRecoilRotation.vector3Value = Vector3.zero;
                        m_WeaponRecoilDuration.floatValue = 0.1f;
                    }

                    GUILayout.FlexibleSpace();
                }
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Camera Recoil", m_CameraRecoil);

        if (m_CameraRecoil.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_CameraRecoil.boolValue))
            {
                if (m_CameraAnimationsController.objectReferenceValue == null && m_CameraRecoil.boolValue)
                    EditorGUILayout.HelpBox("Camera Animations Controller must be assigned to apply the recoil on camera.", MessageType.Warning);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_MinCameraRecoilRotation);
                EditorGUILayout.PropertyField(m_MaxCameraRecoilRotation);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_CameraRecoilDuration);
                EditorGUILayout.PropertyField(m_CameraReturnDuration);
            }
        }
        #endregion

        #region GUN ANIMATOR

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Sprint", m_RunAnimation);

        if (m_RunAnimation.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_RunAnimation.boolValue))
            {
                using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
                {
                    EditorGUILayout.PropertyField(m_RunningPosition, new GUIContent("Sprint Position"));
                    EditorGUILayout.PropertyField(m_RunningRotation, new GUIContent("Sprint Rotation"));

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(m_RunningSpeed, new GUIContent("Sprint Speed"));

                    EditorGUILayout.Space();
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Set as current Position & Rotation", FPSEStyles.leftButton))
                        {
                            m_RunningPosition.vector3Value = m_Target.transform.localPosition;
                            m_RunningRotation.vector3Value = m_Target.transform.localEulerAngles;
                        }

                        if (GUILayout.Button("Reset", FPSEStyles.rightButton))
                        {
                            m_RunningPosition.vector3Value = Vector3.zero;
                            m_RunningRotation.vector3Value = Vector3.zero;
                            m_RunningSpeed.floatValue = 10;
                        }

                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Aim Down Sights", m_AimAnimation);

        if (m_AimAnimation.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_AimAnimation.boolValue))
            {
                using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
                {
                    EditorGUILayout.PropertyField(m_AimingPosition);
                    EditorGUILayout.PropertyField(m_AimingRotation);

                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(m_AimingSpeed);

                    EditorGUILayout.Space();
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Set as current Position & Rotation", FPSEStyles.leftButton))
                        {
                            m_AimingPosition.vector3Value = m_Target.transform.localPosition;
                            m_AimingRotation.vector3Value = m_Target.transform.localEulerAngles;
                        }

                        if (GUILayout.Button("Reset", FPSEStyles.rightButton))
                        {
                            m_AimingPosition.vector3Value = Vector3.zero;
                            m_AimingRotation.vector3Value = Vector3.zero;
                            m_AimingSpeed.floatValue = 10;
                        }

                        GUILayout.FlexibleSpace();
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_HoldBreath);

                EditorGUI.indentLevel = 1;
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_AimInSound);
                EditorGUILayout.PropertyField(m_AimOutSound);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Zoom", m_ZoomAnimation);

        if (m_ZoomAnimation.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_ZoomAnimation.boolValue))
            {
                EditorGUILayout.PropertyField(m_AimFOV);
            }
        }

        EditorGUILayout.Space();

        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Gun Animations", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_Animator);

        if (m_Animator.objectReferenceValue != null)
            EditorGUILayout.PropertyField(m_SpeedParameter);

        EditorGUIHelper.ToggleHeader("Draw", m_Draw);

        if (m_Draw.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Draw.boolValue))
            {
                EditorGUILayout.PropertyField(m_DrawAnimation);
                EditorGUILayout.PropertyField(m_DrawSpeed);
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
                EditorGUILayout.PropertyField(m_HideSpeed);
                EditorGUILayout.PropertyField(m_HideSound);
                EditorGUILayout.PropertyField(m_HideVolume);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Fire", m_Fire);

        if (m_Fire.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Fire.boolValue))
            {
                using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
                {
                    EditorGUILayout.LabelField("HIP Animations", FPSEStyles.midLabel);

                    if (m_FireAnimationList.arraySize == 0)
                    {
                        m_Target.AddShotAnimation();
                        return;
                    }

                    for (int i = 0; i < m_FireAnimationList.arraySize; i++)
                    {
                        SerializedProperty animation = m_FireAnimationList.GetArrayElementAtIndex(i);
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(animation, new GUIContent(i == 0 ? "Fire Animation" : "Fire Animation " + i));

                            using (new EditorGUI.DisabledGroupScope(m_FireAnimationList.arraySize == 1))
                            {
                                EditorGUI.BeginChangeCheck();
                                if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
                                {
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(m_Target, "Undo Inspector");
                                        m_Target.RemoveShotAnimation(i);
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
                                    m_Target.AddShotAnimation();

                                    EditorUtility.SetDirty(m_Target);
                                    return;
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_FireSpeed);

                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
                {
                    EditorGUILayout.LabelField("Aimed Animations", FPSEStyles.midLabel);

                    if (m_AimedFireAnimationList.arraySize == 0)
                    {
                        m_Target.AddAimedShotAnimation();
                        return;
                    }

                    for (int i = 0; i < m_AimedFireAnimationList.arraySize; i++)
                    {
                        SerializedProperty animation = m_AimedFireAnimationList.GetArrayElementAtIndex(i);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(animation, new GUIContent(i == 0 ? "Aimed Fire Animation" : "Aimed Fire Animation " + i));

                            using (new EditorGUI.DisabledGroupScope(m_AimedFireAnimationList.arraySize == 1))
                            {
                                EditorGUI.BeginChangeCheck();
                                if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
                                {
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(m_Target, "Undo Inspector");
                                        m_Target.RemoveAimedShotAnimation(i);
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
                                    m_Target.AddAimedShotAnimation();

                                    EditorUtility.SetDirty(m_Target);
                                    return;
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_AimedFireSpeed);

                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(m_OverrideLastFire);
                if (m_OverrideLastFire.boolValue)
                {
                    EditorGUI.indentLevel = 2;
                    EditorGUILayout.PropertyField(m_LastFireAnimation);
                }

                EditorGUI.indentLevel = 1;
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_FireAnimationType);

                EditorGUILayout.Space();

                using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
                {
                    EditorGUILayout.LabelField("Fire Sounds", FPSEStyles.midLabel);

                    if (m_FireSoundList.arraySize == 0)
                    {
                        m_Target.AddShotSound();
                        return;
                    }

                    for (int i = 0; i < m_FireSoundList.arraySize; i++)
                    {
                        SerializedProperty sound = m_FireSoundList.GetArrayElementAtIndex(i);

                        using (new EditorGUILayout.HorizontalScope())
                        {
                            EditorGUILayout.PropertyField(sound, new GUIContent(i == 0 ? "Fire Sound" : "Fire Sound " + i));

                            using (new EditorGUI.DisabledGroupScope(m_FireSoundList.arraySize == 1))
                            {
                                EditorGUI.BeginChangeCheck();
                                if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
                                {
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(m_Target, "Undo Inspector");
                                        m_Target.RemoveShotSound(i);
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
                                    m_Target.AddShotSound();

                                    EditorUtility.SetDirty(m_Target);
                                    return;
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_OutOfAmmoSound);
                EditorGUILayout.PropertyField(m_FireVolume);
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Reload", m_Reload);

        if (m_Reload.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Reload.boolValue))
            {
                if (m_Target.ReloadType == GunData.ReloadMode.Magazines)
                {
                    EditorGUILayout.PropertyField(m_ReloadAnimation);
                    EditorGUILayout.PropertyField(m_ReloadSpeed);
                    EditorGUILayout.PropertyField(m_ReloadSound);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(m_ReloadEmptyAnimation);
                    EditorGUILayout.PropertyField(m_ReloadEmptySpeed);
                    EditorGUILayout.PropertyField(m_ReloadEmptySound);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(m_ReloadVolume);

                }
                else if (m_Target.ReloadType == GunData.ReloadMode.BulletByBullet)
                {
                    EditorGUILayout.PropertyField(m_StartReloadAnimation);
                    EditorGUILayout.PropertyField(m_StartReloadSpeed);
                    EditorGUILayout.PropertyField(m_StartReloadSound);
                    EditorGUILayout.PropertyField(m_StartReloadVolume);
                    EditorGUILayout.Space();

                    if (m_Target.HasChamber)
                    {
                        EditorGUILayout.PropertyField(m_InsertInChamberAnimation);
                        EditorGUILayout.PropertyField(m_InsertInChamberSpeed);
                        EditorGUILayout.PropertyField(m_InsertInChamberSound);
                        EditorGUILayout.PropertyField(m_InsertInChamberVolume);
                        EditorGUILayout.Space();
                    }
                    
                    EditorGUILayout.PropertyField(m_InsertAnimation);
                    EditorGUILayout.PropertyField(m_InsertSpeed);
                    EditorGUILayout.PropertyField(m_InsertSound);
                    EditorGUILayout.PropertyField(m_InsertVolume);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(m_StopReloadAnimation);
                    EditorGUILayout.PropertyField(m_StopReloadSpeed);
                    EditorGUILayout.PropertyField(m_StopReloadSound);
                    EditorGUILayout.PropertyField(m_StopReloadVolume);
                }
            }
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.ToggleHeader("Melee Attack", m_Melee);

        if (m_Melee.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            using (new EditorGUI.DisabledScope(!m_Melee.boolValue))
            {
                EditorGUILayout.PropertyField(m_MeleeAnimation);
                EditorGUILayout.PropertyField(m_MeleeSpeed);
                EditorGUILayout.PropertyField(m_MeleeSound);
                EditorGUILayout.PropertyField(m_MeleeVolume);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_MeleeDelay);

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

        if (m_Target.HasSecondaryMode)
        {
            EditorGUI.indentLevel = 0;
            EditorGUIHelper.ToggleHeader("Switch Mode", m_SwitchMode);

            if (m_SwitchMode.isExpanded)
            {
                EditorGUI.indentLevel = 1;
                using (new EditorGUI.DisabledScope(!m_SwitchMode.boolValue))
                {
                    EditorGUILayout.PropertyField(m_SwitchModeAnimation);
                    EditorGUILayout.PropertyField(m_SwitchModeSpeed);
                    EditorGUILayout.PropertyField(m_SwitchModeSound);
                    EditorGUILayout.PropertyField(m_SwitchModeVolume);
                }
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
                EditorGUILayout.PropertyField(m_InteractSpeed);
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
                EditorGUILayout.PropertyField(m_VaultSpeed);
                EditorGUILayout.PropertyField(m_VaultSound);
                EditorGUILayout.PropertyField(m_VaultVolume);
            }
        }

        #endregion
    }
}