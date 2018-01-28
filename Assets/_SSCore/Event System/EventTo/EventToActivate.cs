using UnityEngine;
using System.Collections;
using SS;

public class EventToActivate : EventListener
{
	public GameObject targetObj;

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);
        if (targetObj != null)
			targetObj.SetActive(em.paramBool);
		else
			gameObject.SetActive(em.paramBool);
	}
}
