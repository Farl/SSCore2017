using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SS
{
    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(EventArray))]
    public class EventArrayDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propEventArray = property.FindPropertyRelative("eventArray");
            return EditorGUI.GetPropertyHeight(propEventArray);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propEventArray = property.FindPropertyRelative("eventArray");

            EditorGUI.PropertyField(position, propEventArray, label, true);
        }
    }
}