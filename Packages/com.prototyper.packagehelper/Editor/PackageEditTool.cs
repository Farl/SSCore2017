using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace SS.PackageHelper
{
    /**
     * Input a json file, and read it as a package description object
     */
    public class PackageEditTool : PackageTool
    {
        [System.Serializable]
        public class Author
        {
            public string name = "";
            public string email = "";
            public string url = "";
        }

        [System.Serializable]
        public class PackageData : UnityEngine.ScriptableObject, ISerializationCallbackReceiver
        {
            /* Package example
                {
                    "name": "com.[company-name].[package-name]",
                    "version": "1.2.3",
                    "displayName": "Package Example",
                    "description": "This is an example package",
                    "unity": "2019.1",
                    "unityRelease": "0b5",
                    "documentationUrl": "https://example.com/",
                    "changelogUrl": "https://example.com/changelog.html",
                    "licensesUrl": "https://example.com/licensing.html",
                    "dependencies": {
                        "com.[company-name].some-package": "1.0.0",
                        "com.[company-name].other-package": "2.0.0"
                    },
                    "keywords": [
                        "keyword1",
                        "keyword2",
                        "keyword3"
                    ],
                    "author": {
                        "name": "Unity",
                        "email": "unity@example.com",
                        "url": "https://www.unity3d.com"
                    }
                }
            */
            public new string name = "";
            public string version = "";
            public string displayName = "";
            public string description = "";
            public string unity = "";
            public string unityRelease = "";
            public string documentationUrl = "";
            public string changelogUrl = "";
            public string licensesUrl = "";
            public Dictionary<string, string> dependencies = new Dictionary<string, string>();

            [System.NonSerialized]
            public string[] _dependencies_keys = new string[] { };

            [System.NonSerialized]
            public string[] _dependencies_values = new string[] { };

            public string[] keywords = new string[] { };
            public Author author = new Author();
            public bool hideInEditor = false;
            public string license;

            public void OnAfterDeserialize()
            {
                //Debug.Log("OnAfterDeserialize");
                _dependencies_keys = new string[dependencies.Count];
                _dependencies_values = new string[dependencies.Count];
                int i = 0;
                foreach (var kvp in dependencies)
                {
                    _dependencies_keys[i] = kvp.Key;
                    _dependencies_values[i] = kvp.Value;
                    i++;
                }
            }

            public void OnBeforeSerialize()
            {
                //Debug.Log("OnBeforeSerialize");
                dependencies.Clear();
                for (int i = 0; i < _dependencies_keys.Length; i++)
                {
                    dependencies.Add(_dependencies_keys[i], _dependencies_values[i]);
                }
            }
        }

        TextAsset jsonFile;
        PackageData packageData;
        SerializedObject serializedObject;

        public override void OnToolGUI(EditorWindow window)
        {
            if (!isActivated)
                return;
                
            base.OnToolGUI(window);
            jsonFile = EditorGUILayout.ObjectField("Json File", jsonFile, typeof(TextAsset), false) as TextAsset;
            if (jsonFile != null && GUILayout.Button("Read"))
            {
                ReadJson();
            }

            if (packageData == null || serializedObject == null)
                return;
            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            do
            {
                EditorGUILayout.PropertyField(prop);
            } while (prop.NextVisible(false));

            // Draw dependencies dictionary with tool buttons
            EditorGUILayout.LabelField("Dependencies");
            for (int i = 0; i < packageData._dependencies_keys.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                packageData._dependencies_keys[i] = EditorGUILayout.TextField(packageData._dependencies_keys[i]);
                packageData._dependencies_values[i] = EditorGUILayout.TextField(packageData._dependencies_values[i]);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    var list = new List<string>(packageData._dependencies_keys);
                    list.RemoveAt(i);
                    packageData._dependencies_keys = list.ToArray();
                    list = new List<string>(packageData._dependencies_values);
                    list.RemoveAt(i);
                    packageData._dependencies_values = list.ToArray();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+"))
            {
                var list = new List<string>(packageData._dependencies_keys);
                list.Add("");
                packageData._dependencies_keys = list.ToArray();
                list = new List<string>(packageData._dependencies_values);
                list.Add("");
                packageData._dependencies_values = list.ToArray();
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                serializedObject.Update();
            }

            if (GUILayout.Button("Save"))
            {
                SaveJson();
            }
        }

        void SaveJson()
        {
            if (jsonFile == null)
                return;
            if (packageData == null)
                return;

            packageData.OnBeforeSerialize();
            var json = JsonConvert.SerializeObject(packageData, Formatting.Indented);
            
            Debug.Log(json);
            System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(jsonFile), json);
            
            // refresh packages
            UnityEditor.PackageManager.Client.Resolve();
        }

        void ReadJson()
        {
            if (jsonFile == null)
                return;
            packageData = null;
            packageData = CreateInstance<PackageData>();

            Debug.Log(jsonFile.text);

            JsonConvert.PopulateObject(jsonFile.text, packageData);
            packageData.OnAfterDeserialize();

            serializedObject = new SerializedObject(packageData);
            serializedObject.Update();
        }
    }
}
