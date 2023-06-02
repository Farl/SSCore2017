using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace SS.PackageHelper
{
    [System.Serializable]
    public class Author
    {
        public string name = null;
        public string email = null;
        public string url = null;
    }

    [System.Serializable]
    public class Sample
    {
        public string displayName = "";
        public string description = "";
        public string path = "Samples~/Sample1";
    }

    [System.Serializable]
    public class Repository
    {
        public string type = "git";
        public string url = "";
        public string revision = "";
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
        public string version = "0.0.1";
        public string displayName = "";
        public string description = "";
        public string unity = null; // Keep null to not limit minimal version
        public string unityRelease = null;
        public string documentationUrl = null;
        public string changelogUrl = null;
        public string licensesUrl = null;
        public Dictionary<string, string> dependencies = new Dictionary<string, string>();

        [System.NonSerialized]
        public string[] _dependencies_keys = new string[] { };

        [System.NonSerialized]
        public string[] _dependencies_values = new string[] { };

        public string[] keywords = new string[] { };
        public Author author = new Author();
        public bool hideInEditor = false;
        public string license;
        public Sample[] samples = new Sample[] { };
        public Repository repository = new Repository();

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
            if (string.IsNullOrEmpty(unity))
                unity = null;
            if (string.IsNullOrEmpty(unityRelease))
                unityRelease = null;
        }

        /// <summary>
        /// Serialize current object to json
        /// </summary>
        /// <returns></returns>
        public string SerializeObject()
        {
            OnBeforeSerialize();
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            Debug.Log(json);
            return json;
        }

        /// <summary>
        /// Override current object with json data
        /// </summary>
        /// <param name="json"></param>
        public void PopulateObject(string json)
        {
            Debug.Log(json);
            JsonConvert.PopulateObject(json, this);
            OnAfterDeserialize();
        }
    }
}
