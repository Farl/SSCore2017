using UnityEngine;
using System.Collections;

using SS.Legacy;

public class SSVR : MonoBehaviour
{
	protected Quaternion gyroAttitube;
    
	Vector3 newForward = Vector3.forward;
	Vector3 newUp = Vector3.up;

	#if UNITY_EDITOR
	Vector3 mousePos;
#else
    Vector3 gyroForward = Vector3.zero;
    Vector3 gyroUp = Vector3.zero;
#endif

    void Awake()
	{
		SSGUI.Register(DrawGUI);
		Input.gyro.enabled = true;

		#if UNITY_EDITOR
		gyroAttitube = Quaternion.identity;
		#endif
	}

	void OnDestroy()
	{
		SSGUI.Unregister(DrawGUI);
	}

	void Update()
	{
		if (SSCompassInput.GetRingDown())
		{
			Input.gyro.enabled = !Input.gyro.enabled;
		}

		#if UNITY_EDITOR
		if (Input.GetMouseButtonDown(0))
		{
			mousePos = Input.mousePosition;
		}
		else if (Input.GetMouseButton(0))
		{
			mousePos = Input.mousePosition - mousePos;
			Quaternion xRot = Quaternion.AngleAxis(10 * Time.deltaTime * -mousePos.x, transform.up);
			Quaternion yRot = Quaternion.AngleAxis(10 * Time.deltaTime * mousePos.y, transform.right);
			newForward = xRot * yRot * newForward;
			newUp = xRot * yRot * newUp;
			mousePos = Input.mousePosition;
		}
		#else
		gyroAttitube = Input.gyro.attitude;
		newUp = Vector3.up;
		
		gyroForward = (gyroAttitube) * Vector3.forward;
		gyroUp = (gyroAttitube) * Vector3.up;
		
		newForward = GyroForwardToWorld(gyroForward);
		newUp = GyroUpToWorld(gyroUp);
		#endif

		transform.LookAt(transform.position + newForward, newUp);
	}
	
	Vector3 GyroForwardToWorld(Vector3 vec)
	{
		Vector3 worldVec = new Vector3(-vec.y, -vec.z, vec.x);
		return worldVec;
	}
	
	Vector3 GyroUpToWorld(Vector3 vec)
	{
		Vector3 worldVec = new Vector3(vec.y, vec.z, -vec.x);
		return worldVec;
	}

	void DrawGUI()
	{
		/*
		GUILayout.Label("gyroForward=" + gyroForward.ToString());
		GUILayout.Label("gyroUp=" + gyroUp.ToString());
		GUILayout.Label("newForward=" + newForward.ToString());
		GUILayout.Label("newUp=" + newUp.ToString());
		*/
	}
}
