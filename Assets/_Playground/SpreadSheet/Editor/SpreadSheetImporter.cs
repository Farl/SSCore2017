/**
 * Spread sheet import
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using UnityEditor;
using System.IO;
using System;

public class SpreadSheetImporter {

    public class ClassDefine
    {
        public string className;
        public ICell defineCell;
        public List<string> properties = new List<string>();

        public void Input(ICell cell)
        {
            string cellStringValue = cell.ToString();
            if (defineCell != null)
            {
                if (defineCell.RowIndex == cell.RowIndex && cell.ColumnIndex > defineCell.ColumnIndex)
                {
                    // Class properties
                    properties.Add(cellStringValue);
                }
            }
        }

        public string GenerateClass()
        {
            string result = "\t[System.Serializable]\n";
            result += "\tpublic " + className + '\n';
            result += "\t{\n";
            string prevPropertie = string.Empty;
            foreach (string propertie in properties)
            {
                if (propertie.StartsWith("/", StringComparison.InvariantCulture))
                    continue;

                if (propertie != prevPropertie)
                {
                    result += "\t\tpublic " + propertie + ";\n";
                }
                prevPropertie = propertie;
            }
            result += "\t}\n";
            return result;
        }

        public override string ToString()
        {
            string result = '\t' + className + '\n';
            result += "\t{\n";
            foreach (string propertie in properties)
            {
                result += "\t\t" + propertie + '\n';
            }
            result += "\t}\n";
            return result;
        }
    }

    public class ScriptDefine
    {
        const string templatePath = "_Playground/SpreadSheet/Editor/SpreadSheetScriptTemplate.txt";
        const string outputPath = "";

        public List<string> usingNamespaces = new List<string>();
        public string filename;
        public string mainClass;
        public List<ClassDefine> classDefines = new List<ClassDefine>();
        private ClassDefine currentClass;
        public HashSet<string> properties = new HashSet<string>();

        public ScriptDefine()
        {
        }

        public bool Input(ICell cell)
        {
            string cellStringValue = cell.ToString();
            if (cell.ColumnIndex == 0)
            {
                if (cellStringValue.StartsWith("//", System.StringComparison.InvariantCulture))
                {
                    // comments. Ignore
                    return false;
                }
                else if (cellStringValue.StartsWith("class ", System.StringComparison.InvariantCulture))
                {
                    if (mainClass == null)
                    {
                        mainClass = cellStringValue;
                    }
                    else
                    {
                        // class definition
                        currentClass = new ClassDefine()
                        {
                            className = cellStringValue,
                            defineCell = cell
                        };
                        classDefines.Add(currentClass);
                    }
                }
                else if (!string.IsNullOrEmpty(cellStringValue.Trim()) && !properties.Contains(cellStringValue))
                {
                    properties.Add(cellStringValue);
                }
            }
            else
            {
                if (currentClass != null)
                {
                    currentClass.Input(cell);
                }
            }
            return true;
        }

        public override string ToString()
        {
            string result = filename + '\n';
            result += mainClass + '\n';
            result += "{\n";
            foreach (ClassDefine cd in classDefines)
            {
                result += cd.ToString();
            }
            foreach (string propertie in properties)
            {
                result += '\t' + propertie + '\n';
            }
            result += "}\n";
            return result;
        }

        public void GenerateClass()
        {
            string outFilePath = Path.Combine(Path.Combine(Application.dataPath, outputPath), filename);// + ".txt";
            string inFilePath = Path.Combine(Application.dataPath, templatePath);

            using (StreamReader inReader = File.OpenText(inFilePath))
            {
                using (StreamWriter outWriter = File.CreateText(outFilePath))
                {
                    while (!inReader.EndOfStream)
                    {
                        string currLine = inReader.ReadLine();

                        string[] tokens = currLine.Split(new char[] { '[', ']' }, StringSplitOptions.None);
                        if (tokens != null && tokens.Length >= 3)
                        {
                            if (tokens[1] == "ScriptComments")
                            {
                                currLine = currLine.Replace("[ScriptComments]", filename);
                            }
                            else if (tokens[1] == "MainClassName")
                            {
                                currLine = currLine.Replace("[MainClassName]", mainClass);
                            }
                            else if (tokens[1] == "Classes")
                            {
                                string classes = string.Empty;
                                foreach (ClassDefine cd in classDefines)
                                {
                                    classes += cd.GenerateClass();
                                }
                                currLine = currLine.Replace("[Classes]", classes);
                            }
                            else if (tokens[1] == "Properties")
                            {
                                string propertiesStr = string.Empty;
                                foreach (string p in properties)
                                {
                                    propertiesStr += "\tpublic " + p + ";\n";
                                }
                                currLine = currLine.Replace("[Properties]", propertiesStr);
                            }
                            else if (tokens[1] == "UsingNamespaces")
                            {
                                currLine = currLine.Replace("[UsingNamespaces]", "");
                            }
                        }
                        outWriter.WriteLine(currLine);
                    }
                    outWriter.Close();
                }
                inReader.Close();
            }
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Assets/Import/Spread sheet as Script")]
    public static void ImportSpreadSheetAsScript()
    {
        // for all selections
        string[] GUIDs = Selection.assetGUIDs;
        if (GUIDs != null)
        {
            foreach (string guid in GUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                //Debug.Log(path);

                // combine full file path
                string fullPath = Application.dataPath;
                DirectoryInfo di = Directory.GetParent(fullPath);
                fullPath = Path.Combine(di.ToString(), path);
                //Debug.Log(fullPath);

                using (Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    IWorkbook workbook = null;

                    try
                    {
                        if (fullPath.EndsWith(".xls", System.StringComparison.InvariantCulture))
                            workbook = new HSSFWorkbook(stream);
                        else if (fullPath.EndsWith(".xlsx", System.StringComparison.InvariantCulture))
                            workbook = new XSSFWorkbook(stream);
                        else
                        {
                            stream.Close();
                            return;
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }

                    // for each sheet
                    IEnumerator sheetEnum = workbook.GetEnumerator();
                    while (sheetEnum.MoveNext())
                    {
                        var sheet = sheetEnum.Current as ISheet;
                        //Debug.Log(sheet.SheetName);

                        ScriptDefine scriptDefine = new ScriptDefine()
                        {
                            filename = sheet.SheetName,
                        };

                        // for each row
                        IEnumerator rowEnum = sheet.GetRowEnumerator();
                        while (rowEnum.MoveNext())
                        {
                            var row = rowEnum.Current as IRow;
                            if (row != null)
                            {
                                // for each cell
                                IEnumerator cellEnum = row.GetEnumerator();
                                while (cellEnum.MoveNext())
                                {
                                    var cell = cellEnum.Current as ICell;
                                    if (cell != null)
                                    {
                                        if (!scriptDefine.Input(cell))
                                        {
                                            continue;
                                        }
                                        //Debug.LogFormat("{0},{1}={2}.",cell.RowIndex, cell.ColumnIndex, cell.ToString());
                                    }
                                }
                            }
                        }
                        scriptDefine.GenerateClass();
                    }


                    // close file stream
                    stream.Close();
                }
            }
        }

    }
}
