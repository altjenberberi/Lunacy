/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.Rendering;

//[InitializeOnLoad]
public class HelpWindow : EditorWindow
{
    private static int m_MaxTags = 10000;
    private static int m_MaxLayers = 31;
    //private static bool m_ShowAtStartup;

    private static Texture2D m_FPSELogo;
    private static Texture2D m_TheAssetLabLogo;

    /*static HelpWindow ()
    {
        EditorApplication.update += ShowAtStartup;
    }

    private static void ShowAtStartup ()
    {
        if (!PlayerPrefs.HasKey("ShowAtStartup"))
        {
            PlayerPrefs.SetInt("ShowAtStartup", 1);
        }

        m_ShowAtStartup = PlayerPrefs.GetInt("ShowAtStartup") == 1;

        if (m_ShowAtStartup)
        {
            HelpWindow window = (HelpWindow)GetWindow(typeof(HelpWindow), true, "FPS Essentials Kit");
            window.minSize = new Vector2(580, 360);
            window.maxSize = new Vector2(580, 360);
            window.Show();

            if (m_FPSELogo == null)
                m_FPSELogo = EditorGUIHelper.Load<Texture2D>("/Scripts/Editor Resources/", "fpseLogo.png");

            if (m_TheAssetLabLogo == null)
                m_TheAssetLabLogo = EditorGUIHelper.Load<Texture2D>("/Scripts/Editor Resources/", "tal.png");
        }

        EditorApplication.update -= ShowAtStartup;
    }*/

    [MenuItem("Window/FPS Essentials/Help")]
    private static void Init ()
    {
        HelpWindow window = (HelpWindow)GetWindow(typeof(HelpWindow), true, "FPS Essentials Kit");
        window.minSize = new Vector2(580, 360);
        window.maxSize = new Vector2(580, 360);
        window.Show();

        if (m_FPSELogo == null)
            m_FPSELogo = EditorGUIHelper.Load<Texture2D>("/Scripts/Editor Resources/", "fpseLogo.png");

        if (m_TheAssetLabLogo == null)
            m_TheAssetLabLogo = EditorGUIHelper.Load<Texture2D>("/Scripts/Editor Resources/", "tal.png");
    }

    private void OnGUI ()
    {
        using (new GUILayout.VerticalScope())
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label(m_FPSELogo, GUILayout.Width(201), GUILayout.Height(83.5f));
                GUILayout.FlexibleSpace();
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("The Ultimate Unity First Person Shooter Game solution");
                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(48);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Online Documentation"))
                {

                }

                GUILayout.FlexibleSpace();
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Youtube Tutorials"))
                {
                    Application.OpenURL("https://www.youtube.com/channel/UC3WdAAlY_BiFCgToYg6osCA");
                }

                GUILayout.FlexibleSpace();
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Forums"))
                {
                    Application.OpenURL("https://www.theassetlab.com/forum.html");
                }

                GUILayout.FlexibleSpace();
            }

