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
    /**
     * Package Helper
     * 
     * A tool to help create and import packages
     * 
     * @version 0.2.0
     * @package SS.PackageHelper
     * @author Farl
     *
     * Change log:
        *  0.0.1: initial version
        *  0.1.0: add import tool
        *  0.2.0: add edit tool
     */
    public class PackageHelper: EditorWindow
    {

        #region Static
        private const string version = "0.2.0";
        private static Vector2 scrollVec = Vector2.zero;
        #endregion


        #region Public

        [MenuItem("Tools/SS/Package Helper")]
        [MenuItem("Assets/PackageHelper/Create Package")]
        public static void CreateNewPackage()
        {
            var window = EditorWindow.CreateWindow<PackageHelper>();
            window.titleContent = new GUIContent($"Package {version}");
        }
        #endregion

        #region Private & Protected

        private PackageImportTool packageImportTool;
        private PackageCreateTool packageCreateTool;
        private PackageEditTool packageEditTool;

        private void OnEnable()
        {
            packageImportTool = ScriptableObject.CreateInstance<PackageImportTool>();
            packageCreateTool = ScriptableObject.CreateInstance<PackageCreateTool>();
            packageEditTool = ScriptableObject.CreateInstance<PackageEditTool>();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox($"Package Helper {version}", MessageType.Info);

            // Select a tool by EditorToolbar and draw that tool
            EditorGUILayout.EditorToolbar(packageCreateTool, packageImportTool, packageEditTool);

            scrollVec = EditorGUILayout.BeginScrollView(scrollVec);

            packageCreateTool.OnToolGUI(this);
            packageImportTool.OnToolGUI(this);
            packageEditTool.OnToolGUI(this);

            EditorGUILayout.EndScrollView();
        }
        #endregion

    }
}
