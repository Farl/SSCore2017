using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;

namespace SS
{
    public static class TextTable
    {
        #region Private
        private const string specifyLanguageKey = "SpecifyLanguage";
        private static Dictionary<string, TextTablePackage> loadedPackages = new Dictionary<string, TextTablePackage>();
        private static Dictionary<string, string> overrideDictionary = new Dictionary<string, string>();
        private static Dictionary<string, Action<string, string>> overrideNotify = new Dictionary<string, Action<string, string>>();

        private static bool _IsSpecifyLanguage;
        private static bool IsSpecifyLanguage
        {
            get; set;
        }
        private static SystemLanguage specifyLanguage;

        private static string defaultPackage = "TextTable - Default";

        private static string defaultSettings = "TextTableSettings";
        private static TextTableSettings settings;

        private static bool isInit = false;

        private static List<SystemLanguage> supportedLanguage = new List<SystemLanguage>();

        private static Dictionary<string, TMP_FontAsset> fontAssets = new Dictionary<string, TMP_FontAsset>();

        private static List<ITextTableMapping> mappings = new List<ITextTableMapping>();

        private static SystemLanguage FixSupportedLanguage(SystemLanguage input)
        {
            if (supportedLanguage.Count < 0)
                return input;

            var output = input;
            if (supportedLanguage.Contains(input))
                return input;
            return defaultLanguage;
        }

        private static void LoadSettings()
        {
            settings = Resources.Load<TextTableSettings>(defaultSettings);
            if (settings == null)
                return;

            supportedLanguage.Clear();
            supportedLanguage.AddRange(settings.supportedLanguage);
#if UNITY_EDITOR
            if (settings.editorSupportedLanguage != null)
                supportedLanguage.AddRange(settings.editorSupportedLanguage);
#endif
            // Default language
            if (supportedLanguage.Count > 0)
                defaultLanguage = supportedLanguage[0];
        }
        #endregion

        #region Public

        public static SystemLanguage defaultLanguage = SystemLanguage.English;

        public static SystemLanguage GetCurrentLanguage()
        {
            Init();

            var language = Application.systemLanguage;
            if (IsSpecifyLanguage)
                language = specifyLanguage;

            return FixSupportedLanguage(language);
        }

        public static SystemLanguage[] GetSupportLanguage()
        {
            return supportedLanguage.ToArray();
        }

        public static void Init()
        {
            if (isInit)
                return;

            // Load save data
            IsSpecifyLanguage = SS.PlayerPrefs.HasKey(specifyLanguageKey);
            var languageStr = SS.PlayerPrefs.GetString(specifyLanguageKey, @"English");
            SystemLanguage.TryParse<SystemLanguage>(languageStr, out specifyLanguage);

            // Load Settings
            LoadSettings();

            isInit = true;

            // Load default package
            LoadPackage(defaultPackage);

            // Load font assets
            LoadFontAssets(GetCurrentLanguage());

            //
            OnLanguageChanged();
        }

        public static void SetCurrentLanguage(SystemLanguage systemLanguage)
        {
            Init(); // public function need initalize

            systemLanguage = FixSupportedLanguage(systemLanguage);

            var currLanguage = GetCurrentLanguage();

            if (systemLanguage != currLanguage || IsSpecifyLanguage == false)
            {

                List<string> packageNames = new List<string>();
                foreach (var kvp in loadedPackages)
                {
                    packageNames.Add(kvp.Key);
                }

                // Unload
                foreach (var pn in packageNames)
                    UnloadPackage(pn);

                // Change Language
                IsSpecifyLanguage = true;
                specifyLanguage = systemLanguage;
                SS.PlayerPrefs.SetString(specifyLanguageKey, specifyLanguage.ToString());

                // Load
                foreach (var pn in packageNames)
                    LoadPackage(pn);

                // Load font assets
                LoadFontAssets(currLanguage, systemLanguage);

                //
                OnLanguageChanged();
            }
        }

        public static void Register(ITextTableMapping component)
        {
            mappings.Add(component);
        }

        public static void Unregister(ITextTableMapping component)
        {
            mappings.Remove(component);
        }

        public static void LoadPackage(string packageName)
        {
            var language = GetCurrentLanguage();
            var pckPath = $"{packageName}_{language}";
            var pck = Resources.Load<TextTablePackage>(pckPath);
            if (pck != null)
            {
                loadedPackages.TryAdd(packageName, pck);
            }
            else
            {
                Debug.LogError($"Fail to load {pckPath}");

                // load default language
                pck = Resources.Load<TextTablePackage>($"{packageName}_English");
                if (pck == null)
                {
                    Debug.LogError($"Fail to load {packageName}");
                }
                else
                {
                    loadedPackages.TryAdd(packageName, pck);
                }

            }
        }

        public static void UnloadPackage(string packageName)
        {
            if (loadedPackages.ContainsKey(packageName))
            {
                // TOCheck: May cause TMP issue
                //Resources.UnloadAsset(loadedPackages[packageName]);

                loadedPackages.Remove(packageName);
            }
        }

        public static string GetText(string textID)
        {
            string text = textID;
            if (!TryGetText(textID, out text))
            {
                text = textID;
            }
            return string.IsNullOrEmpty(text)? "" : text;
        }

