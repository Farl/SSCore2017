using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.IO;
using UnityEditor.Scripting;

namespace SS.PackageHelper
{
    public class PackageTool
    {
        public virtual GUIContent toolbarIcon
        {
            get { return new GUIContent("", EditorGUIUtility.IconContent("CustomTool").image, this.ToString()); }
        }

        public virtual void OnToolGUI(EditorWindow window)
        {
        }

        public void RefreshPackages()
        {
            UnityEditor.PackageManager.Client.Resolve();
        }

        public virtual void OnEnable()
        {

        }
    }
}
