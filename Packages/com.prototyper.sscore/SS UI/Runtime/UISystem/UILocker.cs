using UnityEngine;
using System.Collections;

namespace SS.Core
{
	public class UILocker : MonoBehaviour {
		
		public string m_flag;
		
		public static void LockRecursive(GameObject go, string flag, bool bLock)
		{
			Lock (go, flag, bLock);

			/*
			Collider[] colliders = go.transform.GetComponentsInChildren<Collider>(true);
			foreach (Collider collider in colliders)
			{
				Lock (collider.gameObject, flag, bLock);
			}
			*/
		}
		
		// Add / Remove UILock component
		public static void Lock(GameObject go, string flag, bool bLock)
		{
			if (bLock)
			{
				bool bAlreadyLockBy = IsLockBy(go, flag);
				if (!bAlreadyLockBy)
				{
					// Add Component
					UILocker uilock = go.AddComponent<UILocker>();
					uilock.m_flag = flag;				
				}
			}
			else
			{
				UILocker[] uilocks = go.GetComponents<UILocker>();
				foreach (UILocker uilock in uilocks)
				{
					if (uilock.m_flag == flag)
					{
						// Remove component
						Destroy(uilock);
					}
				}
			}		
		}
		
		public static bool IsLock(GameObject go)
		{
			UILocker[] uilocks = go.GetComponents<UILocker>();
			return uilocks.Length > 0;
		}
		
		public static bool IsLockBy(GameObject go, string flag)
		{
			UILocker[] uilocks = go.GetComponents<UILocker>();
			foreach (UILocker uilock in uilocks)
			{
				if (uilock != null && uilock.m_flag == flag && uilock.enabled)
					return true;
			}
			return false;
		}
		
		void OnLock(bool bLock)
		{
			/*
			if (collider)
				collider.enabled = !bLock;
				*/
		}
	
		// Use this for initialization
		void OnEnable ()
		{
			// Lock
			OnLock (true);
		}

		void OnDisable()
		{
			UILocker[] uilocks = gameObject.GetComponents<UILocker>();
			foreach (UILocker uilock in uilocks)
			{
				// detect if there is any other locker
				if (uilock != null && uilock != this && uilock.enabled)
				{
					// then don't unlock
					return;
				}
			}
			// If there is no UILock comopnents then "Unlock"
			OnLock (false);
		}
	}
}