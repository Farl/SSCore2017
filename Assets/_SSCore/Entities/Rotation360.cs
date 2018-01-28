using UnityEngine;
using System.Collections;

public class Rotation360 : MonoBehaviour {
	public float duration = 1.0f;
	public Vector3 fromRot = new Vector3(0, 0, 0);
	public Vector3 toRot = new Vector3(0, 0, 0);
	public int quantization = 0;
	private	float stateTimer = 0;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		stateTimer += Time.deltaTime;
		
		if(stateTimer > duration)
		{
			stateTimer -= duration;
		}
		
		if(duration != 0)
		{
			Vector3 currRot = Vector3.Lerp(fromRot, toRot, stateTimer / duration);
			this.transform.rotation = Quaternion.Euler(currRot);
		}
	}
}
