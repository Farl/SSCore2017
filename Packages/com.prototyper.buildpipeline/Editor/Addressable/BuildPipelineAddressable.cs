/**
 * BuildPipelineAddressable.cs
 * Created by: Farl Lee [spring_snow@hotmail.com]
 * This file is part of SS BuildPipeline.
 **/

//#define USE_ADDRESSABLE

#if USE_ADDRESSABLE
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

    using UnityEngine.AddressableAssets;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Build;
    using UnityEditor.AddressableAssets.Settings;

    public static partial class BuildPipelineSystem
    {
        public static string addressable_build_script
            = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
        public static string addressable_settings_asset
            = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        public static string addressable_profile_name = "Default";
        private static AddressableAssetSettings addressableSettings;

        static void getAddressableSettingsObject(string settingsAsset)
        {
            // This step is optional, you can also use the default settings:
            //settings = AddressableAssetSettingsDefaultObject.Settings;

            addressableSettings
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                    as AddressableAssetSettings;

            if (addressableSettings == null)
                Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                                $"a settings object.");
        }

        static void setAddressableProfile(string profile)
        {
            string profileId = addressableSettings.profileSettings.GetProfileId(profile);
            if (string.IsNullOrEmpty(profileId))
                Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                                    $"using current profile instead.");
            else
                addressableSettings.activeProfileId = profileId;
        }

        static void setAddressableBuilder(IDataBuilder builder)
        {
            int index = addressableSettings.DataBuilders.IndexOf((ScriptableObject)builder);

            if (index > 0)
                addressableSettings.ActivePlayerDataBuilderIndex = index;
            else
                Debug.LogWarning($"{builder} must be added to the " +
                                    $"DataBuilders list before it can be made " +
                                    $"active. Using last run builder instead.");
        }

        static bool buildAddressableContent()
        {
            AddressableAssetSettings
                .BuildPlayerContent(out AddressablesPlayerBuildResult result);
            bool success = string.IsNullOrEmpty(result.Error);

            if (!success)
            {
                Debug.LogError("Addressables build error encountered: " + result.Error);
            }
            return success;
        }

        [MenuItem("Build/Addressables/Build Addressables only")]
        public static bool BuildAddressables()
        {
            getAddressableSettingsObject(addressable_settings_asset);
            setAddressableProfile(addressable_profile_name);
            IDataBuilder builderScript
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(addressable_build_script) as IDataBuilder;

            if (builderScript == null)
            {
                Debug.LogError(addressable_build_script + " couldn't be found or isn't a build script.");
                return false;
            }

            setAddressableBuilder(builderScript);

            return buildAddressableContent();
        }

        [MenuItem("Build/Addressables/Build Addressables and Player")]
        public static void BuildAddressablesAndPlayer()
        {
            bool contentBuildSucceeded = BuildAddressables();

            if (contentBuildSucceeded)
            {
                var options = new BuildPlayerOptions();
                BuildPlayerOptions playerSettings
                    = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(options);

                BuildPipeline.BuildPlayer(playerSettings);
            }
        }
    }
}
#endif // USE_ADDRESSABLE