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
    public class PackageImportTool : PackageTool
    {
        override public GUIContent toolbarIcon
        {
            // Use download icon
            get { return new GUIContent("", EditorGUIUtility.IconContent("CloudConnect@2x").image, "Import"); }
        }

        private string outputUrl = "";
        private string version = "";
        private string inputUrl = "git@github.com:USERNAME/REPOSITORY.git";
        private string path = "/Packages/FOLDER_NAME";

        private string prefix = "";

        private AddRequest request;

        public override void OnToolGUI(EditorWindow window)
        {
            if (!isActivated)
                return;
            base.OnToolGUI(window);

            // A help button(with question mark icon) to open unity import custom package url
            if (GUILayout.Button(EditorGUIUtility.IconContent("console.infoicon.sml")))
            {
                Application.OpenURL("https://docs.unity3d.com/Manual/upm-ui-giturl.html");
            }

            version = EditorGUILayout.TextField("Version", version);
            path = EditorGUILayout.TextField("Path", path);
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            inputUrl = EditorGUILayout.TextField("Repo url", inputUrl);
            if (EditorGUI.EndChangeCheck())
            {
                if (inputUrl.StartsWith("git@"))
                    prefix = "git+ssh://";
                else
                    prefix = "";
            }
            if (GUILayout.Button("Paste"))
            {
                inputUrl = EditorGUIUtility.systemCopyBuffer;
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Generate URL"))
            {
                outputUrl = $"{prefix}{inputUrl.Replace(":", "/")}?path={path}#{version}";
            }

            if (!string.IsNullOrEmpty(outputUrl))
            {
                EditorGUILayout.TextField(outputUrl);
                if (GUILayout.Button("Import package"))
                {
                    request = UnityEditor.PackageManager.Client.Add(outputUrl);
                    EditorApplication.update += Progress;

                }
                // Button to copy url to clipboard (button with icon)
                if (GUILayout.Button(EditorGUIUtility.IconContent("Clipboard").image))
                {
                    EditorGUIUtility.systemCopyBuffer = outputUrl;
                }
            }
        }

        void Progress()
        {
            if (request.IsCompleted)
            {
                if (request.Status == StatusCode.Success)
                    Debug.Log("Installed: " + request.Result.packageId);
                else if (request.Status >= StatusCode.Failure)
                    Debug.Log(request.Error.message);

                EditorApplication.update -= Progress;
            }
        }
    }
}
