using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SS.Core
{
    public class LocalizationSettings : ProjectSettingsObjectSync<LocalizationSettings>
    {
        [SerializeField] private List<SystemLanguage> textSupportedLanguage = new List<SystemLanguage>();
        [SerializeField] private List<SystemLanguage> audioSupportedLanguage = new List<SystemLanguage>();

        protected override void OnCreate()
        {
            textSupportedLanguage.Add(SystemLanguage.English);
            audioSupportedLanguage.Add(SystemLanguage.English);
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
