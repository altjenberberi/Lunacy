/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections.Generic;
using Essentials.Controllers;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField]
    private bool m_ShowIndicators = true;

    [SerializeField]
    private Indicator m_IndicatorPrefab;

    [SerializeField]
    private int m_Duration = 3;

    [SerializeField]
    private float m_Radius = 128;

    [SerializeField]
    private int m_MaxIndicators = 5;

    [SerializeField]
    private Transform m_FPController;

    [SerializeField]
    private HealthController m_HealthController;

    private List<Indicator> m_IndicatorList = new List<Indicator>();

    private void Start ()
    {
        m_HealthController.DamageEvent += AddIndicator;
    }

    private void Update ()
    {
        if (m_ShowIndicators)
        {
            foreach (Indicator i in m_IndicatorList)
            {
                float angle = i.GetAngleRelativeToTranform(m_FPController);
                i.Alpha -= Time.deltaTime;

                if (i.Alpha <= 0)
                {
                    m_IndicatorList.Remove(i);
                    Destroy(i.gameObject);
                    return;
                }

                Image img = i.GetComponent<Image>();
                img.color = new Color(img.color.r, img.color.g, img.color.b, i.Alpha);

                RectTransform rt = i.GetComponent<RectTransform>();
                rt.localPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * m_Radius, Mathf.Sin(angle * Mathf.Deg2Rad) * m_Radius, 0);
                rt.localRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            }
        }
    }

    private void AddIndicator (Vector3 position)
    {
        if (m_IndicatorList.Count <= m_MaxIndicators)
        {
            Indicator indicator = Instantiate(m_IndicatorPrefab, transform);
            RectTransform rect = indicator.GetComponent<RectTransform>();

            rect.localScale = Vector3.one;
            indicator.TargetPosition = position;
            indicator.Alpha = m_Duration;

            m_IndicatorList.Add(indicator);
        }
    }
}
