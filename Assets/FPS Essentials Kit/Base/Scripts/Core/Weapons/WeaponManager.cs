/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections;
using System.Collections.Generic;
using Essentials.Controllers;
using Essentials.Input;
using Essentials.Items;
using UnityEngine;

namespace Essentials
{
    namespace Weapons
    {
        [AddComponentMenu("FPS Essentials/Managers/Weapon Manager"), DisallowMultipleComponent]
        public sealed class WeaponManager : MonoBehaviour
        {
            [SerializeField]
            [NotNull]
            private FirstPersonController m_FPController;

            [SerializeField]
            [NotNull]
            private Transform m_CameraTransformReference;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_InteractionRange = 1;

            [SerializeField]
            private WeaponUI m_WeaponUI;

            [SerializeField]
            [Tag(AllowUntagged = false)]
            private string m_AmmoTag = "Ammo";

            [SerializeField]
            [Tag(AllowUntagged = false)]
            private string m_AdrenalinePackTag = "Adrenaline Pack";

            [SerializeField]
            private AudioClip m_ItemPickupSound;

            [SerializeField]
            [Range(0, 1)]
            private float m_ItemPickupVolume = 0.3f;

            [SerializeField]
            private List<Gun> m_EquippedWeaponsList = new List<Gun>();

            [SerializeField]
            private List<Gun> m_WeaponList = new List<Gun>();

            [SerializeField]
            [Tooltip("If Equipped Weapon list is empty, this weapon will be equipped by default.")]
            private Arms m_DefaultWeapon;

            [SerializeField]
            private Grenade m_FragGrenade;

            [SerializeField]
            private Adrenaline m_Adrenaline;

            private bool m_ItemCoolDown;

            private IWeapon m_CurrentWeapon;

            private string m_UseKey = string.Empty;
            private GameObject m_Target;
            private PlayerAudioSource m_PlayerBodySource;

            #region PROPERTIES

            public float Accuracy
            {
                get
                {
                    if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                        return (m_CurrentWeapon as Gun).Accuracy;
                    return -1;
                }

            }

            public int CurrentAmmo
            {
                get
                {
                    if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                        return (m_CurrentWeapon as Gun).CurrentRounds;
                    return -1;
                }

            }

            public int Magazines
            {
                get
                {
                    if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                        return (m_CurrentWeapon as Gun).Magazines;
                    return -1;
                }

            }

            public string GunName
            {
                get
                {
                    if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                        return (m_CurrentWeapon as Gun).GunName + ((m_CurrentWeapon as Gun).HasSecondaryMode ? " - " + (m_CurrentWeapon as Gun).FireMode.ToString() : string.Empty);
                    return string.Empty;
                }

            }

            public Sprite Icon
            {
                get
                {
                    if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                        return (m_CurrentWeapon as Gun).Icon;
                    return null;
                }
            }

            public bool IsShotgun
            {
                get
                {
                    if (m_CurrentWeapon != null && m_CurrentWeapon.GetType() == typeof(Gun))
                        return (m_CurrentWeapon as Gun).FireMode == GunData.FireMode.ShotgunAuto || (m_CurrentWeapon as Gun).FireMode == GunData.FireMode.ShotgunSingle;
                    return false;
                }
            }

