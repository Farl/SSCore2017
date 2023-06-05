namespace SS.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Reflection;
    using System;
    using System.IO;
    using System.Linq;

    using UnityEditor;
    using UnityEditor.SceneManagement;

    public class Launcher : EditorWindow
    {
        HashSet<MethodInfo> disabledMethods = new HashSet<MethodInfo>();
        Vector2 scrollVec;

        private static TypeCache.MethodCollection sectionList;
        [MenuItem("Tools/Launcher")]
        public static void Open()
        {
            var w = EditorWindow.GetWindow<Launcher>();
            w.titleContent = new GUIContent($"Launcher");
        }

        [InitializeOnLoadMethod]
        private static void CheckSection()
        {
            if (sectionList.Count <= 0)
            {
                sectionList = TypeCache.GetMethodsWithAttribute<LauncherSection>();
            }
        }

        private void OnGUI()
        {
            scrollVec = EditorGUILayout.BeginScrollView(scrollVec);
            foreach (var sec in sectionList)
            {
                var a = sec.GetCustomAttribute<LauncherSection>();
                var isDisabled = disabledMethods.Contains(sec);
                var foldOut = EditorGUILayout.Foldout(!isDisabled, (a == null)? sec.Name: $"{sec.Name} = {a.SectionName}");
                if (foldOut != !isDisabled)
                {
                    if (foldOut)
                        disabledMethods.Remove(sec);
                    else
                        disabledMethods.Add(sec);
                }
                if (foldOut)
                {
                    var parameters = sec.GetParameters();
                    //EditorGUILayout.LabelField(parameters.Length.ToString());
                    using (var cHorizontalScope = new GUILayout.HorizontalScope())
                    {
                        // Manual intend level, Effect not only GUILayout but also EditorGUILayout
                        GUILayout.Space(1 * EditorGUIUtility.singleLineHeight);
                        if (!sec.IsStatic)
                        {
                            EditorGUILayout.HelpBox($"Function {sec.Name}(...) should be static", MessageType.Error);
                        }
                        else if (parameters.Length > 0)
                        {
                            EditorGUILayout.HelpBox($"Function {sec.Name}(...) parameter length is incorrect!", MessageType.Error);
                        }
                        else
                        { 
                            using (var cVerticalScope = new GUILayout.VerticalScope())
                            {
                                try
                                {
                                    sec.Invoke(null, null);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogException(ex);
                                }
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }


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
                GC.Collect();
            }
        }

        [LauncherSection("Scene List")]
        private static void DrawSceneList()
        {
            EditorGUILayout.LabelField("Test");
            var editorScenes = EditorBuildSettings.scenes;
            foreach (var scene in editorScenes)
            {
                var fileName = Path.GetFileNameWithoutExtension(scene.path);
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
