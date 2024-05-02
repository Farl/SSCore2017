namespace SS.Core
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Reflection;
    using System;
    using System.Linq;

    using UnityEditor;

    public class Launcher : EditorWindow
    {
        #region Static


        private static List<MethodInfo> methodInfoList = new List<MethodInfo>();

        [MenuItem("Tools/SS/Game Launcher")]
        public static void Open()
        {
            var w = EditorWindow.GetWindow<Launcher>();
            w.titleContent = new GUIContent($"Launcher");
        }

        [InitializeOnLoadMethod]
        private static void CheckSection()
        {
            var sectionList = TypeCache.GetMethodsWithAttribute<LauncherSection>();
            methodInfoList = sectionList.ToList();
        }

        #endregion

        private HashSet<MethodInfo> disabledMethods = new HashSet<MethodInfo>();
        private Vector2 scrollVec;
        private void OnGUI()
        {
            if (methodInfoList == null || methodInfoList.Count == 0)
                return;

            scrollVec = EditorGUILayout.BeginScrollView(scrollVec);
            foreach (var sec in methodInfoList)
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
    }
}
