using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR;

using UnityEditor;

#if USE_OPENXR
using UnityEngine.XR.OpenXR;
using UnityEditor.XR.OpenXR;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace SS
{
    public enum XRDevice
    {
        ViveFocus,
        MetaQuest,
    }

    public static class BuildPipelineImplementXR
    {
        /// <summary>
        ///     Get product name from PlayerSettings
        /// </summary>
        public static string productName
        {
            get
            {
                var buildTarget = EditorUserBuildSettings.activeBuildTarget;
                var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

                var name = PlayerSettings.GetApplicationIdentifier(targetGroup);
                if (string.IsNullOrEmpty(name))
                    name = PlayerSettings.productName;
                
                // Get last identifier
                var tokens = name.Split('.');
                if (tokens.Length > 0)
                {
                    name = tokens[tokens.Length - 1];
                }
                return name;
            }
        }
        public const XRDevice defaultXRDevice = XRDevice.MetaQuest;

        public static bool SetXRPlatform(XRDevice device, BuildTarget buildTarget = BuildTarget.NoTarget)
        {
            if (buildTarget == BuildTarget.NoTarget)
            {
                buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }

#if USE_OPENXR
            var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            var featureSets = OpenXRFeatureSetManager.FeatureSetsForBuildTarget(targetGroup);
            foreach (var featureSet in featureSets)
            {
                Debug.Log(featureSet.name);
                if (featureSet.name.Contains("Vive", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    featureSet.isEnabled = (device == XRDevice.ViveFocus);
                }
            }

            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            if (settings)
            {

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(settings.name);

                var features = settings.GetFeatures();
                foreach (var feature in features)
                {
                    if (feature.name.Contains("Vive", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        feature.enabled = (device == XRDevice.ViveFocus);
                    }
                    else if (feature.name.Contains("Oculus", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        feature.enabled = (device == XRDevice.MetaQuest);
                    }

                    char enabled = (feature.enabled) ? '+' : '-';
                    stringBuilder.AppendLine($"{enabled}{feature.name}");
                }

                Debug.Log(stringBuilder.ToString());
            }
            return true;
#else
            Debug.LogError("OpenXR is not ready. Please install OpenXR (com.unity.xr.openxr) package.");
            return false;
#endif
        }

        public static void Build(XRDevice device, bool isFinal = false, bool isDev = false, bool runPlayer = false)
        {
#if !SS_BUILDPIPELINE
            Debug.Log("BuildPipelineSystem is not ready. Please instal SS BuildPipeline (com.prototyper.buildpipeline) package.");
            return;
#endif
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;

            if (buildTarget != BuildTarget.Android)
            {
                Debug.LogError($"Incorrect build target {buildTarget}");
                return;
            }

            if (!SetXRPlatform(device, buildTarget))
            {
                return;
            }

            string build = isFinal ? "F" + (isDev? "D": "") : (isDev ? "D" : "R");
            var locationPathName = $"Build/{buildTarget.ToString()}/{productName}_{build}_{Application.version}({PlayerSettings.Android.bundleVersionCode})_{device}";
            var buildOptions = BuildOptions.ShowBuiltPlayer;

            if (runPlayer)
                buildOptions |= BuildOptions.AutoRunPlayer;
            if (isDev)
                buildOptions |= BuildOptions.Development;

#if SS_BUILDPIPELINE
            BuildPipelineSystem.BuildPlayer(
                args: new string[] {
                    "-setLocationPathName", locationPathName
                },
                buildTarget: buildTarget,
                scenes: null,
                options: buildOptions,
                extraDefines: (isFinal)? new string[] { "FINAL" } : new string[] { }
            );
#endif
            
            SetXRPlatform(defaultXRDevice, buildTarget);
        }

        #region Meta Quest
        [MenuItem("Build/XR/Build Quest (Dev)")]
        public static void BuildQuestDev()
        {
            Build(XRDevice.MetaQuest, isFinal: false, isDev: true, runPlayer: false);
        }

        [MenuItem("Build/XR/Build Quest (Release)")]
        public static void BuildQuest()
        {
            Build(XRDevice.MetaQuest, isFinal: false, isDev: false, runPlayer: false);
        }

        [MenuItem("Build/XR/Build Quest (Final)")]
        public static void BuildQuestFinal()
        {
            Build(XRDevice.MetaQuest, isFinal: true, isDev: false, runPlayer: false);
        }
        #endregion

        #region Vive Focus
        [MenuItem("Build/XR/Build Focus (Dev)")]
        public static void BuildFocusDev()
        {
            Build(XRDevice.ViveFocus, isFinal: false, isDev: true, runPlayer: false);
        }

        [MenuItem("Build/XR/Build Focus (Release)")]
        public static void BuildFocus()
        {
            Build(XRDevice.ViveFocus, isFinal: false, isDev: false, runPlayer: false);
        }

        [MenuItem("Build/XR/Build Focus (Final)")]
        public static void BuildFocusFinal()
        {
            Build(XRDevice.ViveFocus, isFinal: true, isDev: false, runPlayer: false);
        }
        #endregion

    }

}
