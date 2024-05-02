using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class XRBuildSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        public Dictionary<XRDevice, XRDeviceSettings> settings = new Dictionary<XRDevice, XRDeviceSettings>();
        [SerializeField] private List<XRDevice> settingsKey;
        [SerializeField] private List<XRDeviceSettings> settingsValue;

        public void OnAfterDeserialize()
        {
            settings = new Dictionary<XRDevice, XRDeviceSettings>();
            for (int i = 0; i < settingsKey.Count; i++)
            {
                settings.Add(settingsKey[i], settingsValue[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            settingsKey = new List<XRDevice>(settings.Keys);
            settingsValue = new List<XRDeviceSettings>(settings.Values);
        }
    }

    [System.Serializable]
    public class XRDeviceSettings
    {
        public List<string> featureSet = new List<string>();
        public List<string> features = new List<string>();
    }

}