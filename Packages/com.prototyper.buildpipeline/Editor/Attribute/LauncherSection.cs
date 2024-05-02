namespace SS.Core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEditor;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class LauncherSection : Attribute
    {
        public string SectionName { get; set; }
        public LauncherSection(string str)
        {
            SectionName = str;
        }
    }
}
