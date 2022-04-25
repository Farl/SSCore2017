using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SS
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

		private static string settingsPath
        {
            get { return DataAssetSettings.Instance.settingPath; }
        }
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

        public static T Instance
		{
			get
			{
				if (instance == null)
				{
                    if (!Application.isPlaying)
                    {
#if UNITY_EDITOR
                        // Load from AssetDatabase
                        instance = AssetDatabase.LoadAssetAtPath<T>(fullPath);

                        // Create if load failed
                        if (instance == null)
                        {
                            instance = ScriptableObject.CreateInstance<T>();

                            // Create asset with addressable group
                            string properPath = Path.Combine(DirectoryUtility.ProjectPath, settingsPath);
                            DirectoryUtility.CheckAndCreateDirectory(properPath, false);
                            ResourceSystem.CreateAsset(instance, fullPath);
                        }
#endif
                    }
                    else
                    {
                        // Load from ResourceSystem
                        if (!IsLoading)
                        {
                            IsLoading = true;
                            var oh = ResourceSystem.Load<T>(fullPath, (inst) =>
                            {
                                if (inst == null)
                                {
                                    Debug.LogError($"Load {typeof(T).ToString()} failed");
                                }
                                instance = inst;
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
			Selection.activeObject = Instance;
		}

		public void DirtyEditor()
		{
			EditorUtility.SetDirty(this);
		}
#endif

	}
	
}