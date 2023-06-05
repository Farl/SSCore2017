using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SS.TextTable
{
    [CustomEditor(typeof(TextPackage))]
    public class TextPackageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                TextTableEditor.Open();
            }
            base.OnInspectorGUI();


        }
    }

}