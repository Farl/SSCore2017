using UnityEngine;
using System.Collections;
using SS;

public class EventToTweenRotate : EventToTweenPosition
{
	protected override void SetValue(Vector3 currValue)
	{
		transform.localEulerAngles = currValue;
	}

	protected override Vector3 GetValue()
	{
		return transform.localEulerAngles;
	}
	
	protected override Vector3 Lerp(Vector3 src, Vector3 dst, float factor)
	{
		return new Vector3(Mathf.LerpAngle(src.x, dst.x, factor), Mathf.LerpAngle(src.y, dst.y, factor), Mathf.LerpAngle(src.z, dst.z, factor));
	}
}
