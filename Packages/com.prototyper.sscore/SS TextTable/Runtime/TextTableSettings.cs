using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SS
{
    public class TextTableSettings : ProjectSettingsObject<TextTableSettings>, ISerializationCallbackReceiver
    {
        [Serializable]
        public class Data
        {
            public string guid;
            public string cacheName;
        }

        [SerializeField]
        private List<Data> dataList = new List<Data>();

        public Dictionary<string, Data> dataMap = new Dictionary<string, Data>();

        protected override void OnCreate()
        {
            
        }

        public void Add(string guid, string name)
        {
            if (dataMap.ContainsKey(guid))
            {
            }
            else
            {
                dataMap.Add(guid, new Data()
                {
                    guid = guid,
                    cacheName = name,
                });
            }
        }
        public void Remove(string guid)
        {
            if (dataMap.ContainsKey(guid))
            {
                dataMap.Remove(guid);
            }
        }

        public void OnBeforeSerialize()
        {
            dataList.Clear();
            foreach (var kvp in dataMap)
            {
                dataList.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            dataMap.Clear();
            foreach (var d in dataList)
            {
                if (dataMap.ContainsKey(d.guid))
                {
                    Debug.LogError($"Duplicate GUID {d.guid}");
                }
                else
                {
                    dataMap.Add(d.guid, d);
                }
            }
        }

#if UNITY_EDITOR

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return RegisterSettingsProvider();
        }
#endif
    }

}
