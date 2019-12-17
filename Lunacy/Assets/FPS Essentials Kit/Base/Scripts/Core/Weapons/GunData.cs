/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    namespace Weapons
    {
        [CreateAssetMenu(menuName = "Gun Data", fileName = "Gun Data", order = 101)]
        public sealed class GunData : ScriptableObject
        {
            public enum FireMode
            {
                None,
                FullAuto,
                Single,
                Burst,
                ShotgunSingle,
                ShotgunAuto
            }

            public enum ReloadMode
            {
                Magazines,
                BulletByBullet
            }

            public enum DamageMode
            {
                DecreaseByDistance,
                Constant
            }

            #region GENERAL

            [SerializeField]
            [Tooltip("The name of the gun")]
            private string m_GunName = "Gun";

            [SerializeField]
            [NotNull]
            private Sprite m_Icon;

            [SerializeField]
            [NotNull]
            private GameObject m_DroppablePrefab;

            [SerializeField]
            [Range(0, 5)]
            private float m_Weight = 1;

            [SerializeField]
            [Range(0, 5)]
            private float m_Size = 1.5f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_MeleeDamage = 50;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_MeleeForce = 5;

            #endregion

            #region SHOOTING

            [SerializeField]
            private FireMode m_PrimaryFireMode = FireMode.FullAuto;

            [SerializeField]
            private FireMode m_SecondaryFireMode = FireMode.None;

            [SerializeField]
            [Tooltip("Rate of fire is the frequency at which a specific weapon can fire or launch its projectiles. It is usually measured in rounds per minute (RPM or round/min).")]
            [Range(1, 1000)]
            private int m_PrimaryRateOfFire = 600;

            [SerializeField]
            [Tooltip("Rate of fire is the frequency at which a specific weapon can fire or launch its projectiles. It is usually measured in rounds per minute (RPM or round/min).")]
            [Range(1, 1000)]
            private float m_SecondaryRateOfFire = 600;

            [SerializeField]
            [Tooltip("The force that will be apply if hits a Rigidbody")]
            [MinMax(0, Mathf.Infinity)]
            private float m_Force = 5;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_Range = 50;

            [SerializeField]
            [Tooltip("How many bullets does the shotgun will fire at once")]
            [MinMax(1, Mathf.Infinity)]
            private int m_BulletsPerShoot = 5;

            [SerializeField]
            [Tooltip("How many bullets will be fired sequentially with a single pull of the trigger")]
            [MinMax(1, Mathf.Infinity)]
            private int m_BulletsPerBurst = 3;

            [SerializeField]
            private LayerMask m_AffectedLayers = 1;

            [SerializeField]
            private DamageMode m_DamageMode = DamageMode.Constant;

            [SerializeField]
            private float m_MinDamage = 15;

            [SerializeField]
            private float m_MaxDamage = 30;

            [SerializeField]
            private AnimationCurve m_DamageFalloffCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.4f, 1), new Keyframe(0.6f, 0.5f), new Keyframe(1, 0.5f));

            [SerializeField]
            private bool m_PenetrateObjects = true;

            [SerializeField]
            [Tooltip("The maximum distance that a bullet can travel penetrating an object")]
            [Range(0, 10)]
            private float m_PenetrationPower = 1;

            #endregion

            #region MAGAZINE

            [SerializeField]
            private ReloadMode m_ReloadMode = ReloadMode.Magazines;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private int m_RoundsPerMagazine = 30;

            [SerializeField]
            [Tooltip("Enabling the chamber will add an additional bullet to your weapon.")]
            private bool m_HasChamber = false;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private int m_InitialMagazines = 3;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private int m_MaxMagazines = 6;

            #endregion

            #region ACCURACY

            [SerializeField]
            [Range(0.01f, 1)]
            private float m_HIPAccuracy = 0.6f;

            [SerializeField]
            [Range(0.01f, 1)]
            private float m_AIMAccuracy = 0.9f;

            [SerializeField]
            [Range(0.01f, 1)]
            private float m_MinimumAccuracy = 0.1f;

            [SerializeField]
            [Range(0, 3)]
            private float m_DecreaseRateByWalking = 1;

            [SerializeField]
            [Range(0, 3)]
            private float m_DecreaseRateByShooting = 1;

            #endregion

            #region GUN PROPERTIES

            public string GunName { get { return m_GunName; } }
            public Sprite Icon { get { return m_Icon; } }
            public GameObject DroppablePrefab { get { return m_DroppablePrefab; } }
            public float Weight { get { return m_Weight; } }
            public float Size { get { return m_Size; } }

            public float MeleeForce { get { return m_MeleeForce; } }
            public float MeleeDamage { get { return m_MeleeDamage; } }

            public FireMode PrimaryFireMode { get { return m_PrimaryFireMode; } }
            public FireMode SecondaryFireMode { get { return m_SecondaryFireMode; } }
            public float PrimaryRateOfFire { get { return 60.0f / m_PrimaryRateOfFire; } }
            public float SecondaryRateOfFire { get { return 60.0f / m_SecondaryRateOfFire; } }
            public float Force { get { return m_Force; } }
            public float Range { get { return m_Range; } }
            public int BulletsPerShoot { get { return m_BulletsPerShoot; } }
            public int BulletsPerBurst { get { return m_BulletsPerBurst; } }

            public LayerMask AffectedLayers { get { return m_AffectedLayers; } }

            public DamageMode DamageType { get { return m_DamageMode; } }
            public float Damage { get { return Random.Range(m_MinDamage, m_MaxDamage); } }
            public AnimationCurve DamageFalloffCurve { get { return m_DamageFalloffCurve; } }

            public bool PenetrateObjects { get { return m_PenetrateObjects; } }
            public float PenetrationPower { get { return m_PenetrationPower; } }

            public ReloadMode ReloadType { get { return m_ReloadMode; } }
            public int RoundsPerMagazine { get { return m_RoundsPerMagazine; } }
            public bool HasChamber { get { return m_HasChamber; } }
            public int InitialMagazines { get { return m_InitialMagazines; } }
            public int MaxMagazines { get { return m_MaxMagazines; } }

            public float HIPAccuracy { get { return m_HIPAccuracy; } }
            public float AIMAccuracy { get { return m_AIMAccuracy; } }
            public float MinimumAccuracy { get { return m_MinimumAccuracy; } }
            public float DecreaseRateByWalking { get { return m_DecreaseRateByWalking; } }
            public float DecreaseRateByShooting { get { return m_DecreaseRateByShooting; } }

            #endregion
        }
    }
}

