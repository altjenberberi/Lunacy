/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    namespace Weapons
    {
        [AddComponentMenu("FPS Essentials/Weapon/Gun Pickup"), DisallowMultipleComponent]
        public class GunPickup : MonoBehaviour
        {
            [SerializeField]
            [NotNull]
            protected GunData m_GunData;

            public int ID { get { return m_GunData != null ? m_GunData.GetInstanceID() : -1; } }
        }
    }
}

