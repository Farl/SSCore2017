using UnityEngine;
using System.Collections;
using SS;

public class EnableToEvent : SimpleEventSource
{
	void OnEnable()
	{
		DoEvent();
	}
}
