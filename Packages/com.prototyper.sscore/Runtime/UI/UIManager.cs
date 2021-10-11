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
    }

    public class UIManager : Singleton<UIManager>
    {
        public UIOrderSetting orderSetting;
        private Dictionary<string, int> orderMap = new Dictionary<string, int>();

        protected override void Awake()
        {
            base.Awake();
            foreach (UIOrderSetting.UIOrderData ud in orderSetting.data)
            {
                orderMap.Add(ud.uiType, ud.order);
            }
        }

        public int GetOrder(string uiType)
        {
            if (orderMap.ContainsKey(uiType))
                return orderMap[uiType];
            return -1;
        }

        private void OnValidate()
        {
            int index = 0;
            foreach (UIOrderSetting.UIOrderData ud in orderSetting.data)
            {
                ud.order = index++;
            }
        }
    }
}

