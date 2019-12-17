/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class NotNullAttribute : PropertyAttribute
{
    public readonly string message = "This field can not be null";
    public readonly bool overrideMessage;

    public NotNullAttribute ()
    {
        overrideMessage = false;
    }
    
    public NotNullAttribute (string message)
    {
        overrideMessage = true;
        this.message = message;
    }
}
