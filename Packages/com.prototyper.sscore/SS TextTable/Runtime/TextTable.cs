using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SS
{
    public static class TextTable
    {
        private static HashSet<ITextTableLanguage> textHandlers = new HashSet<ITextTableLanguage>();


        public static void Register(ITextTableLanguage textLanguageHandler)
        {
            textHandlers.Add(textLanguageHandler);
        }
        public static void Unregister(ITextTableLanguage textLanguageHandler)
        {
            textHandlers.Remove(textLanguageHandler);
        }

        private class LocalizationHandler: ITextLanguage
        {
            public void OnTextLanguageChanged(SystemLanguage language, SystemLanguage origLanguage)
            {
                TextTable.OnTextLanguageChanged(language, origLanguage);
            }
        }
        private static Dictionary<string, TextPackage> packageMap = new Dictionary<string, TextPackage>();

        private static bool isInited { get; set; } = false;
        private static bool isInitializing { get; set; } = false;

        private static LocalizationHandler localizationHandler = new LocalizationHandler();

        private static void OnTextLanguageChanged(SystemLanguage language, SystemLanguage origLanguage)
        {
            // Collect package name
            var list = new List<string>();
            foreach (var packageName in packageMap.Keys)
            {
                list.Add(packageName);
            }

            foreach (var packageName in list)
            {
                // Unload original language TextPackage and record package name
                Unload(packageName);
            }
            var count = list.Count;
            foreach (var packageName in list)
            {
                // Load packages with new language
                Load(packageName, (oh) =>
                {
                    count--;
                    if (count <= 0)
                    {
                        // Load complete
                        foreach (var th in textHandlers)
                        {
                            th.OnTextTableLanguageChanged(language, origLanguage);
                        }
                    }
                });
            }
        }

        public static void Initialize()
        {
            if (isInited || isInitializing)
                return;

            isInitializing = true;
            Localization.Register(localizationHandler);

            var defaultPackages = TextTableSettings.Instance.defaultPackages;
            var count = defaultPackages.Count;
            foreach (var package in defaultPackages)
            {
                Load(package, (oh) =>
                {
                    count--;
                    if (count <= 0)
                    {
                        // Load complete
                        foreach (var th in textHandlers)
                        {
                            th.OnTextTableLanguageChanged(Localization.GetTextLanguage(), Localization.GetTextLanguage());
                        }
                    }
                });
            }

            isInitializing = false;
            isInited = true;
        }

        public static void Load(string name, System.Action<ResourceSystem.OperationHandle> onComplete = null)
        {
            Initialize();

            if (packageMap.ContainsKey(name))
            {
                Debug.LogError($"TextPackage [{name}] has already be loaded.");
                return;
            }    
            var language = Localization.GetTextLanguage();
            var resourceName = $"{name}_{language.ToString()}";

            ResourceSystem.OperationHandle oh = null;
            oh = ResourceSystem.Load<TextPackage>(resourceName, (tp) =>
            {
                if (tp == null)
                {
                    Debug.LogError($"Load ({resourceName}) failed.");
                }
                else
                {
                    Debug.Log($"Load ({resourceName}) completed.");
                    packageMap.Add(name, tp);
                }
                onComplete?.Invoke(oh);
            });
        }

        public static void Unload(string name)
        {
            Initialize();

            if (!packageMap.ContainsKey(name))
            {
                Debug.LogError($"TextPackage [{name}] never be loaded.");
                return;
            }
            var language = Localization.GetTextLanguage();
            var filePath = $"{name}_{language.ToString()}";
            ResourceSystem.Unload(filePath);
            packageMap.Remove(name);
        }

        public static bool TryGetText(string textID, out string result)
        {
            Initialize();

            result = null;
            if (string.IsNullOrEmpty(textID))
                return false;
            foreach (var kvp in packageMap)
            {
                if (kvp.Value.data.ContainsKey(textID))
                {
                    result = kvp.Value.data[textID].text;
                    return true;
                }
            }
            return false;
        }
    }
}