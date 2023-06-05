using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

namespace SS
{
    public class AnimatorControllerHelper
    {
        private static List<string> pathList = new List<string>();

        private static void ClearPathList()
        {
            pathList.Clear();
        }

        private static void RefreshPathList()
        {
            foreach (var path in pathList)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
            }
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            ClearPathList();
        }

        [MenuItem("Assets/SS/Add clip to controller")]
        public static void AddClipToController()
        {
            ClearPathList();
            foreach (var obj in Selection.objects)
            {
                if (obj.GetType() == typeof(AnimatorController))
                {
                    var clip = new AnimationClip();
                    clip.name = clip.GetInstanceID().ToString();
                    AssetDatabase.AddObjectToAsset(clip, obj);
                    pathList.Add(AssetDatabase.GetAssetPath(obj));
                }
            }
            RefreshPathList();
        }

        [MenuItem("Assets/SS/Extract clip from controller")]
        public static void ExtractClipFromController()
        {
            ClearPathList();
            foreach (var obj in Selection.objects)
            {
                AnimationClip ac = obj as AnimationClip;
                if (ac != null)
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    pathList.Add(path);
                    var folderPath = (new DirectoryInfo(path)).Parent.ToString();
                    folderPath = Path.GetRelativePath(SS.DirectoryUtility.ProjectPath, folderPath);
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    AssetDatabase.RemoveObjectFromAsset(obj);
                    AssetDatabase.CreateAsset(obj, $"{folderPath}/{fileName}_{obj.name}.anim");
                }
            }
            RefreshPathList();
        }

        [MenuItem("Assets/SS/Merge clips into controller")]
        public static void MergeClipsIntoController()
        {
            ClearPathList();
            AnimatorController ac = null;
            foreach (var obj in Selection.objects)
            {
                if (obj.GetType() == typeof(AnimatorController))
                {
                    if (ac == null)
                    {
                        ac = obj as AnimatorController;
                        pathList.Add(AssetDatabase.GetAssetPath(obj));
                    }
                    else
                    {
                        Debug.LogError($"Multiple controllers in selection");
                        return;
                    }
                }
            }
            if (ac != null)
            {
                foreach (var obj in Selection.objects)
                {
                    if (obj.GetType() == typeof(AnimationClip))
                    {
                        AnimationClip clip = Object.Instantiate(obj) as AnimationClip;
                        clip.name = obj.name.Replace($"{ac.name}_", string.Empty);
                        AssetDatabase.AddObjectToAsset(clip, ac);

                        var path = AssetDatabase.GetAssetPath(obj);
                        AssetDatabase.DeleteAsset(path);
                    }
                }
            }
            RefreshPathList();
        }
    }

}
