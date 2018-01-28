using UnityEngine;
using System.Collections;

public class SSCompassInput : MonoBehaviour
{
	
	#region Compass
	static SSCompassInput instance;
	static Vector3 compassVec;
	static Vector3 compassVecBase;
	static bool bLargeMag = false;

	static bool bRingPress = false;
	static bool bRingDown = false;
	static bool bRingUp = false;

	void UpdateCompass()
	{
		bool bRingPrev = bRingPress;

		compassVec = Input.compass.rawVector;
		
		if (!bLargeMag && compassVec.x > 100)
		{
			compassVecBase = compassVec;
			bLargeMag = true;
		}
		else if (bLargeMag && compassVec.x < 100)
		{
			compassVecBase = compassVec;
			bLargeMag = false;
		}
		
		if (!bRingPress && compassVec.x - compassVecBase.x > Mathf.Abs(compassVecBase.x) / 6f)
		{
			bRingPress = true;
		}
		else if (bRingPress && compassVec.x - compassVecBase.x <= Mathf.Abs(compassVecBase.x) / 6f)
		{
			bRingPress = false;
		}
		
		bRingDown = (!bRingPrev && bRingPress);
		bRingUp = (bRingPrev && !bRingPress);
	}
	#endregion
	
	public static bool GetRingDown()
	{
		return bRingDown;
	}
	public static bool GetRingUp()
	{
		return bRingUp;
	}
	public static bool GetRing()
	{
		return bRingPress;
	}

	void Awake ()
	{
		if (instance != null)
		{
			Destroy (this);
		}
		else
		{
			instance = this;
			
			Input.compass.enabled = true;
			
			DontDestroyOnLoad(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		UpdateCompass();
	}

	void DrawGUI()
	{
		GUILayout.Label("bRingDown=" + bRingDown.ToString());
		GUILayout.Label("compassVec=" + compassVec.ToString());
		GUILayout.Label("compassVecBase=" + compassVecBase.ToString());
	}
}
