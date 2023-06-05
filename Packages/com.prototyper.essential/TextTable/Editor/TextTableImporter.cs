using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Globalization;

using UnityEditor;

namespace SS
{
    public class TextTableImporter : EditorWindow
    {
        private static bool showDebugInfo = false;
        private TextAsset textAsset;
        private List<TextAsset> textAssets = new List<TextAsset>();
        private string overrideName = "TextTable - Default";

        [MenuItem("Assets/SS/Text Table CSV Import")]
        private static void ImportTextTable()
        {
            
            var obj = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(obj);

            var reader = new CSVReader();

            var str = File.ReadAllText(path);
            var contents = reader.ParseCSV(str);


            if (contents.Count > 0)
            {
                // Generate packages
                List<string> fields = contents[0];
                Dictionary<SystemLanguage, TextTablePackage> packageMap = new Dictionary<SystemLanguage, TextTablePackage>();
                for (var cellIdx = 0; cellIdx < fields.Count; cellIdx++)
                {
                    if (Enum.TryParse<SystemLanguage>(fields[cellIdx], out var result))
                    {
                        if (!packageMap.ContainsKey(result))
                        {
                            var pck = ScriptableObject.CreateInstance<TextTablePackage>();
                            packageMap.Add(result, pck);
                        }
                    }
                }

                for (var rowIdx = 1; rowIdx < contents.Count; rowIdx++)
                {
                    var row = contents[rowIdx];

                    // Comment
                    if (row.Count <= 0 || string.IsNullOrEmpty(row[0]) || row[0].StartsWith("//"))
                        continue;

                    var data = new TextTableData()
                    {
                        textID = row[0]
                    };

                    // Get meta data
                    for (var cellIdx = 0; cellIdx < row.Count && cellIdx < fields.Count; cellIdx++)
                    {
                        var cellValue = row[cellIdx];
                        var fieldName = fields[cellIdx];

                        // if is language
                        if (Enum.TryParse<SystemLanguage>(fieldName, out var result))
                        {
                        }
                        else
                        {
                            // set value
                            if (fieldName == "speaker")
                            {
                                data.speaker = cellValue;
                            }
                            else if (fieldName == "type")
                            {
                                data.type = cellValue;
                            }
                        }
                    }
                    // Get text
                    for (var cellIdx = 0; cellIdx < row.Count && cellIdx < fields.Count; cellIdx++)
                    {
                        var cellValue = row[cellIdx];
                        var fieldName = fields[cellIdx];

                        // if is language
                        if (Enum.TryParse<SystemLanguage>(fieldName, out var result))
                        {
                            var tmpData = data;
                            tmpData.text = cellValue;
                            packageMap[result].dataList.Add(tmpData);
                        }
                    }
                }

                // Generate
                foreach (var kvp in packageMap)
                {
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    var outputPath = path.Replace(Path.GetFileName(path), string.Empty).TrimEnd('/', '\\');

                    if (!AssetDatabase.IsValidFolder(Path.Combine(outputPath, "Resources")))
                        AssetDatabase.CreateFolder(outputPath, "Resources");
                    AssetDatabase.CreateAsset(kvp.Value, $"{Path.Combine(outputPath, "Resources", fileName)}_{kvp.Key.ToString()}.asset");
                }

                AssetDatabase.Refresh();
            }
        }
        
        [MenuItem("SS/Text Table")]
        private static void Open()
        {
            var w = EditorWindow.GetWindow<TextTableImporter>("Text");
        }

        private static string LanguageCodeToEnglishName(string code)
        {
            try
            {
                CultureInfo cultureInfo = new CultureInfo(code);

                var name = cultureInfo.EnglishName;
                name = name.Replace(" ", "").Replace("(", "").Replace(")", "");

                if (showDebugInfo)
                    Debug.Log($"{cultureInfo.EnglishName}={name}");

                return name;
            }
            catch (Exception)
            {

            }
            return "";
        }

        public static bool DrawDropPathArea(string title, GUIStyle style, out string[] paths)
        {
            paths = null;

            bool useLastRect = string.IsNullOrEmpty(title);

            if (!useLastRect && style == null)
            {
                style = new GUIStyle(GUI.skin.box);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontStyle = FontStyle.Italic;
                style.fontSize = 12;
            }

            Rect myRect = (useLastRect) ? GUILayoutUtility.GetLastRect() :
                GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));

