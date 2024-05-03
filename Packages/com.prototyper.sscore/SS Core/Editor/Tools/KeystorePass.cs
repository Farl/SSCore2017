using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SS
{

    public class KeystorePass : MonoBehaviour
    {
        [MenuItem("Tools/SS Core/Keystore/Set Prototyper")]
        public static void KeystorePrototyper()
        {
            SetKeystore("lifestyle");
        }

        [MenuItem("Tools/SS Core/Keystore/Set SS")]
        public static void KeystoreSS()
        {
            SetKeystore("ss");
        }

        public static void SetKeystore(string pwd)
        {
            PlayerSettings.Android.keyaliasPass = pwd;
            PlayerSettings.Android.keystorePass = pwd;
            PlayerSettings.keyaliasPass = pwd;
            PlayerSettings.keystorePass = pwd;
        }
    }

}