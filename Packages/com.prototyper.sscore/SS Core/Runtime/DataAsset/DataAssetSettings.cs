using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SS.Core
{
    public class DataAssetSettings : ProjectSettingsObject<DataAssetSettings>
    {
        public string settingPath = "Assets/SS Settings/AAResources";

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
