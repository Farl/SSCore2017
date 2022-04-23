using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SS
{
    //[CustomPropertyDrawer(typeof(TextTableSettings.Data))]
    public class TextTableDataEditor: PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
            //return base.GetPropertyHeight(property, label);
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUILayout.Button("a");
            //base.OnGUI(position, property, label);
        }
    }

}
