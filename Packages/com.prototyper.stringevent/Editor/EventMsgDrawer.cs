using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace SS
{
    [CanEditMultipleObjects]
    public class EventMsgTargetPicker : EditorWindow
    {
        public SerializedProperty property;   
        public static void OpenPicker(SerializedProperty property)
        {
            var window = CreateWindow<EventMsgTargetPicker>();
            window.property = property;
            window.titleContent = new GUIContent(property.serializedObject.targetObject.name);

        }
        private void OnGUI()
        {
            if (property != null)
            {
                EditorGUILayout.PropertyField(property);
                if (GUILayout.Button("Clear"))
                {
                    property.ClearArray();
                }
            }
        }
    }

    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(EventMsg))]
    public class EventMsgDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUIUtility.singleLineHeight * 4;
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            //EditorGUI.BeginProperty(pos, label, property);

            //GUI.Box(pos, string.Empty, new GUIStyle("HelpBox"));

            var propEventID = property.FindPropertyRelative("m_eventID");
            var propTargetObjs = property.FindPropertyRelative("m_targetObj");
            var propBool = property.FindPropertyRelative("m_paramBool");
            var propString = property.FindPropertyRelative("m_paramString");
            var propDelayTime = property.FindPropertyRelative("m_delayTime");
            var propUseTimeScale = property.FindPropertyRelative("m_useTimeScale");

            var currPos = pos.position;
            var lineHeight = EditorGUIUtility.singleLineHeight;

            var rectToggle = new Rect(currPos, new Vector2(24, lineHeight));
            property.isExpanded = EditorGUI.Foldout(rectToggle, property.isExpanded, string.Empty);
            currPos.x = rectToggle.xMax;    // horizontal

            var rectBool = new Rect(currPos, new Vector2(24, lineHeight));
            propBool.boolValue = EditorGUI.Toggle(rectBool, propBool.boolValue);
            currPos.x = rectBool.xMax;    // horizontal

            var rectEventID = new Rect(currPos, new Vector2(pos.xMax - currPos.x, lineHeight));
            propEventID.stringValue = EditorGUI.TextField(rectEventID, propEventID.stringValue);
            currPos = new Vector2(pos.x, rectBool.yMax);    // new line

            if (property.isExpanded)
            {
                var rectTargetObjs = new Rect(currPos, new Vector2(pos.width, lineHeight));
                var count = propTargetObjs.arraySize;
                var pickerLabel = count > 0 ? $"Pick (Local [{count}])" : "Pick (Global)";
                if (GUI.Button(rectTargetObjs, pickerLabel))
                {
                    EventMsgTargetPicker.OpenPicker(propTargetObjs);
                }
                currPos = new Vector2(pos.x, rectTargetObjs.yMax);  // new line

                var rectString = new Rect(currPos, new Vector2(pos.width, lineHeight));
                propString.stringValue = EditorGUI.TextField(rectString, propString.displayName, propString.stringValue);
                currPos = new Vector2(pos.x, rectString.yMax);  // new line

                var rectDelayTime = new Rect(currPos, new Vector2(pos.width * 0.66f, lineHeight));
                propDelayTime.vector2Value = EditorGUI.Vector2Field(rectDelayTime, propDelayTime.displayName, propDelayTime.vector2Value);
                currPos.x = rectDelayTime.xMax;    // horizontal

                var rectTimeScale = new Rect(currPos, new Vector2(pos.xMax - currPos.x, lineHeight));
                propUseTimeScale.boolValue = GUI.Toggle(rectTimeScale, propUseTimeScale.boolValue, propUseTimeScale.displayName);
                currPos = new Vector2(pos.x, rectTimeScale.yMax);  // new line
            }

            //EditorGUI.EndProperty();
        }
    }
}
