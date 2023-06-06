using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SS.PackageHelper
{
    public static class GitUtility
    {
        private static bool showDebugInfo = false;
        public class GitConfigData
        {
            public string gitPath;
            public string currentBranch;
            public string currentRemoteUrl;
            public string currentRemote;
            public string currentRevision;

            public string userName;
            public string userEmail;

            public override string ToString()
            {
                return $"gitPath: {gitPath}\n" +
                    $"currentBranch: {currentBranch}\n" +
                    $"currentRemoteUrl: {currentRemoteUrl}\n" +
                    $"currentRemote: {currentRemote}\n" +
                    $"currentRevision: {currentRevision}\n" +
                    $"userName: {userName}\n" +
                    $"userEmail: {userEmail}\n";
            }
        }

        public static GitConfigData Scan()
        {
            GitConfigData data = new GitConfigData();

            // Get .git folder path from project root parent folder, if not search the parent's parent
            var path = Directory.GetParent(Application.dataPath);
            while (path != null)
            {
                var checkPath = Path.Combine(path.FullName, ".git");
                if (Directory.Exists(checkPath))
                {
                    data.gitPath = checkPath;
                    break;
                }
                path = path.Parent;
            }

            if (!string.IsNullOrEmpty(data.gitPath))
            {
                
                Log(data.gitPath);

                // Read git HEAD file
                var headPath = Path.Combine(data.gitPath, "HEAD");
                if (File.Exists(headPath))
                {
                    var head = File.ReadAllText(headPath);
                    Log(head);
                    data.currentBranch = head.Replace("ref: refs/heads/", "").Trim();

                    // Read git file HEAD
                    var refPath = Path.Combine(data.gitPath, head.Replace("ref: ", "").Trim());
                    if (File.Exists(refPath))
                    {
                        data.currentRevision = File.ReadAllText(refPath);
                        Log(data.currentRevision);
                    }
                }

                if (!string.IsNullOrEmpty(data.currentBranch))
                {
                    Log(data.currentBranch);

                    // Read git config file
                    var configPath = Path.Combine(data.gitPath, "config");
                    if (File.Exists(configPath))
                    {
                        var configFile = File.ReadAllText(configPath);
                        Log(configFile);

                        // Check cuurent branch remote and get url
                        var remote = $"[branch \"{data.currentBranch}\"]";
                        var remoteIndex = configFile.IndexOf(remote);
                        if (remoteIndex > 0)
                        {
                            var remoteConfig = configFile.Substring(remoteIndex);
                            Log(remoteConfig);

                            // Get remote
                            var remoteConfigIndex = remoteConfig.IndexOf("remote = ");
                            if (remoteConfigIndex > 0)
                            {
                                data.currentRemote = remoteConfig.Substring(remoteConfigIndex + 9);
                                data.currentRemote = data.currentRemote.Substring(0, data.currentRemote.IndexOf("\n")).Trim();
                                Log(data.currentRemote);

                                // Get remote config
                                var remoteIndex2 = configFile.IndexOf($"[remote \"{data.currentRemote}\"]");
                                if (remoteIndex2 > 0)
                                {
                                    // Get remote url
                                    var urlIndex = configFile.IndexOf("url = ");
                                    if (urlIndex > 0)
                                    {
                                        var urlConfig = configFile.Substring(urlIndex + 6);
                                        Log(urlConfig);
                                        data.currentRemoteUrl = urlConfig.Substring(0, urlConfig.IndexOf("\n")).Trim();
                                        Log(data.currentRemoteUrl);
                                    }
                                }
                            }
                        }

                        // Get user name and email from config
                        var userSectionIndex = configFile.IndexOf("[user]");
                        if (userSectionIndex > 0)
                        {
                            var userNameIndex = configFile.IndexOf("name = ");
                            if (userNameIndex > 0)
                            {
                                data.userName = configFile.Substring(userNameIndex + 7);
                                data.userName = data.userName.Substring(0, data.userName.IndexOf("\n")).Trim();
                            }
                            var userEmailIndex = configFile.IndexOf("email = ");
                            if (userEmailIndex > 0)
                            {
                                data.userEmail = configFile.Substring(userEmailIndex + 8);
                                data.userEmail = data.userEmail.Substring(0, data.userEmail.IndexOf("\n")).Trim();
                            }
                        }
                    }
                }
            }
            return data;
        }

        private static void Log(string message)
        {
            if (showDebugInfo)
            {
                Debug.Log("GitUtility: " + message);
            }
        }
    }
}
