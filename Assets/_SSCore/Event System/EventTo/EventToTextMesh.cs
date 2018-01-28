using UnityEngine;
using System.Collections;
using SS;

public class EventToTextMesh : EventListener
{
	TextMesh textMesh;

	// Use this for initialization
	void Start () {
		textMesh = GetComponent<TextMesh>();
	}

    protected override void OnEvent(EventMessage em, ref object paramRef)
    {
        base.OnEvent(em, ref paramRef);
        if (textMesh != null && em.paramString != null)
		{
			textMesh.text = em.paramString;
		}
	}
}
