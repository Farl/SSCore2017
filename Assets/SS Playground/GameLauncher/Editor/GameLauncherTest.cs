using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using SS.Core;

[InitializeOnLoad]
public class GameLauncherTest
{
    static GameLauncherTest() { }

    [LauncherSection("Scene List")]
    private static void DrawSceneList()
    {
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
