using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace SS.Core
{
    [CustomPropertyDrawer(typeof(GuidAttribute))]
    public class GuidAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return CalculateGUI(false, new Rect(), property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            CalculateGUI(true, position, property, label);
        }

        /// <summary>
        /// Return height or draw GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private float CalculateGUI(bool draw, Rect position, SerializedProperty property, GUIContent label)
        {
            var baseRect = position;
            baseRect.height = base.GetPropertyHeight(property, label);

            if (!draw)
            {
                position = baseRect;
            }
            else
            {
                
            }
            var currRect = baseRect;

            var wholeRect = position;
            var type = SerializedPropertyUtility.GetType(property);
            if (type == typeof(string))
            {
                if (draw)
                {
                    EditorGUI.PropertyField(currRect, property, label);
                    currRect.y += currRect.height;
                }

                var path = AssetDatabase.GUIDToAssetPath(property.stringValue);
                if (!string.IsNullOrEmpty(path))
                {
                    if (draw)
                    {
                        currRect.height = EditorGUIUtility.singleLineHeight;
                        if (GUI.Button(currRect, $"{path}"))
                        {
                            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                            Selection.activeObject = obj;
                        }
                        currRect.y += currRect.height;
                    }
                    wholeRect.height += EditorGUIUtility.singleLineHeight;
                }
            }
            else
            {
                if (draw)
                    EditorGUI.PropertyField(position, property, label);
            }
            if (draw)
                return 0;
            return wholeRect.height;
        }
    }
}