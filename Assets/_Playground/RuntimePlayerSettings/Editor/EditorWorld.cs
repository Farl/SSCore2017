/**
 * Editor World
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace SS
{
    [InitializeOnLoad]
    public class EditorWorld
    {
        static EditorWorld()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            RuntimePlayerSettings setting = RuntimePlayerSettings.Instance;
            if (setting.scenes == null)
                setting.scenes = new List<string>();


            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            int sceneCount = scenes.Length;

            while(setting.scenes.Count < sceneCount)
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
}
