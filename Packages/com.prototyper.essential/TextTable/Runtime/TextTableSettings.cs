using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    [CreateAssetMenu(fileName ="TextTableSettings", menuName = "SS/Settings/Text Table Settings", order = 1)]
    public class TextTableSettings : ScriptableObject
    {
        [System.Serializable]
        public class FontSettings
        {
            public SystemLanguage language;
            public string checkFontName;
            public string fontAssetPath;
            public bool keepFontAsset = false;
        }

        public bool showDebugInfo = false;
        public string[] defaultPackages = new string[] { "TextTable - Default" };
        public List<SystemLanguage> supportedLanguage = new List<SystemLanguage>();
        public List<SystemLanguage> editorSupportedLanguage = new List<SystemLanguage>();
        public FontSettings[] fontSettings;
    }
}
