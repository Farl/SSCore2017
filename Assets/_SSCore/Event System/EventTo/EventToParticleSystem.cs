using UnityEngine;
using System.Collections;
using SS;

public class EventToParticleSystem : EventListener
{

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);
        if (GetComponent<ParticleSystem>() != null)
		{
			GetComponent<ParticleSystem>().Play(true);
		}
	}
}
