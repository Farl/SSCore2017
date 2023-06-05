using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS.TextTable
{    public interface ITextTableLanguage
    {
        void OnTextTableLanguageChanged(SystemLanguage language, SystemLanguage origLanguage);
    }

}