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
            //get { return new GUIContent("", EditorGUIUtility.IconContent("CloudConnect@2x").image, "Import"); }
            get { return new GUIContent("", EditorGUIUtility.IconContent("Import-Available").image, "Import"); }
        }

        private string outputUrl = "";
        private string version = "";
        private string inputUrl = "git@github.com:USERNAME/REPOSITORY.git";
        private string path = "/Packages/FOLDER_NAME";

        private string prefix = "";

        private AddRequest request;
        private GitUtility.GitConfigData gitConfigData;

        public override void OnEnable()
        {
            base.OnEnable();
            gitConfigData = GitUtility.Scan();
            if (!string.IsNullOrEmpty(gitConfigData.currentRemoteUrl))
            {
                inputUrl = gitConfigData.currentRemoteUrl;
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            // A help button(with question mark icon) to open unity import custom package url
            if (GUILayout.Button(EditorGUIUtility.IconContent("_Help@2X"), GUILayout.Width(40)))
            {
                Application.OpenURL("https://docs.unity3d.com/Manual/upm-ui-giturl.html");
            }
            EditorGUILayout.EndHorizontal();

            version = EditorGUILayout.TextField("Version", version);
            path = EditorGUILayout.TextField("Path", path);
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            inputUrl = EditorGUILayout.TextField("Repo url", inputUrl);
            if (EditorGUI.EndChangeCheck())
            {
            }
            if (GUILayout.Button("Paste", GUILayout.Width(50)))
            {
                inputUrl = EditorGUIUtility.systemCopyBuffer;
            }
            if (GUILayout.Button("SSH/HTTPS", GUILayout.Width(100)))
            {
                inputUrl = ToggleURLStyle(inputUrl);
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Generate URL"))
            {
                var tmpUrl = inputUrl;
                if (inputUrl.StartsWith("git@"))
                {
                    prefix = "git+ssh://";
                    tmpUrl = tmpUrl.Replace(":", "/");
                }
                else
                    prefix = "";
                outputUrl = $"{prefix}{tmpUrl}?path={path}#{version}";
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

        private string ToggleURLStyle(string url)
        {
            // Toogle url between ssh style and https style
            if (url.StartsWith("git@"))
            {
                url = url.Replace(":", "/");
                url = url.Replace("git@", "https://");
            }
            else
            {
                url = url.Replace("https://", "git@");
            }
            return url;
        }
    }
}
