using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SourceCodeExporter : EditorWindow {
    public string fileFilter = "*.cs";

    [MenuItem("Tools/Export/Source Code")]
    public static void Open()
    {
        EditorWindow.GetWindow<SourceCodeExporter>();
        Debug.Log(Application.dataPath);
    }

    private void OnGUI()
    {
        string buffer = "";
        if (GUILayout.Button("Export"))
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "SorceCode.txt")))
            {
                string[] files = Directory.GetFiles(Application.dataPath, fileFilter, SearchOption.AllDirectories);
                foreach (string filePath in files)
                {
                    writer.WriteLine("//// " + Path.GetFileName(filePath));

                    using (StreamReader reader = File.OpenText(filePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            buffer = reader.ReadLine();
                            writer.WriteLine(buffer);
                        }
                        reader.Close();
                    }
                }
                writer.Close();
            }
        }
    }
}
