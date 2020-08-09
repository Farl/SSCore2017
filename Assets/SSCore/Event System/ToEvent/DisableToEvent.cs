using UnityEngine;
using System.Collections;
using SS;

public class DisableToEvent : SimpleEventSource
{
	void OnDisable()
	{
		DoEvent();
	}
}
