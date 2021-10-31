using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Scripting;

namespace SS.PackageHelper
{
    public static class PackageHelper
    {
    }

    public class PackageCreator: EditorWindow
    {
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

        private static string packageName;
        private static string organizationName = "Prototyper";
        private static string prefixName = "SS";
        private static bool addEditorFolder = true;
        private static bool addRuntimeFolder = true;


        private void OnGUI()
        {
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

                Close();
            }
        }
        [MenuItem("Assets/PackageHelper/Create Package")]
        public static void CreateNewPackage()
        {
            var window = EditorWindow.CreateWindow<PackageCreator>();
        }
    }
}
