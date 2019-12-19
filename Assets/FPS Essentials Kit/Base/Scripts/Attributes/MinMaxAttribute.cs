/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class MinMaxAttribute : PropertyAttribute
{
    public readonly float min;
    public readonly float max;

    public MinMaxAttribute (float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}