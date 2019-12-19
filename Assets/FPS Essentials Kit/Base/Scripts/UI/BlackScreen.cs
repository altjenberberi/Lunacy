/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
    [SerializeField]
    private bool m_ShowAtStart;

    [SerializeField]
    private float m_StartDelay;

    [SerializeField]
    private Image m_Blackscreen;

    public bool Show { get; set; }

    // Use this for initialization
    private void Start ()
    {      
        if (m_ShowAtStart)
        {
            Show = true;
            m_Blackscreen.color = new Color(0, 0, 0, 1);
            Invoke("FadeBlackscreen", m_StartDelay);
        }
        else
        {
            m_Blackscreen.color = new Color(0, 0, 0, 0);
        }
    }

    // Update is called once per frame
    private void Update ()
    {
        m_Blackscreen.color = new Color(0, 0, 0, Mathf.MoveTowards(m_Blackscreen.color.a, Show ? 1 : 0, Time.deltaTime));
    }

    private void FadeBlackscreen ()
    {
        Show = false;
    }
}
