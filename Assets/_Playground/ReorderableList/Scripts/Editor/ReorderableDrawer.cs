using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditorInternal;
using UnityEngine.Events;

[CustomPropertyDrawer (typeof(ReorderableAttribute), true)]
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
                h += EditorGUIUtility.singleLineHeight; // header
                h += EditorGUIUtility.singleLineHeight; // footer

                //SerializedProperty parentProp = SerializedPropertyUtility.GetArrayParentProperty(property);
            }
            //h += EditorGUI.GetPropertyHeight(property);
            h += maxHeight;
        }

        return h;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
        
        if (SerializedPropertyUtility.IsArrayElement(property))
        {
            if (SerializedPropertyUtility.IndexOfArrayElement(property) == 0)
            {
                //EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property, true);
                //position.y += EditorGUIUtility.singleLineHeight;

                firstProperty = property;
                SerializedProperty parentProp = SerializedPropertyUtility.GetArrayParentProperty(property);

                if (list == null || list.serializedProperty.propertyPath != parentProp.propertyPath)
                {
                    list = new ReorderableList(parentProp.serializedObject, parentProp, true, true, true, true);

                    list.drawElementCallback += DrawElement;
                    list.elementHeightCallback += ElementHeightCallback;
                    list.drawHeaderCallback += DrawHeader;
                    //list.drawFooterCallback += DrawFooter;
                }

                if (list != null)
                {
                    list.elementHeight = maxHeight;
                    //list.DoLayoutList();
                    list.DoList(new Rect(position.x, position.y, position.width, maxHeight));
                }
                
            }
        }
    }

    void DrawFooter(Rect rect)
    {

    }

    void DrawHeader(Rect rect)
    {
        if (list != null)
        {
        }
    }

    string GetIndex(string path)
    {
        int startIndex = path.LastIndexOf('[') + 1;
        if (startIndex <= 0)
            return null;
        return path.Substring(startIndex, path.LastIndexOf(']') - startIndex);
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        if (list != null)
        {
            Rect pos = rect;
            SerializedProperty sp = list.serializedProperty.GetArrayElementAtIndex(index);
            if (sp.hasVisibleChildren)
            {
                GUI.Box(rect, "");
                float h = 0;
                string idx = GetIndex(sp.propertyPath);
                sp.NextVisible(true);
                do
                {
                    h = EditorGUI.GetPropertyHeight(sp, true);
                    EditorGUI.PropertyField(new Rect(rect.x, pos.y, rect.width, h), sp, true);
                    pos.y += h;
                } while (sp.NextVisible(false) && idx.Equals(GetIndex(sp.propertyPath)));
            }
            else
            {
                EditorGUI.PropertyField(rect, sp, true);
            }
        }
    }

    private float ElementHeightCallback(int index)
    {
        if (list != null)
        {
            SerializedProperty sp = list.serializedProperty.GetArrayElementAtIndex(index);
            float h = 0;
            if (sp.hasVisibleChildren)
            {
                string idx = GetIndex(sp.propertyPath);
                sp.NextVisible(true);
                do
                {
                    h += EditorGUI.GetPropertyHeight(sp, true);
                } while (sp.NextVisible(false) && idx.Equals(GetIndex(sp.propertyPath)));
            }
            else
            {
                h += EditorGUI.GetPropertyHeight(sp);
            }
            maxHeight = Mathf.Max(maxHeight, h);
            return maxHeight;
        }
        return 0;
    }
}
