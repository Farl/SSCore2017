using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StringFlag
{
	HashSet<string> flags = new HashSet<string>();

	public bool IsEmpty()
	{
		return flags.Count == 0;
	}

	public void Clear()
	{
		flags.Clear();
	}

	public bool CheckFlag(string flag)
	{
		return flags.Contains(flag);
	}

	public void AddFlag(string flag)
	{
		if (!flags.Contains(flag))
			flags.Add(flag);
	}

	public void RemoveFlag(string flag)
	{
		if (flags.Contains(flag))
			flags.Remove(flag);
	}

	public void Dump()
	{
		foreach (string str in flags)
		{
			Debug.Log(str);
		}
	}
}