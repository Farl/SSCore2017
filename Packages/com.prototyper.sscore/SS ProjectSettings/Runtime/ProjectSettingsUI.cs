#if UNITY_EDITOR
namespace SS
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class ProjectSettingsUI
    {
        private static T GetInstance<T>() where T : ProjectSettingsObject<T>
        {
            var t = typeof(T);
            var instanceProperty = t.GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
            if (instanceProperty != null)
            {
                T settings = (T)instanceProperty.GetValue(null);
                return settings;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label">By default the last token of the path is used as display name if no label is provided.</param>
        /// <param name="keywords">Populate the search keywords to enable smart search filtering and label highlighting:</param>
        public static SettingsProvider Register<T>(string label = null, HashSet<string> searchKeywords = null) where T : ProjectSettingsObject<T>
        {
            var typeName = typeof(T).ToString();
            var settings = GetInstance<T>();
            SerializedObject so = (settings != null)? new SerializedObject(settings): null;

            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider($"SS Project/{typeName}", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = (string.IsNullOrEmpty(label)? $"{typeName}": label),

                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    if (settings != null)
                    {
                        if (GUILayout.Button($"{AssetDatabase.GetAssetPath(settings)}"))
                        {
                            Selection.activeObject = settings;
                        }
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
                },
            };
            // Keywords
            if (searchKeywords != null)
            {
                provider.keywords = searchKeywords;
            }
            else
            {
                // Add keywords automatically
                var tokens = new HashSet<string>();
                var itr = so.GetIterator();
                if (itr.Next(true)) // first element
                {
                    do
                    {
                        var k = itr.displayName;
                        if (!tokens.Contains(k))
                            tokens.Add(k);
                    }
                    while (itr.NextVisible(false));
                }
                provider.keywords = tokens;
            }
            return provider;
        }
    }
}
#endif