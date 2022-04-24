using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SS
{
    public class TextTableSampleGameMain : MonoBehaviour
    {
        public Text debugElement;

        private Application.LogCallback onLog;

        private void Awake()
        {
            if (debugElement != null)
            {
                debugElement.text = GetType().Name;
            }
            onLog = (condition, stackTrace, logType) =>
            {
                if (debugElement != null)
                {
                    var parent = debugElement.transform.parent;
                    var go = GameObject.Instantiate(debugElement.gameObject);
                    go.transform.SetParent(parent);
                    var t = go.GetComponent<Text>();
                    t.text = condition;
                    if (logType == LogType.Error || logType == LogType.Exception)
                        t.color = Color.red;
                }
            };
            Application.logMessageReceived += onLog;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= onLog;
        }

        // Start is called before the first frame update
        void Start()
        {
            TextTable.Initialize();
        }

        public void SetLanguage(string language)
        {
            if (language == "Auto")
                Localization.SetTextLanguage(Localization.LanguageType.Auto);
            else
            {
                if (System.Enum.TryParse<SystemLanguage>(language, out var lang))
                {
                    Localization.SetTextLanguage(Localization.LanguageType.SystemLanguage, lang);
                }
            }
        }
    }
}
