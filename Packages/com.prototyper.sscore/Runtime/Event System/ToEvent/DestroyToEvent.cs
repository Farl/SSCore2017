using UnityEngine;
using System.Collections;
using SS;

public class DestroyToEvent : SimpleEventSource
{
	void OnDestroy()
	{
		DoEvent();
	}
}
