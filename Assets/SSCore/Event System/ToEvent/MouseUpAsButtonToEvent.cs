using UnityEngine;
using System.Collections;
using SS;

public class MouseUpAsButtonToEvent : SimpleEventSource
{
	void OnMouseUpAsButton()
	{
		DoEvent();
	}
}
