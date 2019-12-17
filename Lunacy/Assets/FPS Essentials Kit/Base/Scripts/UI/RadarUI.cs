/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarUI : MonoBehaviour
{
    public struct RadarObject
    {
        public Image Icon { get; private set; }
        public GameObject Owner { get; private set; }

        public RadarObject (Image icon, GameObject owner)
        {
            Icon = icon;
            Owner = owner;
        }
    }

    [SerializeField]
    private Transform m_Player;

    [SerializeField]
    private float m_Scale = 2;

    [SerializeField]
    private RectTransform m_Background;

    [SerializeField]
    private RectTransform m_Compass;

    private static List<RadarObject> m_RadarObjectList = new List<RadarObject>();

    private void Update ()
    {
        if (!GameplayManager.Instance.IsDead)
        {
            m_Compass.localRotation = Quaternion.Euler(new Vector3(0, 0, m_Player.eulerAngles.y));
            DrawRadarElements();
        }   
    }

    public static void RegisterRadarObject (GameObject obj, Image img)
    {
        Image image = Instantiate(img);
        m_RadarObjectList.Add(new RadarObject(image, obj));
    }

    public static void RemoveRadarObject (GameObject o)
    {
        List<RadarObject> newList = new List<RadarObject>();
        for (int i = 0; i < m_RadarObjectList.Count; i++)
        {
            if (m_RadarObjectList[i].Owner == o)
            {
                Destroy(m_RadarObjectList[i].Icon);
                continue;
            }
            else
                newList.Add(m_RadarObjectList[i]);
        }

        m_RadarObjectList.RemoveRange(0, m_RadarObjectList.Count);
        m_RadarObjectList.AddRange(newList);
    }

    private void DrawRadarElements ()
    {
        foreach (RadarObject ro in m_RadarObjectList)
        {
            Vector3 radarPos = (ro.Owner.transform.position - m_Player.position);
            float dstToObject = Vector3.Distance(m_Player.position, ro.Owner.transform.position) * m_Scale;
            float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - m_Player.eulerAngles.y;

            radarPos.x = dstToObject * Mathf.Cos(deltay * Mathf.Deg2Rad) * -1;
            radarPos.z = dstToObject * Mathf.Sin(deltay * Mathf.Deg2Rad);

            if (ro.Icon.transform.parent != m_Background.transform)
                ro.Icon.transform.SetParent(m_Background.transform);

            ro.Icon.transform.localPosition = new Vector3(radarPos.x, radarPos.z, 0);
            ro.Icon.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
