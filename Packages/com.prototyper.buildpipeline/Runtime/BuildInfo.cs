using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SS
{
    public class BuildInfo : ScriptableObject
    {
        public string buildTime;
        public string version;
        public string versionCode;
        public string userName;
        public string[] defineSymbols;
    }
}
