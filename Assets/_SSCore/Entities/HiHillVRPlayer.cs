using UnityEngine;
using System.Collections;

public class HiHillVRPlayer : MonoBehaviour {
	public GameObject targetObj;

	Vector3 origOffset;

	// Use this for initialization
	void Start ()
	{
		if (targetObj != null)
		{
			origOffset = targetObj.transform.position - transform.position;
		}
	
	}
	
	// Update is called once per frame
	void Update () {
		if (targetObj != null)
		{
			Vector3 targetDir = gameObject.transform.forward.normalized;

			float yFactor = targetDir.y;
						
			targetDir.y = 0;

			if (targetDir.sqrMagnitude <= 0.1f)
			{
				targetDir = gameObject.transform.up.normalized;
				targetDir.y = 0;

			}
			
			targetDir = targetDir.normalized;
			
			targetObj.transform.LookAt(targetObj.transform.position + targetDir);
			
			targetObj.transform.position = transform.position + origOffset;
			
			if (yFactor < 0)
			{
				targetObj.transform.position = targetObj.transform.position + yFactor * targetDir * 0.5f;
			}
		}
	}
}
