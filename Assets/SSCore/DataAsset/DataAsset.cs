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
				return className.Substring(className.LastIndexOf(".") + 1);
			}
		}
        const string mainFolder = "_SSCore";
		const string settingsParentPath = "Assets/" + mainFolder;
		const string settingsPath = mainFolder + "/Resources";
		const string settingsAssetExtension = ".asset";
		
		private static T instance;
		
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (T)Resources.Load<T>(settingsAssetName);
					if (instance == null)
					{
						// If not found, autocreate the asset object.
						instance = CreateInstance<T>();
						
						#if UNITY_EDITOR
						string properPath = Path.Combine(Application.dataPath, settingsPath);
						if (!Directory.Exists(properPath))
						{
							AssetDatabase.CreateFolder(settingsParentPath, "Resources");
						}
						
						string fullPath = Path.Combine(Path.Combine("Assets", settingsPath),
						                               settingsAssetName + settingsAssetExtension
						                               );
						AssetDatabase.CreateAsset(instance, fullPath);
						#endif
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