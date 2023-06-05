using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SS
{
    public class TextTablePackage : ScriptableObject, ISerializationCallbackReceiver
    {
        public List<TextTableData> dataList = new List<TextTableData>();
        private Dictionary<string, TextTableData> dictionary = new Dictionary<string, TextTableData>();

        public bool TryGetData(string key, out TextTableData data)
        {
            data = new TextTableData();
            return dictionary.TryGetValue(key, out data);
        }

        public void OnAfterDeserialize()
        {
            dictionary.Clear();
            foreach (var data in dataList)
            {
                dictionary.TryAdd(data.textID, data);
            }
        }

        public void OnBeforeSerialize()
        {
        }
    }
}
