using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace SS
{
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var guiEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = guiEnabled;
        }
    }
}
