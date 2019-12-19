/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;
using UnityEngine.UI;

public class RadarTarget : MonoBehaviour
{
    [SerializeField]
    private Image m_Image;

    private void Start ()
    {
        RadarUI.RegisterRadarObject(gameObject, m_Image);
    }

    private void OnDestroy ()
    {
        RadarUI.RemoveRadarObject(gameObject);
    }
}
