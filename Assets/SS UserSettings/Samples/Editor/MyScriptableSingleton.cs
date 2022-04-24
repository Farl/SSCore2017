using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[FilePath("SS/SSUserPreference.asset", FilePathAttribute.Location.PreferencesFolder)]
[FilePath("Temp/SS/SSUserPreference.asset", FilePathAttribute.Location.ProjectFolder)]
public class SSUserPreference : ScriptableSingleton<SSUserPreference>
{
    [SerializeField]
    float m_Number = 42;

    [SerializeField]
    List<string> m_Strings = new List<string>();

    public void Modify()
    {
        m_Number *= 2;
        m_Strings.Add("Foo" + m_Number);

        Save(true);
        Debug.Log("Saved to: " + GetFilePath());
    }

    public void Log()
    {
        Debug.Log("SSUserPreference state: " + JsonUtility.ToJson(this, true));
    }

    internal static SSUserPreference GetOrCreateSettings()
    {
        instance.hideFlags &= ~HideFlags.NotEditable;
        var settings = instance;
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}


static class MySingletonMenuItems
{
    [MenuItem("SSUserPreference/Log")]
    static void LogMySingletonState()
    {
        SSUserPreference.instance.Log();
    }

    [MenuItem("SSUserPreference/Modify")]
    static void ModifyMySingletonState()
    {
        SSUserPreference.instance.Modify();
    }
}


// Register a SettingsProvider using IMGUI for the drawing framework:
static class SSUserPreferenceIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateSSUserPreferenceSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Preference window.
        var provider = new SettingsProvider("SS/SSUserPreference", SettingsScope.User)
        {
            // By default the last token of the path is used as display name if no label is provided.
            label = "SS User Preference",

            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
            guiHandler = (searchContext) =>
            {
                var settings = SSUserPreference.GetSerializedSettings();
                EditorGUILayout.PropertyField(settings.FindProperty("m_Number"), new GUIContent("My Number"));
                EditorGUILayout.PropertyField(settings.FindProperty("m_Strings"), new GUIContent("My String"));
                settings.ApplyModifiedProperties();
            },

            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] { "Number", "Strings" })
        };

        return provider;
    }
}