            if (!useLastRect)
                GUI.Box(myRect, title, style);

            if (myRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                }

                if (Event.current.type == EventType.DragPerform)
                {
                    Event.current.Use();
                    if (DragAndDrop.paths.Length > 0)
                    {
                        paths = DragAndDrop.paths;

                        return true;
                    }
                }
            }
            return false;
        }

        private static void ImportPhraseCSV(TextAsset textAsset, string overrideName)
        {
            var path = AssetDatabase.GetAssetPath(textAsset);

            var reader = new CSVReader();
            var str = textAsset.text;
            var contents = reader.ParseCSV(str);

            if (contents.Count > 0)
            {
                // Generate packages
                List<string> fields = contents[0];
                Dictionary<SystemLanguage, TextTablePackage> packageMap = new Dictionary<SystemLanguage, TextTablePackage>();

                if (fields.Count >= 2)
                {
                    if (Enum.TryParse<SystemLanguage>(LanguageCodeToEnglishName(fields[1]), out var result))
                    {
                        if (!packageMap.ContainsKey(result))
                        {
                            var pck = ScriptableObject.CreateInstance<TextTablePackage>();
                            packageMap.Add(result, pck);
                        }
                    }
                }

                for (var rowIdx = 1; rowIdx < contents.Count; rowIdx++)
                {
                    var row = contents[rowIdx];

                    // Comment
                    if (row.Count <= 0 || string.IsNullOrEmpty(row[0]) || row[0].StartsWith("//"))
                        continue;

                    var data = new TextTableData()
                    {
                        textID = row[0]
                    };

                    // Get meta data
                    for (var cellIdx = 0; cellIdx < row.Count && cellIdx < fields.Count; cellIdx++)
                    {
                        var cellValue = row[cellIdx];
                        var fieldName = fields[cellIdx];

                        // if is language
                        if (Enum.TryParse<SystemLanguage>(LanguageCodeToEnglishName(fieldName), out var result))
                        {
                        }
                        else
                        {
                            // set value
                            if (fieldName == "speaker")
                            {
                                data.speaker = cellValue;
                            }
                            else if (fieldName == "type")
                            {
                                data.type = cellValue;
                            }
                        }
                    }
                    // Get text
                    for (var cellIdx = 0; cellIdx < row.Count && cellIdx < fields.Count; cellIdx++)
                    {
                        var cellValue = row[cellIdx];
                        var fieldName = fields[cellIdx];

                        // if is language
                        if (Enum.TryParse<SystemLanguage>(LanguageCodeToEnglishName(fieldName), out var result))
                        {
                            var tmpData = data;
                            tmpData.text = cellValue;
                            packageMap[result].dataList.Add(tmpData);
                        }
                    }
                }

                // Generate
                foreach (var kvp in packageMap)
                {
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    if (!string.IsNullOrEmpty(overrideName))
                        fileName = overrideName;
                    var outputPath = path.Replace(Path.GetFileName(path), string.Empty).TrimEnd('/', '\\');

                    if (!AssetDatabase.IsValidFolder(Path.Combine(outputPath, "Resources")))
                        AssetDatabase.CreateFolder(outputPath, "Resources");
                    AssetDatabase.CreateAsset(kvp.Value, $"{Path.Combine(outputPath, "Resources", fileName)}_{kvp.Key.ToString()}.asset");
                }

                AssetDatabase.Refresh();
            }
        }

        private void OnGUI()
        {
            showDebugInfo = EditorGUILayout.Toggle("Show Debug Info", showDebugInfo);
            EditorGUILayout.Space();
            overrideName = EditorGUILayout.TextField(overrideName);
            if (DrawDropPathArea("Drop CSVs here", null, out var paths))
            {
                textAssets.Clear();
                foreach (var p in paths)
                {
                    var obj = AssetDatabase.LoadAssetAtPath<TextAsset>(p);
                    if (obj)
                        textAssets.Add(obj);
                }
            }
            foreach (var ta in textAssets)
            {
                textAsset = EditorGUILayout.ObjectField(ta, typeof(TextAsset), false) as TextAsset;
            }
            if (textAsset && GUILayout.Button("Import Phrase CSV"))
            {
                foreach (var ta in textAssets)
                {
                    ImportPhraseCSV(ta, overrideName);
                }
            }
        }
    }
}
