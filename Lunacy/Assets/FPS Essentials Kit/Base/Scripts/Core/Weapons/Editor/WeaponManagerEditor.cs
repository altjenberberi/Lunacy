/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;
using Essentials.Weapons;

[CustomEditor(typeof(WeaponManager))]
public sealed class WeaponManagerEditor : Editor
{
    private WeaponManager m_Target;

    private SerializedProperty m_FPController;
    private SerializedProperty m_CameraTransformReference;
    private SerializedProperty m_InteractionRange;
    private SerializedProperty m_AmmoTag;
    private SerializedProperty m_AdrenalinePackTag;
    private SerializedProperty m_WeaponUI;
    private SerializedProperty m_EquippedWeaponsList;
    private SerializedProperty m_ItemPickupSound;
    private SerializedProperty m_ItemPickupVolume;
    private SerializedProperty m_WeaponList;
    private SerializedProperty m_DefaultWeapon;
    private SerializedProperty m_FragGrenade;
    private SerializedProperty m_Adrenaline;

    private void OnEnable ()
    {
        m_Target = (target as WeaponManager);
        m_FPController = serializedObject.FindProperty("m_FPController");
        m_CameraTransformReference = serializedObject.FindProperty("m_CameraTransformReference");
        m_InteractionRange = serializedObject.FindProperty("m_InteractionRange");
        m_AmmoTag = serializedObject.FindProperty("m_AmmoTag");
        m_AdrenalinePackTag = serializedObject.FindProperty("m_AdrenalinePackTag");
        m_WeaponUI = serializedObject.FindProperty("m_WeaponUI");
        m_EquippedWeaponsList = serializedObject.FindProperty("m_EquippedWeaponsList");
        m_ItemPickupSound = serializedObject.FindProperty("m_ItemPickupSound");
        m_ItemPickupVolume = serializedObject.FindProperty("m_ItemPickupVolume");
        m_WeaponList = serializedObject.FindProperty("m_WeaponList");
        m_DefaultWeapon = serializedObject.FindProperty("m_DefaultWeapon");
        m_FragGrenade = serializedObject.FindProperty("m_FragGrenade");
        m_Adrenaline = serializedObject.FindProperty("m_Adrenaline");
    }

    public override void OnInspectorGUI ()
    {
        //Update the serializedProperty - always do this in the beginning of OnInspectorGUI
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_FPController, new GUIContent("FPS Controller"));
        EditorGUILayout.PropertyField(m_CameraTransformReference);
        EditorGUILayout.PropertyField(m_InteractionRange);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_AmmoTag);
        EditorGUILayout.PropertyField(m_AdrenalinePackTag);
        EditorGUILayout.PropertyField(m_ItemPickupSound);
        EditorGUILayout.PropertyField(m_ItemPickupVolume);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_WeaponUI);

        EditorGUILayout.Space();
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Equipped Weapons", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
        {
            if (m_EquippedWeaponsList.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No weapon slot", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Add new slot"))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddWeaponSlot();

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_EquippedWeaponsList.arraySize; i++)
                {
                    ShowEquippedWeapons(i);
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Weapon List", EditorStyles.boldLabel);

        using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
        {
            if (m_WeaponList.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No weapon available", MessageType.Info);

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Add new weapon"))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddWeapon();

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_WeaponList.arraySize; i++)
                {
                    ShowWeaponList(i);
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_DefaultWeapon);

        EditorGUILayout.Space();
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Items", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(m_FragGrenade);
        EditorGUILayout.PropertyField(m_Adrenaline);

        //Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI
        serializedObject.ApplyModifiedProperties();
    }

    private void ShowEquippedWeapons (int index)
    {
        SerializedProperty weapon = m_EquippedWeaponsList.GetArrayElementAtIndex(index);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (weapon.objectReferenceValue != null)
            {
                EditorGUILayout.LabelField("Slot " + (index + 1) + "  →\t" + weapon.objectReferenceValue.name);
            }
            else
            {
                EditorGUILayout.LabelField("Slot " + (index + 1) + "  →\tEmpty");
            }

            if (weapon.objectReferenceValue != null)
            {
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Unequip", FPSEStyles.button))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.UnequipWeapon(index);

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.RemoveWeaponSlot(index);

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
                    m_Target.AddWeaponSlot();

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }
        }
    }

    private void ShowWeaponList (int index)
    {
        SerializedProperty weapon = m_WeaponList.GetArrayElementAtIndex(index);

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.PropertyField(weapon, new GUIContent("Weapon " + (index + 1)));

            if (weapon.objectReferenceValue != null)
            {
                if (!m_Target.IsEquipped((Essentials.IWeapon)weapon.objectReferenceValue) && m_Target.HasFreeSlot)
                {
                    EditorGUI.BeginChangeCheck();
                    if (GUILayout.Button("Equip", FPSEStyles.button))
                    {
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(m_Target, "Undo Inspector");
                            m_Target.EquipWeapon(index);

                            EditorUtility.SetDirty(m_Target);
                            return;
                        }
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
            {
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(m_Target, "Undo Inspector");
                    m_Target.RemoveWeapon(index);

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
                    m_Target.AddWeapon();

                    EditorUtility.SetDirty(m_Target);
                    return;
                }
            }
        }
    }
}