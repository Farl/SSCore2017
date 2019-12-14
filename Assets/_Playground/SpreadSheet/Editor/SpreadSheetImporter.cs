using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using UnityEditor;
using System.IO;

public class SpreadSheetImporter {

    [MenuItem("Assets/Spread Sheet Importer")]
    public static void Import()
    {
        string[] GUIDs = Selection.assetGUIDs;
        if (GUIDs != null)
        {
            foreach (string guid in GUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log(path);

                string fullPath = Application.dataPath;
                DirectoryInfo di = Directory.GetParent(fullPath);
                fullPath = Path.Combine(di.ToString(), path);

                Debug.Log(fullPath);

                using (Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    IWorkbook workbook = null;

                    try
                    {
                        if (fullPath.EndsWith(".xls", System.StringComparison.InvariantCulture))
                            workbook = new HSSFWorkbook(stream);
                        else
                            workbook = new XSSFWorkbook(stream);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogException(e);
                    }

                    int sheetCount = workbook.NumberOfSheets;
                    for (int sIdx = 0; sIdx < sheetCount; sIdx++)
                    {
                        ISheet sheet = workbook.GetSheetAt(sIdx);
                        Debug.Log(sheet.SheetName);

                        IEnumerator rowEnum = sheet.GetRowEnumerator();
                        while (rowEnum.MoveNext())
                        {
                            var row = rowEnum.Current as IRow;
                            if (row != null)
                            {
                                IEnumerator cellEnum = row.GetEnumerator();
                                while (cellEnum.MoveNext())
                                {
                                    var cell = cellEnum.Current as ICell;
                                    if (cell != null)
                                    {
                                        Debug.LogFormat("{0},{1}={2}.",cell.RowIndex, cell.ColumnIndex, cell.ToString());
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }

    }
}
