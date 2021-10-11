using UnityEngine;
using System.Collections;
using SS;

public class BecameVisibleToEvent : SimpleEventSource
{
	void OnBecameInvisible()
	{
		DoEvent();
	}
}
