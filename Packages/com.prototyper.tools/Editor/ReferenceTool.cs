using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class ReferenceTool : EditorWindow
{
    const string editorTitle = "Ref Tool";
    const string version = "1.0.0";

    private Vector2 scrollPosition;

    private string findGUID;
    private string findPath;
    private string replaceGUID;
    private string replacePath;
    private string extFilters = "mat;prefab;unity;asset";
    private HashSet<string> extToggles = new HashSet<string>(new string[] { "mat", "prefab", "unity" });
    private int sizeLimit = -1; // file size check
    private int currPage;
    private int elementPerPage = 10;

    private class ReferenceGUID
    {
        private string _guid;
        private int _count = 0;
        public ReferenceGUID(string guid, int count = 1)
        {
            _guid = guid;
            _count = count;
        }
        public void AddReference(int count = 1)
        {
            _count += count;
        }
        public int Count => _count;
    }

    private class ReferenceData
    {
        private Dictionary<string, ReferenceGUID> data = new Dictionary<string, ReferenceGUID>();
        public ReferenceData(string guid, int count = 1)
        {
            Add(guid, count);
        }
        public int Count => data.Count;
        public void Add(string guid, int count = 1)
        {
            if (data.TryGetValue(guid, out var value))
            {
                value.AddReference(count);
            }
            else
            {
                data.Add(guid, new ReferenceGUID(guid, count));
            }
        }
        public void Draw(ref int page, int elePerPage)
        {
            var count = data.Count;
            var maxPage = Mathf.CeilToInt((float)count / elePerPage);
            page = Mathf.Clamp(page, 0, maxPage - 1);
            var currPageMin = page * elePerPage;
            var currPageMax = ((page + 1) * elePerPage) - 1;
            var i = 0;
            foreach (var kvp in data)
            {
                if (i > currPageMax)
                    break;
                if (i >= currPageMin)
                {
                    var path = AssetDatabase.GUIDToAssetPath(kvp.Key);
                    if (GUILayout.Button($"[{kvp.Value.Count}] {path}"))
                    {
                        var obj = AssetDatabase.LoadMainAssetAtPath(path);
                        Selection.activeObject = obj;
                    }
                }
                i++;
            }
        }
        public void Replace(string from, string to)
        {
            foreach (var kvp in data)
            {
                var path = AssetDatabase.GUIDToAssetPath(kvp.Key);
                if (!string.IsNullOrEmpty(path))
                {
                    var text = File.ReadAllText(path);
                    text.Replace(from, to);
                    File.WriteAllText(path, text);
                }
            }
            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }
    }

    private Dictionary<string, ReferenceData> dictionary = new Dictionary<string, ReferenceData>();

    [MenuItem("Tools/SS/Reference Tool")]
    static void Open()
    {
        var w = EditorWindow.CreateWindow<ReferenceTool>();
        w.titleContent = new GUIContent($"{editorTitle} {version}");
    }

    private void CleanReference()
    {
        dictionary.Clear();
    }

    private void AddReference(string guid, string refByGUID, int count = 1)
    {
        if (dictionary.TryGetValue(guid, out var value))
        {
            value.Add(refByGUID, count);
        }
        else
        {
            dictionary.Add(guid, new ReferenceData(refByGUID, count));
        }
    }

    private string DrawDropArea(string title, bool useLastRect)
    {
        GUIStyle GuistyleBoxDND = new GUIStyle(GUI.skin.box);
        GuistyleBoxDND.alignment = TextAnchor.MiddleCenter;
        GuistyleBoxDND.fontStyle = FontStyle.Italic;
        GuistyleBoxDND.fontSize = 12;
        GUI.skin.box = GuistyleBoxDND;

        Rect myRect = (useLastRect)? GUILayoutUtility.GetLastRect():
            GUILayoutUtility.GetRect(0, 20, GUILayout.ExpandWidth(true));

        if (!useLastRect)
            GUI.Box(myRect, title, GuistyleBoxDND);

        if (myRect.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }

            if (Event.current.type == EventType.DragPerform)
            {
                if (DragAndDrop.paths.Length > 0)
                {
                    return DragAndDrop.paths[0];
                }
                Event.current.Use();
            }
        }
        return null;
    }

    private void DrawGUIDPicker(ref string path, ref string guid, string label)
    {
        bool validPath = !string.IsNullOrEmpty(path);
        if (GUILayout.Button(validPath? path: $"Drag and Drop here") && validPath)
        {
            var obj = AssetDatabase.LoadMainAssetAtPath(path);
            if (obj)
            {
                Selection.activeObject = obj;
            }
        }

        var dropResult = DrawDropArea(null, useLastRect: true);
        if (!string.IsNullOrEmpty(dropResult))
        {
            path = dropResult;
            guid = AssetDatabase.AssetPathToGUID(path);
        }
        EditorGUI.BeginChangeCheck();
        guid = EditorGUILayout.TextField(label, guid);
        if (EditorGUI.EndChangeCheck())
        {
            path = AssetDatabase.GUIDToAssetPath(guid);
        }
    }

    private void OnGUI()
    {
        DrawGUIDPicker(ref findPath, ref findGUID, "Find GUID");

        EditorGUILayout.Separator();

        DrawGUIDPicker(ref replacePath, ref replaceGUID, "Replcae GUID");
        if (!string.IsNullOrEmpty(findGUID) && !string.IsNullOrEmpty(replaceGUID) && GUILayout.Button("Replace"))
        {
            if (dictionary.ContainsKey(findGUID))
            {
                dictionary[findGUID].Replace(findGUID, replaceGUID);
            }
        }

        EditorGUILayout.Separator();

        EditorGUI.BeginChangeCheck();
        extFilters = EditorGUILayout.DelayedTextField("Extension filters", extFilters);
        var exts = extFilters.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        // File size check
        sizeLimit = EditorGUILayout.IntField("(TODO) File size limit", sizeLimit);

        if (EditorGUI.EndChangeCheck())
        {
            extToggles.Clear();
            foreach (var ext in exts)
            {
                extToggles.Add(ext);
            }
        }

        foreach (var ext in exts)
        {
            var currToggle = extToggles.Contains(ext);
            var newToggle = EditorGUILayout.Toggle(ext, extToggles.Contains(ext));
            if (newToggle != currToggle)
            {
                if (newToggle)
                    extToggles.Add(ext);
                else
                    extToggles.Remove(ext);
            }
        }

        if (GUILayout.Button("Scan"))
        {
            EditorUtility.DisplayProgressBar("Scan", null, 0);
            CleanReference();
            currPage = 0;

            var assetDir = Application.dataPath;
            var assetFiles = Directory.GetFiles(assetDir, "*.*", SearchOption.AllDirectories);

            var projectDir = (new DirectoryInfo(assetDir)).Parent.ToString().Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var packageDir = Path.Combine(projectDir, "Packages");
            var packageFiles = Directory.GetFiles(packageDir, "*.*", SearchOption.AllDirectories);

            List<string> files = new List<string>();
            files.AddRange(assetFiles);
            files.AddRange(packageFiles);

            var matchFiles = files.FindAll((s) =>
            {
                var ls = s.ToLower();
                foreach (var ext in extToggles)
                {
                    if (ls.EndsWith(ext))
                        return true;
                }
                return false;
            });

            var matchCount = matchFiles.Count;
            var matchIdx = 0;
            foreach (var file in matchFiles)
            {
                var modCount = Math.Max(1, matchCount / 100);
                if (matchIdx % (modCount) == 0 && EditorUtility.DisplayCancelableProgressBar($"Scan {matchIdx}/{matchCount}", file, (float)matchIdx / matchCount))
                {
                    // Interrupt
                    break;
                }

                var relPath = file.Replace(projectDir, "")
                    .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var guid = AssetDatabase.AssetPathToGUID(relPath);

                if (!string.IsNullOrEmpty(guid))
                {
                    var text = File.ReadAllText(file);
                    Regex regex = new Regex("guid: [0-9a-f]{32}", RegexOptions.None);
                    var matchCollection = regex.Matches(text);
                    foreach (Match m in matchCollection)
                    {
                        var refGUID = m.Value.Replace("guid: ", string.Empty);
                        AddReference(refGUID, guid);
                    }
                }
                else
                {
                    // Can ignore in Samples~ folder
                    Debug.LogWarning($"{relPath} can't get GUID from AssetDatabase");
                }
                matchIdx++;
            }
            EditorUtility.ClearProgressBar();
        }

        // Reference result
        var count = (!string.IsNullOrEmpty(findGUID) && dictionary.ContainsKey(findGUID)) ? dictionary[findGUID].Count : 0;
        EditorGUILayout.LabelField($"Reference count: {count}/{dictionary.Count}");


        elementPerPage = Mathf.Max(1, EditorGUILayout.IntField("Element limit", elementPerPage));
        EditorGUILayout.BeginHorizontal();
        GUILayout.Button("|<");
        if (GUILayout.Button("<"))
        {
            currPage = Mathf.Max(0, currPage - 1);
        }
        currPage = EditorGUILayout.IntField(currPage);
        if (GUILayout.Button(">"))
        {
            currPage++;
        }
        GUILayout.Button(">|");
        EditorGUILayout.EndHorizontal();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        if (!string.IsNullOrEmpty(findGUID) && dictionary.TryGetValue(findGUID, out var value))
        {
            value.Draw(ref currPage, elementPerPage);
        }
        EditorGUILayout.EndScrollView();

    }
}
