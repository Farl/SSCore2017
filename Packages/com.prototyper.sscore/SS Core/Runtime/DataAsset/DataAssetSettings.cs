using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SS.Core
{
    public class DataAssetSettings : ProjectSettingsObjectSync<DataAssetSettings>
    {

#pragma warning disable 414
        [SerializeField] private string settingPathAddressable = "Assets/SS Settings/AAResources";
        [SerializeField] private string setttingPathResources = "Assets/SS Settings/Resources";
#pragma warning restore 414

        public string settingPath
        {
            get
            {
#if USE_ADDRESSABLES
                return settingPathAddressable;
#else
                return setttingPathResources;
#endif
            }
        }

        protected override void OnCreate()
        {
        }

#if UNITY_EDITOR
        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return RegisterSettingsProvider();
        }
#endif
    }
}