            GUILayout.FlexibleSpace();
            using (new GUILayout.HorizontalScope())
            {
                bool fix_tagsAndLayers = !TagExists("Ammo") || !TagExists("Adrenaline Pack") || !LayerExists("Viewmodel");
                bool fix_colorSpace = PlayerSettings.colorSpace != ColorSpace.Linear;
                bool fix_renderingPath = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier1).renderingPath != RenderingPath.DeferredShading
                    || EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier2).renderingPath != RenderingPath.DeferredShading
                    || EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3).renderingPath != RenderingPath.DeferredShading;

                if (fix_tagsAndLayers || fix_colorSpace || fix_renderingPath)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        EditorGUILayout.HelpBox((fix_tagsAndLayers ? "There are Tags and/or Layers missing in the project \n" : string.Empty)
                            + (fix_colorSpace ? "Use Linear Color Space \n" : string.Empty)
                            + (fix_renderingPath ? "Use Deferred Rendering Path" : string.Empty), MessageType.Warning);

                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("Fix now"))
                            {
                                if (!TagExists("Ammo"))
                                    AddTag("Ammo");

                                if (!TagExists("Adrenaline Pack"))
                                    AddTag("Adrenaline Pack");

                                if (!LayerExists("Viewmodel"))
                                    AddLayer("Viewmodel");

                                if (PlayerSettings.colorSpace != ColorSpace.Linear)
                                {
                                    PlayerSettings.colorSpace = ColorSpace.Linear;
                                    Debug.Log("Changed color space to Linear");
                                }

                                TierSettings tier1 = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier1);
                                if (tier1.renderingPath != RenderingPath.DeferredShading)
                                {
                                    tier1.renderingPath = RenderingPath.DeferredShading;
                                    EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier1, tier1);
                                    Debug.Log("Changed Graphics Tier 1 Rendering Path to Deferred");
                                }

                                TierSettings tier2 = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier2);
                                if (tier2.renderingPath != RenderingPath.DeferredShading)
                                {
                                    tier2.renderingPath = RenderingPath.DeferredShading;
                                    EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier2, tier2);
                                    Debug.Log("Changed Graphics Tier 2 Rendering Path to Deferred");
                                }

                                TierSettings tier3 = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3);
                                if (tier3.renderingPath != RenderingPath.DeferredShading)
                                {
                                    tier3.renderingPath = RenderingPath.DeferredShading;
                                    EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3, tier3);
                                    Debug.Log("Changed Graphics Tier 3 Rendering Path to Deferred");
                                }
                            }
                        }
                    }
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(10);

            /*using (new GUILayout.HorizontalScope())
            {
                m_ShowAtStartup = GUILayout.Toggle(m_ShowAtStartup, "Show At Startup");

                if (GUI.changed)
                    PlayerPrefs.SetInt("ShowAtStartup", m_ShowAtStartup ? 1 : 0);
            }*/

            if (GUI.Button(new Rect(457, 267, 118, 89), m_TheAssetLabLogo, FPSEStyles.none))
            {
                Application.OpenURL("https://www.theassetlab.com/");
            }
        }
    }

    //Original code written by Leslie Young and ctwheels
    //https://answers.unity.com/questions/33597/is-it-possible-to-create-a-tag-programmatically.html

    public static bool AddTag (string tagName)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // Tags Property
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        if (tagsProp.arraySize >= m_MaxTags)
        {
            Debug.Log("No more tags can be added to the Tags property. You have " + tagsProp.arraySize + " tags");
            return false;
        }

        // if not found, add it
        if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
        {
            int index = tagsProp.arraySize;

            // Insert new array element
            tagsProp.InsertArrayElementAtIndex(index);

            SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
            sp.stringValue = tagName;

            Debug.Log("Tag: " + tagName + " has been added");

            // Save settings
            tagManager.ApplyModifiedProperties();
            return true;
        }
        return false;
    }

    // Checks to see if tag exists
    public static bool TagExists (string tagName)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // Tags Property
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        return PropertyExists(tagsProp, 0, m_MaxTags, tagName);
    }

    public static bool AddLayer (string layerName)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // Layers Property
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        if (!PropertyExists(layersProp, 0, m_MaxLayers, layerName))
        {
            SerializedProperty sp;

            // Start at layer 9th index -> 8 (zero based) => first 8 reserved for unity / greyed out
            for (int i = 8, j = m_MaxLayers; i < j; i++)
            {
                sp = layersProp.GetArrayElementAtIndex(i);
                if (sp.stringValue == "")
                {
                    // Assign string value to layer
                    sp.stringValue = layerName;

                    Debug.Log("Layer: " + layerName + " has been added");

                    // Save settings
                    tagManager.ApplyModifiedProperties();
                    return true;
                }
                if (i == j)
                    Debug.Log("All allowed layers have been filled");
            }
        }
        return false;
    }

    // Checks to see if layer exists
    public static bool LayerExists (string layerName)
    {
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // Layers Property
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        return PropertyExists(layersProp, 0, m_MaxLayers, layerName);
    }

    // Checks if the value exists in the property
    private static bool PropertyExists (SerializedProperty property, int start, int end, string value)
    {
        for (int i = start; i < end; i++)
        {
            SerializedProperty t = property.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(value))
            {
                return true;
            }
        }
        return false;
    }
}
