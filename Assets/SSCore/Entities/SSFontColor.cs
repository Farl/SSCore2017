using UnityEngine;
using System.Collections;

public class SSFontColor : MonoBehaviour {
	public Color color;

	void Update()
	{
		GetComponent<Renderer>().material.color = color;
	}
}
