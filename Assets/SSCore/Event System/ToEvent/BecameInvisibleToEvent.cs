using UnityEngine;
using System.Collections;
using SS;

public class BecameInvisibleToEvent : SimpleEventSource
{
	void OnBecameInvisible()
	{
		DoEvent();
	}
}
