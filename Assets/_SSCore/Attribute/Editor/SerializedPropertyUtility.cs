using UnityEngine;
using System.Collections;

using UnityEditor;
using System.Linq;
using System;

using System.Reflection;

public static class SerializedPropertyUtility
{
	public static bool IsArrayElement(SerializedProperty prop)
	{
		return (prop.propertyPath[prop.propertyPath.Length - 1] == ']');
	}

	public static int IndexOfArrayElement(SerializedProperty prop)
	{
		if (IsArrayElement(prop))
			return Convert.ToInt32(prop.propertyPath.Substring(prop.propertyPath.LastIndexOf("[")).Replace("[","").Replace("]",""));
		else
			return 0;
	}

	public static SerializedProperty GetArrayParentProperty(SerializedProperty property)
	{
		//Debug.Log("Searching parent of " + property.propertyPath + "...");
		if (!IsArrayElement(property))
			return null;

		// Backup
		var path = property.propertyPath;
		var parentPath = path.Substring(0, path.LastIndexOf(".data["));
		var currProp = property.Copy();
		currProp.Reset ();

		// Stupid search
		var prevProp = currProp.Copy ();
		bool nextProperty = currProp.Next(true);
		while (nextProperty)
		{
			if (currProp.propertyPath == path)
			{
				//Debug.Log("... Found " + prevProp.propertyPath);
				return prevProp;
			}

			// record only array property
			if (currProp.propertyPath == parentPath)
				prevProp = currProp.Copy ();

			nextProperty = currProp.Next(true);
		}
		return null;
	}
	
	public static object GetParent(SerializedProperty prop)
	{
		var path = prop.propertyPath.Replace(".Array.data[", "[");
		object obj = prop.serializedObject.targetObject;
		var elements = path.Split('.');
		foreach(var element in elements.Take(elements.Length-1))
		{
			if(element.Contains("["))
			{
				var elementName = element.Substring(0, element.IndexOf("["));
				var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
				obj = GetValue(obj, elementName, index);
			}
			else
			{
				obj = GetValue(obj, element);
			}
		}
		return obj;
	}
	
	public static object GetValue(object source, string name)
	{
		if(source == null)
			return null;
		var type = source.GetType();
		var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		if(f == null)
		{
			var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if(p == null)
				return null;
			return p.GetValue(source, null);
		}
		return f.GetValue(source);
	}
	
	public static object GetValue(object source, string name, int index)
	{
		var enumerable = GetValue(source, name) as IEnumerable;
		var enm = enumerable.GetEnumerator();
		while(index-- >= 0)
			enm.MoveNext();
		return enm.Current;
	}
}