using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SS
{
    public enum DialogPriority
    {
        Normal = 0,
        Low = -1,
        High = 1,
    }

    public class DialogRequestData
    {
        public int priority = 0;
        public int handle = -1;
        public string typeName;
        public bool autoClose = false;
        public bool delayButton = false;
        public string titleID;
        public string descID;
        public string soundEventID;
        public string speakerID;
        public DialogButtonSetup[] buttonSetup;
        public Action<DialogRequestData> onShow;
        public Action<DialogRequestData, bool> onHide;
        public Action<DialogRequestData> onSoundEnd;
        public DialogPanel currPanel;
        public object[] additionalData;
        public IDialogSystem dialogSystem;
        public float soundLength;
    }

    public class DialogButtonSetup
    {
        public string buttonText;
        public Action onButton;
    }
}
