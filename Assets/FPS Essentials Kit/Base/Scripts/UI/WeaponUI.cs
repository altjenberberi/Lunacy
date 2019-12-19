/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using Essentials.Controllers;
using Essentials.Items;
using Essentials.Weapons;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [Header("Weapon")]

    [SerializeField]
    private Text m_WeaponName;

    [SerializeField]
    private Text m_CurrentAmmo;

    [SerializeField]
    private Text m_Magazines;

    [SerializeField]
    private Image m_WeaponIcon;

    [SerializeField]
    private Text m_Pickup;

    [Header("Crosshair")]

    [SerializeField]
    private RectTransform m_Up;

    [SerializeField]
    private RectTransform m_Down;

    [SerializeField]
    private RectTransform m_Right;

    [SerializeField]
    private RectTransform m_Left;

    [Header("Crosshair Shotgun")]

    [SerializeField]
    private RectTransform m_ShotgunUp;

    [SerializeField]
    private RectTransform m_ShotgunDown;

    [SerializeField]
    private RectTransform m_ShotgunRight;

    [SerializeField]
    private RectTransform m_ShotgunLeft;

    [SerializeField]
    private FirstPersonController m_FPController;

    [SerializeField]
    private WeaponManager m_WeaponManager;

    [Header("Health")]

    [SerializeField]
    private Image m_Health;

    [SerializeField]
    private Image m_Stamina;

    [SerializeField]
    private HealthController m_HealthController;

    [Header("Grenades")]

    [SerializeField]
    private GameObject m_GrenadeUI;

    [SerializeField]
    private Text m_GrenadesAmount;

    [SerializeField]
    private Grenade m_Grenade;

    [Header("Adrenaline")]

    [SerializeField]
    private GameObject m_AdrenalineUI;

    [SerializeField]
    private Text m_AdrenalineAmount;

    [SerializeField]
    private Adrenaline m_Adrenaline;

    private float m_Offset = 128;

    private void Update ()
    {
        m_Stamina.fillAmount = m_FPController.StaminaPercent;
        m_Health.fillAmount = m_HealthController.HealthPercent;
        m_WeaponName.text = m_WeaponManager.GunName;
        m_CurrentAmmo.text = m_WeaponManager.CurrentAmmo != -1 ? m_WeaponManager.CurrentAmmo.ToString() : "---";
        m_Magazines.text = m_WeaponManager.Magazines != -1 ? m_WeaponManager.Magazines.ToString() : "---";

        if (m_WeaponManager.Icon != null)
        {
            m_WeaponIcon.enabled = true;
            m_WeaponIcon.sprite = m_WeaponManager.Icon;
        }
        else
            m_WeaponIcon.enabled = false;

        m_Up.gameObject.SetActive(!m_FPController.IsAiming && m_WeaponManager.Accuracy != -1 && !m_WeaponManager.IsShotgun);
        m_Down.gameObject.SetActive(!m_FPController.IsAiming && m_WeaponManager.Accuracy != -1 && !m_WeaponManager.IsShotgun);
        m_Right.gameObject.SetActive(!m_FPController.IsAiming && m_WeaponManager.Accuracy != -1 && !m_WeaponManager.IsShotgun);
        m_Left.gameObject.SetActive(!m_FPController.IsAiming && m_WeaponManager.Accuracy != -1 && !m_WeaponManager.IsShotgun);

        m_ShotgunUp.gameObject.SetActive(!m_FPController.IsAiming && m_WeaponManager.Accuracy != -1 && m_WeaponManager.IsShotgun);
        m_ShotgunDown.gameObject.SetActive(!m_FPController.IsAiming && m_WeaponManager.Accuracy != -1 && m_WeaponManager.IsShotgun);
        m_ShotgunRight.gameObject.SetActive(!m_FPController.IsAiming && m_WeaponManager.Accuracy != -1 && m_WeaponManager.IsShotgun);
        m_ShotgunLeft.gameObject.SetActive(!m_FPController.IsAiming && m_WeaponManager.Accuracy != -1 && m_WeaponManager.IsShotgun);

        if (m_WeaponManager.Accuracy != -1)
        {
            if(m_WeaponManager.IsShotgun)
            {
                m_ShotgunUp.localPosition = new Vector3(0, m_Offset * (1 - m_WeaponManager.Accuracy));
                m_ShotgunDown.localPosition = new Vector3(0, -m_Offset * (1 - m_WeaponManager.Accuracy));
                m_ShotgunRight.localPosition = new Vector3(m_Offset * (1 - m_WeaponManager.Accuracy), 0);
                m_ShotgunLeft.localPosition = new Vector3(-m_Offset * (1 - m_WeaponManager.Accuracy), 0);
            }
            else
            {
                m_Up.localPosition = new Vector3(0, m_Offset * (1 - m_WeaponManager.Accuracy));
                m_Down.localPosition = new Vector3(0, -m_Offset * (1 - m_WeaponManager.Accuracy));
                m_Right.localPosition = new Vector3(m_Offset * (1 - m_WeaponManager.Accuracy), 0);
                m_Left.localPosition = new Vector3(-m_Offset * (1 - m_WeaponManager.Accuracy), 0);
            }
        }

        if (m_Grenade.Amount > 0)
        {
            m_GrenadeUI.SetActive(true);
            m_GrenadesAmount.text = m_Grenade.Amount.ToString();
        }
        else
        {
            m_GrenadeUI.SetActive(false);
        }

        if (m_Adrenaline.Amount > 0)
        {
            m_AdrenalineUI.SetActive(true);
            m_AdrenalineAmount.text = m_Adrenaline.Amount.ToString();
        }
        else
        {
            m_AdrenalineUI.SetActive(false);
        }
    }

    public void ShowPickupMessage (string message)
    {
        m_Pickup.gameObject.SetActive(true);
        m_Pickup.text = message;
    }

    public void HidePickupMessage ()
    {
        m_Pickup.gameObject.SetActive(false);
    }
}
