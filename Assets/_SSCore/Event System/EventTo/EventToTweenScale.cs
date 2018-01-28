using UnityEngine;
using System.Collections;
using SS;

public class EventToTweenScale : EventToTweenPosition
{
	protected override void SetValue(Vector3 currValue)
	{
		transform.localScale = currValue;
	}
	protected override Vector3 GetValue()
	{
		return transform.localScale;
	}
}