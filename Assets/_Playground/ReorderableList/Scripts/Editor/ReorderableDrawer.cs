using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditorInternal;
using UnityEngine.Events;

[CustomPropertyDrawer (typeof(ReorderableAttribute))]
public class ReorderableDrawer : PropertyDrawer
{
    float maxHeight;
    SerializedProperty firstProperty;
    string displayName;
	ReorderableList list;

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
        float h = 0; // base.GetPropertyHeight(property, label);
        if (SerializedPropertyUtility.IsArrayElement(property))
        {
            if (SerializedPropertyUtility.IndexOfArrayElement(property) == 0)
            {
                //SerializedProperty parentProp = SerializedPropertyUtility.GetArrayParentProperty(property);
                //h += EditorGUI.GetPropertyHeight(property);
            }
        }
        return h;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
        //maxHeight = 0;

        if (SerializedPropertyUtility.IsArrayElement(property))
        {
            if (SerializedPropertyUtility.IndexOfArrayElement(property) == 0)
            {
                firstProperty = property;
                SerializedProperty parentProp = SerializedPropertyUtility.GetArrayParentProperty(property);
                //EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property, true);
                position.y += EditorGUIUtility.singleLineHeight;

                parentProp.serializedObject.Update();

                if (list == null)
                {
                    list = new ReorderableList(parentProp.serializedObject, parentProp, true, true, true, true);

                    list.drawElementCallback += DrawElement;
                    list.elementHeightCallback += ElementHeightCallback;
                    list.drawHeaderCallback += DrawHeader;
                }

                if (list != null)
                {
                    list.elementHeight = maxHeight;
                    list.DoLayoutList();
                }
                
                parentProp.serializedObject.ApplyModifiedProperties();
            }
        }
	}

    void DrawHeader(Rect rect)
    {
        if (list != null)
        {
            GUI.Label(rect, list.serializedProperty.propertyPath);
        }
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        if (list != null)
        {
            SerializedProperty sp = list.serializedProperty.GetArrayElementAtIndex(index);
            sp.serializedObject.Update();
            EditorGUI.PropertyField(rect, sp, true);
            sp.serializedObject.ApplyModifiedProperties();
        }
    }

    private float ElementHeightCallback(int index)
    {
        if (list != null)
        {
            SerializedProperty sp = list.serializedProperty.GetArrayElementAtIndex(index);
            maxHeight = Mathf.Max(maxHeight, EditorGUI.GetPropertyHeight(sp));
            return maxHeight;
        }
        return 0;
    }
}
