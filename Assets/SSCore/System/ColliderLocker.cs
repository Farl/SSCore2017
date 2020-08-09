using UnityEngine;
using System.Collections;

public class ColliderLocker : MonoBehaviour {
	StringFlag flags = new StringFlag();

	static ColliderLocker GetColliderLocker(GameObject go)
	{
		if (go == null)
			return null;
		ColliderLocker cl = go.GetComponent<ColliderLocker>();
		if (cl == null)
			cl = go.AddComponent<ColliderLocker>();
		return cl;
	}
	
	public static void Lock(GameObject go, string flag, bool recursive = true)
	{
		if (go == null)
			return;
		Collider[] colliders = go.GetComponentsInChildren<Collider>(true);
		foreach (Collider coll in colliders)
		{
			GetColliderLocker(coll.gameObject).Lock(flag);
		}
	}
	
	public static void Unlock(GameObject go, string flag, bool recursive = true)
	{
		if (go == null)
			return;
		Collider[] colliders = go.GetComponentsInChildren<Collider>(true);
		foreach (Collider coll in colliders)
		{
			GetColliderLocker(coll.gameObject).Unlock(flag);
		}
	}

	public void Lock(string flag)
	{
		flags.AddFlag(flag);
		if (GetComponent<Collider>())
		{
			GetComponent<Collider>().enabled = false;
		}
	}

	public void Unlock(string flag)
	{
		flags.RemoveFlag(flag);
		if (flags.IsEmpty())
		{
			GetComponent<Collider>().enabled = true;
		}
	}

	[ContextMenu("Dump")]
	void Dump()
	{
		flags.Dump();
	}
}
