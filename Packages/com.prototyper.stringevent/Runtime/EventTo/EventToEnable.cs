using UnityEngine;
using System.Collections;
using SS;

public class EventToEnable : EventListener
{
	public Component targetComp;

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);

        if (targetComp != null)
		{
			Behaviour behavior = targetComp as Behaviour;
			Renderer ren = targetComp as Renderer;
			Collider coll = targetComp as Collider;

			if (behavior)
			{
				behavior.enabled = em.paramBool;
			}
			if (ren)
			{
				ren.enabled = em.paramBool;
			}
			if (coll)
			{
				coll.enabled = em.paramBool;
			}
		}
	}
}