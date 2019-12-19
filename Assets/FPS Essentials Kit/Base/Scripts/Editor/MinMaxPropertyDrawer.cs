/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
internal sealed class MinMaxPropertyDrawer : PropertyDrawer
{
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var minMax = attribute as MinMaxAttribute;

        if (property.propertyType == SerializedPropertyType.Float)
        {
            var content = new GUIContent(label.text, "["+ minMax.min + " , " + minMax.max + "]");
            EditorGUI.PropertyField(position, property, content);
            property.floatValue = Mathf.Clamp(property.floatValue, minMax.min, minMax.max);
        }
        else if (property.propertyType == SerializedPropertyType.Integer)
        {
            var content = new GUIContent(label.text, "[" + minMax.min + " , " + minMax.max + "]");
            EditorGUI.PropertyField(position, property, content);
            property.intValue = Mathf.Clamp(property.intValue, float.IsNegativeInfinity(minMax.min) ? int.MinValue : (int)minMax.min,
                                            float.IsPositiveInfinity(minMax.max) ? int.MaxValue : (int)minMax.max);
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use MinMax with float or int.");
        }
    }
}