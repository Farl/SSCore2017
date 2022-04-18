using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace SS
{
    public static class DirectoryUtility
    {
        public static string ProjectPath
        {
            get {
                return Path.GetDirectoryName(Application.dataPath);
                // return (new DirectoryInfo(Application.dataPath)).Parent.ToString();
            }

        }

        public static void CheckParentFolderRecursive(DirectoryInfo directoryInfo)
        {
            //Debug.Log(directoryInfo);
            if (directoryInfo == null || string.IsNullOrEmpty(directoryInfo.ToString()))
                return;

            if (!Directory.Exists(directoryInfo.ToString()))
            {
                var parentDirectoryInfo = directoryInfo.Parent;
                CheckParentFolderRecursive(parentDirectoryInfo);
                var parentPath = parentDirectoryInfo.ToString().Replace(ProjectPath, "").TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                //Debug.Log($"{parentPath} create {directoryInfo.Name}");
#if UNITY_EDITOR
                AssetDatabase.CreateFolder(parentPath, directoryInfo.Name);
#else
                Directory.CreateDirectory(Path.Combine(parentPath, directoryInfo.Name));
#endif
            }

        }
        public static void CheckAndCreateDirectory(string path)
        {
            CheckParentFolderRecursive(new DirectoryInfo(path));
        }
    }
}
