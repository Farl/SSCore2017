using UnityEngine;
using System.Collections;
using SS;

public class EventToAudio : EventListener
{
	public AudioClip audioClip;

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);

        if (audioClip == null)
			return;

		if (em.paramExtra != null && em.paramExtra.Length > 0)
		{
			if (em.paramExtra[0].GetType() == typeof(Vector3))
			{
				AudioSource.PlayClipAtPoint(audioClip, (Vector3)em.paramExtra[0]);
				return;
			}
		}

		AudioSource.PlayClipAtPoint(audioClip, transform.position);
	}
}
