using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using System.IO;
using UnityEditor.Scripting;

namespace SS.PackageHelper
{
    public class PackageHelper: EditorWindow
    {

        #region Static
        private const string version = "0.0.1";
        private static string packageName;
        private static string organizationName = "Prototyper";
        private static string prefixName = "SS";
        private static bool addEditorFolder = true;
        private static bool addRuntimeFolder = true;
        #endregion
        
        #region Enums & Classes
        private class PackageData
        {
            public PackageData(string packageName)
            {
                name = packageName;
            }
            public string name = "";
            public string displayName = "";
            public string version = "0.0.1";
            public string description = "Description";
        }

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

        private class PackageTool : EditorTool
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

        private class PackageImportTool : PackageTool
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
                        UnityEditor.PackageManager.Client.Add(outputUrl);
                    }
                    // Button to copy url to clipboard (button with icon)
                    if (GUILayout.Button(EditorGUIUtility.IconContent("Clipboard").image))
                    {
                        EditorGUIUtility.systemCopyBuffer = outputUrl;
                    }
                }
            }
        }

        private class PackageCreateTool : PackageTool
        {
            override public GUIContent toolbarIcon
            {
                // Use create icon
                get { return new GUIContent("", EditorGUIUtility.IconContent("CreateAddNew").image, "Create"); }
            }

            public override void OnToolGUI(EditorWindow window)
            {
                if (!isActivated)
                    return;
                base.OnToolGUI(window);

                packageName = EditorGUILayout.TextField("Package Name", packageName);
                organizationName = EditorGUILayout.TextField("Organization", organizationName);
                prefixName = EditorGUILayout.TextField("Prefix", prefixName);

                addEditorFolder = EditorGUILayout.Toggle("Add Editor folder", addEditorFolder);
                addRuntimeFolder = EditorGUILayout.Toggle("Add Runtime folder", addRuntimeFolder);
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
                        var pd = new PackageData(fullPackageName)
                        { displayName = $"{prefixName} {packageName}" };
                        var json = UnityEditor.EditorJsonUtility.ToJson(pd);
                        sw.Write(json);
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

                    AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

                    window.Close();
                }
            }
        }
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

        private void OnEnable()
        {
            packageImportTool = new PackageImportTool();
            packageCreateTool = new PackageCreateTool();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox($"Package Helper {version}", MessageType.Info);

            // Select a tool by EditorToolbar and draw that tool
            EditorGUILayout.EditorToolbar(packageCreateTool, packageImportTool);

            packageCreateTool.OnToolGUI(this);
            packageImportTool.OnToolGUI(this);
        }
        #endregion

    }
}
