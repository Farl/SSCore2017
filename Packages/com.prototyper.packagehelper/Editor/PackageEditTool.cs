using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace SS.PackageHelper
{
    /**
     * Input a json file, and read it as a package description object
        * 2023-06-02: fix minimal unity version issue.
     */
    public class PackageEditTool : PackageTool
    {
        TextAsset jsonFile;
        PackageData packageData;
        SerializedObject serializedObject;
        private string[] packageFolders;

        override public GUIContent toolbarIcon
        {
            // use icon In-Development
            get { return new GUIContent("", EditorGUIUtility.IconContent("CustomTool").image, "List and Edit"); }

        }

        private void ListAllLocalPackages()
        {
            if (GUILayout.Button("List all local packages"))
            {
                var packagesFolder = Application.dataPath + "/../Packages";
                packageFolders = Directory.GetDirectories(packagesFolder, "*", SearchOption.TopDirectoryOnly);
            }
            EditorGUILayout.Separator();

            if (packageFolders == null)
                return;

            foreach (var folder in packageFolders)
            {
                var filename = Path.GetFileName(folder);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(filename))
                {
                    // Select packageJson file in Project window
                    var packageJson = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>($"Packages/{filename}/package.json");
                    if (packageJson != null)
                    {
                        jsonFile = packageJson as TextAsset;
                        ReadJson();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public override void OnToolGUI(EditorWindow window)
        {
            base.OnToolGUI(window);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            jsonFile = EditorGUILayout.ObjectField("Json File", jsonFile, typeof(TextAsset), false) as TextAsset;
            if (jsonFile != null && EditorGUI.EndChangeCheck())
            {
                ReadJson();
            }
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                jsonFile = null;
            }
            EditorGUILayout.EndHorizontal();

            if (jsonFile == null || packageData == null || serializedObject == null)
            {
                ListAllLocalPackages();
                return;
            }

            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            do
            {
                EditorGUILayout.PropertyField(prop);
            } while (prop.NextVisible(false));

            // Draw dependencies dictionary with tool buttons
            EditorGUILayout.LabelField("Dependencies");
            for (int i = 0; i < packageData._dependencies_keys.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                packageData._dependencies_keys[i] = EditorGUILayout.TextField(packageData._dependencies_keys[i]);
                packageData._dependencies_values[i] = EditorGUILayout.TextField(packageData._dependencies_values[i]);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    var list = new List<string>(packageData._dependencies_keys);
                    list.RemoveAt(i);
                    packageData._dependencies_keys = list.ToArray();
                    list = new List<string>(packageData._dependencies_values);
                    list.RemoveAt(i);
                    packageData._dependencies_values = list.ToArray();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+"))
            {
                var list = new List<string>(packageData._dependencies_keys);
                list.Add("");
                packageData._dependencies_keys = list.ToArray();
                list = new List<string>(packageData._dependencies_values);
                list.Add("");
                packageData._dependencies_values = list.ToArray();
            }
            EditorGUILayout.Space();

            // Detect all folders in Samples~ folder in current package (by package json file)
            EditorGUILayout.LabelField("Samples in Samples~ folder:");
            var packageFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(jsonFile));
            var allSamplesInPackage = Directory.GetDirectories(packageFolder, "Samples~", SearchOption.AllDirectories);
            foreach (var samplesFolder in allSamplesInPackage)
            {
                var samples = Directory.GetDirectories(samplesFolder, "*", SearchOption.TopDirectoryOnly);
                foreach (var samplePath in samples)
                {
                    var relativeSamplePath = samplePath.Replace(packageFolder + "/", "");
                    var sampleName = Path.GetFileName(samplePath);
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("+", GUILayout.Width(20)))
                    {
                        ArrayUtility.Add<Sample>(ref packageData.samples,
                            new Sample()
                            {
                                path = $"{relativeSamplePath}",
                                displayName = $"{sampleName}"
                            }
                        );
                        serializedObject.Update();
                    }
                    EditorGUILayout.LabelField(Path.GetFileName(samplePath));
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel = 0;

            if (serializedObject.ApplyModifiedProperties())
            {
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Save"))
            {
                SaveJson();
            }
        }

        void SaveJson()
        {
            if (jsonFile == null)
                return;
            if (packageData == null)
                return;

            System.IO.File.WriteAllText(AssetDatabase.GetAssetPath(jsonFile), packageData.SerializeObject());

            // refresh packages
            RefreshPackages();
        }

        void ReadJson()
        {
            if (jsonFile == null)
                return;
            packageData = null;
            packageData = ScriptableObject.CreateInstance<PackageData>();
            packageData.PopulateObject(jsonFile.text);

            serializedObject = new SerializedObject(packageData);
            serializedObject.Update();
        }
    }
}
