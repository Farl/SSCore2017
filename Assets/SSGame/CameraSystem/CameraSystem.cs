/**
 * Camera System
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class CameraSystem : Singleton<CameraSystem> {

        public Camera currCamera;

        protected override void Awake()
        {
            base.Awake();

            if (!currCamera)
            {
                currCamera = GetComponentInChildren<Camera>();
            }
        }
    }
}
