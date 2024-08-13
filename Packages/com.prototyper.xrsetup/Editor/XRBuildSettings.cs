using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USE_OPENXR
using UnityEngine.XR.OpenXR;
using UnityEditor.XR.OpenXR;
using UnityEditor.XR.OpenXR.Features;
#endif

#if USE_XR_MANAGEMENT
using UnityEngine.XR.Management;
using UnityEditor.XR.Management;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SS
{
    [System.Serializable]
    public class XRDeviceSettings
    {
        public bool auto => featureSet.Count == 0 && features.Count == 0;
        public List<string> featureSet = new List<string>();
        public List<string> features = new List<string>();
    }

    public class XRBuildSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        #region Static

        public const string xrBuildSettingsPath = "Assets/Settings/XRBuildSettings.asset";
        public static XRBuildSettings xrBuildSettings;
        private static bool isLoadSettings = false;

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

        public static void LoadDeviceSettings(XRDevice device, BuildTarget buildTarget = BuildTarget.NoTarget)
        {
            if (isLoadSettings)
            {
                return;
            }
            isLoadSettings = true;
            BuildPipelineImplementXR.SetXRPlatform(device, buildTarget);
            isLoadSettings = false;
        }

        public static XRBuildSettings GetXRBuildSettings()
        {
            if (xrBuildSettings == null)
            {
#if UNITY_EDITOR
                if (EditorBuildSettings.TryGetConfigObject(nameof(XRBuildSettings), out xrBuildSettings) == false)
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
                    else
                    {

                    }
                    EditorBuildSettings.AddConfigObject(nameof(XRBuildSettings), xrBuildSettings, true);
                }
                else
                {
                    Debug.Log("XRBuildSettings found in EditorBuildSettings");
                }
#endif
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

        #region Public
        public Dictionary<XRDevice, XRDeviceSettings> settings = new Dictionary<XRDevice, XRDeviceSettings>();
        public XRDevice DefaultXRDevice => _defaultXRDevice;
        #endregion 

        #region Inspector
        [SerializeField] private XRDevice _defaultXRDevice;
        [SerializeField] private List<XRDevice> settingsKey;
        [SerializeField] private List<XRDeviceSettings> settingsValue;
        #endregion 


        public void OnAfterDeserialize()
        {
            settings = new Dictionary<XRDevice, XRDeviceSettings>();
            for (int i = 0; i < settingsKey.Count; i++)
            {
                settings.Add(settingsKey[i], settingsValue[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            settingsKey = new List<XRDevice>(settings.Keys);
            settingsValue = new List<XRDeviceSettings>(settings.Values);
        }
    }

}