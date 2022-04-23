
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using SS;

[CustomPropertyDrawer(typeof(UIOrderSetting), false)]
public class UIOrderSettingEditor : PropertyDrawer
{
    float maxHeight;
    SerializedProperty firstProperty;
    string displayName;
    ReorderableList list;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = EditorGUIUtility.singleLineHeight * 3;
        SerializedProperty orderSetting = property.FindPropertyRelative("data");
        if (orderSetting != null)
        {
            h += orderSetting.arraySize * 1 * EditorGUIUtility.singleLineHeight;
        }
        return h;
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty orderSetting = property.FindPropertyRelative("data");
        if (orderSetting != null)
        {
            if (list == null)
            {
                list = new ReorderableList(orderSetting.serializedObject, orderSetting, true, true, true, true);

                list.drawElementCallback += DrawElement;
                list.elementHeight = 1 * EditorGUIUtility.singleLineHeight;
                //list.drawHeaderCallback += DrawHeader;
                //list.drawFooterCallback += DrawFooter;
            }

            if (list != null)
            {
                //list.elementHeight = maxHeight;
                //list.DoLayoutList();
                list.DoList(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight));
            }
        }
    }

    void DrawFooter(Rect rect)
    {

    }

    void DrawHeader(Rect rect)
    {
    }

    private void DrawElement(Rect rect, int index, bool active, bool focused)
    {
        if (list != null)
        {
            SerializedProperty orderSetting = list.serializedProperty;
            if (orderSetting != null)
            {
                SerializedProperty element = orderSetting.GetArrayElementAtIndex(index);
                SerializedProperty uiType = element.FindPropertyRelative("uiType");
                SerializedProperty order = element.FindPropertyRelative("order");
                Rect uiTypeRect = rect;
                uiTypeRect.width -= 50;
                Rect orderRect = new Rect(uiTypeRect.x + uiTypeRect.width, uiTypeRect.y, 50, uiTypeRect.height);
                uiType.stringValue = EditorGUI.TextField(uiTypeRect, uiType.stringValue);
                order.intValue = EditorGUI.IntField(orderRect, order.intValue);
            }
        }
    }

}
