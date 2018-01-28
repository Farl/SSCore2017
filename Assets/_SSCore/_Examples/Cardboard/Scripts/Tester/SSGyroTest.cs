using UnityEngine;
using System.Collections;

public class SSGyroTest : MonoBehaviour {

	public GameObject testObj;
	Quaternion origQuat;

	// Use this for initialization
	void Start () {
		SSGUI.Register(DrawGUI);
		Input.gyro.enabled = true;
		Reset();
	}

	void Reset()
	{
		origQuat = Input.gyro.attitude;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Quaternion newRot = Input.gyro.attitude * Quaternion.Inverse(origQuat);
		
		Vector3 newForward = newRot * Vector3.forward;
		Vector3 newRight = newRot * Vector3.right;
		Vector3 newUp = newRot * Vector3.up;

		if (testObj != null)
		{
			Debug.DrawLine(testObj.transform.position, testObj.transform.position + newRight, Color.red);
			Debug.DrawLine(testObj.transform.position, testObj.transform.position + newUp, Color.green);
			Debug.DrawLine(testObj.transform.position, testObj.transform.position + newForward, Color.blue);

			// testObj.transform.LookAt(testObj.transform.position + Input.gyro.gravity, Vector3.up);
			testObj.transform.LookAt(testObj.transform.position + newForward, newUp);
			// testObj.transform.localRotation = newRot;
		}
	}

	void DrawGUI()
	{
		Input.gyro.enabled = GUILayout.Toggle(Input.gyro.enabled, "Gryo Enabled");
		if (GUILayout.Button("Reset Gyro"))
		{
			Reset ();
		}
	}
}
