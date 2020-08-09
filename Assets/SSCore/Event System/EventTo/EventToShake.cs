using UnityEngine;
using System.Collections;
using SS;

public class EventToShake : EventListener {

	//Vector3 origPos;
	Vector3 origScale;
	
	public Vector3 scaleFactor;

	float timeFactor;
	public float minTimeFactor = 5;
	public float maxTimeFactor = 15;

	float timer;

	void Start()
	{
		//origPos = transform.position;
		origScale = transform.localScale;
		timeFactor = maxTimeFactor;
	}
	
	protected override void OnEvent(bool paramBool)
	{
		timeFactor = maxTimeFactor;
	}
	
	// Update is called once per frame
	void Update () {
		//Vector3 newPos = transform.localPosition;
		Vector3 newScale = transform.localScale;

		
		newScale.x = Mathf.Sin(timer) * scaleFactor.x + origScale.x;
		newScale.y = Mathf.Cos(timer) * scaleFactor.y + origScale.y;
		newScale.z = Mathf.Sin(timer) * scaleFactor.z + origScale.z;


		transform.localScale = Vector3.Lerp (transform.localScale, newScale, 0.5f);

		timer += Time.deltaTime * timeFactor;

		timeFactor = Mathf.Lerp(timeFactor, minTimeFactor, 0.02f);
	}
}
