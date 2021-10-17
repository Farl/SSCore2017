using UnityEngine;
using System.Collections;
using SS;

public class EventToAnim : EventListener
{
	public Animation targetAnim;
	public string clipName;
	public AnimationClip clip;


    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);
        if (targetAnim == null)
			return;
		if (clipName != null)
		{
			AnimationState animState = targetAnim[clipName];
			if (animState)
			{
				targetAnim.CrossFade(clipName);
				return;
			}
		}
		if (clip != null)
		{
			targetAnim.AddClip(clip, clip.name);
			targetAnim.CrossFade(clip.name);
		}
	}
}
