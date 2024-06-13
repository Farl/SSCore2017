using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SS.Core;

namespace SS
{
    public partial class LauncherBase
    {
        [LauncherSection("PlayerPrefs & EditorPrefs")]
        private static void DrawPrefs()
        {
            if (GUILayout.Button("Clear PlayerPrefs"))
            {
                PlayerPrefs.DeleteAll();
            }
            if (GUILayout.Button("Clear EditorPrefs"))
            {
                EditorPrefs.DeleteAll();
            }
            if (GUILayout.Button("GC"))
            {
                System.GC.Collect();
            }
        }
    }
}