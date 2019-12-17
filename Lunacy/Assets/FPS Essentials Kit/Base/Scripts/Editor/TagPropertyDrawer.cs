/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TagAttribute))]
internal sealed class TagPropertyDrawer : PropertyDrawer
{
    public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.BeginProperty(position, label, property);

            var attrib = attribute as TagAttribute;

            if (attrib.AllowUntagged)
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            }
            else
            {
                var tags = (from t in UnityEditorInternal.InternalEditorUtility.tags
                            where t != "Untagged"
                            select new GUIContent(t)).ToArray();
                var stag = property.stringValue;
                int index = -1;
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i].text == stag)
                    {
                        index = i;
                        break;
                    }
                }
                index = EditorGUI.Popup(position, label, index, tags);
                property.stringValue = index >= 0 ? tags[index].text : null;
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
