using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS;

public class KillBox : PTTrigger
{
    protected override Color GetGizmosMeshColor(bool focused)
    {
        return new Color(1, 0, 0, 0.2f);
    }
    protected override Color GetGizmosWireColor(bool focused)
    {
        return new Color(1, 0, 0, 1f);
    }

    protected override void OnTriggerEnter(Collider coll)
    {
        EventMessage em = new EventMessage(coll.gameObject, "Kill", coll.gameObject, true);
        EventManager.SendObjectEvent(em);
    }
}
