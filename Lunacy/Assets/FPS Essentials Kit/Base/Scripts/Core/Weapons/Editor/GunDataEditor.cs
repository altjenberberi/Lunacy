/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using Essentials.Weapons;

[CustomEditor(typeof(GunData))]
public sealed class GunDataEditor : Editor
{
    private SerializedProperty m_GunName;
    private SerializedProperty m_Icon;
    private SerializedProperty m_DroppablePrefab;
    private SerializedProperty m_Weight;
    private SerializedProperty m_Size;
    private SerializedProperty m_MeleeDamage;
    private SerializedProperty m_MeleeForce;

    private SerializedProperty m_PrimaryFireMode;
    private SerializedProperty m_SecondaryFireMode;
    private SerializedProperty m_PrimaryRateOfFire;
    private SerializedProperty m_SecondaryRateOfFire;

    private SerializedProperty m_Force;
    private SerializedProperty m_Range;

    private SerializedProperty m_BulletsPerShoot;
    private SerializedProperty m_BulletsPerBurst;

    private SerializedProperty m_AffectedLayers;

    private SerializedProperty m_DamageMode;
    private SerializedProperty m_MinDamage;
    private SerializedProperty m_MaxDamage;
    private SerializedProperty m_DamageFalloffCurve;

    private SerializedProperty m_PenetrateObjects;
    private SerializedProperty m_PenetrationPower;

    private SerializedProperty m_ReloadMode;
    private SerializedProperty m_RoundsPerMagazine;
    private SerializedProperty m_HasChamber;
    private SerializedProperty m_InitialMagazines;
    private SerializedProperty m_MaxMagazines;

    private SerializedProperty m_HIPAccuracy;
    private SerializedProperty m_AIMAccuracy;
    private SerializedProperty m_MinimumAccuracy;
    private SerializedProperty m_DecreaseRateByShooting;
    private SerializedProperty m_DecreaseRateByWalking;

    private void OnEnable ()
    {
        m_GunName = serializedObject.FindProperty("m_GunName");
        m_Icon = serializedObject.FindProperty("m_Icon");
        m_DroppablePrefab = serializedObject.FindProperty("m_DroppablePrefab");
        m_Weight = serializedObject.FindProperty("m_Weight");
        m_Size = serializedObject.FindProperty("m_Size");
        m_MeleeDamage = serializedObject.FindProperty("m_MeleeDamage");
        m_MeleeForce = serializedObject.FindProperty("m_MeleeForce");

        m_PrimaryFireMode = serializedObject.FindProperty("m_PrimaryFireMode");
        m_SecondaryFireMode = serializedObject.FindProperty("m_SecondaryFireMode");
        m_PrimaryRateOfFire = serializedObject.FindProperty("m_PrimaryRateOfFire");
        m_SecondaryRateOfFire = serializedObject.FindProperty("m_SecondaryRateOfFire");
        m_Force = serializedObject.FindProperty("m_Force");
        m_Range = serializedObject.FindProperty("m_Range");
        m_BulletsPerShoot = serializedObject.FindProperty("m_BulletsPerShoot");
        m_BulletsPerBurst = serializedObject.FindProperty("m_BulletsPerBurst");

        m_MinDamage = serializedObject.FindProperty("m_MinDamage");
        m_MaxDamage = serializedObject.FindProperty("m_MaxDamage");
        m_DamageFalloffCurve = serializedObject.FindProperty("m_DamageFalloffCurve");

        m_PenetrateObjects = serializedObject.FindProperty("m_PenetrateObjects");
        m_PenetrationPower = serializedObject.FindProperty("m_PenetrationPower");

        m_ReloadMode = serializedObject.FindProperty("m_ReloadMode");
        m_RoundsPerMagazine = serializedObject.FindProperty("m_RoundsPerMagazine");
        m_HasChamber = serializedObject.FindProperty("m_HasChamber");
        m_InitialMagazines = serializedObject.FindProperty("m_InitialMagazines");
        m_MaxMagazines = serializedObject.FindProperty("m_MaxMagazines");

        m_DamageMode = serializedObject.FindProperty("m_DamageMode");
        m_AffectedLayers = serializedObject.FindProperty("m_AffectedLayers");

        m_HIPAccuracy = serializedObject.FindProperty("m_HIPAccuracy");
        m_AIMAccuracy = serializedObject.FindProperty("m_AIMAccuracy");
        m_MinimumAccuracy = serializedObject.FindProperty("m_MinimumAccuracy");
        m_DecreaseRateByShooting = serializedObject.FindProperty("m_DecreaseRateByShooting");
        m_DecreaseRateByWalking = serializedObject.FindProperty("m_DecreaseRateByWalking");
    }

