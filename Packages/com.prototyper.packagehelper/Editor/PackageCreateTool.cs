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

    public class PackageCreateTool : PackageTool
    {
        #region Enums & Classes

        private class AssemblyDefinitionReferenceAsset
        {
            public AssemblyDefinitionReferenceAsset(string _name, bool isEditor)
            {
                name = _name;
                if (isEditor)
                {
                    includePlatforms = new string[] { "Editor" };
                }
            }
            public string name = "";
            public string rootNamespace = "";
            public string[] includePlatforms;
        }

        #endregion

        private static string packageName;
        private static string organizationName = "Prototyper";
        private static string prefixName = "SS";
        private static bool addEditorFolder = true;
        private static bool addRuntimeFolder = true;
        private string userName;
        private string userEmail;
        private GitUtility.GitConfigData gitConfigData;

        override public GUIContent toolbarIcon
        {
            // Use create icon
            get { return new GUIContent("", EditorGUIUtility.IconContent("CreateAddNew").image, "Create"); }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            gitConfigData = GitUtility.Scan();
            if (!string.IsNullOrEmpty(gitConfigData.userName))
            {
                userName = gitConfigData.userName;
            }
            else
            {
                userName = System.Environment.UserName;
            }
            if (!string.IsNullOrEmpty(gitConfigData.userEmail))
            {
                userEmail = gitConfigData.userEmail;
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);

            packageName = EditorGUILayout.TextField("Package Name", packageName);
            organizationName = EditorGUILayout.TextField("Organization", organizationName);
            prefixName = EditorGUILayout.TextField("Prefix", prefixName);

            addEditorFolder = EditorGUILayout.Toggle("Add Editor folder", addEditorFolder);
            addRuntimeFolder = EditorGUILayout.Toggle("Add Runtime folder", addRuntimeFolder);

            userName = EditorGUILayout.TextField("User Name", userName);
            userEmail = EditorGUILayout.TextField("User Email", userEmail);

            if (GUILayout.Button("Create"))
            {
                var fullPackageName = $"com.{organizationName.ToLower()}.{packageName.ToLower()}";
                var assetPath = Application.dataPath;
                var assetDirInfo = new DirectoryInfo(assetPath);
                var projectPath = assetDirInfo.Parent.ToString();
                var packagePath = Path.Combine(projectPath, "Packages");
                var newPackagePath = Path.Combine(packagePath, fullPackageName);
                Directory.CreateDirectory(newPackagePath);
                var newPackageJsonPath = Path.Combine(newPackagePath, $"package.json");
                using (var sw = File.CreateText(newPackageJsonPath))
                {
                    var pd = new PackageData()
                    {
                        name = fullPackageName,
                        displayName = $"{prefixName} {packageName}",
                        description = packageName,
                        author = new Author(){ name = userName, email = userEmail },
                    };
                    sw.Write(pd.SerializeObject());
                    sw.Close();
                }

                var asmDefName = $"{prefixName}.{packageName}";

                if (addEditorFolder)
                {
                    var editorPath = Path.Combine(newPackagePath, "Editor");
                    Directory.CreateDirectory(editorPath);

                    var editorAsmDefName = $"{asmDefName}.Editor";
                    var asmDefPath = Path.Combine(editorPath, $"{editorAsmDefName}.asmdef");
                    using (var sw = File.CreateText(asmDefPath))
                    {
                        var asmDef = new AssemblyDefinitionReferenceAsset($"{editorAsmDefName}", true);
                        var json = UnityEditor.EditorJsonUtility.ToJson(asmDef);
                        sw.Write(json);
                        sw.Close();
                    }
                }
                if (addRuntimeFolder)
                {
                    var runtimePath = Path.Combine(newPackagePath, "Runtime");
                    Directory.CreateDirectory(runtimePath);

                    var runtimeAsmDefName = $"{asmDefName}";
                    var asmDefPath = Path.Combine(runtimePath, $"{runtimeAsmDefName}.asmdef");
                    using (var sw = File.CreateText(asmDefPath))
                    {
                        var asmDef = new AssemblyDefinitionReferenceAsset($"{runtimeAsmDefName}", false);
                        var json = UnityEditor.EditorJsonUtility.ToJson(asmDef);
                        sw.Write(json);
                        sw.Close();
                    }
                }

                // Refresh packages
                RefreshPackages();
            }
        }
    }
}
