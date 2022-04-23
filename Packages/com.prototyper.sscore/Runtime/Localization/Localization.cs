using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public static class Localization
    {
        private static HashSet<ITextLanguage> textHandlers = new HashSet<ITextLanguage>();
        private static HashSet<IAudioLanguage> audioHandlers = new HashSet<IAudioLanguage>();

        public static void Register(ITextLanguage textLanguageHandler)
        {
            textHandlers.Add(textLanguageHandler);
        }
        public static void Unregister(ITextLanguage textLanguageHandler)
        {
            textHandlers.Remove(textLanguageHandler);
        }
        public static void Register(IAudioLanguage audioLanguageHandler)
        {
            audioHandlers.Add(audioLanguageHandler);
        }
        public static void Unregister(IAudioLanguage audioLanguageHandler)
        {
            audioHandlers.Remove(audioLanguageHandler);
        }

        public class LanguageSetup
        {
            private HashSet<SystemLanguage> supportedLanguage = new HashSet<SystemLanguage>();
            public bool IsAutoDetect { get; set; } = true;
            private SystemLanguage currLanguage;
            public SystemLanguage CurrentLanguage
            {
                get {
                    if (IsAutoDetect)
                    {
                        CurrentLanguage = Application.systemLanguage;
                    }
                    return currLanguage;
                }
                set {
                    if (IsAutoDetect)
                    {
                        IsAutoDetect = false;
                    }
                    currLanguage = value;
                }
            }

            public LanguageSetup(SystemLanguage defaultLanguage)
            {
                IsAutoDetect = false;
                currLanguage = SystemLanguage.English;
            }

            public LanguageSetup()
            {
                IsAutoDetect = true;
            }
        }

        private static bool IsInited { get; set; } = false;
        private static LanguageSetup textLanguage = new LanguageSetup();
        private static LanguageSetup audioLanguage = new LanguageSetup();


        public enum LanguageType
        {
            Auto,
            SystemLanguage,
            String
        }


        static public void Initialize()
        {
            if (IsInited)
                return;

            // Load setting (supported language, default language...)

            // Load save file

            // Switch language and load default text table, ...

            IsInited = true;
        }

        static public SystemLanguage GetTextLanguage()
        {
            return textLanguage.CurrentLanguage;
        }

        static public void SetTextLanguage(LanguageType type, SystemLanguage language = SystemLanguage.English)
        {
            var origLanguage = textLanguage.CurrentLanguage;
            if (type == LanguageType.Auto)
            {
                if (textLanguage.IsAutoDetect == false)
                {
                    textLanguage.IsAutoDetect = true;
                }
            }
            else if (type == LanguageType.SystemLanguage)
            {
                if (textLanguage.CurrentLanguage != language)
                {
                    textLanguage.CurrentLanguage = language;
                }
            }
            if (origLanguage != textLanguage.CurrentLanguage)
            {
                foreach (var i in textHandlers)
                {
                    i.OnTextLanguageChanged(textLanguage.CurrentLanguage, origLanguage);
                }
            }
        }

        static public SystemLanguage GetAudioLanguage()
        {
            return audioLanguage.CurrentLanguage;
        }

        static public void SetAudioLanguage(LanguageType type, SystemLanguage language = SystemLanguage.English)
        {
            var origLanguage = audioLanguage.CurrentLanguage;
            if (type == LanguageType.Auto)
            {
                if (audioLanguage.IsAutoDetect == false)
                {
                    audioLanguage.IsAutoDetect = true;
                }
            }
            else if (type == LanguageType.SystemLanguage)
            {
                if (audioLanguage.CurrentLanguage != language)
                {
                    audioLanguage.CurrentLanguage = language;
                }
            }
            if (origLanguage != audioLanguage.CurrentLanguage)
            {
                foreach (var i in audioHandlers)
                {
                    i.OnAudioLanguageChanged(audioLanguage.CurrentLanguage, origLanguage);
                }
            }
        }
    }
}
