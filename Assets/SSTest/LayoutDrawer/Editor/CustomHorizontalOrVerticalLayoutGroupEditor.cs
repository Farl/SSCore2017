using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(CustomHorizontalOrVerticalLayoutGroup), true)]
[CanEditMultipleObjects]
public class CustomHorizontalOrVerticalLayoutGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
