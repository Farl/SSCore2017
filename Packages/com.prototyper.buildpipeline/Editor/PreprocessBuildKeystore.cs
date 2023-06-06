using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

namespace SS
{
    public class PreprocessBuildKeystore : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        private const string KEYSTORES_FILE = "keystores.ini";
        public void OnPreprocessBuild(BuildReport report)
        {
#if UNITY_ANDROID
            if (PlayerSettings.Android.useCustomKeystore && !string.IsNullOrEmpty(PlayerSettings.Android.keystoreName))
            {
                bool keystoresExist = false;
                // Open .keystores file in current user home directory, read as ini file
                // [keystore name]
                // keystorePass = xxx
                // keyaliasPass = xxx
                try
                {
                    using (var streamReader = File.OpenText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), KEYSTORES_FILE)))
                    {
                        keystoresExist = true;
                        string line;
                        string keystoreName = null;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            if (line.StartsWith("[") && line.EndsWith("]"))
                            {
                                keystoreName = line.Substring(1, line.Length - 2);
                            }
                            else if (line.StartsWith("keystorePass"))
                            {
                                // search by keystore name, get the keystore password
                                if (keystoreName == PlayerSettings.Android.keystoreName)
                                {
                                    PlayerSettings.Android.keystorePass = line.Substring(line.IndexOf("=") + 1).Trim();
                                }
                            }
                            else if (line.StartsWith("keyaliasPass"))
                            {
                                // search by keystore name, get the keyalias password
                                if (keystoreName == PlayerSettings.Android.keystoreName)
                                {
                                    PlayerSettings.Android.keyaliasPass = line.Substring(line.IndexOf("=") + 1).Trim();
                                }
                            }
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    Debug.Log("File not found. Generate a new keystores map.");
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }

                // if .keystores doesn't exist, generate a new .keystores file
                if (keystoresExist == false)
                {
                    try
                    {
                        using (var streamWriter = File.CreateText(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), KEYSTORES_FILE)))
                        {
                            streamWriter.WriteLine("[" + PlayerSettings.Android.keystoreName + "]");
                            streamWriter.WriteLine("keystorePass = " + PlayerSettings.Android.keystorePass);
                            streamWriter.WriteLine("keyaliasPass = " + PlayerSettings.Android.keyaliasPass);
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
#endif
        }
    }
}
