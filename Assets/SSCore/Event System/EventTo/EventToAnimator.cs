using UnityEngine;
using System.Collections;
using SS;

public class EventToAnimator : EventListener
{
	public Animator targetAnimator;
	public string stateName;

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);
        if (targetAnimator == null)
		{
			return;
		}

		targetAnimator.Play(stateName);
	}
}
