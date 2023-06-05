using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    [CreateAssetMenu(fileName ="TextTableSettings", menuName = "HG/Settings/Text Table Settings", order = 1)]
    public class TextTableSettings : ScriptableObject
    {
        public List<SystemLanguage> supportedLanguage = new List<SystemLanguage>();
        public List<SystemLanguage> editorSupportedLanguage = new List<SystemLanguage>();
    }
}
