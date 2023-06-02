using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace SS
{
    public class BuildInfoEditor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }


        [MenuItem("Tools/SS/Create build info")]
        private static void CreateBuildInfo()
        {
            CreateBuildInfo(null);
            ModifyBuildInfo(null);
        }
        private static void CreateBuildInfo(BuildReport report)
        {
            var info = ScriptableObject.CreateInstance<BuildInfo>();
            info.buildTime = report.summary.buildStartedAt.ToLocalTime().ToString("yyyy-MM-dd_HH:mm:ss");
            info.version = Application.version;
#if UNITY_ANDROID
            info.versionCode = PlayerSettings.Android.bundleVersionCode.ToString();
#elif UNITY_IOS
            info.versionCode = PlayerSettings.iOS.buildNumber;
#endif
            info.userName = Environment.UserName;

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateAsset(info, "Assets/Resources/BuildInfo.asset");
        }

        private static void ModifyBuildInfo(BuildReport report)
        {
            var info = AssetDatabase.LoadAssetAtPath<BuildInfo>("Assets/Resources/BuildInfo.asset");
            if (info == null)
                return;
            PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget), out info.defineSymbols);
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            CreateBuildInfo(report);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            ModifyBuildInfo(report);
        }
    }
}
