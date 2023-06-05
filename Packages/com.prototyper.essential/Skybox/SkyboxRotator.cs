/**
 * by Farl
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS
{
    public class SkyboxRotator : MonoBehaviour
    {
        [SerializeField] private float speed = 0.1f;
        private Material skyboxMaterial;

        private IEnumerator Start()
        {
            var timer = 0f;
            while (true)
            {
                if (!skyboxMaterial)
                {
                    skyboxMaterial = new Material(RenderSettings.skybox);
                    RenderSettings.skybox = skyboxMaterial;
                }
                if (skyboxMaterial)
                {
                    skyboxMaterial.SetFloat("_Rotation", timer);
                }
                timer += Time.deltaTime * speed;
                yield return null;
            }
        }
    }
}