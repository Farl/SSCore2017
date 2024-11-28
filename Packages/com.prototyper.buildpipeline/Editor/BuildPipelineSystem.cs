/**
 * BuildPipelineSystem.cs
 * Created by: Farl Lee [spring_snow@hotmail.com]
 * This file is part of SS BuildPipeline.
 **/
namespace SS
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using System.Reflection;

    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Assertions;

    using UnityEditor;
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;
    using UnityEditor.SceneManagement;

    public class BuildCommandAttribute : Attribute
    {
        public string command;
        public bool beforeBuild = false;
        public bool afterBuild = false;
        public BuildCommandAttribute(string command, bool beforeBuild, bool afterBuild)
        {
            this.command = command;
            this.beforeBuild = beforeBuild;
            this.afterBuild = afterBuild;
        }
    }

    public static partial class BuildPipelineSystem
    {
        #region Build Menu Item
        [MenuItem("Build/Build Player")]
        public static void BuildPlayer()
        {
            if (!Application.isBatchMode)
                BuildPlayer(null, EditorUserBuildSettings.activeBuildTarget, null, BuildOptions.ShowBuiltPlayer);
            else
                BuildPlayerWithArguments();
        }

        [MenuItem("Build/Build Player (With Arguments)")]
        public static void BuildPlayerWithArguments()
        {
            var args = Environment.GetCommandLineArgs();
            if (!Application.isBatchMode)
            {
                var arguments = string.Empty;
                foreach (var arg in args)
                {
                    arguments += $"{arg} ";
                }
                BuildPipelineEditor.OpenWindow(arguments, null, BuildOptions.None);
            }
            else
                BuildPlayer(args, EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem("Build/Build Player (Final)")]
        public static void BuildPlayerFinal()
        {
            BuildPlayer(null, EditorUserBuildSettings.activeBuildTarget, null, BuildOptions.None, new string[] { "FINAL" });
        }
        #endregion

        #region Build Command

        private static string ReadTempFileLine(string fileName)
        {
            var tempFilePath = Path.Combine((new DirectoryInfo(Application.dataPath)).Parent.ToString(), "Temp", $"{fileName}.txt");
            string result = null;
            using (var fs = File.OpenText(tempFilePath))
            {
                result = fs.ReadLine();
            }
            return result;
        }

        private static void WriteTempFileLine(string fileName, string line)
        {
            var tempFilePath = Path.Combine((new DirectoryInfo(Application.dataPath)).Parent.ToString(), "Temp", $"{fileName}.txt");
            using (var fs = File.CreateText(tempFilePath))
            {
                fs.WriteLine(line);
                fs.Close();
            }
        }

        [BuildCommand("-setLocationPathName", true, true)]
        public static void BuildCommandSetLocationPathName(string arg, bool beforeBuild, ref BuildPlayerOptions buildPlayerOptions)
        {
            if (string.IsNullOrEmpty(arg))
                return;
            if (beforeBuild)
            {
                buildPlayerOptions.locationPathName = $"{arg}{GetBuildExtensionName(buildPlayerOptions.target)}";
            }
        }

        /// <summary>
        /// Set Build Options
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="beforeBuild"></param>
        /// <param name="buildPlayerOptions"></param>
        [BuildCommand("-setOptions", true, true)]
        public static void BuildCommandSetOptions(string arg, bool beforeBuild, ref BuildPlayerOptions buildPlayerOptions)
        {
            if (string.IsNullOrEmpty(arg))
                return;

            if (beforeBuild)
            {
                BuildOptions buildOptions = BuildOptions.None;
                var tokens = arg.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var token in tokens)
                {
                    if (Enum.TryParse<BuildOptions>(token, out var result))
                    {
                        buildOptions |= result;
                    }
                }
                buildPlayerOptions.options = buildOptions;
            }
        }

        [BuildCommand("-addDefines", true, true)]
        public static void BuildCommandAddDefines(string arg, bool beforeBuild, ref BuildPlayerOptions buildPlayerOptions)
        {
            if (string.IsNullOrEmpty(arg))
                return;

            if (beforeBuild)
            {
                var tokens = arg.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var newSymbols = new List<string>(tokens);
                if (buildPlayerOptions.extraScriptingDefines != null)
                    newSymbols.AddRange(buildPlayerOptions.extraScriptingDefines);

                buildPlayerOptions.extraScriptingDefines = newSymbols.ToArray();
            }
        }

        [BuildCommand("-removeDefines", true, true)]
        public static void BuildCommandRemoveDefines(string arg, bool beforeBuild, ref BuildPlayerOptions buildPlayerOptions)
        {
            if (string.IsNullOrEmpty(arg))
                return;

            var tokens = new List<string>(arg.Split(',', StringSplitOptions.RemoveEmptyEntries));
            var buildTargetGroup = buildPlayerOptions.targetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
            string[] defineSymbols = null;

            if (beforeBuild)
            {
                PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out defineSymbols);

                // Save original symbols
                var writeLine = "";
                var newSymbols = new List<string>();
                foreach (var s in defineSymbols)
                {
                    if (!tokens.Contains(s))
                        newSymbols.Add(s);
                    writeLine += $"{s},";
                }
                WriteTempFileLine("BuildCommandRemoveDefines", writeLine);

                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newSymbols.ToArray());
            }
            else
            {
                var readLine = ReadTempFileLine("BuildCommandRemoveDefines");
                if (readLine != null)
                {
                    Debug.Log(readLine);
                    var symbols = readLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (symbols != null)
                    {
                        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, symbols);
                    }
                }
            }
        }

        [BuildCommand("-setDefines", true, true)]
        public static void BuildCommandSetDefines(string arg, bool beforeBuild, ref BuildPlayerOptions buildPlayerOptions)
        {
            if (string.IsNullOrEmpty(arg))
                return;

            var tokens = arg.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            string[] defineSymbols = null;

            if (beforeBuild)
            {
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup, out defineSymbols);

                // Save original symbols
                var writeLine = "";
                foreach (var s in defineSymbols)
                {
                    writeLine += $"{s},";
                }
                WriteTempFileLine("BuildCommandSetDefines", writeLine);

                var newSymbols = new List<string>(tokens);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newSymbols.ToArray());
            }
            else
            {
                var readLine = ReadTempFileLine("BuildCommandSetDefines");
                if (readLine != null)
                {
                    var symbols = readLine.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (symbols != null)
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
                }
            }
        }
        #endregion

        #region Build command action

        private static Dictionary<string, MethodInfo> cmdActions = null;

        public static string[] GetBuildCommands()
        {
            InitCommandMap();
            var cmds = new List<string>(cmdActions.Keys);
            return cmds.ToArray();
        }

        private static void InitCommandMap()
        {
            if (cmdActions == null)
                cmdActions = new Dictionary<string, MethodInfo>();
            else
                return;

            // Create command action mapping
            var methods = TypeCache.GetMethodsWithAttribute(typeof(BuildCommandAttribute));
            foreach (var m in methods)
            {
                var bc = m.GetCustomAttribute<BuildCommandAttribute>();
                if (bc != null)
                {
                    cmdActions.Add(bc.command.ToLower(), m);
                }
            }
        }

        private static void DoCommands(string[] args, bool beforeBuild, ref BuildPlayerOptions buildPlayerOptions)
        {
            if (args == null)
                return;
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                Debug.Log(arg);
                if (cmdActions.TryGetValue(arg.ToLower(), out var action))
                {
                    string argument = null;
                    if (i + 1 < args.Length)
                    {
                        argument = args[i + 1];
                    }

                    if (action != null)
                    {
                        var parameters = action.GetParameters();
                        if (parameters != null && parameters.Length == 3 &&
                            parameters[0].ParameterType == typeof(string) &&
                            parameters[1].ParameterType == typeof(bool) &&
                            parameters[2].ParameterType == typeof(BuildPlayerOptions).MakeByRefType())
                        {
                            var arguments = new object[] { argument, beforeBuild, buildPlayerOptions };
                            action?.Invoke(null, arguments);
                            buildPlayerOptions = (BuildPlayerOptions)arguments[2];
                            continue;
                        }
                    }
                    Debug.LogError($"Mehtod {arg} doesn't match!");
                }
            }
        }
        #endregion

        private static string GetBuildExtensionName(BuildTarget buildTarget)
        {
            var extension = string.Empty;

            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneOSX:
                    extension = ".app";
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    extension = ".exe";
                    break;
                case BuildTarget.Android:
                    if (EditorUserBuildSettings.buildAppBundle)
                        extension = ".aab";
                    else
                        extension = ".apk";
                    break;
            }
            return extension;
        }

        private static string BuildLocationPathName(BuildTarget buildTarget)
        {
            var ext = GetBuildExtensionName(buildTarget);
            var path = $"Build/{buildTarget.ToString()}/{Application.productName}{ext}";
            return path;
        }

        public static void OpenDefaultPlayerFolder()
        {
            EditorUtility.RevealInFinder(BuildLocationPathName(EditorUserBuildSettings.activeBuildTarget));
        }

        public static void BuildPlayer(string[] args, BuildTarget buildTarget, List<string> scenes = null, BuildOptions options = BuildOptions.None, string[] extraDefines = null)
        {
            if (scenes == null || scenes.Count == 0)
            {
                scenes = new List<string>();

                var editorScenes = EditorBuildSettings.scenes;

                foreach (var es in editorScenes)
                {
                    if (es.enabled)
                    {
                        //Debug.Log(es.path);
                        scenes.Add(es.path);
                    }
                };
            }
            var check = scenes == null || scenes.Count == 0;
            Assert.IsFalse(check, "Nothing to build.");

            if (check)
            {
                return;
            }
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenes.ToArray();
            
            buildPlayerOptions.locationPathName = BuildLocationPathName(buildTarget);
            DirectoryUtility.CheckAndCreateDirectory(buildPlayerOptions.locationPathName, true);

            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.options = options;
            buildPlayerOptions.targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            buildPlayerOptions.extraScriptingDefines = extraDefines;

            InitCommandMap();
            DoCommands(args, true, ref buildPlayerOptions);
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            DoCommands(args, false, ref buildPlayerOptions);

            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(0);
                }
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit((int)summary.result);
                }
            }
        }

        public class BuildPreprocess : IPreprocessBuildWithReport
        {
            public int callbackOrder => 0;

            public void OnPreprocessBuild(BuildReport report)
            {
            }
        }
        public class BuildPostprocess : IPostprocessBuildWithReport
        {
            public int callbackOrder => 0;

            public void OnPostprocessBuild(BuildReport report)
            {
            }
        }
    }
}