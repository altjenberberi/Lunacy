/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

public static class FPSEStyles
{
    public static GUIStyle leftLabel;
    public static GUIStyle midLabel;
    public static GUIStyle rightLabel;

    public static GUIStyle header;
    public static GUIStyle headerFoldout;
    public static GUIStyle headerCheckbox;

    public static GUIStyle background;

    public static GUIStyle leftButton;
    public static GUIStyle midButton;
    public static GUIStyle rightButton;

    public static GUIStyle button;
    public static GUIStyle miniButton;

    public static GUIStyle none;

    static FPSEStyles ()
    {
        header = new GUIStyle("ShurikenModuleTitle")
        {
            font = (new GUIStyle("Label")).font,
            border = new RectOffset(15, 7, 4, 4),
            fixedHeight = 22,
            contentOffset = new Vector2(20f, -2f)
        };

        none = new GUIStyle();

        headerFoldout = new GUIStyle("Foldout");
        headerCheckbox = new GUIStyle("ShurikenCheckMark");

        miniButton = new GUIStyle("minibutton");
        leftButton = new GUIStyle("ButtonLeft");
        midButton = new GUIStyle("ButtonMid");
        rightButton = new GUIStyle("ButtonRight");

        background = new GUIStyle("HelpBox");

        leftLabel = new GUIStyle("Label")
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleLeft
        };

        midLabel = new GUIStyle("Label")
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter
        };

        rightLabel = new GUIStyle("Label")
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleRight
        };

        button = new GUIStyle("button")
        {
            fixedHeight = 18
        };
    }
}
