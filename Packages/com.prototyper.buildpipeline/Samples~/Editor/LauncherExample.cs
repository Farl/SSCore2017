using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SS.Core;

[InitializeOnLoad]
public class LauncherExample
{
    static LauncherExample() { }

    [LauncherSection("Test 1")]
    private static void Test(int a = 1)
    {
        EditorGUILayout.LabelField("Test 1!");
    }

    [LauncherSection("Test 2")]
    private void Test2()
    {
        EditorGUILayout.LabelField("Test 2!");
    }

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
