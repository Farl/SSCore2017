using UnityEngine;
using System.Collections;
using SS;

public class EventToTweenPosition : EventListener
{
	public AnimationCurve curve;
	public float duration;
	public Vector3 start;
	public Vector3 end;
	public EventArray eventWhenDone;
	float timer = 0;
	public bool currentAsStart;

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);
        TweenStart();
	}

	void Start()
	{
		enabled = false;
	}

	void TweenStart()
	{
		enabled = true;
		timer = 0;
		if (currentAsStart)
		{
			start = GetValue();
		}
	}

	void Update()
	{
		if (timer > duration)
		{
			SetValue (end);
			eventWhenDone.Broadcast(this);
			enabled = false;
		}
		else
		{
			float factor = curve.Evaluate(timer / duration);
			Vector3 currValue = Lerp(start, end, factor);
			SetValue (currValue);
		}
		timer += Time.deltaTime;
	}

	protected virtual Vector3 Lerp(Vector3 src, Vector3 dst, float factor)
	{
		return src * (1 - factor) + dst * (factor);
	}
	
	[ContextMenu("Copy current value to start")]
	void CopyToStart()
	{
		start = GetValue();
	}
	
	[ContextMenu("Copy current value to end")]
	void CopyToEnd()
	{
		end = GetValue();
	}
	
	[ContextMenu("Set start value to current")]
	void CopyStartToCurrent()
	{
		SetValue(start);
	}
	
	[ContextMenu("Set end value to current")]
	 void CopyEndToCurrent()
	{
		SetValue(end);
	}
	
	protected virtual Vector3 GetValue()
	{
		return transform.localPosition;
	}
	
	protected virtual void SetValue(Vector3 currValue)
	{
		transform.localPosition = currValue;
	}
}
