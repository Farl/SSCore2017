using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class TextTableSampleGameMain : MonoBehaviour
    {
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

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (TextTable.TryGetText("UI_Confirm", out var result))
                {
                    Debug.Log(result);
                }
            }
        }
    }
}
