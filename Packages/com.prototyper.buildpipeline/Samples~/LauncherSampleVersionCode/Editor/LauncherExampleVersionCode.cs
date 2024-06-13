using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SS.Core;

namespace SS
{
    public partial class LauncherBase
    {

        private static void BuildNumberOffsetiOS(int offset)
        {
            if (int.TryParse(PlayerSettings.iOS.buildNumber, out var result))
            {
                PlayerSettings.iOS.buildNumber = (result + offset).ToString();
            }
        }

        private static void BuildNumberOffsetAndroid(int offset)
        {
            var result = PlayerSettings.Android.bundleVersionCode;
            {
                PlayerSettings.Android.bundleVersionCode = (result + offset);
            }
        }

        [LauncherSection("Version Code")]
        private static void DrawVersionCode()
        {
            // iOS
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("iOS Build Number", PlayerSettings.iOS.buildNumber, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("+", GUILayout.Width(25)))
            { BuildNumberOffsetiOS(1); }
            if (GUILayout.Button("-", GUILayout.Width(25)))
            { BuildNumberOffsetiOS(-1); }
            EditorGUILayout.EndHorizontal();

            // Android
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Android Build Number", PlayerSettings.Android.bundleVersionCode.ToString(), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("+", GUILayout.Width(25)))
            { BuildNumberOffsetAndroid(1); }
            if (GUILayout.Button("-", GUILayout.Width(25)))
            { BuildNumberOffsetAndroid(-1); }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Save Project"))
            {
                AssetDatabase.SaveAssets();
            }
        }
    }

}
