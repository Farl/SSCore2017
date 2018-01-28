using UnityEngine;
using System.Collections;

// This is not an editor script. The property attribute class should be placed in a regular script file.
public class RangeAttribute : PropertyAttribute
{
	public float min;
	public float max;
	
	public RangeAttribute (float min, float max)
	{
		this.min = min;
		this.max = max;
	}
}