using UnityEngine;
using System.Collections;

public class SSAccelerationTest : MonoBehaviour {
	public GameObject testObj;

	// Use this for initialization
	void Start ()
	{
		SSGUI.Register(DrawGUI);
	}
	
	// Update is called once per frame
	void Update () {
		if (testObj != null)
		{
			testObj.transform.LookAt(testObj.transform.position + Input.acceleration, Vector3.up);
		}
	}

	void DrawGUI()
	{
		GUILayout.Label("accelerationEventCount = " + Input.accelerationEventCount.ToString());
	}
}
