using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;
using System;

using System.Reflection;

namespace SS
{
    public class TextTableImporter: AssetPostprocessor
    {
        
        private class TextTableDefine
        {
            public List<SpreadSheetImporter.FieldDefine> fields = new List<SpreadSheetImporter.FieldDefine>();
            private SpreadSheetImporter.FieldDefine lastField;

            public static Dictionary<string, TextTableDefine> textTableMap = new Dictionary<string, TextTableDefine>();

            public TextTableDefine()
            {

            }

            public static TextTableDefine GetOrAddTextTable(string sheetName)
            {
                if (textTableMap.TryGetValue(sheetName, out TextTableDefine textTable))
                {
                    return textTable;
                }
                else
                {
                    TextTableDefine textTableDefine = new TextTableDefine();
                    textTableMap.Add(sheetName, textTableDefine);
                    return textTable;
                }
            }

            internal bool Input(SpreadSheet.Cell cell)
            {
                string cellStringValue = cell.ToString();
                
                
                return true;
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (importedAssets.Length <= 0)
                    return;

            //Debug.Log($"Start to process {importedAssets.Length} assets");

            foreach (var importedAsset in importedAssets)
            {
                // TODO
                //Import(importedAsset);
            }
        }

        private static string outputPath => TextTableSettings.Instance.assetPath;

        private static void Import(string importedAsset)
        {
            // Ignore temporary files
            if (importedAsset.Contains("~$"))
                return;

            if (importedAsset.IndexOf("TextTable", System.StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                var packageCreated = false;
                var importedGUID = AssetDatabase.GUIDFromAssetPath(importedAsset).ToString();
                var spreadSheet = new SpreadSheet();

                spreadSheet.Load(importedAsset);

                // For each sheet
                foreach (SpreadSheet.Sheet sheet in spreadSheet)
                {
                    var sheetName = sheet.SheetName;
                    Debug.Log(sheet.SheetName);

                    if (sheetName.StartsWith(";") || sheetName.StartsWith("//"))
                        continue;

                    List<string> firstRow = new List<string>();

                    Dictionary<string, TextPackage> textPackages = new Dictionary<string, TextPackage>();
                    Dictionary<string, TextTableData> textTable = new Dictionary<string, TextTableData>();

                    // Pass 1
                    foreach (SpreadSheet.Row row in sheet)
                    {
                        TextTableData textData = null;

                        foreach (SpreadSheet.Cell cell in row)
                        {
                            //Debug.Log(cell.GetValue<string>());

                            var cellStringValue = cell.GetStringValue();
                            int rowIdx = cell.RowIndex;
                            int colIdx = cell.ColumnIndex;

                            // Collect fields
                            if (rowIdx == 0)
                            {
                                firstRow.Add(cellStringValue);

                                TextPackage tpInst;
                                if (cellStringValue == "Global")
                                {
                                    tpInst = ScriptableObject.CreateInstance<TextPackage>();
                                    tpInst.isGlobal = true;
                                }
                                else if (Enum.TryParse<SystemLanguage>(cellStringValue, out var language))
                                {
                                    tpInst = ScriptableObject.CreateInstance<TextPackage>();
                                    tpInst.language = language;
                                }
                                else
                                {
                                    continue;
                                }

                                if (tpInst != null)
                                {
                                    tpInst.packageName = sheetName;
                                    tpInst.referenceFileGUIDs.Add(AssetDatabase.GUIDFromAssetPath(importedAsset).ToString());
                                    textPackages.Add(cellStringValue, tpInst);
                                }
                            }
                            else if (colIdx == 0)
                            {
                                if (cellStringValue.StartsWith("//") || string.IsNullOrEmpty(cellStringValue))
                                {
                                    // Comment or empty
                                    continue;
                                }
                                else
                                {
                                    // Create text data
                                    textData = new TextTableData()
                                    {
                                        textID = cellStringValue,
                                    };
                                    
                                    // Check duplicate
                                    if (textTable.ContainsKey(textData.textID))
                                    {
                                        Debug.LogError($"Duplicate textID {textData.textID} row: [{rowIdx}]");
                                    }
                                    else
                                    {
                                        textTable.Add(textData.textID, textData);
                                    }
                                }
                            }
                        }
                    }

                    // Pass 2
                    foreach (SpreadSheet.Row row in sheet)
                    {
                        //Debug.Log(row.RowNum);
                        var textID = "";

                        // Foreach cell
                        foreach (SpreadSheet.Cell cell in row)
                        {
                            //Debug.Log(cell.GetValue<string>());

                            var cellStringValue = cell.GetStringValue();
                            int rowIdx = cell.RowIndex;
                            int colIdx = cell.ColumnIndex;

                            if (rowIdx == 0)
                            {
                                // Ignore
                                continue;
                            }
                            else if (colIdx == 0)
                            {
                                if (cellStringValue.StartsWith("//") || string.IsNullOrEmpty(cellStringValue))
                                {
                                    // Comment or empty
                                    continue;
                                }
                                textID = cellStringValue;
                            }
                            else
                            {
                                string caption = firstRow[colIdx];
                                if (caption == "Global" || Enum.TryParse<SystemLanguage>(caption, out var language))
                                {
                                    if (!textPackages[caption].data.ContainsKey(textID))
                                    {
                                        // Duplicate
                                        var textData = textTable[textID].ShallowCopy();
                                        textPackages[caption].data.Add(textID, textData);
                                    }

                                    textPackages[caption].data[textID].text = cellStringValue;
                                }
                            }
                        }
                    }

                    // Create TextPacakges

                    foreach (var kvp in textPackages)
                    {
                        var tp = kvp.Value;
                        var path = Path.Combine(outputPath, $"{tp.fileName}.asset");
                        DirectoryUtility.CheckAndCreateDirectory(path);
                        AssetDatabase.CreateAsset(tp, path);
                        tp.AddGUID(importedGUID);
                        packageCreated = true;

                    }
                }
                spreadSheet.Clear();

                if (packageCreated)
                {
                    TextTableSettings.Instance.Add(importedGUID, importedAsset);
                    AssetDatabase.Refresh();
                }
            }
        }

        [MenuItem("Assets/SS Import/TextTable", priority = 0)]
        public static void ImportTextTable()
        {
            // for all selections
            string[] GUIDs = Selection.assetGUIDs;
            if (GUIDs != null)
            {
                foreach (string guid in GUIDs)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    //Debug.Log(path);

                    Import(path);
                }
            }
        }
    }

}