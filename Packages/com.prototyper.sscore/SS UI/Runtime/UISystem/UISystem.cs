using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SS
{
		
	public static class UISystem
	{
		
		public class UIPanelInfo
		{
			public string name;
			public List<GameObject> panelList = new List<GameObject>();
			public UIPanelInfo() {}
			
			public void Register(GameObject go)
			{
				Clean ();
				if (go != null)
				{
					panelList.Add (go);
				}
			}

			public void Unregister(GameObject go)
			{
				Clean ();
				if (go != null)
				{
					panelList.Remove (go);
				}
			}

			public GameObject[] GetObjects()
			{
				Clean ();
				return panelList.ToArray();
			}
			
			// Clear null objects
			public void Clean()
			{
				List<GameObject> removeList = new List<GameObject>();
				foreach (GameObject go in panelList)
				{
					if (go == null)
					{
						removeList.Add(go);
					}
				}
				foreach (GameObject removeObj in removeList)
				{
					panelList.Remove(removeObj);
				}
			}
		}
		
		private static Hashtable panelMap = new Hashtable();
		private static Dictionary<string, UIBase> uiBaseMap = new Dictionary<string, UIBase>();
		
		public static UIPanelInfo GetOrAddUIPanelInfo(string name)
		{
			if (panelMap.ContainsKey(name))
			{
			}
			else
			{
				panelMap[name] = new UIPanelInfo();
			}
			return panelMap[name] as UIPanelInfo;
		}

		public static UIPanelInfo GetUIPanelInfo(string name)
		{
			if (panelMap.ContainsKey(name))
				return panelMap[name] as UIPanelInfo;
			else
				return null;
		}
	
		public static void Register(UIBase uiBase)
        {
			if (uiBase == null)
				return;
			if (!uiBaseMap.ContainsKey(uiBase.UIType))
            {
				uiBaseMap.Add(uiBase.UIType, uiBase);
            }
			else
            {
				Debug.LogError($"Duplicate UIBase {uiBase.UIType}");
            }
			RegisterObject(uiBase.gameObject, uiBase.UIType);
        }

		public static void Unregister(UIBase uiBase)
		{
			if (uiBase == null)
				return;
			if (uiBaseMap.ContainsKey(uiBase.UIType))
            {
				uiBaseMap.Remove(uiBase.UIType);
			}
			else
			{
				Debug.LogError($"There is no UIBase {uiBase.UIType}");
			}
			UnregisterObject(uiBase.gameObject, uiBase.UIType);
		}

		public static T Get<T>(string uiType) where T : UIBase
        {
			if (uiBaseMap.TryGetValue(uiType, out var uiBase))
            {
				return uiBase as T;
            }
			return null;
        }

		public static void RegisterObject(GameObject panelObj, string UIID)
		{
			UIPanelInfo info = GetOrAddUIPanelInfo(UIID);
			if (info != null)
			{
				info.Register(panelObj);
				
				// Detect is blocked by other UI
				if (UISystem.IsBlock(UIID))
				{
					UIBlockSystemLock(panelObj, true);
				}
			}
		}

		public static void UnregisterObject (GameObject panelObj, string UIID)
		{
			UIPanelInfo info = GetUIPanelInfo(UIID);
			if (info != null)
			{
				info.Unregister (panelObj);

				if (UISystem.IsBlock(UIID))
				{
					UIBlockSystemLock(panelObj, false);
				}
			}
		}

		public static void UIBlockSystemLock(GameObject go, bool isLock = true)
		{
			if (go != null)
			{
				UILocker.LockRecursive(go, "UISystem Block System", isLock);
			}
		}
		
		public static void UIBlockSystemLock(GameObject[] goList, bool isLock = true)
		{
			if (goList != null)
			{
				foreach (GameObject go in goList)
				{
					UIBlockSystemLock(go, isLock);
				}
			}
		}
		
		public static GameObject[] GetObjects(string name)
		{
			UIPanelInfo info = GetUIPanelInfo(name);
			if (info != null)
			{
				return info.GetObjects();
			}
			return null;
		}
		
		public static GameObject GetObject(string name)
		{
			GameObject[] list = GetObjects(name);
			if (list != null && list.Length >= 1)
			{
				return list[0];
			}
			return null;
		}
		
		// Block system (auto lock)
		static Hashtable blockMap = new Hashtable();
		
		public class UIBlockInfo
		{
			public List<UIBlocker> blockFrom = new List<UIBlocker>();
			
			public void Block(UIBlocker bFrom)
			{
				Clean();
				blockFrom.Add (bFrom);			
			}
			public void Clean()
			{
				// To Check!!
				blockFrom.RemoveAll(item => item == null);
				
				//
				List <UIBlocker> removeList = new List<UIBlocker>();
				foreach (UIBlocker b in blockFrom)
				{
					if (b == null)
					{
						removeList.Add(b);
					}
				}
				foreach (UIBlocker bRemove in removeList)
				{
					blockFrom.Remove(bRemove);
				}
			}
			public bool IsEmpty()
			{
				Clean();
				return blockFrom.Count == 0;
			}
		}
		
		public static UIBlockInfo GetBlockInfo(string name)
		{
			if (blockMap.ContainsKey(name))
			{
				return blockMap[name] as UIBlockInfo;
			}
			return null;
		}
		
		public static UIBlockInfo GetOrAddBlockInfo(string name)
		{
			if (!blockMap.ContainsKey(name))
			{
				blockMap[name] = new UIBlockInfo();
			}
			return blockMap[name] as UIBlockInfo;
		}
		
		public static bool IsBlock(string UIID)
		{
			UIBlockInfo bInfo = GetBlockInfo(UIID);
			if (bInfo == null)
			{
				return false;
			}
			else
			{
				return !bInfo.IsEmpty();
			}
		}
		
		public static void Block(UIBlocker block)
		{
			if (block == null || block.UIID == null || block.blockUIs == null)
				return;
			
			// Block each UI in the list
			foreach (string target in block.blockUIs)
			{
				Block(target, block);
			}
		}
		
		public static void Block(string blockTo, UIBlocker blockFrom)
		{
			if (blockTo == null)
				return;
			
			bool isBlocked = IsBlock(blockTo);
			
			UIBlockInfo bInfo = GetOrAddBlockInfo(blockTo);
			if (bInfo != null)
			{
				bInfo.Block(blockFrom);
				if (!isBlocked)
				{
					UIBlockSystemLock(GetObjects(blockTo));
				}
			}
		}
		
		public static void Unblock(UIBlocker block)
		{
			if (block == null || block.UIID == null || block.blockUIs == null)
				return;
			
			// Unblock each UI in the list
			foreach (string target in block.blockUIs)
			{
				Unblock(target, block);
			}
		}
		
		public static void Unblock(string blockTo, UIBlocker blockFrom)
		{
			UIBlockInfo bInfo = GetBlockInfo(blockTo);
			if (bInfo != null)
			{
				bInfo.blockFrom.Remove (blockFrom);
				CleanBlocker();
				
				if (!IsBlock(blockTo))
				{
					UIBlockSystemLock(GetObjects(blockTo), false);
				}
			}
		}
		
		public static void CleanBlocker()
		{
			List<string> removeList = new List<string>();
			foreach (DictionaryEntry entry in blockMap)
			{
				UIBlockInfo bInfo = entry.Value as UIBlockInfo;
				if (bInfo == null || bInfo.IsEmpty())
				{
					removeList.Add(entry.Key as string);
				}
			}
			foreach (string bInfoName in removeList)
			{
				blockMap.Remove(bInfoName);
			}
		}

		// Dump
		public static void Dump(bool filter = false)
		{
			if (panelMap == null)
				return;
			
			foreach (DictionaryEntry entry in panelMap)
			{
				string key = entry.Key as string;
				UIPanelInfo panel = entry.Value as UIPanelInfo;
				if (panel != null)
				{
					if (!filter)
					{
						int count = panel.panelList.Count;
						panel.Clean();
					}
					else
					{
						panel.Clean();
					}
				}
				else
				{
					if (!filter)
                    {

                    }
				}
			}
		}

        public static GameObject Spawn(string uiID)
        {
            return null;
        }

        public static string GetPageEventID(string id)
        {
            return string.Format("UISystem.Open({0})", id);
        }
	}
}