        public static bool TryGetText(string textID, out string text)
        {
            return TryGetTextWithNotify(textID, out text, null);
        }

        public static bool TryGetTextWithNotify(string textID, out string text, Action<string, string> callback)
        {
            text = $"!{textID}!";

            List<string> collectTextIDs = null;
            if (callback != null)
            {
                collectTextIDs = new List<string>();
            }
            bool result = TryGetTextInternal(textID, out text, 0, collectTextIDs);

            if (collectTextIDs != null)
            {
                //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (var id in collectTextIDs)
                {
                    RegisterOverrideNotify(id, callback);
                    //sb.Append($"{id}, ");
                }
                //Debug.Log(sb.ToString());
            }
            return result;
        }

        public static void SetOverrideString(string textID, string text)
        {
            if (text == null)
            {
                overrideDictionary.Remove(textID);
            }
            else
            {
                if (overrideDictionary.ContainsKey(textID))
                {
                    overrideDictionary[textID] = text;
                }
                else
                {
                    overrideDictionary.Add(textID, text);
                }
                // Notify
                if (overrideNotify.ContainsKey(textID))
                {
                    overrideNotify[textID]?.Invoke(textID, text);
                }
            }
        }

        private static void OnLanguageChanged()
        {
            foreach (var map in mappings)
            {
                if (map != null)
                {
                    map.OnLanguageChanged();
                }
            }
        }

        private static void LoadFontAssets(SystemLanguage language)
        {
            // Load
            foreach (var fs in settings.fontSettings)
            {
                if (fs.language == language)
                {
                    var fontAsset = Resources.Load<TMP_FontAsset>(fs.fontAssetPath);
                    if (fontAsset != null)
                    {
                        fontAssets.Add(fs.fontAssetPath, fontAsset);
                    }
                    else
                    {
                        Debug.LogError($"{fs.fontAssetPath} does not exist.");
                    }
                }
            }
        }

        private static void LoadFontAssets(SystemLanguage from, SystemLanguage to)
        {
            if (settings == null)
                return;

            // Unlaod
            foreach (var kvp in fontAssets)
            {
                Resources.UnloadAsset(kvp.Value);
            }
            fontAssets.Clear();

            // Load
            LoadFontAssets(to);
        }

        public static bool TryGetFontAsset(string origFontAssetName, out TMPro.TMP_FontAsset fontAsset)
        {
            fontAsset = null;

            if (settings == null)
                return false;
            foreach (var fs in settings.fontSettings)
            {
                if (fs.checkFontName.Equals(origFontAssetName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (fontAssets.ContainsKey(fs.fontAssetPath))
                    {
                        fontAsset = fontAssets[fs.fontAssetPath];
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool TryGetTextInternal(string textID, out string text, int depth, List<string> collectTextIDs)
        {
            Init(); // public function need initalize

            if (collectTextIDs != null)
            {
                collectTextIDs.Add(textID);
            }

            var result = false;
            text = $"!{textID}!";
            if (!overrideDictionary.TryGetValue(textID, out text))
            {
                foreach (var kvp in loadedPackages)
                {
                    if (kvp.Value.TryGetData(textID, out var data))
                    {
                        text = data.text;
                        result = true;
                    }
                }
            }
            else
            {
                result = true;
            }

            // Parse
            if (result)
            {
                text = ParseText(text, depth + 1, collectTextIDs);
            }

            return result;
        }

        private static string ParseText(string text, int depth, List<string> collectTextIDs)
        {
            var tokens = text.Split('$');
            if (tokens == null || tokens.Length <= 1)
                return text;
            for (int i = 1; i < tokens.Length; i += 2)
            {
                var token = tokens[i];
                if (!string.IsNullOrEmpty(token))
                {
                    if (depth > 3)
                    {
                        tokens[i] = $"DEPTH_OUT_OF_RANGE({depth})";
                    }
                    else if (TryGetTextInternal(token, out var parsedStr, 0, collectTextIDs))
                    {
                        tokens[i] = parsedStr;
                    }
                    else if (TextTableMapping.TryGetVariableText(token, out var variableStr))
                    {
                        tokens[i] = variableStr;
                    }
                }
                else
                {
                    tokens[i] = "$";
                }
            }
            var result = string.Empty;
            foreach (var token in tokens)
            {
                result += token;
            }
            return result;
        }

        public static void RegisterOverrideNotify(string textID, Action<string, string> callback)
        {
            if (callback == null || string.IsNullOrEmpty(textID))
                return;
            if (overrideNotify.ContainsKey(textID))
            {
                overrideNotify[textID] -= callback;
                overrideNotify[textID] += callback;
            }
            else
            {
                var action = new Action<string, string>((k, v) => { });
                action += callback;
                overrideNotify.Add(textID, action);
            }
        }

        public static void UnregisterOverrideNotify(string textID, Action<string, string> callback)
        {
            if (callback == null || string.IsNullOrEmpty(textID))
                return;
            if (overrideNotify.ContainsKey(textID))
            {
                overrideNotify[textID] -= callback;
            }
        }

        #endregion
    }
}
