using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    /**
     * DebugMenuFollow
     * This class is used to follow the camera position and rotation when the debug menu is active.
     **/
    public class DebugMenuFollow : EntityBase
    {
        public float distance = 1f;
        protected override void OnEntityAwake()
        {
            base.OnEntityAwake();
            DebugMenu.onMenuToggle += OnMenuToggle;
        }

        private void OnMenuToggle(bool isOn)
        {
            if (isOn)
            {
                //transform.rotation = Camera.main.transform.rotation;
                transform.position = Camera.main.transform.position;
                transform.position += Camera.main.transform.forward * distance;
                transform.LookAt(transform.position + Camera.main.transform.forward, Vector3.up);
            }
        }

        protected override void OnEntityDestroy()
        {
            base.OnEntityDestroy();
            DebugMenu.onMenuToggle -= OnMenuToggle;
        }
    }

}