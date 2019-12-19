/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    namespace Input
    {
        using Input = UnityEngine.Input;

        /// <summary>
        /// Interface for handling user Input 
        /// </summary>
        public static class InputManager
        {
            private static InputBindings m_InputBindings;

            /// <summary>
            /// Loads the first InputBindings found in the game
            /// </summary>
            private static bool FindInputBindings ()
            {
                if (GameplayManager.Instance.Bindings != null)
                {
                    m_InputBindings = GameplayManager.Instance.Bindings;
                    return true;
                } 

                InputBindings[] foundItems = (InputBindings[])Resources.FindObjectsOfTypeAll(typeof(InputBindings));

                if (foundItems == null || foundItems.Length == 0)
                {
                    Debug.LogError("No InputBindings found in your project.");
                    return false;
                }
                else
                {
                    m_InputBindings = foundItems[0];
                    return true;
                }
            }

            /// <summary>
            /// Returns true if the user presses the same button twice
            /// </summary>
            public static bool DoubleTap (string buttonName, float interval)
            {
                if (Mathf.Abs(Time.timeScale) < float.Epsilon || !Input.anyKeyDown)
                    return false;

                Button b = FindButton(buttonName);
                if (b != null)
                {
                    foreach (string key in b.Keys)
                    {
                        if (Input.GetKeyDown(key) && b.LastUseTime + interval > Time.time)
                        {
                            b.LastUseTime = 0;
                            return true;
                        }
                        if (Input.GetKeyUp(key))
                        {
                            b.LastUseTime = Time.time;
                        }
                    }
                }
                return false;
            }

            /// <summary>
            /// Returns true if the user keeps the button pressed in a time interval
            /// </summary>
            public static bool HoldButton (string buttonName, float time)
            {
                if (Mathf.Abs(Time.timeScale) < float.Epsilon || !Input.anyKey)
                    return false;

                Button b = FindButton(buttonName);
                if (b != null)
                {
                    foreach (string key in b.Keys)
                    {
                        if (Input.GetKeyDown(key))
                        {
                            b.LastUseTime = Time.time;
                        }
                        if (Input.GetKey(key) && Time.time - b.LastUseTime > time)
                        {
                            b.LastUseTime = Time.time;
                            return true;
                        }
                    }
                }
                return false;
            }

            /// <summary>
            /// Return true if the user is pressing the button
            /// </summary>
            public static bool GetButton (string buttonName)
            {
                if (Mathf.Abs(Time.timeScale) < float.Epsilon || !Input.anyKey)
                    return false;

                Button b = FindButton(buttonName);
                if (b != null)
                {
                    foreach (string key in b.Keys)
                    {
                        if (Input.GetKey(key))
                            return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Return true if the user pressed the button
            /// </summary>
            public static bool GetButtonDown (string buttonName)
            {
                if (Mathf.Abs(Time.timeScale) < float.Epsilon || !Input.anyKeyDown)
                    return false;

                Button b = FindButton(buttonName);
                if (b != null)
                {
                    foreach (string key in b.Keys)
                    {
                        if (Input.GetKeyDown(key))
                            return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Return true if the user release the button
            /// </summary>
            public static bool GetButtonUp (string buttonName)
            {
                if (Mathf.Abs(Time.timeScale) < float.Epsilon)
                    return false;

                Button b = FindButton(buttonName);
                if (b != null)
                {
                    foreach (string key in b.Keys)
                    {
                        if (Input.GetKeyUp(key))
                            return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Returns the value of the virtual axis with smoothing filtering
            /// </summary>
            public static float GetAxis (string axisName)
            {
                if (Mathf.Abs(Time.timeScale) < float.Epsilon)
                    return 0;

                Axis a = FindAxis(axisName);
                if (a != null)
                {
                    float target = GetAxisRaw(a);
                    a.CurrentValue = Mathf.MoveTowards(a.CurrentValue, target, ((Mathf.Abs(target) > Mathf.Epsilon) ? Time.deltaTime / a.Sensitivity : Time.deltaTime * a.Gravity));
                    return (Mathf.Abs(a.CurrentValue) < a.DeadZone) ? 0 : a.CurrentValue;
                }
                return 0;
            }

            /// <summary>
            /// Returns the value of the virtual axis
            /// </summary>
            private static float GetAxisRaw (Axis a)
            {
                if (Input.GetKey(a.PositiveKey))
                {
                    return 1;
                }
                if (Input.GetKey(a.NegativeKey))
                {
                    return -1;
                }
                return 0;
            }

            /// <summary>
            /// Returns the value of the virtual axis without smoothing filtering
            /// </summary>
            public static float GetAxisRaw (string axisName)
            {
                if (Mathf.Abs(Time.timeScale) < float.Epsilon)
                    return 0;

                Axis a = FindAxis(axisName);
                return GetAxisRaw(a);
            }

            /// <summary>
            /// Returns the first found button with the given name
            /// </summary>
            private static Button FindButton (string name)
            {
                if (m_InputBindings == null)
                {
                    if (!FindInputBindings())
                        return null;
                }

                for (int i = 0; i < m_InputBindings.Buttons.Count; i++)
                {
                    if (m_InputBindings.Buttons[i].Name.Equals(name))
                        return m_InputBindings.Buttons[i];
                }

                Debug.LogError("InputManager: Button '" + name + "' was not found.");
                return null;
            }

            /// <summary>
            /// Returns the first found axis with the given name
            /// </summary>
            private static Axis FindAxis (string name)
            {
                if (m_InputBindings == null)
                {
                    if (!FindInputBindings())
                        return null;
                }

                for (int i = 0; i < m_InputBindings.Axes.Count; i++)
                {
                    if (m_InputBindings.Axes[i].Name.Equals(name))
                        return m_InputBindings.Axes[i];
                }

                Debug.LogError("InputManager: Axis '" + name + "' was not found.");
                return null;
            }

            public static void EditButton (string buttonName, string keyName, string newKey)
            {
                Button b = FindButton(buttonName);
                if (b != null)
                {
                    b.EditKey(keyName, newKey);
                }
            }

            public static void EditAxis (string axisName, string positiveKey, string negativeKey)
            {
                Axis a = FindAxis(axisName);
                if (a != null)
                {
                    a.EditAxis(positiveKey, negativeKey);
                }
            }

            /// <summary>
            /// Returns all buttons registered in the InputBinding
            /// </summary>
            public static Button[] GetButtonData ()
            {
                if (m_InputBindings == null)
                    return FindInputBindings() ? m_InputBindings.Buttons.ToArray() : null;

                return m_InputBindings.Buttons.ToArray();
            }

            /// <summary>
            /// Returns all axes registered in the InputBinding
            /// </summary>
            public static Axis[] GetAxisData ()
            {
                if (m_InputBindings == null)
                    return FindInputBindings() ? m_InputBindings.Axes.ToArray() : null;

                return m_InputBindings.Axes.ToArray();
            }
        }
    }
}