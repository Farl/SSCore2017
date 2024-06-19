#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;

namespace HG
{
    public class iOSPostProcess : IPostprocessBuildWithReport
    {
        internal static PlistDocument GetInfoPlist(string plistPath)
        {
            // Get the plist file
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            return plist;
        }

        internal static string GetProjectName(PlistDocument plist)
        {
            string projectname = plist.root["CFBundleDisplayName"].AsString();
            return projectname;
        }

        /*! @brief required by the IPostprocessBuild interface. Set high to let other postprocess scripts run first. */
        public int callbackOrder
        {
            get { return 100; }
        }

        public void OnPostprocessBuild(BuildReport report)
        {

            var buildTarget = report.summary.platform;
            var path = report.summary.outputPath;

            if (buildTarget == BuildTarget.iOS)
            {
                string plistPath = Path.Combine(path, "Info.plist");
                PlistDocument info = GetInfoPlist(plistPath);
                PlistElementDict rootDict = info.root;

                // App Uses Non-Exempt Encryption
                // https://iter01.com/17513.html
                // https://developer.apple.com/documentation/bundleresources/information_property_list/itsappusesnonexemptencryption
                rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

                // Write the file
                info.WriteToFile(plistPath);
            }
        }
    }
}
#endif