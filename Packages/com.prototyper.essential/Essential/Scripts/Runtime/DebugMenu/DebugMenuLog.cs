using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace SS
{
    public class DebugMenuLog : MonoBehaviour
    {
        public Button button;
        public TMP_Text log;
        public TMP_Text callStack;

        private void Awake()
        {
            if (button)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        private void OnClick()
        {
            if (log)
                GUIUtility.systemCopyBuffer = log.text;
            if (callStack)
            {
                callStack.gameObject.SetActive(!callStack.gameObject.activeSelf);
            }
        }

        private void OnDestroy()
        {
            if (button)
            {
                button.onClick.RemoveListener(OnClick);
            }
        }
    }
}
