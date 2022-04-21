using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Create a new type of Settings Asset.
class MyCustomSettings : ProjectSettingsObject<MyCustomSettings>
{
    #region Public
    public int Number => m_Number;
    #endregion

    [SerializeField]
    private int m_Number;

    [SerializeField]
    private string m_SomeString;

    protected override void OnCreate()
    {
        m_Number = 42;
        m_SomeString = "The answer to the universe";
    }

#if UNITY_EDITOR
    // Register a SettingsProvider using IMGUI for the drawing framework:
    [SettingsProvider]
    public static SettingsProvider CreateSettingsProvider()
    {
        //return RegisterSettingsProvider("Custom Setting", new HashSet<string>(new[] { "Number", "Some String" }));
        return RegisterSettingsProvider(label: null, keywords: null);
    }
#endif
}