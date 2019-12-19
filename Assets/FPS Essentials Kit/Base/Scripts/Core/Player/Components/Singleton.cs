/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance = default(T);

    public static T Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public virtual void Awake ()
    {
        if (m_Instance != this)
            m_Instance = this as T;
    }

    public virtual void OnDestroy ()
    {
        if (m_Instance == this)
            m_Instance = null;
    }
}