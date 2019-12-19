/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System;
using UnityEngine;

namespace Essentials
{
    namespace Controllers
    {
        using Input = UnityEngine.Input;

        [Serializable]
        public sealed class MouseLook
        {
            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_HorizontalSensitivity = 5f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_VerticalSensitivity = 5f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_AimingHorizontalSensitivity = 1f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_AimingVerticalSensitivity = 1f;

            [SerializeField]
            private bool m_ClampVerticalRotation = true;

            [SerializeField]
            private float m_MinimumX = -90f; // Minimum vertical angle

            [SerializeField]
            private float m_MaximumX = 90f; // Maximum vertical angle

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_Smoothness = 1;

            public bool Controllable { get; set; }

            private Quaternion m_CharacterTargetRot;
            private Quaternion m_CameraTargetRot;

            public void Init (Transform character, Transform camera)
            {
                m_CharacterTargetRot = character.localRotation;
                m_CameraTargetRot = camera.localRotation;
            }

            public void LookAt (Transform character, Vector3 position)
            {
                Vector3 characterDirection = position - character.position;
                characterDirection.y = 0;

                // Forces the character to look at a target
                m_CharacterTargetRot = Quaternion.Slerp(m_CharacterTargetRot, Quaternion.LookRotation(characterDirection), 10 * Time.deltaTime);
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot, 10 * Time.deltaTime);
            }

            public void UpdateRotation (Transform character, Transform camera, bool isAiming)
            {
                if (!Controllable)
                    return;

                // Avoids the mouse looking if the game is effectively paused
                if (Mathf.Abs(Time.timeScale) < float.Epsilon)
                    return;

                float yRot = (GameplayManager.Instance.InvertHorizontalAxis ? -Input.GetAxis("Mouse X") : Input.GetAxis("Mouse X")) * (isAiming ? m_AimingHorizontalSensitivity : m_HorizontalSensitivity);
                float xRot = (GameplayManager.Instance.InvertVerticalAxis ? -Input.GetAxis("Mouse Y") : Input.GetAxis("Mouse Y")) * (isAiming ? m_AimingVerticalSensitivity : m_VerticalSensitivity);

                m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
                m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

                if (m_ClampVerticalRotation)
                    m_CameraTargetRot = Math3DUtility.ClampRotationAroundXAxis(m_CameraTargetRot, -m_MaximumX, -m_MinimumX);

                if (m_Smoothness > 0)
                {
                    character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot, 10 / m_Smoothness * Time.deltaTime);
                    camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot, 10 / m_Smoothness * Time.deltaTime);
                }
                else
                {
                    character.localRotation = m_CharacterTargetRot;
                    camera.localRotation = m_CameraTargetRot;
                }
            }
        }
    }
}
