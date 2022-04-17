using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public static class Localization
    {
        static public HashSet<SystemLanguage> supportedLanguage = new HashSet<SystemLanguage>();

        static public SystemLanguage currLanguage;

        public enum LanguageType
        {
            Auto,
            SystemLanguage,
            String
        }


        static public void Initialize()
        {
            // Load setting (supported language, default language...)

            // Load save file

            // Switch language and load default text table, ...
        }

        static public SystemLanguage GetCurrentLanguage()
        {
            return currLanguage;
        }

        static public void SetCurrentLanguage(SystemLanguage language)
        {
            currLanguage = language;
        }
    }
}
