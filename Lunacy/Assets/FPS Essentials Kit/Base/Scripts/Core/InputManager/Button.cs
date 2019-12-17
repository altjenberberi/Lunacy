/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Essentials
{
    namespace Input
    {
        [Serializable]
        public sealed class Button
        {
            [SerializeField]
            private string m_Name;

            [SerializeField]
            private List<string> m_Keys; // List with all the keys linked to this button

            [SerializeField]
            private float m_LastUseTime; // Time since startup which the button was used for the last time

            #region PROPERTIES

            public string Name { get { return m_Name; } }
            public List<string> Keys { get { return m_Keys; } }

            public float LastUseTime
            {
                set { m_LastUseTime = value; }
                get { return m_LastUseTime; }
            }

            #endregion

            public Button (string name, string defaultKey)
            {
                m_Name = name;
                m_Keys = new List<string>
                {
                    defaultKey
                };

                m_LastUseTime = 0;
            }

            public void AddNewKey (string keyName)
            {
                // Link a new key to this button
                if (m_Keys != null)
                    m_Keys.Add(keyName);
            }

            public void RemoveKey (int index)
            {
                // Unlink a key used in this button
                m_Keys.RemoveAt(index);
            }

            public void EditKey (string keyName, string newKey)
            {
                for (int i = 0; i < m_Keys.Count; i++)
                {
                    if (m_Keys[i] == keyName)
                        m_Keys[i] = newKey;
                }
            }
        }
    }
}
