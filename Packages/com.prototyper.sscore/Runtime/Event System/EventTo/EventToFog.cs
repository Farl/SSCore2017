using UnityEngine;
using System.Collections;
using SS;

public class EventToFog : EventListener
{
	public float fogDensity;

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);
        enabled = true;
	}

	void Start()
	{
		enabled = false;
	}

	void Update()
	{
		RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensity, Time.deltaTime);
		if (Mathf.Abs (RenderSettings.fogDensity - fogDensity) < 0.1)
		{
			RenderSettings.fogDensity = fogDensity;
			enabled = false;
		}
	}
}
