using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    [System.Serializable]
    public class UIOrderSetting
    {
        [System.Serializable]
        public class UIOrderData
        {
            public string uiType;
            public int order;
        }
        public List<UIOrderData> data = new List<UIOrderData>();

        public static explicit operator UIOrderSetting(UnityEngine.Object v)
        {
            throw new NotImplementedException();
        }
    }

    public class UIManager : MonoBehaviour
    {
        public UIOrderSetting orderSetting;
    }
}

