#if !USE_UNITY_PLAYERPREFS
//#define USE_ALT_PLAYERPREFS
#endif

namespace SS
{
    public static class PlayerPrefs
    {
        public static void DeleteAll()
        {
#if USE_ALT_PLAYERPREFS
            ZPlayerPrefs.DeleteAll();
#else
            UnityEngine.PlayerPrefs.DeleteAll();
#endif
        }

        public static void DeleteKey(string key)
        {
#if USE_ALT_PLAYERPREFS
            ZPlayerPrefs.DeleteKey(key);
#else
            UnityEngine.PlayerPrefs.DeleteKey(key);
#endif
        }

        public static float GetFloat(string key)
        {
#if USE_ALT_PLAYERPREFS
            return ZPlayerPrefs.GetFloat(key);
#else
            return UnityEngine.PlayerPrefs.GetFloat(key);
#endif
        }

        public static float GetFloat(string key, float defaultValue, bool isDecrypt = true)
        {
#if USE_ALT_PLAYERPREFS
            return ZPlayerPrefs.GetFloat(key, defaultValue, isDecrypt);
#else
            return UnityEngine.PlayerPrefs.GetFloat(key, defaultValue);
#endif
        }

        public static int GetInt(string key)
        {
#if USE_ALT_PLAYERPREFS
            return ZPlayerPrefs.GetInt(key);
#else
            return UnityEngine.PlayerPrefs.GetInt(key);
#endif
        }

        public static int GetInt(string key, int defaultValue, bool isDecrypt = true)
        {
#if USE_ALT_PLAYERPREFS
            return ZPlayerPrefs.GetInt(key, defaultValue, isDecrypt);
#else
            return UnityEngine.PlayerPrefs.GetInt(key, defaultValue);
#endif
        }

        public static string GetString(string key)
        {
#if USE_ALT_PLAYERPREFS
            return ZPlayerPrefs.GetString(key);
#else
            return UnityEngine.PlayerPrefs.GetString(key);
#endif
        }

        public static string GetRowString(string key)
        {
#if USE_ALT_PLAYERPREFS
            return ZPlayerPrefs.GetString(key);
#else
            return UnityEngine.PlayerPrefs.GetString(key);
#endif
        }

        public static string GetString(string key, string defaultValue)
        {
#if USE_ALT_PLAYERPREFS
            return ZPlayerPrefs.GetString(key, defaultValue);
#else
            return UnityEngine.PlayerPrefs.GetString(key, defaultValue);
#endif
        }

        public static string GetRowString(string key, string defaultValue)
        {
#if USE_ALT_PLAYERPREFS
            return ZPlayerPrefs.GetString(key, defaultValue);
#else
            return UnityEngine.PlayerPrefs.GetString(key, defaultValue);
#endif
        }

        public static bool HasKey(string key)
        {
#if USE_ALT_PLAYERPREFS
            return ZPlayerPrefs.HasKey(key);
#else
            return UnityEngine.PlayerPrefs.HasKey(key);
#endif
        }

        public static void Save()
        {
#if USE_ALT_PLAYERPREFS
            ZPlayerPrefs.Save();
#else
            UnityEngine.PlayerPrefs.Save();
#endif
        }

        public static void SetFloat(string key, float value)
        {
#if USE_ALT_PLAYERPREFS
            ZPlayerPrefs.SetFloat(key, value);
#else
            UnityEngine.PlayerPrefs.SetFloat(key, value);
#endif
        }

        public static void SetInt(string key, int value)
        {
#if USE_ALT_PLAYERPREFS
            ZPlayerPrefs.SetInt(key, value);
#else
            UnityEngine.PlayerPrefs.SetInt(key, value);
#endif
        }

        public static void SetString(string key, string value)
        {
#if USE_ALT_PLAYERPREFS
            ZPlayerPrefs.SetString(key, value);
#else
            UnityEngine.PlayerPrefs.SetString(key, value);
#endif
        }

        public static void Initialize(string newPassword, string newSalt)
        {
#if USE_ALT_PLAYERPREFS
            ZPlayerPrefs.Initialize(newPassword, newSalt);
#else
#endif
        }

        public static void UseSecure(bool useSecure)
        {
#if USE_ALT_PLAYERPREFS
            ZPlayerPrefs.useSecure = useSecure;
#else
#endif
        }
    }
}
