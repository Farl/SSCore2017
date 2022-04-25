using UnityEngine;
using System.Collections;

public class SSVRTrace : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Ray viewRay = GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f));
		RaycastHit hitInfo;
		Debug.DrawRay(viewRay.origin, viewRay.direction);
		if (Physics.Raycast(viewRay.origin, viewRay.direction, out hitInfo, 1000f))
		{
			VRInputModule.SetTargetObject(hitInfo.collider.gameObject);
		}
		else
		{
			VRInputModule.SetTargetObject(null);
		}
	}
}
