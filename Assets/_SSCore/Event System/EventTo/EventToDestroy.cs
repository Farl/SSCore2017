using UnityEngine;
using System.Collections;
using SS;

public class EventToDestroy : EventListener
{
	public Object targetObj;
	public Object[] targetObjs;

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);

        if (targetObj != null)
		{
			Destroy(targetObj);
		}
		if (targetObjs != null)
		{
			foreach(Object obj in targetObjs)
			{
				if (obj != null)
				{
					Destroy(obj);
				}
			}
		}
	}
}
