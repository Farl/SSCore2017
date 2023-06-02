using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace SS.PackageHelper
{
    public class PackageSampleTool : PackageTool
    {
        private List<string> packagePaths = new List<string>();
        private bool needRefresh = false;

        public override GUIContent toolbarIcon
        {
            // Use icon "d_FolderOpened Icon"
            get { return new GUIContent("", EditorGUIUtility.IconContent("d_Project").image, "Samples"); }
        }

        private void Refresh()
        {
            packagePaths.Clear();
            // Find all "Samples" and "Samples~" folders in Packages folder
            var packagesFolder = Application.dataPath + "/../Packages";
            var packageFolders = Directory.GetDirectories(packagesFolder, "Samples*", SearchOption.AllDirectories);
            foreach (var folder in packageFolders)
            {
                packagePaths.Add(folder);
            }
            needRefresh = false;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Refresh();
        }

        private void ToggleSampleFolder(string path, string parentFolder, bool isHidden)
        {
            // Move all files and folders in path to path~ or vice versa
            string newPath = Path.Combine(parentFolder, (isHidden ? "Samples" : "Samples~"));

            // Get all files and folders in path
            var files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
            var folders = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            // Check if new path exists, if not create it
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            // Move all files and folders to new path
            foreach (var p in files)
            {
                var newPathInPackageFolder = p.Replace(path, newPath);
                Debug.Log($"[File] Moving {p} to {newPathInPackageFolder}");
                try
                {
                    File.Move(p, newPathInPackageFolder);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            foreach (var p in folders)
            {
                var newPathInPackageFolder = p.Replace(path, newPath);
                Debug.Log($"[Folder] Moving {p} to {newPathInPackageFolder}");
                try
                {
                    Directory.Move(p, newPathInPackageFolder);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            if (files.Length > 0 && folders.Length > 0)
                needRefresh = true;
        }

        public override void OnToolGUI(EditorWindow window)
        {
            bool showAll = false;
            bool hideAll = false;
            bool removeEmpty = false;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show All"))
            {
                showAll = true;
            }
            if (GUILayout.Button("Hide All"))
            {
                hideAll = true;
            }
            if (GUILayout.Button("Remove Empty"))
            {
                removeEmpty = true;
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Refresh"))
            {
                Refresh();
            }

            foreach (var path in packagePaths)
            {
                var filename = Path.GetFileName(path);
                bool isHidden = filename.Contains("~");
                var pathInPackageFolder = path.Replace(Application.dataPath + "/../Packages/", "");
                var parentFolder = Path.GetDirectoryName(path);
                var buttonName = isHidden ? "Show" : "Hide";

                if (removeEmpty)
                {
                    // Remove empty folders
                    var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                    if (files.Length == 0)
                    {
                        Debug.Log($"Removing empty folder {path}");
                        Directory.Delete(path, true);
                        File.Delete($"{path}.meta");
                        needRefresh = true;
                    }
                }
                else if (showAll && isHidden)
                {
                    ToggleSampleFolder(path, parentFolder, isHidden);
                }
                else if (hideAll && !isHidden)
                {
                    ToggleSampleFolder(path, parentFolder, isHidden);
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    GUI.backgroundColor = isHidden ? new Color(0.4f, 0.4f, 0.8f) : Color.white;
                    if (GUILayout.Button("P", GUILayout.Width(20)))
                    {
                        // Get pathInPackageFolder root folder
                        var pathRoot = pathInPackageFolder.Split('/')[0];
                        // Select packageJson file in Project window
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>($"Packages/{pathRoot}/package.json");
                    }
                    if (GUILayout.Button(pathInPackageFolder))
                    {
                        if (isHidden)
                        {
                            // Reveal in finder
                            EditorUtility.RevealInFinder(path.Replace(Application.dataPath + "/../", ""));
                        }
                        else
                        {
                            // Select package folder in Project window
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>($"Packages/{pathInPackageFolder}");
                        }
                    }
                    if (GUILayout.Button(buttonName, GUILayout.Width(100)))
                    {
                        ToggleSampleFolder(path, parentFolder, isHidden);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            GUI.backgroundColor = Color.white;
            
            if (needRefresh)
            {
                RefreshPackages();
                Refresh();
            }

        }
    }
}
