using UnityEngine;
using System.Collections;
using SS;

public class EventToSpawner : EventListener {

	public string m_spawnResource;
	public GameObject m_spawnObj;

	GameObject spawnObj;

	protected override void OnEvent(bool paramBool)
	{
		if (spawnObj != null)
			return;

		if (m_spawnResource != null && m_spawnResource != "")
			spawnObj = (GameObject)GameObject.Instantiate(Resources.Load (m_spawnResource), transform.position, transform.rotation);
        if (m_spawnObj != null)
            spawnObj = Spawn(m_spawnObj, null, transform);
	}

    public static GameObject Spawn(GameObject spawnObj, Transform parent = null, Transform transform = null)
    {
        GameObject spawnedObj = null;
        spawnedObj = (GameObject)GameObject.Instantiate(spawnObj, transform.position, transform.rotation);
        if (parent)
        {
            spawnedObj.transform.SetParent(parent, true);
        }
        if (transform)
        {
            spawnedObj.transform.localRotation = transform.localRotation;
            spawnedObj.transform.localScale = transform.localScale;
        }
        return spawnedObj;
    }
}
