using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS;

public class OnTriggerToEvent : PTTrigger
{
    public EventArray onEnter;
    public EventArray onLeave;

    protected override void OnTriggerEnter(Collider coll)
    {
        base.OnTriggerEnter(coll);
        onEnter.Broadcast(coll.gameObject);
    }

    protected override void OnTriggerExit(Collider coll)
    {
        base.OnTriggerExit(coll);
        onLeave.Broadcast(coll.gameObject);
    }
}
