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
     * @version 0.3.1
     * @package SS.PackageHelper
     * @author Farl
     *
     * Change log:
        * 0.0.1: initial version
        * 0.1.0: add import tool
        * 0.2.0: add edit tool
        * 0.2.1: fix minimal unity version issue.
        * 0.3.0: Support for manage samples
        * 0.3.1: Add GitUtility to auto fill some fields
     */
    public class PackageHelper: EditorWindow
    {

        #region Static
        private const string version = "0.3.1";
        private static Vector2 scrollVec = Vector2.zero;
        #endregion


        #region Public

        [MenuItem("Tools/SS/Package Helper")]
        [MenuItem("Assets/PackageHelper/Create Package")]
        public static void CreateNewPackage()
        {
            var window = EditorWindow.CreateWindow<PackageHelper>();
        }
        #endregion

        #region Private & Protected
        private int toolbarIndex = 0;

        private PackageImportTool packageImportTool = new PackageImportTool();
        private PackageCreateTool packageCreateTool = new PackageCreateTool();
        private PackageEditTool packageEditTool = new PackageEditTool();
        private PackageSampleTool packageSampleTool = new PackageSampleTool();

        private void OnEnable()
        {
            titleContent = new GUIContent($"Package {version}");
            packageCreateTool.OnEnable();
            packageImportTool.OnEnable();
            packageEditTool.OnEnable();
            packageSampleTool.OnEnable();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox($"Package Helper {version}", MessageType.Info);

            // Select a tool by EditorToolbar and draw that tool
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, new GUIContent[] {
                packageCreateTool.toolbarIcon,
                packageImportTool.toolbarIcon,
                packageEditTool.toolbarIcon,
                packageSampleTool.toolbarIcon
            });

            scrollVec = EditorGUILayout.BeginScrollView(scrollVec);

            switch (toolbarIndex)
            {
                case 0: packageCreateTool.OnToolGUI(this); break;
                case 1: packageImportTool.OnToolGUI(this); break;
                case 2: packageEditTool.OnToolGUI(this); break;
                case 3: packageSampleTool.OnToolGUI(this); break;
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Git"))
            {
                var data = GitUtility.Scan();
                Debug.Log(data);
            }
        }
        #endregion

    }
}
