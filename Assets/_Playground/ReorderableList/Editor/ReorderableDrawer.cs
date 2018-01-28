using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;
using UnityEditorInternal;
using UnityEngine.Events;

[CustomPropertyDrawer (typeof(ReorderableAttribute))]
public class ReorderableDrawer : PropertyDrawer
{

	ReorderableList list;

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight (property, label);
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		//base.OnGUI (position, property, label);

		if (GUILayout.Button ("Clear")) {
			list = null;
		}

		PropertyAttribute propertyAttribute = this.attribute;

		FieldInfo fieldInfo = this.fieldInfo;

		var obj = fieldInfo.GetValue (property.serializedObject.targetObject);

		if (list == null) {
			list = new ReorderableList ((IList)obj, typeof(string), true, false, true, true);

			list.drawElementCallback += DrawElement;

			Debug.Log (propertyAttribute);
			Debug.Log (obj);

			if (property.isArray) {
				SerializedProperty sp = property.Copy ();
				do {
					Debug.Log(sp.propertyPath);
				} while (sp.NextVisible (false));
			}
		}

		if (list != null)
		{
			list.DoLayoutList ();
		}
	}

	private void DrawElement(Rect rect, int index, bool active, bool focused)
	{
		GUILayout.TextArea ("test");
	}

	~ReorderableDrawer()
	{
		if (list != null) {
			list.drawElementCallback -= DrawElement;
		}
	}
}
