using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Core
{
    public class ButtonAttribute : PropertyAttribute
    {
        public string displayName;
        public string methodName;
        public float padding = 12.0f;
        public ButtonAttribute(string name, string method)
        {
            displayName = name;
            methodName = method;
        }
    }

}