using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SS
{
    public static class TextTable
    {
        public static void Load(string name)
        {

        }

        public static void Unload(string name)
        {

        }

        public static bool TryGetText(string textID, out string result)
        {
            result = null;
            if (string.IsNullOrEmpty(textID))
                return false;
            result = textID;
            return true;
        }
    }
}