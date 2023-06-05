using UnityEngine;
using System.Collections;

namespace SS.Legacy
{
    public class SSFontColor : MonoBehaviour
    {
        public Color color;

        void Update()
        {
            GetComponent<Renderer>().material.color = color;
        }
    }

}
