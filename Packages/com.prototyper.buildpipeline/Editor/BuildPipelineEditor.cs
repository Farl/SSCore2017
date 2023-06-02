/**
 * 
 **/

namespace SS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class BuildPipelineEditor : EditorWindow
    {
        public string arguments = string.Empty;
        public string extraDefines = string.Empty;
        public BuildOptions buildOptions = BuildOptions.ShowBuiltPlayer;
        public string supportBuildCommand;

        [MenuItem("Build/Open Editor")]
        public static void OpenWindow()
        {
            var w = EditorWindow.GetWindow<BuildPipelineEditor>();
            w.titleContent = new GUIContent("Build");
        }

        public static void OpenWindow(string arguments, string extraDefines, BuildOptions buildOptions)
        {
            var w = EditorWindow.GetWindow<BuildPipelineEditor>();
            w.titleContent = new GUIContent("Build");
            w.arguments = arguments;
            w.extraDefines = extraDefines;
            w.buildOptions = buildOptions;
        }

        private void RefreshSupportBuildCommand()
        {
            var cmds = BuildPipelineSystem.GetBuildCommands();
            supportBuildCommand = string.Empty;
            foreach (var cmd in cmds)
            {
                supportBuildCommand += $"{cmd}\n";
            }
        }

        private void OnEnable()
        {
            RefreshSupportBuildCommand();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Open default folder"))
            {
                BuildPipelineSystem.OpenDefaultPlayerFolder();
            }

            EditorGUILayout.LabelField(new GUIContent("Support commands:"));
            EditorGUILayout.TextArea(supportBuildCommand);
            if (GUILayout.Button("Refresh support build commands"))
            {
                RefreshSupportBuildCommand();
            }

            if (arguments == null)
                arguments = string.Empty;
            EditorGUILayout.LabelField(new GUIContent("Arguments:"));
            arguments = EditorGUILayout.TextArea(arguments);

            if (extraDefines == null)
                extraDefines = string.Empty;
            EditorGUILayout.LabelField(new GUIContent("Extra Define Symbols:"));
            extraDefines = EditorGUILayout.TextArea(extraDefines);

            buildOptions = (BuildOptions)EditorGUILayout.EnumFlagsField(new GUIContent("Build Options"), buildOptions);

            if (GUILayout.Button("Build"))
            {
                var args = arguments.Split(" \n", System.StringSplitOptions.RemoveEmptyEntries);
                var defineSymbols = extraDefines.Split(" \n", System.StringSplitOptions.RemoveEmptyEntries);

                BuildPipelineSystem.BuildPlayer(
                    args,
                    EditorUserBuildSettings.activeBuildTarget,
                    null,
                    buildOptions,
                    defineSymbols
                );
            }
        }
    }
}
