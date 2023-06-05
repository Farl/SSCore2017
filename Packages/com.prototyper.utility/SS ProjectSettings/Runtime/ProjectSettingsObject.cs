using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System;
using SS;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace SS
{
    public abstract partial class ProjectSettingsObject<T> : ProjectSettingsBase where T : ProjectSettingsObject<T>
    {
        #region Abstract
        protected abstract void OnCreate();

        #endregion

        #region Enums & Classes
        
        #endregion

        #region Static

        private const string k_SettingsPath = "Assets/SS Settings/Resources";

        // This is the path that Addressables Groups tool automatically moves the asset to.
        private static string asyncPath => $"{k_SettingsPath}_moved/{resourceName}.asset";

        private static string syncPath => $"{k_SettingsPath}/{resourceName}.asset";

        protected static string path
        {
            get
            {
#if USE_ADDRESSABLES
                return asyncPath;
#else
                return syncPath;
#endif
            }
        }

        protected static string altPath
        {
            get
            {
#if USE_ADDRESSABLES
                return syncPath;
#else
                return asyncPath;
#endif
            }
        }

        protected static string resourceName => $"{typeof(T).ToString()}";
        protected static T _instance;

        public static bool IsExist => (_instance != null);

        public static ResourceSystem.OperationHandle LoadAsync(Action<T> onCompleted = null)
        {
            Action<T> onLoaded = (x) =>
            {
                if (x != null)
                {
                    _instance = x;
                }
            };
            return ResourceSystem.Load<T>(resourceName: resourceName, onComplete: onLoaded + onCompleted);
        }

        #if UNITY_EDITOR

        private static T _editorInstance;

        public static T editorInstance
        {
            get
            {
                if (_editorInstance == null)
                {
                    _editorInstance = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (_editorInstance == null)
                    {
                        _editorInstance = AssetDatabase.LoadAssetAtPath<T>(altPath);
                        if (_editorInstance != null)
                        {
                            Debug.LogError($"Can't find {path} but found {altPath}. Please move it to the correct path or it won't be loaded in runtime.");
                        }
                    }
                }
                return _editorInstance;
            }
        }
        #endif
        

        public static T Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                // Get
                if (!Application.isPlaying)
                {
#if UNITY_EDITOR
                    _instance = editorInstance;
#endif
                }
                else
                {
#if USE_ADDRESSABLES
                    Debug.LogError($"Do not call Instance before LoadAsync when using Addressables. Automatically use default value.");
#else
                    _instance = Resources.Load<T>(resourceName);
#endif
                }

                if (_instance == null)
                {
                    // Create
                    _instance = ScriptableObject.CreateInstance<T>();
                    _instance.OnCreate();

                    // Asset
                    if (_instance != null)
                    {
#if UNITY_EDITOR
                        DirectoryUtility.CheckAndCreateDirectory(path, true);
                        AssetDatabase.CreateAsset(_instance, path);
                        AssetDatabase.SaveAssets();
#else
#endif
                    }
                }
                return _instance;
            }
        }

#if UNITY_EDITOR
        protected static SettingsProvider RegisterSettingsProvider(string label = null, HashSet<string> keywords = null)
        {
            return ProjectSettingsUI.Register<T>(label, keywords);
        }
#endif
#endregion

    }
}
