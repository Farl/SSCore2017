
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;
using UnityEngine.XR;

#if USE_OPENXR
using UnityEngine.XR.OpenXR;
using UnityEditor.XR.OpenXR;
using UnityEditor.XR.OpenXR.Features;
#endif

#if USE_XR_MANAGEMENT
using UnityEngine.XR.Management;
using UnityEditor.XR.Management;
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

        #region XR Build Settings

        public const string xrBuildSettingsPath = "Assets/Settings/XRBuildSettings.asset";
        public static XRBuildSettings xrBuildSettings;

        public static void SaveDeviceSettings(XRDevice device, BuildTarget buildTarget = BuildTarget.NoTarget)
        {
            var bs = GetXRBuildSettings();
            var currXRDeviceSettings = GetXRDeviceSettings(device);

            if (buildTarget == BuildTarget.NoTarget)
            {
                buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }
            var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            // Clear first
            currXRDeviceSettings.featureSet.Clear();
            currXRDeviceSettings.features.Clear();

#if USE_OPENXR
            // OpenXR Feature Set
            var featureSets = OpenXRFeatureSetManager.FeatureSetsForBuildTarget(targetGroup);
            foreach (var featureSet in featureSets)
            {
                if (featureSet.isEnabled)
                {
                    currXRDeviceSettings.featureSet.Add(featureSet.name);
                }
            }

            // OpenXR Settings
            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            if (settings)
            {
                var features = settings.GetFeatures();
                foreach (var feature in features)
                {
                    if (feature.enabled)
                    {
                        currXRDeviceSettings.features.Add(feature.name);
                    }
                }
            }
#endif

            EditorUtility.SetDirty(bs);
        }

        public static XRBuildSettings GetXRBuildSettings()
        {
            if (xrBuildSettings == null)
            {
                xrBuildSettings = AssetDatabase.LoadAssetAtPath<XRBuildSettings>(xrBuildSettingsPath);
                if (xrBuildSettings == null)
                {
                    // Create one
                    xrBuildSettings = ScriptableObject.CreateInstance<XRBuildSettings>();
                    xrBuildSettings.settings = new Dictionary<XRDevice, XRDeviceSettings>
                    {
                        { XRDevice.ViveFocus, new XRDeviceSettings() },
                        { XRDevice.MetaQuest, new XRDeviceSettings() }
                    };
                    DirectoryUtility.CheckAndCreateDirectory(xrBuildSettingsPath, withFileName: true);
                    AssetDatabase.CreateAsset(xrBuildSettings, xrBuildSettingsPath);
                    AssetDatabase.SaveAssets();
                }
            }
            return xrBuildSettings;
        }

        public static XRDeviceSettings GetXRDeviceSettings(XRDevice device)
        {
            var bs = GetXRBuildSettings();
            if (bs.settings.ContainsKey(device) == false)
            {
                bs.settings.Add(device, new XRDeviceSettings());
            }
            if (bs.settings.TryGetValue(device, out var settings))
            {
                return settings;
            }
            return null;
        }
        #endregion

        private static bool CheckName(string name, params string[] compare)
        {
            foreach (var item in compare)
            {
                if (name.Contains(item, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SetupOpenXR(XRDevice device, BuildTarget buildTarget)
        {
#if USE_OPENXR
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Setup OpenXR for {device.ToString()}");
            
            var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            // Feature Sets
            stringBuilder.AppendLine($"Feature Sets:");
            var featureSets = OpenXRFeatureSetManager.FeatureSetsForBuildTarget(targetGroup);
            foreach (var featureSet in featureSets)
            {
                if (CheckName(featureSet.name, "Vive"))
                {
                    featureSet.isEnabled = device == XRDevice.ViveFocus;
                }

                stringBuilder.AppendLine($"{(featureSet.isEnabled ? '+' : '-')}{featureSet.name}");
            }

            // Feature
            stringBuilder.AppendLine($"Features:");
            var settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            if (settings)
            {
                stringBuilder.AppendLine($"OpenXR Settings ({settings.name}");

                var features = settings.GetFeatures();
                foreach (var feature in features)
                {
                    if (CheckName(feature.name, "Vive"))
                    {
                        feature.enabled = device == XRDevice.ViveFocus;
                    }
                    else if (CheckName(feature.name, "Meta", "Quest", "Oculus"))
                    {
                        // Exception for "OculusQuestFeature" which is deprecated
                        if (CheckName(feature.name, "OculusQuestFeature"))
                        {
                            feature.enabled = false;
                        }
                        else
                        {
                            feature.enabled = device == XRDevice.MetaQuest;
                        }
                    }
                    else
                    {
                        // Do nothing
                    }

                    stringBuilder.AppendLine($"{(feature.enabled ? "<color=green>+</color>" : "<color=red>-</color>")}{feature.name}");
                }
            }

            Debug.Log(stringBuilder.ToString());
            return settings != null;
#else
            return false;
#endif
        }

        public static bool SetXRPlatform(XRDevice device, BuildTarget buildTarget = BuildTarget.NoTarget)
        {
            if (buildTarget == BuildTarget.NoTarget)
            {
                buildTarget = EditorUserBuildSettings.activeBuildTarget;
            }
            var targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            bool isSupported = false;
#if USE_OPENXR
            if (SetupOpenXR(device, buildTarget))
            {
                isSupported = true;
            }

#elif USE_XR_MANAGEMENT
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(targetGroup);
            if (settings)
            {
                var manager = settings.Manager;
                if (manager)
                {
                    if (manager.activeLoaders.Count > 0)
                    {
                        foreach (var loader in manager.activeLoaders)
                        {
                            if (device == XRDevice.MetaQuest && loader.name.Contains("Oculus", System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                isSupported = true;
                                break;
                            }
                            // Todo: Validate Vive loader
                            if (device == XRDevice.ViveFocus && loader.name.Contains("Vive", System.StringComparison.InvariantCultureIgnoreCase))
                            {
                                isSupported = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"No active loader! {device.ToString()} loader must be active.");
                    }
                }
            }

#else
            Debug.LogError("XR Management is not ready. Please install com.unity.xr.management package.");

#endif
            return isSupported;
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

            string build = isFinal ? "F" + (isDev ? "D" : "") : (isDev ? "D" : "R");
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
                extraDefines: (isFinal) ? new string[] { "FINAL" } : new string[] { }
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