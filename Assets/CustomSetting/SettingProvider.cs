using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SS;

// Create a new type of Settings Asset.
class MyCustomSettings : SettingObject<MyCustomSettings>
{
    [SerializeField]
    private int m_Number;

    [SerializeField]
    private string m_SomeString;

    protected override void OnCreate()
    {
        m_Number = 42;
        m_SomeString = "The answer to the universe";
    }

    [MenuItem("Test/Test Create Asset")]
    public static void TestCreateAsset()
    {
        var path = $"Assets/Test/Test/Test/Test.asset";
        var so = ScriptableObject.CreateInstance<MyCustomSettings>();
        DirectoryUtility.CheckAndCreateDirectory(path);

        AssetDatabase.CreateAsset(so, path);
    }

    // Register a SettingsProvider using IMGUI for the drawing framework:
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        return SettingIMGUIRegister.Register<MyCustomSettings>("Custom Setting", new HashSet<string>(new[] { "Number", "Some String" }));
    }
}

namespace SS
{
    public abstract class SettingObject<T> : ScriptableObject where T : SettingObject<T>
    {
        protected static string path { get { return $"Assets/Settings/Resources/{typeof(T).ToString()}.asset"; } }
        protected abstract void OnCreate();

        public static T GetOrCreateSettings()
        {
            T settings = AssetDatabase.LoadAssetAtPath<T>(path);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<T>();
                settings.OnCreate();

                DirectoryUtility.CheckAndCreateDirectory(path);
                AssetDatabase.CreateAsset(settings, path);

                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        protected static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    public static class SettingIMGUIRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keywords">Populate the search keywords to enable smart search filtering and label highlighting:</param>
        public static SettingsProvider Register<T>(string label = null, HashSet<string> keywords = null) where T : SettingObject<T>
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider($"Project/{typeof(T).ToString()}", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = label,

                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    SerializedObject settings = null;

                    var t = typeof(T);
                    var mi = t.GetMethod("GetSerializedSettings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy);
                    if (mi != null)
                    {
                        settings = (SerializedObject)mi.Invoke(null, null);

                        var itr = settings.GetIterator();
                        if (itr.Next(true)) // first element
                        {
                            do
                            {
                                EditorGUILayout.PropertyField(itr);
                            }
                            while (itr.NextVisible(false));
                        }

                        settings.ApplyModifiedProperties();
                    }
                },

                keywords = keywords
            };
            return provider;
        }
    }
}
