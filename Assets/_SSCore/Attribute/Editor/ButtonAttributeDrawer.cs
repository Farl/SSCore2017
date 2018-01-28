using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

[CustomPropertyDrawer(typeof(ButtonAttribute))]
public class ButtonAttributeDrawer : PropertyDrawer {
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ButtonAttribute ba = this.attribute as ButtonAttribute;

        if (ba != null)
        {
            if (GUI.Button(new Rect(position.x + ba.padding, position.y, position.width - ba.padding * 2f, EditorGUIUtility.singleLineHeight), string.Format("{0} [{1}]", ba.displayName, property.serializedObject.targetObject.ToString())))
            {
                Type t = property.serializedObject.targetObject.GetType();
                MethodInfo mi = t.GetMethod(ba.methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (mi != null)
                {
                    mi.Invoke(property.serializedObject.targetObject, new object[] { });
                }
            }
        }
        position.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property, label);
    }
}
