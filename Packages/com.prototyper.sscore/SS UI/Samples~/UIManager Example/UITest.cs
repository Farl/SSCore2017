using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Core
{
    public class UITest : MonoBehaviour
    {
        public void OpenUI(string uiType)
        {
            var uiBase = UISystem.Get<UIBase>(uiType);
            if (uiBase == null)
                return;
            
            if (uiBase.IsShow)
                uiBase.Hide();
            else
                uiBase.Show();
        }
    }
}
