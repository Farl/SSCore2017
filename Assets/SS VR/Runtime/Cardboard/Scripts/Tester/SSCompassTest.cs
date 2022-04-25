using UnityEngine;
using System.Collections;

public class SSCompassTest : MonoBehaviour
{
	public Vector3 maxRawVector, minRawVector;
	public GameObject testObj;

	void Reset()
	{
		maxRawVector = minRawVector = Input.compass.rawVector;
	}

	void CheckMaxMin(Vector3 rawVector)
	{
		maxRawVector = Vector3.Max(rawVector, maxRawVector);
		minRawVector = Vector3.Min(rawVector, minRawVector);
	}

	// Use this for initialization
	void Start ()
	{
		SSGUI.Register(DrawGUI);
		Input.compass.enabled = true;
		Reset();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Debug.DrawLine(transform.position, transform.position + Input.compass.rawVector);
		if (testObj != null)
		{
			testObj.transform.LookAt(testObj.transform.position + Input.compass.rawVector, Vector3.up);
		}
	}

	void DrawGUI()
	{
		if (GUILayout.Button("Reset"))
		{
			Reset ();
		}

		// Used to enable or disable compass. Note, that if you want Input.compass.trueHeading property to contain a valid value,
		// you must also enable location updates by calling Input.location.Start().
		Input.compass.enabled = GUILayout.Toggle(Input.compass.enabled, "Enable Compass");
		if (Input.compass.enabled)
		{
			//Input.location.Start ();
		}

		// The heading in degrees relative to the geographic North Pole. (Read Only)
		float trueHeading = Input.compass.trueHeading;
		GUILayout.Label("trueHeading = " + trueHeading);

		// Timestamp (in seconds since 1970) when the heading was last time updated. (Read Only)
		double timestamp = Input.compass.timestamp;
		GUILayout.Label("timestamp = " + timestamp);

		// The raw geomagnetic data measured in microteslas. (Read Only)
		Vector3 rawVector = Input.compass.rawVector;
		GUILayout.Label("rawVector = " + rawVector);
		GUILayout.Label("maxRawVector = " + maxRawVector);
		GUILayout.Label("minRawVector = " + minRawVector);

		CheckMaxMin(rawVector);

		// The heading in degrees relative to the magnetic North Pole. (Read Only)
		float magneticHeading = Input.compass.magneticHeading;
		GUILayout.Label("magneticHeading = " + magneticHeading);

		#if UNITY_4_5
		// Accuracy of heading reading in degrees.
		// Negative value mean unreliable reading. If accuracy is not supported or not available,
		// 0 is returned. Not all platforms support this pricise accuracy, so the value may vary between few constant values.
		float headingAccuracy = Input.compass.headingAccuracy;
		GUILayout.Label("headingAccuracy = " + headingAccuracy);
		#endif


	}
}
