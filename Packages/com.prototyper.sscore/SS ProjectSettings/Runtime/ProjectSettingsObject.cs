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
    public abstract partial class ProjectSettingsObject<T> : ScriptableObject where T : ProjectSettingsObject<T>
    {
        #region Abstract
        protected abstract void OnCreate();
        #endregion

        #region Static
        protected static string path => $"Assets/SS Settings/Resources/{resourceName}.asset";

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
                    _instance = AssetDatabase.LoadAssetAtPath<T>(path);
#endif
                }
                else
                {
                    _instance = Resources.Load<T>(resourceName);
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
                        DirectoryUtility.CheckAndCreateDirectory(path);
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
