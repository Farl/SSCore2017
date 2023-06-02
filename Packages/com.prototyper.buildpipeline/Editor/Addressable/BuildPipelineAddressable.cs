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

    public static class BuildPipelineSystem
    {
        public static string build_script
            = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
        public static string settings_asset
            = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        public static string profile_name = "Default";
        private static AddressableAssetSettings settings;

        static void getSettingsObject(string settingsAsset)
        {
            // This step is optional, you can also use the default settings:
            //settings = AddressableAssetSettingsDefaultObject.Settings;

            settings
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                    as AddressableAssetSettings;

            if (settings == null)
                Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                                $"a settings object.");
        }

        static void setProfile(string profile)
        {
            string profileId = settings.profileSettings.GetProfileId(profile);
            if (String.IsNullOrEmpty(profileId))
                Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                                    $"using current profile instead.");
            else
                settings.activeProfileId = profileId;
        }

        static void setBuilder(IDataBuilder builder)
        {
            int index = settings.DataBuilders.IndexOf((ScriptableObject)builder);

            if (index > 0)
                settings.ActivePlayerDataBuilderIndex = index;
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
            getSettingsObject(settings_asset);
            setProfile(profile_name);
            IDataBuilder builderScript
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(build_script) as IDataBuilder;

            if (builderScript == null)
            {
                Debug.LogError(build_script + " couldn't be found or isn't a build script.");
                return false;
            }

            setBuilder(builderScript);

            return buildAddressableContent();
        }

        [MenuItem("Build/Build Addressables and Player")]
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