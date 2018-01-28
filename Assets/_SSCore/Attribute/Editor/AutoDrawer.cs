using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;

// The property drawer class should be placed in an editor script, inside a folder called Editor.
// Tell the RangeDrawer that it is a drawer for properties with the RangeAttribute.
[CustomPropertyDrawer (typeof(AutoAttribute))]
public class AutoDrawer : PropertyDrawer {
	
	public static bool showDebug = false;
	public static float normalHeight = 16;
	public static float smallHeight = 16;
	public static float indentWidth = 4;
	public static float frameWidth = 4;
	public static float debugTool = 20;
	public static float doubleHeight = 16+16;
	public static float arrayElementWidth = 12;
	public static float labelWidthPercent = 0.3f;

	public static Vector2 marginWidth = new Vector2(8+10, 4);
	
	//static float foldoutWidth = 10;	// 4.2
	static float foldoutWidth = 0;	// 4.3
	static float labelWidth = 150;

	/*static*/ GUISkin mySkin = null;
	/*static*/ Texture2D[] boxTex;
	
	bool isCalcHeight = false;
	float currHeight;
	string currProperty;
	
	InsertDeleteElement action = null;

	// For array special case (4.3)
	bool isArrayElement = false;
	int arrayElementIndex = -1;
	
	// For array elements
	class InsertDeleteElement
	{
		SerializedProperty property;
		bool isInsert;
		int index;
		public InsertDeleteElement(SerializedProperty prop, bool bInsert, int i)
		{
			property = prop.Copy();
			isInsert = bInsert;
			index = i;
		}
		public void Action()
		{
			if (property != null)
			{
				if (isInsert)
				{
					property.arraySize++;
					for (int j = property.arraySize - 2; j > index; j--)
					{
						property.MoveArrayElement(j-1, j);
					}
				}
				else
				{
					for (int j = index; j < property.arraySize - 1; j++)
					{
						property.MoveArrayElement(j+1, j);
					}
					property.arraySize--;
				}
			}
		}
	}
		
