/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;
using Essentials.Input;

[CustomEditor(typeof(InputBindings))]
public sealed class InputBindingsEditor : Editor
{
    private InputBindings m_Target;

    private SerializedProperty m_Buttons;
    private SerializedProperty m_Axes;

    private static int m_ToolbarIndex = 0;

    private void OnEnable ()
    {
        m_Target = (InputBindings)target;

        m_Buttons = serializedObject.FindProperty("m_Buttons");
        m_Axes = serializedObject.FindProperty("m_Axes");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update();

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();
            m_ToolbarIndex = GUILayout.Toolbar(m_ToolbarIndex, new string[] { "Axes", "Buttons" }, GUILayout.Height(24), GUILayout.Width(256));
            GUILayout.FlexibleSpace();
        }

        EditorGUILayout.Space();

        if (m_ToolbarIndex == 0)
        {
            for (int i = 0; i < m_Axes.arraySize; i++)
            {
                DrawAxes(i);
                EditorGUILayout.Space();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("New Axis", FPSEStyles.miniButton, GUILayout.Height(28), GUILayout.Width(128)))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddAxis("Axis", string.Empty, string.Empty, 1, 1, 0.01f);

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }

                GUILayout.FlexibleSpace();
            }
        }
        else if (m_ToolbarIndex == 1)
        {
            for (int i = 0; i < m_Buttons.arraySize; i++)
            {
                DrawButtons(i);
                EditorGUILayout.Space();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("New Button", FPSEStyles.miniButton, GUILayout.Height(28), GUILayout.Width(128)))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(m_Target, "Undo Inspector");
                        m_Target.AddButton("Button", string.Empty);

                        EditorUtility.SetDirty(m_Target);
                        return;
                    }
                }

                GUILayout.FlexibleSpace();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawAxes (int index)
    {
        SerializedProperty axis = m_Axes.GetArrayElementAtIndex(index);

        SerializedProperty axisName = axis.FindPropertyRelative("m_Name");
        SerializedProperty axisPositiveKey = axis.FindPropertyRelative("m_PositiveKey");
        SerializedProperty axisNegativeKey = axis.FindPropertyRelative("m_NegativeKey");
        SerializedProperty axisSensitivity = axis.FindPropertyRelative("m_Sensitivity");
        SerializedProperty axisGravity = axis.FindPropertyRelative("m_Gravity");
        SerializedProperty m_DeadZone = axis.FindPropertyRelative("m_DeadZone");

        using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
        {
            using (new EditorGUILayout.HorizontalScope(FPSEStyles.header))
            {
                GUILayout.Label(axisName.stringValue);
                GUILayout.FlexibleSpace();

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Remove", FPSEStyles.miniButton))
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove " + axisName.stringValue + " ?", "Yes", "No"))
                        {
                            Undo.RecordObject(m_Target, "Undo Inspector");
                            m_Target.RemoveAxis(index);
                            EditorUtility.SetDirty(target);
                            return;
                        }
                    }
                }
            }

            EditorGUILayout.PropertyField(axisName);
            EditorGUILayout.PropertyField(axisPositiveKey);
            EditorGUILayout.PropertyField(axisNegativeKey);
            EditorGUILayout.PropertyField(axisSensitivity);
            EditorGUILayout.PropertyField(axisGravity);
            EditorGUILayout.PropertyField(m_DeadZone);
        }
    }

    private void DrawButtons (int index)
    {
        SerializedProperty button = m_Buttons.GetArrayElementAtIndex(index);

        SerializedProperty buttonName = button.FindPropertyRelative("m_Name");
        SerializedProperty buttonKeys = button.FindPropertyRelative("m_Keys");

        using (new EditorGUILayout.VerticalScope(FPSEStyles.background))
        {
            using (new EditorGUILayout.HorizontalScope(FPSEStyles.header))
            {
                GUILayout.Label(buttonName.stringValue);
                GUILayout.FlexibleSpace();

                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button("Remove", FPSEStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove " + buttonName.stringValue + " ?", "Yes", "No"))
                    {
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(m_Target, "Undo Inspector");
                            m_Target.RemoveButton(index);
                            EditorUtility.SetDirty(m_Target);
                            return;
                        }
                    }
                }
            }

            EditorGUILayout.PropertyField(buttonName);

            EditorGUILayout.Space();

            for (int i = 0; i < buttonKeys.arraySize; i++)
            {
                SerializedProperty Key = buttonKeys.GetArrayElementAtIndex(i);

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(Key, new GUIContent(i == 0 ? "Default Key" : "Custom Key " + i));

                    using (new EditorGUI.DisabledGroupScope(buttonKeys.arraySize == 1))
                    {
                        EditorGUI.BeginChangeCheck();
                        if (GUILayout.Button("-", FPSEStyles.leftButton, GUILayout.Width(24)))
                        {
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(m_Target, "Undo Inspector");
                                m_Target.Buttons[index].RemoveKey(i);

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
                            m_Target.Buttons[index].AddNewKey(Key.stringValue);

                            EditorUtility.SetDirty(m_Target);
                            return;
                        }
                    }
                }
            }
        }
    }
}