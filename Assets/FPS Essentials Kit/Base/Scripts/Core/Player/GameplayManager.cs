/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using Essentials.Input;
using UnityEngine;

[AddComponentMenu("FPS Essentials/Managers/Game Manager"), DisallowMultipleComponent]
public sealed class GameplayManager : Singleton<GameplayManager>
{
    public enum ActionMode
    {
        Press,
        Hold
    }

    [SerializeField]
    private ActionMode m_AimStyle = ActionMode.Press;

    [SerializeField]
    private ActionMode m_CrouchStyle = ActionMode.Press;

    [SerializeField]
    private ActionMode m_SprintStyle = ActionMode.Press;

    [SerializeField]
    private ActionMode m_LeanStyle = ActionMode.Press;

    [SerializeField]
    private bool m_InvertHorizontalAxis = false;

    [SerializeField]
    private bool m_InvertVerticalAxis = false;

    [SerializeField]
    [Range(50, 90)]
    private float m_FieldOfView = 60;

    [SerializeField]
    [NotNull("A Input Bindings must be assigned to be included in the compiled game")]
    private InputBindings m_InputBindings;

    public bool IsDead { get; set; }

    public ActionMode CrouchStyle
    {
        get { return m_CrouchStyle; }
        set { m_CrouchStyle = value; }
    }

    public ActionMode AimStyle
    {
        get { return m_AimStyle; }
        set { m_AimStyle = value; }
    }

    public ActionMode SprintStyle
    {
        get { return m_SprintStyle; }
        set { m_SprintStyle = value; }
    }

    public ActionMode LeanStyle
    {
        get { return m_LeanStyle; }
        set { m_LeanStyle = value; }
    }

    public bool InvertHorizontalAxis
    {
        get { return m_InvertHorizontalAxis; }
        set { m_InvertHorizontalAxis = value; }
    }

    public bool InvertVerticalAxis
    {
        get { return m_InvertVerticalAxis; }
        set { m_InvertVerticalAxis = value; }
    }

    public float FieldOfView
    {
        get { return m_FieldOfView; }
        set { m_FieldOfView = Mathf.Clamp(value, 50, 80); }
    }

    public InputBindings Bindings { get { return m_InputBindings; } }
}
