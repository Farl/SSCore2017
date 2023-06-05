using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SS.Core
{
	#if UNITY_EDITOR
	//[InitializeOnLoad]
	#endif
	public class DataAsset<T> : ScriptableObject
		where T : ScriptableObject
	{
		static string settingsAssetName {
			get
			{
				string className = typeof(T).ToString();
				return className.Substring(className.LastIndexOf(".") + 1) + settingsAssetExtension;
			}
		}

		private static string settingsPath => DataAssetSettings.Instance.settingPath;
		const string settingsAssetExtension = ".asset";
        private static string fullPath => $"{settingsPath}/{settingsAssetName}";    // Unity use '/'

        public static bool IsLoading
        {
            get
            {
                return isLoading.Contains(typeof(T));
            }
            private set
            {
                if (value)
                {
                    if (!isLoading.Contains(typeof(T)))
                        isLoading.Add(typeof(T));
                }
                else
                {
                    isLoading.Remove(typeof(T));
                }
            }
        }
        private static HashSet<Type> isLoading = new HashSet<Type>();
        private static T instance = null;

        public static void GetInstance(Action<T> onComplete)
        {
            if (instance != null)
            {
                onComplete?.Invoke(instance);
            }
            else
            {
                // Load from ResourceSystem
                var oh = ResourceSystem.Load<T>(fullPath, (inst) =>
                {
                    if (inst == null)
                    {
                        Debug.LogWarning($"Load {typeof(T).ToString()} failed");
                        inst = ScriptableObject.CreateInstance<T>();
                    }
                    instance = inst;
                    onComplete?.Invoke(instance);
                });
            }
        }

#if UNITY_EDITOR
        private static T _editorInstance;

        public static T editorInstance
        {
            get
            {
                // In Editor always load immediately
                if (_editorInstance == null)
                {
                    _editorInstance = AssetDatabase.LoadAssetAtPath<T>(fullPath);
                    if (_editorInstance == null)
                    {
                        _editorInstance = ScriptableObject.CreateInstance<T>();
                        // Create asset with addressable group
                        string properPath = Path.Combine(DirectoryUtility.ProjectPath, settingsPath);
                        DirectoryUtility.CheckAndCreateDirectory(properPath, false);
                        ResourceSystem.CreateAsset(_editorInstance, fullPath);
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
				if (instance == null)
				{
                    if (!Application.isPlaying)
                    {
#if UNITY_EDITOR
                        instance = editorInstance;
#endif
                    }
                    else
                    {
                        // Load from ResourceSystem
                        if (!IsLoading)
                        {
                            IsLoading = true;
                            GetInstance((inst) =>
                            {
                                IsLoading = false;
                            });
                        }
                    }
                }
                return instance;
			}
		}
		
#if UNITY_EDITOR
		public static void EditNow()
		{
			Selection.activeObject = editorInstance;
		}

		public void DirtyEditor()
		{
			EditorUtility.SetDirty(this);
		}
#endif

	}
	
}