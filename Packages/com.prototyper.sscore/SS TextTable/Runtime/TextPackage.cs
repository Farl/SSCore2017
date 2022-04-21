using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace SS
{
    // Should in the same name script to use ScriptableObject.CreateInstance
    public class TextPackage : ScriptableObject, ISerializationCallbackReceiver
    {
        public List<string> referenceFileGUIDs = new List<string>();
        public Dictionary<string, TextTableData> data = new Dictionary<string, TextTableData>();

        public string fileName
        {
            get
            {
                string l = (isGlobal) ? "Global" : language.ToString();
                return $"{packageName}_{l}";
            }
        }
        public string packageName;
        public bool isGlobal = false;
        public SystemLanguage language;

        public List<TextTableData> Value = new List<TextTableData>();

        public void AddGUID(string guid)
        {
            if (referenceFileGUIDs.Contains(guid))
                return;
            referenceFileGUIDs.Add(guid);
        }

        public void RemoveGUID(string guid)
        {
            referenceFileGUIDs.Remove(guid);
        }

        public void OnBeforeSerialize()
        {
            Value.Clear();
            foreach (var kvp in data)
            {
                Value.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            data.Clear();
            foreach (var v in Value)
            {
                if (!data.ContainsKey(v.textID))
                {
                    data.Add(v.textID, v);
                }
                else
                {
                    Debug.LogError(@"Duplicate textID {v.textID} = {v.text}");
                }
            }
        }
    }

    [Serializable]
    public class TextTableData
    {
        public string textID;

        // Metadata
        public string speaker;
        public string type;

        public string text;

        public TextTableData ShallowCopy()
        {
            return (TextTableData)this.MemberwiseClone();
        }
    }
}