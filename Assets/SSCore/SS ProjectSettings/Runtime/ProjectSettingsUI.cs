#if UNITY_EDITOR
namespace SS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class ProjectSettingsUI
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="label">By default the last token of the path is used as display name if no label is provided.</param>
        /// <param name="keywords">Populate the search keywords to enable smart search filtering and label highlighting:</param>
        public static SettingsProvider Register<T>(string label = null, HashSet<string> searchKeywords = null) where T : ProjectSettingsObject<T>
        {
            var typeName = typeof(T).ToString();

            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider($"SS Project/{typeName}", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = (string.IsNullOrEmpty(label)? $"{typeName}": label),

                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {

                    var t = typeof(T);
                    var instanceProperty = t.GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
                    if (instanceProperty != null)
                    {
                        T settings = (T)instanceProperty.GetValue(null);

                        if (settings != null)
                        {
                            if (GUILayout.Button($"{AssetDatabase.GetAssetPath(settings)}"))
                            {
                                Selection.activeObject = settings;
                            }

                            SerializedObject so = new SerializedObject(settings);
                            var itr = so.GetIterator();
                            if (itr.Next(true)) // first element
                            {
                                do
                                {
                                    EditorGUILayout.PropertyField(itr);
                                }
                                while (itr.NextVisible(false));
                            }

                            so.ApplyModifiedProperties();
                        }
                        else
                        {
                        }
                    }
                },
            };
            if (searchKeywords != null)
                provider.keywords = searchKeywords;
            return provider;
        }
    }
}
#endif