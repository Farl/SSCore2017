/**
 * Editor World
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace SS.Core
{
    [InitializeOnLoad]
    public class EditorWorld:
        IPreprocessBuild
    {
        static EditorWorld()
        {
            EditorApplication.update += Update;
        }

        public int callbackOrder
        {
            get
            { return 0; }
        }

        static void Update()
        {
            // Prepare runtime player settings
            RuntimePlayerSettings setting = RuntimePlayerSettings.Instance;

            if (setting != null)
            {
                // Update scenes
                if (setting.scenes == null)
                    setting.scenes = new List<string>();
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                int sceneCount = scenes.Length;

                while (setting.scenes.Count < sceneCount)
                {
                    setting.scenes.Add(string.Empty);
                }
                while (setting.scenes.Count > sceneCount)
                {
                    setting.scenes.RemoveAt(sceneCount);
                }
                for (int i = 0; i < sceneCount; i++)
                {
                    if (scenes[i].path != setting.scenes[i])
                    {
                        setting.scenes[i] = scenes[i].path;
                    }
                }
            }
        }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            // Prepare runtime player settings
            RuntimePlayerSettings setting = RuntimePlayerSettings.Instance;

            // Update machine name
            setting.userName = System.Environment.UserName;

            // Update build time
            setting.buildTime = System.DateTime.Now.ToString();
        }
    }
}
