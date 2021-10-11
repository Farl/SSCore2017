using UnityEngine;
using System.Collections;
using SS;

public class EventToTeleport : EventListener
{
	public Transform teleportTo;

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);
        if (teleportTo != null)
		{
			transform.position = teleportTo.position;
		}
	}
}
