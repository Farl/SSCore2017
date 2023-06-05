/**
 * Runtime player settings
 * scene list
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SS.Core
{
    public class RuntimePlayerSettings : DataAsset<RuntimePlayerSettings>
    {
        // Scene list in build settings
        public List<string> scenes = new List<string>();

        public string userName;

        public string buildTime;
    }
}