            public bool HasFreeSlot
            {
                get
                {
                    for (int i = 0; i < m_EquippedWeaponsList.Count; i++)
                    {
                        if (m_EquippedWeaponsList[i] == null)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            #endregion

            #region EDITOR

            public void AddWeaponSlot ()
            {
                m_EquippedWeaponsList.Add(null);
            }

            public void RemoveWeaponSlot (int index)
            {
                m_EquippedWeaponsList.RemoveAt(index);
            }

            public void AddWeapon ()
            {
                m_WeaponList.Add(null);
            }

            public void RemoveWeapon (int index)
            {
                m_WeaponList.RemoveAt(index);
            }

            #endregion

            private void Start ()
            {
                // Disable all weapons
                if (m_DefaultWeapon != null)
                    m_DefaultWeapon.Viewmodel.SetActive(false);

                for (int i = 0; i < m_WeaponList.Count; i++)
                {
                    if (m_WeaponList[i] != null)
                        m_WeaponList[i].Viewmodel.SetActive(false);
                }

                // Select initial weapons
                if (m_DefaultWeapon != null)
                    if (GetWeaponIndexOnList(m_DefaultWeapon.Identifier) != -1)
                        EquipWeapon(GetWeaponIndexOnList(m_DefaultWeapon.Identifier));

                if (m_EquippedWeaponsList.Count > 0)
                {
                    for (int i = 0; i < m_EquippedWeaponsList.Count; i++)
                    {
                        if (m_EquippedWeaponsList[i] != null)
                        {
                            Select(m_EquippedWeaponsList[i]);
                            break;
                        }

                        if (i == m_EquippedWeaponsList.Count - 1)
                            if (m_DefaultWeapon != null)
                                Select(m_DefaultWeapon);
                    }
                }
                else
                {
                    if (m_DefaultWeapon != null)
                        Select(m_DefaultWeapon);
                }

                CalculateWeight();

                Button[] buttons = InputManager.GetButtonData();
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i].Name == "Use")
                    {
                        m_UseKey = buttons[i].Keys[0];
                    }
                }

                InvokeRepeating("Search", 0, 0.1f);
                m_PlayerBodySource = AudioManager.Instance.RegisterSource("PlayerBodySource", AudioManager.Instance.transform, spatialBlend: 0);
            }

            private void Update ()
            {
                // Analyze the character's target
                SearchForWeapons();
                SearchForAmmo();
                SearchForAdrenaline();
                SearchInteractiveObjects();

                if (!m_FPController.Controllable)
                    return;

                // Switch equipped weapons
                if (m_CurrentWeapon != null)
                {
                    if (m_CurrentWeapon.CanSwitch)
                    {
                        for (int i = 0; i < m_EquippedWeaponsList.Count; i++)
                        {
                            if (m_EquippedWeaponsList[i] != null)
                            {
                                if (UnityEngine.Input.GetKeyDown(GetWeaponKeyCode(i)) && m_CurrentWeapon.Identifier != m_EquippedWeaponsList[i].Identifier)
                                {
                                    StartCoroutine(Switch(m_CurrentWeapon, m_EquippedWeaponsList[i]));
                                }
                            }
                        }
                    }
                }

                // Throw grenade
                if (!m_ItemCoolDown)
                {
                    if (InputManager.GetButtonDown("Grenade") && m_CurrentWeapon != null && m_CurrentWeapon.CanUseItems && m_FragGrenade != null && m_FragGrenade.Amount > 0)
                    {
                        StartCoroutine(ThrowGrenade());
                    }
                }

                // Use adrenaline
                if (!m_ItemCoolDown)
                {
                    if (InputManager.GetButtonDown("Adrenaline") && m_CurrentWeapon != null && m_CurrentWeapon.CanUseItems && m_Adrenaline != null && m_Adrenaline.Amount > 0)
                    {
                        StartCoroutine(AdrenalineShot());
                    }
                }
            }

            private IEnumerator ThrowGrenade ()
            {
                m_ItemCoolDown = true;

                m_CurrentWeapon.Deselect();
                yield return new WaitForSeconds(m_CurrentWeapon.HideAnimationLength);
                m_CurrentWeapon.Viewmodel.SetActive(false);

                m_FragGrenade.gameObject.SetActive(true);
                m_FragGrenade.Use();

                yield return new WaitForSeconds(m_FragGrenade.PullAndThrowLenght);
                m_FragGrenade.gameObject.SetActive(false);

                m_CurrentWeapon.Viewmodel.SetActive(true);
                m_CurrentWeapon.Select();
                m_ItemCoolDown = false;
            }

