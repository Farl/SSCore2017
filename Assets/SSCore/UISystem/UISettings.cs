using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SS
{
	public class UISettings : DataAsset<UISettings>, ISerializationCallbackReceiver
	{
		[System.Serializable]
		public class UIPageData
		{
			public string name;
			// Prefab
			public GameObject gameObject;
			// Blocker
			public string[] blockUI;
			
		}
		
		public class UIPageInstance
		{
			public List<GameObject> goList;
		}
		
		[System.Serializable]
		public class UICurveData
		{
			public string name;
			public AnimationCurve curve;
		}
		
		public List<UIPageData> list = new List<UIPageData>();
		public List<UICurveData> curveList = new List<UICurveData>();
		
		protected Dictionary<string, UIPageData> dictionary = new Dictionary<string, UIPageData>();
		
		public AnimationCurve GetCurve(string curveID)
		{
			foreach (UICurveData data in curveList)
			{
				if (data.name == curveID)
				{
					return data.curve;
				}
			}
			return null;
		}
		
		public void OnAfterDeserialize ()
		{
			// list to dictionary
			
			dictionary.Clear();
			
			foreach (UIPageData entry in list)
			{
				while (dictionary.ContainsKey(entry.name))
				{
					entry.name = entry.name.Insert(0, "_");
				}
				dictionary.Add(entry.name, entry);
			}
		}
		
		public void OnBeforeSerialize ()
		{
			// dictionary to list
			
			list.Clear();
			
			foreach (KeyValuePair<string, UIPageData> kvp in dictionary)
			{
				UIPageData v = kvp.Value as UIPageData;
				list.Add(v);
			}
		}

		public static UIPageData GetPageData(string id)
		{
			UIPageData data = null;
			UISettings.Instance.dictionary.TryGetValue(id, out data);
			return data;
		}
		
		#if UNITY_EDITOR
		[MenuItem("SS/Edit UI Settings")]
		static void Edit()
		{
			EditNow();
		}
		#endif
	}
	
}