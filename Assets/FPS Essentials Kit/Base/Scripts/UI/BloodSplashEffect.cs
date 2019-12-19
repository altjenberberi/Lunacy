/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

[AddComponentMenu("FPS Essentials/Effects/Blood Splash Effect"), DisallowMultipleComponent]
public class BloodSplashEffect : MonoBehaviour
{
    [SerializeField]
    private Texture2D m_BloodTexture = null;

    [SerializeField]
    private Texture2D m_BloodNormalMap = null;

    [SerializeField]
    [Range(0, 1)]
    private float m_BloodAmount = 0.0f;

    [SerializeField]
    [MinMax(0, Mathf.Infinity)]
    private float m_Distortion = 1.0f;

    private Material m_Material = null;

    //Properties
    public float BloodAmount { get { return m_BloodAmount; } set { m_BloodAmount = value; } }

    private void OnRenderImage (RenderTexture src, RenderTexture dest)
    {
        if (m_Material == null)
            m_Material = new Material(Shader.Find("Hidden/BloodSplashEffect"));

        if (m_Material == null)
            return;

        //Send data into Shader
        if (m_BloodTexture != null)
            m_Material.SetTexture("_BloodTex", m_BloodTexture);

        if (m_BloodNormalMap != null)
            m_Material.SetTexture("_BloodBump", m_BloodNormalMap);

        m_Material.SetFloat("_Distortion", m_Distortion);
        m_Material.SetFloat("_BloodAmount", m_BloodAmount);

        Graphics.Blit(src, dest, m_Material);
    }
}
