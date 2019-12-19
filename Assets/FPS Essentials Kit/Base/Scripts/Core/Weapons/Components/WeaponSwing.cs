/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System;
using Essentials.Controllers;
using UnityEngine;

namespace Essentials
{
    namespace Weapons
    {
        using Input = UnityEngine.Input;

        [Serializable]
        public sealed class WeaponSwing
        {
            public enum SwingTarget
            {
                Fist,
                Weapon
            }

            [SerializeField]
            private float m_TiltAngle = 5f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_MaxTiltAngle = 10f;

            [SerializeField]
            private float m_SwingAngle = 4f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_MaxSwingAngle = 8f;

            [SerializeField]
            [Range(0, 1)]
            private float m_Speed = 0.5f;

            //[SerializeField]
            //private bool m_Tremor = false;

            //[SerializeField]
            //[Range(0, 1)]
            //private float m_TremorAmount = 0.5f;

            [SerializeField]
            private SwingTarget m_SwingTarget = SwingTarget.Weapon;

            private Vector3 m_TargetPos;
            private Quaternion m_TargetRot;

            internal void Init (Transform weaponSwing)
            {
                m_TargetPos = weaponSwing.localPosition;
                m_TargetRot = weaponSwing.localRotation;
            }

            internal void Swing (Transform weaponSwing, FirstPersonController FPController)
            {
                if (Mathf.Abs(Time.timeScale) < float.Epsilon)
                    return;

                float yRot = Mathf.Clamp(Input.GetAxis("Mouse X") * -m_SwingAngle, -m_MaxSwingAngle, m_MaxSwingAngle);
                float xRot = Mathf.Clamp(Input.GetAxis("Mouse Y") * -m_SwingAngle, -m_MaxSwingAngle, m_MaxSwingAngle);
                float zRot = FPController.Velocity.sqrMagnitude > 1 ? Mathf.Clamp(FPController.GetInput().x * -m_TiltAngle, -m_MaxTiltAngle, m_MaxTiltAngle) : 0;

                //if (FPController.IsAiming && m_Tremor)
                //{
                //    yRot += UnityEngine.Random.Range(-1.0f, 1.0f) * m_TremorAmount;
                //    xRot += UnityEngine.Random.Range(-1.0f, 1.0f) * m_TremorAmount;
                //}

                if (m_SwingTarget == SwingTarget.Fist)
                {
                    m_TargetRot = Quaternion.Euler(xRot, yRot, zRot);
                    m_TargetPos = new Vector3(-yRot / 100 + zRot / 500, xRot / 100, 0);

                    if (FPController.IsAiming)
                        m_TargetPos /= 2;
                }
                else
                {
                    m_TargetRot = Quaternion.Euler(-xRot, yRot, zRot);
                    m_TargetPos = new Vector3(zRot / 500, 0, 0);
                }

                weaponSwing.localPosition = Vector3.Lerp(weaponSwing.localPosition, m_TargetPos, Time.deltaTime * m_Speed * 10);
                weaponSwing.localRotation = Quaternion.Slerp(weaponSwing.localRotation, m_TargetRot, Time.deltaTime * m_Speed * 10);
            }
        }
    }
}

