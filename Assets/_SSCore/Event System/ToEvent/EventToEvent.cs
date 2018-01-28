using UnityEngine;
using System.Collections;
using SS;

public class EventToEvent : EventListener
{
	public EventArray eventArray;
	
	[ContextMenu("Do Event")]
	public void DoEvent()
	{
		eventArray.Broadcast(this);
	}

	protected override void OnEvent(EventMessage em, ref object paramRef)
	{
        base.OnEvent(em, ref paramRef);
		DoEvent();
	}
}
