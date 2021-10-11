using UnityEngine;
using System.Collections;
using SS;

public class StartToEvent : SimpleEventSource
{
	[ContextMenu("Do Event")]
	void Start()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			return;
		}
#endif
		DoEvent();
	}
}