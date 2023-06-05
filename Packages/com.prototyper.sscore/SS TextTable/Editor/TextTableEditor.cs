using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SS.TextTable
{
    public class TextTableEditor : EditorWindow
    {
        [MenuItem("Tools/SS/Text Table")]
        public static void Open()
        {
            var w = EditorWindow.GetWindow<TextTableEditor>();
            w.titleContent = new GUIContent($"Text Table");
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                var ts = TextTableSettings.Instance;
                var so = new SerializedObject(ts);

                // Draw everthing
                var itr = so.GetIterator();
                if (itr.Next(true))
                {
                    while (itr.Next(false))
                    {
                        EditorGUILayout.PropertyField(itr);
                    }
                }
                //EditorGUILayout.PropertyField(so.FindProperty("dataList"));
            }
            else
            {
                // Don't try to create when in Runtime
                EditorGUILayout.HelpBox("Application.isPlaying", MessageType.Warning);
            }
        }
    }

}