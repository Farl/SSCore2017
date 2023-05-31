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
    public class PackageTool : EditorTool
    {
        protected bool isActivated = false;

        public override void OnActivated()
        {
            base.OnActivated();
            isActivated = true;
        }

        public override void OnWillBeDeactivated()
        {
            base.OnWillBeDeactivated();
            isActivated = false;
        }
    }
}
