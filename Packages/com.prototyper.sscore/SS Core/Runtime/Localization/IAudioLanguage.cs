using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Core
{
    public interface IAudioLanguage
    {
        public void OnAudioLanguageChanged(SystemLanguage language, SystemLanguage origLanguage);
    }
}
