/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System;
using UnityEngine;

namespace Essentials
{
    namespace Input
    {
        [Serializable]
        public sealed class Axis
        {
            [SerializeField]
            private string m_Name;

            [SerializeField]
            private string m_PositiveKey;

            [SerializeField]
            private string m_NegativeKey;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_Sensitivity; // Speed in units per second that the axis will move toward the target value.This is for digital devices only

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_Gravity; // Speed in units per second that the axis falls toward neutral when no buttons are pressed

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_DeadZone; // If the axis has a value less than DeadZone will be returned 0

            #region PROPERTIES

            public string Name { get { return m_Name; } }
            public string PositiveKey { get { return m_PositiveKey; } }
            public string NegativeKey { get { return m_NegativeKey; } }
            public float Sensitivity { get { return m_Sensitivity; } }
            public float Gravity { get { return m_Gravity; } }
            public float DeadZone { get { return m_DeadZone; } }
            public float CurrentValue { get; set; }

            #endregion

            public Axis (string name, string positiveKey, string negativeKey, float sensitivity, float gravity, float deadZone)
            {
                m_Name = name;
                m_PositiveKey = positiveKey;
                m_NegativeKey = negativeKey;
                m_Sensitivity = sensitivity;
                m_Gravity = gravity;
                m_DeadZone = deadZone;
                CurrentValue = 0;
            }

            public void EditAxis (string positiveKey, string negativeKey)
            {
                m_PositiveKey = positiveKey;
                m_NegativeKey = negativeKey;
            }
        }
    }
}
