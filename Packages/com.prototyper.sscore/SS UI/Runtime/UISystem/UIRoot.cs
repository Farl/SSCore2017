using UnityEngine;
using System.Collections.Generic;

namespace SS
{
    public class UIRoot : InitBehaviour
    {
        public Canvas canvas;

        public static Dictionary<string, Canvas> canvasList = new Dictionary<string, Canvas>();

        protected override void Awake()
        {
            if (canvas)
                canvasList.Add(canvas.name, canvas);

            base.Awake();
        }

        private void OnDestroy()
        {
            if (canvas)
                canvasList.Remove(canvas.name);
        }

        public override void OnInit()
        {
            UIBase[] uiBases = GetComponentsInChildren<UIBase>();
            List<UIBase> uiList = new List<UIBase>();

            foreach (UIBase uiBase in uiBases)
            {
                uiBase.Init();
                uiList.Add(uiBase);
            }

            // OrderSetting
            if (UIManager.IsAlive)
            {
                uiList.Sort((x, y) => UIManager.Instance.GetOrder(x.uiType).CompareTo(UIManager.Instance.GetOrder(y.uiType)));
                foreach (UIBase uiBase in uiList)
                {
                    uiBase.transform.SetAsLastSibling();
                }
            }
        }

        public override int InitOrder
        {
            get
            {
                return 0;
            }
        }
    }
}