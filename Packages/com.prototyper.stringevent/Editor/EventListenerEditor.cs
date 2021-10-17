using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SS
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(EventListener), true)]
    public class EventListenerEditor : Editor
    {
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var obj = target as EventListener;

            var propListenOnlyEnabled = serializedObject.FindProperty("listenOnlyEnabled");
            EditorGUILayout.PropertyField(propListenOnlyEnabled);

            var propRecvGlobal = serializedObject.FindProperty("m_recvGlobal");
            var propRecvLocal = serializedObject.FindProperty("m_recvLocal");
            string[] enumList = { "None", "Global", "Local", "Both" };

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            int selectIdx = (propRecvGlobal.boolValue? 1: 0) + (propRecvLocal.boolValue? 2: 0);
            selectIdx = EditorGUILayout.Popup(selectIdx, enumList, GUILayout.Width(64));
            if (EditorGUI.EndChangeCheck())
            {
                propRecvGlobal.boolValue = (selectIdx % 2) == 1;
                propRecvLocal.boolValue = (selectIdx >> 1) >= 1;
            }

            var propEventID = serializedObject.FindProperty("m_eventID");
            EditorGUILayout.PropertyField(propEventID);

            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            
            base.OnInspectorGUI();
        }
    }
}
