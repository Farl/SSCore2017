using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SS.Core;

namespace SS
{
    public partial class LauncherBase
    {
        [LauncherSection("Scene List")]
        private static void DrawSceneList()
        {
            EditorGUILayout.LabelField("Test");
            var editorScenes = EditorBuildSettings.scenes;
            foreach (var scene in editorScenes)
            {
                var fileName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                var content = new GUIContent($"{fileName}");
                content.tooltip = scene.path;
                if (GUILayout.Button(content))
                {
                    var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(scene.path);
                    Selection.activeObject = obj;
                }
            }
        }
    }
}