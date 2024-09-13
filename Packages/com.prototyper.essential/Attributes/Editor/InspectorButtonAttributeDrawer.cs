using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace SS
{
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonAttributeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var h = EditorGUI.GetPropertyHeight(property, label);
            return h + EditorGUIUtility.singleLineHeight;
        }

        void GUIButton(Rect newRect, SerializedProperty property, string label, string methodName)
        {
            if (GUI.Button(newRect, label))
            {
                var tos = property.serializedObject.targetObjects;
                if (tos != null)
                {
                    foreach (var o in tos)
                    {
                        var oType = o.GetType();
                        var method = oType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new System.Type[] {}, null);
                        method?.Invoke(o, null);
                    }
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var newRect = position;
            newRect.height = EditorGUIUtility.singleLineHeight;

            InspectorButtonAttribute a = (InspectorButtonAttribute)attribute;
            if (a.methods != null && a.labels != null)
            {
                var count = Mathf.Min(a.methods.Length, a.labels.Length);
                for (int i = 0; i < count; i++)
                {
                    newRect.xMin = (i * position.width / (float)count) + (position.xMin);
                    newRect.width = position.width / (float)count;
                    GUIButton(newRect, property, a.labels[i], a.methods[i]);
                }
            }
            else if (!string.IsNullOrEmpty(a.method))
            {
                GUIButton(newRect, property, a.label, a.method);
            }

            position.yMin = newRect.yMax;
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
