/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Collections.Generic;
using UnityEngine;

namespace Essentials
{
    namespace Input
    {
        [CreateAssetMenu(menuName = "Input Bindings", fileName = "New Input Bindings", order = 101)]
        public sealed class InputBindings : ScriptableObject
        {
            [SerializeField]
            private List<Button> m_Buttons = new List<Button>();

            [SerializeField]
            private List<Axis> m_Axes = new List<Axis>();

            #region PROPERTIES

            public List<Button> Buttons { get { return m_Buttons; } }
            public List<Axis> Axes { get { return m_Axes; } }

            #endregion

            #region IMPUT MANAGER

            public void AddButton (string name, string defaultKey)
            {
                m_Buttons.Add(new Button(name, defaultKey));
            }

            public void RemoveButton (int index)
            {
                m_Buttons.RemoveAt(index);
            }

            public void AddAxis (string name, string positiveKey, string negativeKey, float sensitivity, float gravity, float deadZone)
            {
                m_Axes.Add(new Axis(name, positiveKey, negativeKey, sensitivity, gravity, deadZone));
            }

            public void RemoveAxis (int index)
            {
                m_Axes.RemoveAt(index);
            }

            #endregion
        }
    }
}