    public override void OnInspectorGUI ()
    {
        //Update the serializedProperty - always do this in the beginning of OnInspectorGUI
        serializedObject.Update();

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("General Settings", m_GunName);

        if (m_GunName.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_GunName, new UnityEngine.GUIContent("Name"));
            EditorGUILayout.PropertyField(m_Icon);
            EditorGUILayout.PropertyField(m_DroppablePrefab);

            EditorGUILayout.PropertyField(m_Weight);
            EditorGUILayout.PropertyField(m_Size);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_MeleeDamage);
            EditorGUILayout.PropertyField(m_MeleeForce);
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Shooting Settings", m_PrimaryFireMode);

        if (m_PrimaryFireMode.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_PrimaryFireMode);

            EditorGUILayout.PropertyField(m_SecondaryFireMode);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_PrimaryRateOfFire);

            if ((GunData.FireMode)m_SecondaryFireMode.enumValueIndex != GunData.FireMode.None)
                EditorGUILayout.PropertyField(m_SecondaryRateOfFire);

            EditorGUILayout.PropertyField(m_Force);
            EditorGUILayout.PropertyField(m_Range);

            if (m_Range.floatValue <= 0)
                EditorGUILayout.HelpBox("Range must be greater than 0.", MessageType.Warning);

            if ((GunData.FireMode)m_PrimaryFireMode.enumValueIndex == GunData.FireMode.ShotgunAuto
                || (GunData.FireMode)m_PrimaryFireMode.enumValueIndex == GunData.FireMode.ShotgunSingle
                || (GunData.FireMode)m_SecondaryFireMode.enumValueIndex == GunData.FireMode.ShotgunAuto
                || (GunData.FireMode)m_SecondaryFireMode.enumValueIndex == GunData.FireMode.ShotgunSingle)
            {
                EditorGUILayout.PropertyField(m_BulletsPerShoot);

                if (m_BulletsPerShoot.intValue < 1)
                    EditorGUILayout.HelpBox("Bullets per shot must be greater than 0.", MessageType.Warning);
            }

            if ((GunData.FireMode)m_PrimaryFireMode.enumValueIndex == GunData.FireMode.Burst
                || (GunData.FireMode)m_SecondaryFireMode.enumValueIndex == GunData.FireMode.Burst)
            {
                EditorGUILayout.PropertyField(m_BulletsPerBurst);

                if (m_BulletsPerBurst.intValue < 1)
                    EditorGUILayout.HelpBox("Bullets per burst must be greater than 0.", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(m_AffectedLayers);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_DamageMode);

            EditorGUIHelper.MinMaxSlider("Damage", m_MinDamage, m_MaxDamage, 0, 100, "F0");

            if ((GunData.DamageMode)m_DamageMode.enumValueIndex == GunData.DamageMode.DecreaseByDistance)
                EditorGUILayout.PropertyField(m_DamageFalloffCurve);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_PenetrateObjects);

            if (m_PenetrateObjects.boolValue)
                EditorGUILayout.PropertyField(m_PenetrationPower);
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Magazine Settings", m_ReloadMode);

        if (m_ReloadMode.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_ReloadMode);

            EditorGUILayout.PropertyField(m_RoundsPerMagazine);

            if (m_RoundsPerMagazine.intValue < 1)
                EditorGUILayout.HelpBox("Bullets per magazine must be greater than 0.", MessageType.Warning);

            EditorGUILayout.PropertyField(m_HasChamber);

            EditorGUILayout.PropertyField(m_InitialMagazines);

            if (m_InitialMagazines.intValue < 0)
                EditorGUILayout.HelpBox("Initial magazines must be greater than or equal to 0.", MessageType.Warning);

            EditorGUILayout.PropertyField(m_MaxMagazines);

            if (m_MaxMagazines.intValue < 0)
                EditorGUILayout.HelpBox("Max magazines must be greater than or equal to 0.", MessageType.Warning);
        }

        EditorGUI.indentLevel = 0;
        EditorGUIHelper.FoldoutHeader("Accuracy Settings", m_HIPAccuracy);

        if (m_HIPAccuracy.isExpanded)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_HIPAccuracy);
            EditorGUILayout.PropertyField(m_AIMAccuracy);
            EditorGUILayout.PropertyField(m_MinimumAccuracy);
            EditorGUILayout.PropertyField(m_DecreaseRateByShooting);
            EditorGUILayout.PropertyField(m_DecreaseRateByWalking);
        }

        //Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI
        serializedObject.ApplyModifiedProperties();
    }
}