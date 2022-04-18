using UnityEngine;
using System.Collections.Generic;
using System.IO;

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
        const string mainFolder = "SSTest";
        const string subFolderName = "AAResources";
		const string settingsParentPath = "Assets" + "/" + mainFolder;
		const string settingsPath = mainFolder + "/" + subFolderName;
		const string settingsAssetExtension = ".asset";
		
		private static T instance;


        static void OnLoadComplete(T loadedInst)
        {
            if (loadedInst != null)
            {
                if (instance != null)
                {
                    instance = loadedInst;
                }
            }
            if (instance == null)
            {
                // If not found, autocreate the asset object.
                instance = ResourceSystem.CreateInstance<T>();

#if UNITY_EDITOR
                string properPath = Path.Combine(DirectoryUtility.ProjectPath, "Assets", mainFolder, subFolderName);
                DirectoryUtility.CheckParentFolderRecursive(new DirectoryInfo(properPath));

                string fullPathWithoutExt = Path.Combine("Assets", mainFolder, subFolderName, settingsAssetName);

                ResourceSystem.CreateAsset(instance, fullPathWithoutExt, settingsAssetExtension);
#endif
            }
        }


        public static T Instance
		{
			get
			{
				if (instance == null)
				{
#if UNITY_EDITOR
                    string fullPath = Path.Combine(Path.Combine("Assets", settingsPath),
                                                   settingsAssetName + settingsAssetExtension
                                                   );
                    instance = AssetDatabase.LoadAssetAtPath<T>(fullPath);

                    // Create if doesn't exist
                    if (instance == null)
                    {
                        OnLoadComplete(null);
                    }
#else
                    ResourceSystem.Load<T>(settingsAssetName, OnLoadComplete);
#endif
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