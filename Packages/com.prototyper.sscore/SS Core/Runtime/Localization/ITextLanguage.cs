using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Core
{
    public interface ITextLanguage
    {
        public void OnTextLanguageChanged(SystemLanguage language, SystemLanguage origLanguage);
    }
}
