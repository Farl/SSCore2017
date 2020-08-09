using UnityEngine;
using System.Collections;
using SS;

public class AwakeToEvent : SimpleEventSource
{
	void Awake()
	{
		DoEvent();
	}
}
