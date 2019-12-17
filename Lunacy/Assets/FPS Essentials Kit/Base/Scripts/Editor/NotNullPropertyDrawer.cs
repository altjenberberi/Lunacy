/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NotNullAttribute))]
internal sealed class NotNullPropertyDrawer : PropertyDrawer
{
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        var notnull = attribute as NotNullAttribute;

        if (property.objectReferenceValue == null && property.propertyType == SerializedPropertyType.ObjectReference)
        {
            var content = new GUIContent(" " + label.text, EditorGUIUtility.IconContent("icons/d_console.warnicon.sml.png").image, 
                notnull.overrideMessage ? notnull.message : "The field " + property.displayName + " can not be null");

            EditorGUI.PropertyField(position, property, content);
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
