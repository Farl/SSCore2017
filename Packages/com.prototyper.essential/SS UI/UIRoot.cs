using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SS
{
    public class UIRoot : MonoBehaviour
    {
        private bool isSetup = false;
        private BaseRaycaster raycaster;
        private void Awake()
        {
            UIManager.Register(this);

            raycaster = GetComponent<BaseRaycaster>();
            if (raycaster)
            {
                DebugMenu.AddToggle("UI", "Enable Raycaster", () => raycaster.enabled, (b) => raycaster.enabled = b);
            }
        }

        private void OnDestroy()
        {
            UIManager.Unregister(this);

            DebugMenu.Remove("Enable Raycaster");
        }

        public void Setup()
        {
            if (isSetup)
            {
                return;
            }
            var entities = GetComponentsInChildren<UIEntity>(true);
            foreach (var e in entities)
            {
                // Force awake
                e.gameObject.SetActive(true);

                e.OnRootInitialize();
                isSetup = true;
            }
        }
    }
}
