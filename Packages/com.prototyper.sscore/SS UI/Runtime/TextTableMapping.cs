using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SS
{
    public class TextTableMapping : MonoBehaviour, ITextTableLanguage
    {
        [SerializeField]
        private string textID;

        [SerializeField]
        private Text textComp;

        public void OnTextTableLanguageChanged(SystemLanguage language, SystemLanguage origLanguage)
        {
            SetupText();
        }

        void SetupText()
        {
            if (textComp == null)
            {
                textComp = GetComponent<Text>();
            }

            if (textComp != null)
            {
                if (TextTable.TryGetText(textID, out var result))
                {
                    textComp.text = result;
                }
                else
                {
                    textComp.text = textID;
                }
            }
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            // Register to TextTable
            TextTable.Register(this);
            SetupText();
        }
    }
}
