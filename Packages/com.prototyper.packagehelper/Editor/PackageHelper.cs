using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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
            public string version = "0.0.1";
            public string description = "Description";
        }
        private static string packageName;
        private void OnGUI()
        {
            packageName = EditorGUILayout.TextField(packageName);
            if (GUILayout.Button("Create"))
            {
                var fullPackageName = $"com.prototyper.{packageName}";
                var assetPath = Application.dataPath;
                var assetDirInfo = new DirectoryInfo(assetPath);
                var projectPath = assetDirInfo.Parent.ToString();
                var packagePath = Path.Combine(projectPath, "Packages");
                var newPackagePath = Path.Combine(packagePath, fullPackageName);
                Directory.CreateDirectory(newPackagePath);
                var newPackageJsonPath = Path.Combine(newPackagePath, $"package.json");
                using (var sw = File.CreateText(newPackageJsonPath))
                {
                    var pd = new PackageData(fullPackageName);
                    var json = UnityEditor.EditorJsonUtility.ToJson(pd);
                    sw.Write(json);
                    sw.Close();
                    AssetDatabase.Refresh();
                }
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
