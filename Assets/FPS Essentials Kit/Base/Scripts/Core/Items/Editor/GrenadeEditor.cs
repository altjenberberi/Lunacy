/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using Essentials.Items;

[CustomEditor(typeof(Grenade))]
public sealed class GrenadeEditor : Editor
{
    Grenade m_Target;
    SerializedProperty m_Grenade;
    SerializedProperty m_ThrowTransformReference;
    SerializedProperty m_ThrowForce;
    SerializedProperty m_DelayToInstantiate;
    SerializedProperty m_Amount;
    SerializedProperty m_MaxAmount;
    SerializedProperty m_InfiniteGrenades;
    SerializedProperty m_Animator;
    SerializedProperty m_PullAnimation;
    SerializedProperty m_PullSound;
    SerializedProperty m_PullVolume;
    SerializedProperty m_ThrowAnimation;
    SerializedProperty m_ThrowSound;
    SerializedProperty m_ThrowVolume;

    private void OnEnable ()
    {
        m_Target = (target as Grenade);
        m_Target.DisableShadowCasting();

        m_Grenade = serializedObject.FindProperty("m_Grenade");
        m_ThrowTransformReference = serializedObject.FindProperty("m_ThrowTransformReference");
        m_ThrowForce = serializedObject.FindProperty("m_ThrowForce");
        m_DelayToInstantiate = serializedObject.FindProperty("m_DelayToInstantiate");
        m_Amount = serializedObject.FindProperty("m_Amount");
        m_MaxAmount = serializedObject.FindProperty("m_MaxAmount");
        m_InfiniteGrenades = serializedObject.FindProperty("m_InfiniteGrenades");
        m_Animator = serializedObject.FindProperty("m_Animator");
        m_PullAnimation = serializedObject.FindProperty("m_PullAnimation");
        m_PullSound = serializedObject.FindProperty("m_PullSound");
        m_PullVolume = serializedObject.FindProperty("m_PullVolume");
        m_ThrowAnimation = serializedObject.FindProperty("m_ThrowAnimation");
        m_ThrowSound = serializedObject.FindProperty("m_ThrowSound");
        m_ThrowVolume = serializedObject.FindProperty("m_ThrowVolume");
    }

    public override void OnInspectorGUI ()
    {
        //Update the serializedProperty - always do this in the beginning of OnInspectorGUI
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_Grenade);
        EditorGUILayout.PropertyField(m_ThrowTransformReference);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_ThrowForce);
        EditorGUILayout.PropertyField(m_DelayToInstantiate);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_InfiniteGrenades);

        EditorGUI.indentLevel = 0;
        if (!m_InfiniteGrenades.boolValue)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(m_Amount);
            EditorGUILayout.PropertyField(m_MaxAmount);
        }

        EditorGUI.indentLevel = 0;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grenade Animations", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_Animator);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_PullAnimation);
        EditorGUILayout.PropertyField(m_PullSound);
        EditorGUILayout.PropertyField(m_PullVolume);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_ThrowAnimation);
        EditorGUILayout.PropertyField(m_ThrowSound);
        EditorGUILayout.PropertyField(m_ThrowVolume);

        serializedObject.ApplyModifiedProperties();
    }
}