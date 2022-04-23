using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{    public interface ITextTableLanguage
    {
        void OnTextTableLanguageChanged(SystemLanguage language, SystemLanguage origLanguage);
    }

}