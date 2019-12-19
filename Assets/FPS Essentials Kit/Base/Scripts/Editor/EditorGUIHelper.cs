/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;
using System.Reflection;

public static class EditorGUIHelper
{
    private static MethodInfo m_BoldFontMethodInfo = null;

    #region Search and Load Assets

    static string m_EditorResourcesPath = string.Empty;
    private static string path = string.Empty;

    internal static string EditorResourcesPath
    {
        get
        {
            if (string.IsNullOrEmpty(m_EditorResourcesPath))
            {
                string path;

                if (SearchForEditorResourcesPath(out path))
                    m_EditorResourcesPath = path;
                else
                    Debug.LogError("Unable to locate editor resources.");
            }

            return m_EditorResourcesPath;
        }
    }

    private static bool SearchForEditorResourcesPath (out string path)
    {
        path = string.Empty;

        string searchStr = EditorGUIHelper.path;
        string str = null;

        foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
        {
            if (assetPath.Contains(searchStr))
            {
                str = assetPath;
                break;
            }
        }

        if (str == null)
            return false;

        path = str.Substring(0, str.LastIndexOf(searchStr) + searchStr.Length);
        return true;
    }

    #endregion

    //Load assets to use in editor
    internal static T Load<T> (string path, string name) where T : Object
    {
        EditorGUIHelper.path = path;
        return AssetDatabase.LoadAssetAtPath<T>(EditorResourcesPath + name);
    }

    //Unity has a private method called EditorGUIUtility.SetBoldDefaultFont, wich they use internally to
    //show modified prefab values in bold in the inspector. We are using reflection to acess this method
    //so we can mirror this behaviour for our costom inspectors.
    //Warning: Using reflection like this is sneaky and can break in future versions if Unity decides to
    //move or rename this method
    public static void SetBoldDefaultFont (bool value)
    {
        if (m_BoldFontMethodInfo == null)
        {
            m_BoldFontMethodInfo = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
        }

        m_BoldFontMethodInfo.Invoke(null, new[] { value as object });
    }

    public static Rect GetRect (float fieldSize = 18)
    {
        return GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(fieldSize));
    }

    //Cria um simples cabeçalho com a função de Foldout
    public static void FoldoutHeader (string title, SerializedProperty property)
    {
        EditorGUILayout.Space();

        var rect = GUILayoutUtility.GetRect(16f, 22f, FPSEStyles.header);
        GUI.Box(rect, title, FPSEStyles.header);

        var foldoutRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        var e = Event.current;

        if (e.type == EventType.Repaint)
            FPSEStyles.headerFoldout.Draw(foldoutRect, false, false, property.isExpanded, false);

        if (e.type == EventType.MouseDown)
        {
            if (rect.Contains(e.mousePosition))
            {
                property.isExpanded = !property.isExpanded;
                e.Use();
            }
        }
    }

    //Cria um cabeçalho dinâmico com função de Foldout e um toggle para ativar e desativar uma propriedade
    public static void ToggleHeader (string title, SerializedProperty property)
    {
        EditorGUILayout.Space();

        //bool display = property.isExpanded;

        var rect = GUILayoutUtility.GetRect(16f, 22f, FPSEStyles.header);
        GUI.Box(rect, title, FPSEStyles.header);

        var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);
        var e = Event.current;

        if (e.type == EventType.Repaint)
            FPSEStyles.headerCheckbox.Draw(toggleRect, false, false, property.boolValue, false);

        if (e.type == EventType.MouseDown)
        {
            if (toggleRect.Contains(e.mousePosition))
            {
                property.boolValue = !property.boolValue;
                e.Use();
            }
            else if (rect.Contains(e.mousePosition))
            {
                property.isExpanded = !property.isExpanded;
                e.Use();
            }
        }
        //return display;
    }

    public static void MinMaxSlider (string label, SerializedProperty minValue, SerializedProperty maxValue, float minLimit, float maxLimit, string format = "F1")
    {
        EditorGUI.BeginChangeCheck();
        using (new EditorGUILayout.HorizontalScope())
        {
            SetBoldDefaultFont(minValue.prefabOverride || maxValue.prefabOverride);

            EditorGUILayout.PrefixLabel(label);

            GUILayout.Label("Min: " + minValue.floatValue.ToString(format));

            float _minValue = minValue.floatValue;
            float _maxValue = maxValue.floatValue;

            EditorGUILayout.MinMaxSlider(ref _minValue, ref _maxValue, minLimit, maxLimit);

            if (EditorGUI.EndChangeCheck())
            {
                minValue.floatValue = _minValue;
                maxValue.floatValue = _maxValue;
            }

            GUILayout.Label("Max: " + maxValue.floatValue.ToString(format));
        }
    }

    public static void LerpAnimationDrawer (SerializedProperty property, bool enableSetAndReset = false, Transform transf = null)
    {
        SerializedProperty m_TargetPosition = property.FindPropertyRelative("m_TargetPosition");
        SerializedProperty m_TargetRotation = property.FindPropertyRelative("m_TargetRotation");
        SerializedProperty m_Duration = property.FindPropertyRelative("m_Duration");
        SerializedProperty m_ReturnDuration = property.FindPropertyRelative("m_ReturnDuration");

        EditorGUILayout.PropertyField(m_TargetPosition);
        EditorGUILayout.PropertyField(m_TargetRotation);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(m_Duration);
        EditorGUILayout.PropertyField(m_ReturnDuration);

        if (enableSetAndReset)
        {
            EditorGUILayout.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Set as current Position & Rotation", FPSEStyles.leftButton))
                {
                    m_TargetPosition.vector3Value = transf.localPosition;
                    m_TargetRotation.vector3Value = transf.localEulerAngles;
                }

                if (GUILayout.Button("Reset", FPSEStyles.rightButton))
                {
                    m_TargetPosition.vector3Value = Vector3.zero;
                    m_TargetRotation.vector3Value = Vector3.zero;
                    m_Duration.floatValue = 0;
                    m_ReturnDuration.floatValue = 0;
                }

                GUILayout.FlexibleSpace();
            }
        }
    }
}