	public float GetPropertyHeight (SerializedProperty property, SerializedProperty origProp, GUIContent label)
	{
		bool backupArrayFlag = isArrayElement;
		Rect position = new Rect(0, 0, 100, 100);
		float yMax = 0;
		bool backupFlag = isCalcHeight;
		currProperty = property.propertyPath;
		isCalcHeight = true;
		AutoPropertyField(position, property.Copy (), origProp.Copy (), label, 0, out yMax);
		isCalcHeight = backupFlag;
		currHeight = (yMax - position.y);
		isArrayElement = backupArrayFlag;
		return currHeight;
	}
	
	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		SerializedProperty origProp = property;
		bool backupFlag = isArrayElement;
		if (SerializedPropertyUtility.IsArrayElement(property))
		{
			isArrayElement = true;
			arrayElementIndex = SerializedPropertyUtility.IndexOfArrayElement(property);
			origProp = SerializedPropertyUtility.GetArrayParentProperty(property);
		}
		else
		{
			origProp = property.Copy();
		}
		float height = GetPropertyHeight(property, origProp, label) + ( 1 ) + ((showDebug)? debugTool: 0);
		isArrayElement = backupFlag;
		return height;
	}
	
	static void CopyGUISkin(GUISkin src, out GUISkin dst)
	{
		dst = (GUISkin)ScriptableObject.CreateInstance(typeof(GUISkin));
		foreach(PropertyInfo propertyInfo in typeof(GUISkin).GetProperties(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance))
		{
			if (propertyInfo.PropertyType == typeof(GUISettings))
			{
				GUISettings settings = (GUISettings) propertyInfo.GetValue(src, null);
		        dst.settings.cursorColor = settings.cursorColor;
		        dst.settings.cursorFlashSpeed = settings.cursorFlashSpeed;
		        dst.settings.doubleClickSelectsWord = settings.doubleClickSelectsWord;
		        dst.settings.selectionColor = settings.selectionColor;
		        dst.settings.tripleClickSelectsLine = settings.tripleClickSelectsLine;				
			}
			else
				propertyInfo.SetValue(dst, propertyInfo.GetValue(src, null), null);
		}
	}
	
	/*static*/ void SetupGUIStyle()
	{
		GUI.skin = null;

		if (mySkin == null)
		{
			//mySkin = Resources.Load ("AutoAttributeSkin") as GUISkin;
			if (mySkin == null)
			{
				return;
			}
			//CopyGUISkin(GUI.skin, out mySkin);
			
			mySkin.button.fontSize = 10;
			mySkin.button.clipping = 0;
			//mySkin.button.normal.textColor = Color.white;

			/*
			Texture2D srcBox = mySkin.box.normal.background;
			if (srcBox!= null && boxTex == null)
			{
				boxTex = new Texture2D[2];
				
				int border = 1;
				//Color[] color = new Color[2] {new Color(0.3f, 0.4f, 0.5f, 1), new Color(0.5f, 0.4f, 0.3f, 1)};
				Color[] color = new Color[2] {new Color(0.3f, 0.4f, 0.5f, 1), new Color(0.3f, 0.5f, 0.3f, 1)};
				for (int texID = 0; texID < 2; texID++)
				{
					boxTex[texID] = new Texture2D(srcBox.width, srcBox.height, srcBox.format, true, true);
					for (int i = 0; i < boxTex[texID].width; i++)
					{
						for (int j = 0; j < boxTex[texID].height; j++)
						{
							if (i < border || j < border || i >= boxTex[texID].width - border || j >= boxTex[texID].height - border)
							{
								boxTex[texID].SetPixel(i, j, color[texID]);
							}
							else
							{
								float factor = (boxTex[texID].height > 1)? j / (float)(boxTex[texID].height - 1): 1;
								float factor2 = (Mathf.Cos (factor * Mathf.PI * 2) + 1) / 2;
								
								Color colorInside = color[texID] * factor2 + color[texID] * 0.5f * (1-factor2);	// LERP
								colorInside.a = 1;
								boxTex[texID].SetPixel(i, j, colorInside);
							}
						}
					}
			   		boxTex[texID].Apply();
				}
			}
			if (boxTex != null && boxTex[0] != null)
			{
				mySkin.box.normal.background = boxTex[0];
			}
			*/
		}
		
		GUI.skin = mySkin;
	}

	// Entry Point
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		if (showDebug)
		{
			Rect sliderRect = new Rect(0, position.y, position.width, normalHeight);
			labelWidthPercent = EditorGUI.Slider(sliderRect, labelWidthPercent, 0, 1);
			sliderRect.y += debugTool;
			position.yMin += debugTool;
		}
		
		float yMax = position.yMax;
		currHeight = 0;
		EditorGUI.indentLevel = 0;
		isCalcHeight = false;

		SetupGUIStyle();
		
		// Inspector Label Width
		labelWidth = position.width * labelWidthPercent;
		EditorGUIUtility.labelWidth = labelWidth;
        
		
		currProperty = property.propertyPath;
		SerializedProperty origProp = null;
		
		// Special case of array element (4.3)
		if (SerializedPropertyUtility.IsArrayElement(property))
		{
			isArrayElement = true;
			arrayElementIndex = SerializedPropertyUtility.IndexOfArrayElement(property);
			origProp = SerializedPropertyUtility.GetArrayParentProperty(property);
		}
		else
		{
			origProp = property.Copy();
		}

		AutoPropertyField(position, property, origProp, label, 0, out yMax);
		
		// delay action
		if (action != null)
		{
			action.Action();
			action = null;
		}
		
		GUI.skin = null;
	}
	
	bool AutoPropertyField(Rect position, SerializedProperty property, SerializedProperty origProp, GUIContent label, int level, out float yMax)
	{
        
		bool bNextVisible = true;
		
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		if (!isCalcHeight)
			EditorGUI.BeginProperty(new Rect(position.x, position.y, position.width, normalHeight), label, property);
		
		bNextVisible = AutoArrayField(position, property, origProp.Copy(), label, level, out yMax);
		
		if (!isCalcHeight)
			EditorGUI.EndProperty();
		
		return bNextVisible;
	}
	
	float GetXRange(SerializedProperty prop)
	{
		return (prop.depth + 0) * indentWidth;
	}
	
	// For each array element (include size)
	bool AutoArrayField(Rect position, SerializedProperty prop, SerializedProperty origProp, GUIContent label, int level, out float yMax)
	{
		bool bNextVisible = true;
		
		yMax = position.yMax;

		Rect newPos = new Rect(position);
		newPos.y += marginWidth.y;
		newPos.height = normalHeight;
		
		Vector2 xRangeFrame = new Vector2(GetXRange(prop) + frameWidth, position.xMax - GetXRange(prop) - frameWidth);
		
		// Frame Box
		if (!isCalcHeight)
		{
			Rect boxRect = new Rect(position);
			boxRect.x = xRangeFrame[0];
			boxRect.xMax = xRangeFrame[1];
			//boxRect.yMax = yMax;
			
			string storeCurrProp = currProperty;
			boxRect.height = this.GetPropertyHeight(prop, origProp, label);
			currProperty = storeCurrProp;
            
            if (prop.depth % 2 == 0)
            {
                if (EditorGUIUtility.isProSkin)
                    GUI.Box(boxRect, "", "box");
                else
                    GUI.Box(boxRect, "", "box");
            }
            else
                GUI.Box(boxRect, "", "box");
		}
		
		// Array Index
		int i = -1;

		bool bArrayEleSpecialCase = false;
		if (isArrayElement)
		{
			i = arrayElementIndex;
			isArrayElement = false;
			bArrayEleSpecialCase = true;
		}
        
		do {
			Vector2 xRangeContent = new Vector2(GetXRange(prop) + marginWidth.x, position.xMax - GetXRange(prop) - marginWidth.x);
			
			bool bGeneric = false;
			
			// Get displayName by Reflection
			if (!isCalcHeight)
			{
				label.text = prop.displayName;
			}

			float elementWidth = labelWidth;
			if (i >= 0)
				elementWidth = arrayElementWidth;	// array element
			
			// Is Array Element
			if (!isCalcHeight && (origProp.isArray && (origProp.isExpanded || bArrayEleSpecialCase)))
			{
				if (i >= 0)
				{
					// Label
					label.text = " ";
					Rect toolPos = new Rect(elementWidth + xRangeContent[0] - foldoutWidth, newPos.y, smallHeight, smallHeight);
					
					// Array element move up
					GUI.enabled = i > 0;
					if (GUI.Button (toolPos, "^"))
					{
						if (origProp.MoveArrayElement(i, i - 1))
						{
							bool temp = origProp.GetArrayElementAtIndex(i).isExpanded;
							origProp.GetArrayElementAtIndex(i).isExpanded = origProp.GetArrayElementAtIndex(i-1).isExpanded;
							origProp.GetArrayElementAtIndex(i-1).isExpanded = temp;
						}
					}
					toolPos.x += smallHeight;
					
					// Array element move down
					GUI.enabled = i < origProp.arraySize - 1;
					if (GUI.Button (toolPos, "v"))
					{
						if (origProp.MoveArrayElement(i, i + 1))
						{
							bool temp = origProp.GetArrayElementAtIndex(i).isExpanded;
							origProp.GetArrayElementAtIndex(i).isExpanded = origProp.GetArrayElementAtIndex(i+1).isExpanded;
							origProp.GetArrayElementAtIndex(i+1).isExpanded = temp;
						}
					}
					toolPos.x += smallHeight;
					
					// Array element remvoe
					GUI.enabled = origProp.arraySize > 0;
					if (GUI.Button (toolPos, "-"))
						action = new InsertDeleteElement(origProp, false, i);
					toolPos.x += smallHeight;
					
					// Array element insert
					GUI.enabled = true;
					if (GUI.Button (toolPos, "+"))
						action = new InsertDeleteElement(origProp, true, i);
					toolPos.x += smallHeight;
					
					// Array Element Label
					string elementIDStr = string.Format("{0}", i);
					if (!prop.hasVisibleChildren && !prop.isExpanded)
					{
						var nextDisplayName = "[" + elementIDStr + "]";
						GUI.Label(new Rect(toolPos.x, toolPos.y, nextDisplayName.Length * 10, toolPos.height), nextDisplayName);
					}
					else
					{
						var nextProp = prop.Copy();
						if (nextProp.NextVisible(true) && nextProp.propertyType == SerializedPropertyType.String)
						{
							var nextDisplayName = "[" + elementIDStr + "] = \"" + nextProp.stringValue + "\"";
							GUI.Label(new Rect(toolPos.x, toolPos.y, nextDisplayName.Length * 10, toolPos.height), nextDisplayName);
						}
					}
				}
			}
			
			// Is Array or Custom class
			if (prop.propertyType == SerializedPropertyType.Generic && (prop.isArray || prop.hasChildren))
			{
				// Fold out
				if (!isCalcHeight)
				{
					prop.isExpanded = EditorGUI.Foldout(new Rect(xRangeContent[0] - foldoutWidth, newPos.y, elementWidth, normalHeight), prop.isExpanded, label.text);
				}
					
				newPos.y += normalHeight;
				
				if (prop.isExpanded)
				{
					// Enter children
					SerializedProperty parentProp = prop.Copy ();
					bNextVisible = prop.NextVisible(true);
					if (!bNextVisible)
						return bNextVisible;
					
					// Into Generic Property
					bNextVisible = AutoPropertyField(newPos, prop, parentProp, label, level + 1, out yMax);
					newPos.y = yMax;
					
					bGeneric = true;
				}
			}
			else
			{
				//
				CustomPropertyField(newPos, prop, label, out yMax);
				newPos.y = yMax;
			}

			// special case
			if (bArrayEleSpecialCase)
			{
				bNextVisible = false;
				bArrayEleSpecialCase = false;
			}
			
			// Next Visible
			if (!bGeneric && bNextVisible)
			{
				bNextVisible = prop.NextVisible(false);
			}
			
			i++;
			
		} while (bNextVisible && prop.depth > origProp.depth);
		
		yMax = newPos.y + marginWidth.y;
		
		return bNextVisible;
	}
	
	void CustomPropertyField(Rect position, SerializedProperty prop, GUIContent label, out float yMax)
	{
		Vector2 xRangeContent = new Vector2(GetXRange(prop) + marginWidth.x, position.xMax - GetXRange(prop) - marginWidth.x);
		
		position.x = xRangeContent[0];
		position.xMax = xRangeContent[1];
		position.height = normalHeight;
		
		Rect controlPos = new Rect(position);
		controlPos.x += labelWidth;
		controlPos.width -= labelWidth;
		controlPos.height = normalHeight;
		
		switch(prop.propertyType)
		{
		case SerializedPropertyType.ArraySize:
		{
			if (currProperty != prop.propertyPath)
			{
				position.y += UnknownPropertyField(position, prop, label);
				break;
			}
			if (!isCalcHeight)
			{
				EditorGUI.LabelField(position, label);
				EditorGUI.BeginChangeCheck ();
				
				int intValue = EditorGUI.IntField(controlPos, prop.intValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.intValue = intValue;
				}
			}
			
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.Integer:
		{
			if (currProperty != prop.propertyPath)
			{
				position.y += UnknownPropertyField(position, prop, label);
				break;
			}
			if (!isCalcHeight)
			{
				//GUI.Label(position, new GUIContent(label.text), "label");
				controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));

				EditorGUI.BeginChangeCheck ();
				
				int intValue = EditorGUI.IntField(controlPos, prop.intValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.intValue = intValue;
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.Float:
		{
			if (currProperty != prop.propertyPath)
			{
				position.y += UnknownPropertyField(position, prop, label);
				break;
			}
			if (!isCalcHeight)
			{
				controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));

				EditorGUI.BeginChangeCheck ();
				
				float floatValue = EditorGUI.FloatField(controlPos, prop.floatValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.floatValue = floatValue;
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.String:
		{
			if (!isCalcHeight)
			{
				controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));

				EditorGUI.BeginChangeCheck ();
				
				string str = EditorGUI.TextField(controlPos, prop.stringValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.stringValue = str;
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.Color:
		{
			if (!isCalcHeight)
			{
				controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));
				EditorGUI.BeginChangeCheck ();
				
				Color color = EditorGUI.ColorField(controlPos, prop.colorValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.colorValue = color;
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.LayerMask:
		{
			if (!isCalcHeight)
			{
				controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));
				EditorGUI.BeginChangeCheck ();
				
				int layer = EditorGUI.LayerField(controlPos, prop.intValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.intValue = layer;
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.Vector2:
		{
			if (!isCalcHeight)
			{
				EditorGUI.LabelField (position, label);
				EditorGUI.BeginChangeCheck ();

				Vector2 vector2Value = EditorGUI.Vector2Field(controlPos, "", prop.vector2Value);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.vector2Value = vector2Value;
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.Quaternion:
		{
			if (!isCalcHeight)
			{
				EditorGUI.LabelField (position, label);
				EditorGUI.BeginChangeCheck ();
				
				controlPos.y -= normalHeight;
				Vector4 vector4Value = EditorGUI.Vector4Field(controlPos, "", new Vector4(prop.quaternionValue.x, prop.quaternionValue.y, prop.quaternionValue.z, prop.quaternionValue.w));
				
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.quaternionValue = new Quaternion(vector4Value.x, vector4Value.y, vector4Value.z, vector4Value.w);
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.Vector3:
		{
			if (!isCalcHeight)
			{
				EditorGUI.LabelField (position, label);
				EditorGUI.BeginChangeCheck ();

				Vector3 vector3Value = EditorGUI.Vector3Field(controlPos, "", prop.vector3Value);
				
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.vector3Value = vector3Value;
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.Rect:
		{
			if (!isCalcHeight)
			{
				controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));
				EditorGUI.BeginChangeCheck ();
				
				Rect rectValue = EditorGUI.RectField(controlPos, prop.rectValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.rectValue = rectValue;
				}
			}
			position.y += doubleHeight;
			break;
		}
		case SerializedPropertyType.Gradient:
		case SerializedPropertyType.Character:
		{
			if (!isCalcHeight)
			{
				// TODO
				EditorGUI.LabelField(position, prop.propertyType.ToString());
				EditorGUI.LabelField (controlPos, "Unsupport type");
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.AnimationCurve:
		{
			if (!isCalcHeight)
			{
				controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));
				EditorGUI.BeginChangeCheck ();
				
				AnimationCurve animationCurveValue = EditorGUI.CurveField(controlPos, prop.animationCurveValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.animationCurveValue = animationCurveValue;
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.Bounds:
		{
			if (!isCalcHeight)
			{
				controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));
				EditorGUI.BeginChangeCheck ();
				
				Bounds boundsValue = EditorGUI.BoundsField(controlPos, prop.boundsValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.boundsValue = boundsValue;
				}
			}
			position.y += doubleHeight;
			break;
		}
		case SerializedPropertyType.Boolean:
		{
			if (!isCalcHeight)
			{
				controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));
				EditorGUI.BeginChangeCheck ();
				
				bool boolValue = EditorGUI.Toggle(controlPos, prop.boolValue);
		
				// Code to execute if GUI.changed
				// was set to true inside the block of code above.
				if (EditorGUI.EndChangeCheck ())
				{
					prop.boolValue = boolValue;
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.ObjectReference:
		{
			if (currProperty != prop.propertyPath)
			{
				position.y += UnknownPropertyField(position, prop, label);
				break;
			}
			if (!isCalcHeight)
			{
				Type objType = null;
				SerializedProperty testProp = prop;
				if (SerializedPropertyUtility.IsArrayElement(prop))
				{
					object obj = SerializedPropertyUtility.GetParent(prop);
					objType = obj.GetType();
				}

				Type currType = testProp.serializedObject.targetObject.GetType();
				FieldInfo field = currType.GetField(testProp.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					objType = field.FieldType;
				}
				if (objType != null)
				{
					controlPos = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), new GUIContent(label.text));
					EditorGUI.BeginChangeCheck ();
					UnityEngine.Object objectReferenceValue = EditorGUI.ObjectField(controlPos, prop.objectReferenceValue, objType, true);
			
					// Code to execute if GUI.changed
					// was set to true inside the block of code above.
					if (EditorGUI.EndChangeCheck ())
					{
						prop.objectReferenceValue = objectReferenceValue;
					}
				}
			}
			position.y += normalHeight;
			break;
		}
		case SerializedPropertyType.Enum:
		{
			if (currProperty != prop.propertyPath)
			{
				position.y += UnknownPropertyField(position, prop, label);
				break;
			}
			if (!isCalcHeight)
			{
				Type currType = prop.serializedObject.targetObject.GetType();
				FieldInfo field = currType.GetField(prop.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					Type enumType = field.FieldType;
					
					FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
					if (fields != null)
					{
						int[] intArray = new int[fields.Length];
						for (int i = 0; i < fields.Length; i++)
						{
							intArray[i] = (int)Convert.ChangeType(fields[i].GetValue (null), typeof(int));
						}
						prop.enumValueIndex = (prop.enumValueIndex < 0)? 0: prop.enumValueIndex;
						EditorGUI.LabelField(position, label);
						
						EditorGUI.BeginChangeCheck ();
						int intValue = EditorGUI.IntPopup(controlPos, prop.intValue, prop.enumNames, intArray);
		
						// Code to execute if GUI.changed
						// was set to true inside the block of code above.
						if (EditorGUI.EndChangeCheck ())
							prop.intValue = intValue;
					}
				}
			}
			position.y += normalHeight;
			break;
		}
		default:
		{
			if (currProperty != prop.propertyPath)
			{
				position.y += UnknownPropertyField(position, prop, label);
				break;
			}
			if (!isCalcHeight)
			{
				Type currType = prop.serializedObject.targetObject.GetType();
				FieldInfo field = currType.GetField(prop.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field != null)
				{
					EditorGUI.LabelField(position, field.FieldType.ToString());
				}
			}
			position.y += normalHeight;
			break;
		}
		}
		yMax = position.y;
	}
	
	float UnknownPropertyField(Rect position, SerializedProperty property, GUIContent label)
	{
		if ((int)property.propertyType > 15 && property.hasChildren)
			position.x -= foldoutWidth;
		if (property.propertyPath != currProperty)
			position.height = EditorGUI.GetPropertyHeight(property);
		else
			position.height = normalHeight;
		if (!isCalcHeight)
		{
			EditorGUI.PropertyField(position, property, label, true);
		}
		return position.height;
	}
}