            private IEnumerator AdrenalineShot ()
            {
                m_ItemCoolDown = true;

                m_CurrentWeapon.Deselect();
                yield return new WaitForSeconds(m_CurrentWeapon.HideAnimationLength);
                m_CurrentWeapon.Viewmodel.SetActive(false);

                m_Adrenaline.gameObject.SetActive(true);
                m_Adrenaline.Use();

                yield return new WaitForSeconds(m_Adrenaline.ShotLenght);
                m_Adrenaline.gameObject.SetActive(false);

                m_CurrentWeapon.Viewmodel.SetActive(true);
                m_CurrentWeapon.Select();
                m_ItemCoolDown = false;
            }

            private void SearchForWeapons ()
            {
                if (m_Target != null)
                {
                    GunPickup target = m_Target.GetComponent<GunPickup>();

                    if (target != null)
                    {
                        IWeapon weapon = GetWeaponByID(target.ID);

                        if (m_CurrentWeapon != null)
                        {
                            if (weapon != null && m_CurrentWeapon.CanSwitch)
                            {
                                if (!IsEquipped(weapon))
                                {
                                    if (HasFreeSlot)
                                    {
                                        if (m_WeaponUI != null)
                                            m_WeaponUI.ShowPickupMessage("HOLD <color=#FF9200FF>" + m_UseKey + "</color> TO PICK UP \n <color=#FF9200FF>" + (weapon as Gun).InspectorName + "</color>");

                                        if (InputManager.HoldButton("Use", 0.25f))
                                        {
                                            EquipWeapon(GetWeaponIndexOnList(weapon.Identifier));
                                            Destroy(target.transform.gameObject);
                                            StartCoroutine(Switch(m_CurrentWeapon, weapon));
                                            m_PlayerBodySource.ForcePlay(m_ItemPickupSound, m_ItemPickupVolume);
                                            CalculateWeight();
                                        }
                                    }
                                    else
                                    {
                                        if (m_WeaponUI != null)
                                            m_WeaponUI.ShowPickupMessage("HOLD <color=#FF9200FF>" + m_UseKey + "</color> TO SWAP \n <color=#FF9200FF>" + (m_CurrentWeapon as Gun).InspectorName +
                                                "</color> FOR THE <color=#FF9200FF>" + (weapon as Gun).InspectorName + "</color>");

                                        if (InputManager.HoldButton("Use", 0.25f))
                                        {
                                            UnequipWeapon(GetEquippedWeaponIndexOnList(m_CurrentWeapon.Identifier));
                                            EquipWeapon(GetWeaponIndexOnList(weapon.Identifier));
                                            StartCoroutine(DropAndSwitch(m_CurrentWeapon, weapon, target));
                                            CalculateWeight();
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_WeaponUI != null)
                                        m_WeaponUI.ShowPickupMessage("<color=#FF9200FF>" + (weapon as Gun).InspectorName + "</color> ALREADY EQUIPPED");
                                }
                            }
                        }
                        else
                        {
                            if (HasFreeSlot)
                            {
                                if (m_WeaponUI != null)
                                    m_WeaponUI.ShowPickupMessage("HOLD <color=#FF9200FF>" + m_UseKey + "</color> TO PICK UP \n<color=#FF9200FF>" + (weapon as Gun).InspectorName + "</color>");
                                if (InputManager.HoldButton("Use", 0.25f))
                                {
                                    EquipWeapon(GetWeaponIndexOnList(weapon.Identifier));
                                    Destroy(target.transform.gameObject);
                                    Select(weapon);
                                    CalculateWeight();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_WeaponUI != null)
                            m_WeaponUI.HidePickupMessage();
                    }
                }
                else
                {
                    if (m_WeaponUI != null)
                        m_WeaponUI.HidePickupMessage();
                }
            }

            private void SearchForAmmo ()
            {
                if (!m_ItemCoolDown && m_EquippedWeaponsList.Count > 0 && m_CurrentWeapon != null && m_CurrentWeapon.CanUseItems)
                {
                    if (m_Target != null)
                    {
                        if (m_Target.tag == m_AmmoTag)
                        {
                            if (m_WeaponUI != null)
                                m_WeaponUI.ShowPickupMessage("PRESS <color=#FF9200FF>" + m_UseKey + "</color> TO PICK UP AMMO");

                            if (InputManager.GetButtonDown("Use"))
                            {
                                StartCoroutine(RefillAmmo());
                            }
                        }
                    }
                }
            }

            private void SearchForAdrenaline ()
            {
                if (!m_ItemCoolDown && m_EquippedWeaponsList.Count > 0 && m_CurrentWeapon != null && m_CurrentWeapon.CanUseItems)
                {
                    if (m_Target != null)
                    {
                        if (m_Target.tag == m_AdrenalinePackTag)
                        {
                            if (m_WeaponUI != null)
                                m_WeaponUI.ShowPickupMessage("PRESS <color=#FF9200FF>" + m_UseKey + "</color> TO PICK UP ADRENALINE SHOTS");

                            if (InputManager.GetButtonDown("Use"))
                            {
                                StartCoroutine(RefillItem(new IUsable[] { m_Adrenaline }));
                            }
                        }
                    }
                }
            }

            private void SearchInteractiveObjects ()
            {
                if (!m_ItemCoolDown)
                {
                    if (m_Target != null)
                    {
                        IActionable target = m_Target.GetComponent<IActionable>();

                        if (target != null)
                        {
                            if (m_WeaponUI != null)
                                m_WeaponUI.ShowPickupMessage("PRESS <color=#FF9200FF>" + m_UseKey + "</color> TO " + target.Message());

                            if (InputManager.GetButtonDown("Use"))
                            {
                                StartCoroutine(Interact(target));
                            }
                        }
                    }
                }
            }

            private IEnumerator RefillAmmo ()
            {
                m_ItemCoolDown = true;

                m_CurrentWeapon.Deselect();
                yield return new WaitForSeconds(m_CurrentWeapon.HideAnimationLength);
                m_CurrentWeapon.Viewmodel.SetActive(false);

                for (int i = 0; i < m_EquippedWeaponsList.Count; i++)
                {
                    if (m_EquippedWeaponsList[i] != null)
                        m_EquippedWeaponsList[i].Refill();
                }

                if (m_FragGrenade != null)
                    m_FragGrenade.Refill();

                yield return new WaitForSeconds(1f);

                m_CurrentWeapon.Viewmodel.SetActive(true);
                m_CurrentWeapon.Select();
                m_ItemCoolDown = false;
            }

            private IEnumerator RefillItem (IUsable[] items)
            {
                m_ItemCoolDown = true;

                m_CurrentWeapon.Interact();
                yield return new WaitForSeconds(m_CurrentWeapon.InteractDelay);

                for (int i = 0; i < items.Length; i++)
                {
                    items[i].Refill();
                }

                m_PlayerBodySource.ForcePlay(m_ItemPickupSound, m_ItemPickupVolume);

                yield return new WaitForSeconds(Mathf.Max(m_CurrentWeapon.InteractAnimationLength - m_CurrentWeapon.InteractDelay, 0));
                m_ItemCoolDown = false;
            }

            private IEnumerator Interact (IActionable target)
            {
                m_ItemCoolDown = true;

                m_CurrentWeapon.Interact();
                yield return new WaitForSeconds(m_CurrentWeapon.InteractDelay);

                target.Interact();

                yield return new WaitForSeconds(Mathf.Max(m_CurrentWeapon.InteractAnimationLength - m_CurrentWeapon.InteractDelay, 0));
                m_ItemCoolDown = false;
            }


            private void Search ()
            {
                Vector3 direction = m_CameraTransformReference.TransformDirection(Vector3.forward);
                Vector3 origin = m_CameraTransformReference.transform.position;

                Ray ray = new Ray(origin, direction);
                RaycastHit hitInfo;

                m_Target = Physics.Raycast(ray, out hitInfo, m_InteractionRange, Physics.AllLayers, QueryTriggerInteraction.Collide) ? hitInfo.collider.gameObject : null;
            }

            private IEnumerator Switch (IWeapon current, IWeapon target)
            {
                current.Deselect();
                yield return new WaitForSeconds(current.HideAnimationLength);
                current.Viewmodel.SetActive(false);
                Select(target);
            }

            private IEnumerator DropAndSwitch (IWeapon current, IWeapon target, GunPickup drop)
            {
                current.Deselect();
                yield return new WaitForSeconds((current as Gun).HideAnimationLength);

                if ((current as Gun).DroppablePrefab != null)
                    Instantiate((current as Gun).DroppablePrefab, drop.transform.position, drop.transform.rotation);
                Destroy(drop.transform.gameObject);

                current.Viewmodel.SetActive(false);
                Select(target);
            }

            private void Select (IWeapon weapon)
            {
                m_CurrentWeapon = weapon;
                weapon.Viewmodel.SetActive(true);
                weapon.Select();
            }

            private void CalculateWeight ()
            {
                float weight = 0;
                for (int i = 0; i < m_EquippedWeaponsList.Count; i++)
                {
                    if (m_EquippedWeaponsList[i] != null && m_EquippedWeaponsList[i].GetType() == typeof(Gun))
                        weight += m_EquippedWeaponsList[i].Weight;
                }
                m_FPController.Weight = Mathf.RoundToInt(weight);
            }

            public void EquipWeapon (int index)
            {
                if (HasFreeSlot)
                {
                    for (int i = 0; i < m_EquippedWeaponsList.Count; i++)
                    {
                        if (m_EquippedWeaponsList[i] == null)
                        {
                            m_EquippedWeaponsList[i] = m_WeaponList[index];
                            return;
                        }
                    }
                }
            }

            public void UnequipWeapon (int index)
            {
                m_EquippedWeaponsList[index] = null;
            }

            public bool IsEquipped (IWeapon weapon)
            {
                if (m_DefaultWeapon != null)
                {
                    if (weapon.Identifier == m_DefaultWeapon.Identifier)
                        return true;
                }

                for (int i = 0; i < m_EquippedWeaponsList.Count; i++)
                {
                    if (m_EquippedWeaponsList[i] != null)
                    {
                        if (m_EquippedWeaponsList[i].Identifier == weapon.Identifier)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public int GetEquippedWeaponIndexOnList (int id)
            {
                for (int i = 0; i < m_EquippedWeaponsList.Count; i++)
                {
                    if (m_EquippedWeaponsList[i] != null)
                    {
                        if (m_EquippedWeaponsList[i].Identifier == id)
                        {
                            return i;
                        }
                    }
                }
                Debug.LogError("The weapon is not on the equipped weapon list");
                return -1;
            }

            public int GetWeaponIndexOnList (int id)
            {
                for (int i = 0; i < m_WeaponList.Count; i++)
                {
                    if (m_WeaponList[i] != null)
                    {
                        if (m_WeaponList[i].Identifier == id)
                        {
                            return i;
                        }
                    }
                }
                return -1;
            }

            public IWeapon GetWeaponByID (int id)
            {
                for (int i = 0; i < m_WeaponList.Count; i++)
                {
                    if (m_WeaponList[i] != null)
                    {
                        if (m_WeaponList[i].Identifier == id)
                        {
                            return m_WeaponList[i];
                        }
                    }
                }
                Debug.LogError("The weapon is not on the weapon list");
                return null;
            }

            private KeyCode GetWeaponKeyCode (int index)
            {
                switch (index)
                {
                    case 0:
                        return KeyCode.Alpha1;
                    case 1:
                        return KeyCode.Alpha2;
                    case 2:
                        return KeyCode.Alpha3;
                    case 3:
                        return KeyCode.Alpha4;
                    case 4:
                        return KeyCode.Alpha5;
                    case 5:
                        return KeyCode.Alpha6;
                    case 6:
                        return KeyCode.Alpha7;
                    case 7:
                        return KeyCode.Alpha8;
                    case 8:
                        return KeyCode.Alpha9;
                    case 9:
                        return KeyCode.Alpha0;
                }
                return KeyCode.None;
            }
        }
    }
}

